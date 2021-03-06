using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// 操作を扱います。
    public interface ICommand
    {
        /// 元に戻す。
        void Undo();

        /// やり直す。
        void Redo();

        /// 実行する。
        bool Execute();
    }

    /// node属性
    public struct NodeAttr
    {
        /// 回転
        public Quaternion rotation;
        /// 移動
        public Vector3 translation;
    }

    /// node操作
    public class NodeCommand : ICommand
    {
        //操作対象node
        public readonly TMONode node = null;
        /// 変更前の属性
        NodeAttr old_attr;
        /// 変更後の属性
        NodeAttr new_attr;

        public readonly Figure fig = null;

        /// node操作を生成します。
        public NodeCommand(Figure fig, TMONode node)
        {
            this.fig = fig;
            this.node = node;
            this.old_attr.rotation = node.Rotation;
            this.old_attr.translation = node.Translation;
        }

        /// 元に戻す。
        public void Undo()
        {
            node.Rotation = old_attr.rotation;
            node.Translation = old_attr.translation;
            fig.UpdateBoneMatricesWithoutTMOFrame();
        }

        /// やり直す。
        public void Redo()
        {
            node.Rotation = new_attr.rotation;
            node.Translation = new_attr.translation;
            fig.UpdateBoneMatricesWithoutTMOFrame();
        }

        /// 実行する。
        public bool Execute()
        {
            this.new_attr.rotation = node.Rotation;
            this.new_attr.translation = node.Translation;
            bool updated = old_attr.rotation != new_attr.rotation || old_attr.translation != new_attr.translation;
            return updated;
        }
    }

    /// スキンウェイト属性
    public struct SkinWeightAttr
    {
        /// ボーン参照インデックス
        public int bone_index;
        /// ウェイト値
        public float weight;
    }

    /// スキンウェイト操作
    public class SkinWeightCommand
    {
        //操作対象スキンウェイト
        SkinWeight skin_weight = null;
        /// 変更前の属性
        public SkinWeightAttr old_attr;
        /// 変更後の属性
        public SkinWeightAttr new_attr;

        /// スキンウェイト操作を生成します。
        public SkinWeightCommand(SkinWeight skin_weight)
        {
            this.skin_weight = skin_weight;
        }

        /// 変更を元に戻す。
        public void Undo()
        {
            skin_weight.bone_index = old_attr.bone_index;
            skin_weight.weight = old_attr.weight;
        }

        /// 変更をやり直す。
        public void Redo()
        {
            skin_weight.bone_index = new_attr.bone_index;
            skin_weight.weight = new_attr.weight;
        }
    }

    /// ウェイトの計算方法
    public enum WeightOperation
    {
        /// なし
        None,
        /// 加算
        Gain,
        /// 減算
        Reduce,
        /// 代入
        Assign
    }

    /// 頂点操作
    public class VertexCommand
    {
        //操作対象頂点
        Vertex vertex;
        //スキンウェイト操作リスト
        List<SkinWeightCommand> skin_weight_commands = new List<SkinWeightCommand>();

        TSOSubMesh sub_mesh = null;
        TSONode selected_node = null;
        float weight;
        WeightOperation weight_op = WeightOperation.None;

        /// 頂点操作を生成します。
        public VertexCommand(TSOSubMesh sub_mesh, TSONode selected_node, Vertex vertex, float weight, WeightOperation weight_op)
        {
            this.sub_mesh = sub_mesh;
            this.selected_node = selected_node;
            this.vertex = vertex;
            this.weight = weight;
            this.weight_op = weight_op;
        }

        /// 変更を元に戻す。
        public void Undo()
        {
            //前提: vertex.skin_weights の各要素は変更の前後で同じインスタンスである
            foreach (SkinWeightCommand skin_weight_command in this.skin_weight_commands)
            {
                skin_weight_command.Undo();
            }
            vertex.FillSkinWeights(); //for sort
            vertex.GenerateBoneIndices();
        }

        /// 変更をやり直す。
        public void Redo()
        {
            //前提: vertex.skin_weights の各要素は変更の前後で同じインスタンスである
            foreach (SkinWeightCommand skin_weight_command in this.skin_weight_commands)
            {
                skin_weight_command.Redo();
            }
            vertex.FillSkinWeights(); //for sort
            vertex.GenerateBoneIndices();
        }

        /// 選択ボーンに対応するウェイトを加算する。
        /// returns: ウェイトを変更したか
        public bool Execute()
        {
            foreach (SkinWeight skin_weight in vertex.skin_weights)
            {
                SkinWeightCommand skin_weight_command = new SkinWeightCommand(skin_weight);
                this.skin_weight_commands.Add(skin_weight_command);
            }
            //処理前の値を記憶する。
            {
                int nskin_weight = 0;
                foreach (SkinWeight skin_weight in vertex.skin_weights)
                {
                    this.skin_weight_commands[nskin_weight].old_attr.bone_index = skin_weight.bone_index;
                    this.skin_weight_commands[nskin_weight].old_attr.weight = skin_weight.weight;
                    nskin_weight++;
                }
            }

            bool updated = false;

            //選択ボーンに対応するウェイトを検索する。
            SkinWeight selected_skin_weight = null;
            foreach (SkinWeight skin_weight in vertex.skin_weights)
            {
                TSONode bone = sub_mesh.GetBone(skin_weight.bone_index);
                if (bone == selected_node)
                {
                    selected_skin_weight = skin_weight;
                    break;
                }
            }

            bool prepare_bone = false;
            switch (weight_op)
            {
                case WeightOperation.Gain:
                    prepare_bone = weight > 0;
                    break;
                case WeightOperation.Assign:
                    prepare_bone = weight > 0;
                    break;
            }

            //選択ボーンに対応するウェイトがなければ、最小値を持つウェイトを置き換える。
            if (selected_skin_weight == null && prepare_bone)
            {
                //サブメッシュのボーン参照に指定ノードが含まれるか。
                int bone_index = Array.IndexOf(sub_mesh.bone_indices, selected_node.Id);
                if (bone_index == -1)
                    bone_index = sub_mesh.AddBone(selected_node);
                if (bone_index != -1)
                {
                    selected_skin_weight = vertex.skin_weights[3]; //前提: vertex.skin_weights の要素数は 4 かつ並び順はウェイト値の降順
                    selected_skin_weight.bone_index = bone_index;
                    selected_skin_weight.weight = 0.0f;
                }
            }

            //選択ボーンに対応するウェイトを加算する。
            if (selected_skin_weight != null)
            {
                updated = true;

                float w0 = selected_skin_weight.weight; //変更前の対象ウェイト値
                float m0 = 1.0f - w0;                   //変更前の残りウェイト値
                float w1;                               //変更後の対象ウェイト値
                switch (weight_op)
                {
                    case WeightOperation.Gain:
                        w1 = w0 + weight;
                        break;
                    case WeightOperation.Reduce:
                        w1 = w0 - weight;
                        break;
                    case WeightOperation.Assign:
                        w1 = weight;
                        break;
                    default:
                        w1 = w0;
                        break;
                }
                //clamp 0.0f .. 1.0f
                if (w1 > 1.0f) w1 = 1.0f;
                if (w1 < 0.0f) w1 = 0.0f;

                float d1 = w1 - w0; //実際の加算値
                float m1 = 0.0f;    //減算後の残りウェイト値
                if (m0 != 0)
                {
                    //残りウェイトを減算する。
                    foreach (SkinWeight skin_weight in vertex.skin_weights)
                    {
                        if (skin_weight == selected_skin_weight)
                            continue;

                        float w2 = skin_weight.weight - skin_weight.weight * d1 / m0;
                        if (w2 < 0.001f)
                            w2 = 0.0f;//微小ウェイトは捨てる。

                        skin_weight.weight = w2;
                        m1 += w2;
                    }
                }
                selected_skin_weight.weight = 1.0f - m1;
            }

            //処理後の値を記憶する。
            {
                int nskin_weight = 0;
                foreach (SkinWeight skin_weight in vertex.skin_weights)
                {
                    this.skin_weight_commands[nskin_weight].new_attr.bone_index = skin_weight.bone_index;
                    this.skin_weight_commands[nskin_weight].new_attr.weight = skin_weight.weight;
                    nskin_weight++;
                }
            }
            return updated;
        }
    }

    /// サブメッシュ操作
    public class SubMeshCommand
    {
        //操作対象サブメッシュ
        TSOSubMesh sub_mesh = null;
        //頂点操作リスト
        List<VertexCommand> vertex_commands = new List<VertexCommand>();

        Figure fig = null;
        TSONode selected_node = null;
        float weight;
        WeightOperation weight_op;
        Vector3 center;
        float radius;

        /// サブメッシュ操作を生成します。
        public SubMeshCommand(Figure fig, TSOSubMesh sub_mesh, TSONode selected_node, float weight, WeightOperation weight_op, Vector3 center, float radius)
        {
            this.fig = fig;
            this.sub_mesh = sub_mesh;
            this.selected_node = selected_node;
            this.weight = weight;
            this.weight_op = weight_op;
            this.center = center;
            this.radius = radius;
        }

        /// 変更を元に戻す。
        public void Undo()
        {
            foreach (VertexCommand vertex_command in this.vertex_commands)
            {
                vertex_command.Undo();
            }
            this.sub_mesh.WriteBuffer();
        }

        /// 変更をやり直す。
        public void Redo()
        {
            foreach (VertexCommand vertex_command in this.vertex_commands)
            {
                vertex_command.Redo();
            }
            this.sub_mesh.WriteBuffer();
        }

        /// 選択ボーンに対応するウェイトを加算する。
        /// returns: ウェイトを変更したか
        public bool Execute()
        {
            bool updated = false;

            for (int i = 0; i < sub_mesh.vertices.Length; i++)
            {
                Vertex v = sub_mesh.vertices[i];

                if (v.selected)
                {
                    VertexCommand vertex_command = new VertexCommand(sub_mesh, selected_node, v, weight, weight_op);
                    
                    if (vertex_command.Execute())
                    {
                        v.FillSkinWeights();
                        v.GenerateBoneIndices();
                        this.vertex_commands.Add(vertex_command);
                        updated = true;
                    }
                }
            }
            return updated;
        }
    }

    /// メッシュ操作
    public class MeshCommand : ICommand
    {
        //操作対象メッシュ
        TSOMesh mesh = null;
        //サブメッシュ操作リスト
        List<SubMeshCommand> sub_mesh_commands = new List<SubMeshCommand>();

        Figure fig = null;
        TSONode selected_node = null;
        float weight;
        WeightOperation weight_op;
        Vector3 center;
        float radius;

        /// メッシュ操作を生成します。
        public MeshCommand(Figure fig, TSOMesh mesh, TSONode selected_node, float weight, WeightOperation weight_op, Vector3 center, float radius)
        {
            this.fig = fig;
            this.mesh = mesh;
            this.selected_node = selected_node;
            this.weight = weight;
            this.weight_op = weight_op;
            this.center = center;
            this.radius = radius;
        }

        /// 変更を元に戻す。
        public void Undo()
        {
            foreach (SubMeshCommand sub_mesh_command in this.sub_mesh_commands)
            {
                sub_mesh_command.Undo();
            }
        }

        /// 変更をやり直す。
        public void Redo()
        {
            foreach (SubMeshCommand sub_mesh_command in this.sub_mesh_commands)
            {
                sub_mesh_command.Redo();
            }
        }

        /// 選択ボーンに対応するウェイトを加算する。
        public bool Execute()
        {
            bool updated = false;

            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
            {
                //操作を生成する。
                SubMeshCommand sub_mesh_command = new SubMeshCommand(fig, sub_mesh, selected_node, weight, weight_op, center, radius);

                if (sub_mesh_command.Execute())
                {
                    sub_mesh.WriteBuffer();
                    this.sub_mesh_commands.Add(sub_mesh_command);
                    updated = true;
                }
            }
            return updated;
        }
    }

    /// <summary>
    /// TSOFileをDirect3D上でレンダリングします。
    /// </summary>
public class WeightViewer : Viewer
{
    internal Mesh sphere = null;
    internal Texture dot_texture = null;

    /// <summary>
    /// effect handle for LocalBoneSels
    /// TSOWeight extension
    /// </summary>
    protected EffectHandle handle_LocalBoneSels;

    /// <summary>
    /// viewerを生成します。
    /// </summary>
    public WeightViewer()
    {
        this.Rendering += delegate()
        {
            RenderDerived();
        };
        LineColor = Color.FromArgb(100, 100, 230); //from MikuMikuDance
        SelectedLineColor = Color.FromArgb(255, 0, 0); //red
    }

    /// get path to dot.bmp
    public static string GetDotBitmapPath()
    {
        return Path.Combine(Application.StartupPath, @"dot.bmp");
    }

    /// <summary>
    /// deviceを作成します。
    /// </summary>
    /// <param name="control">レンダリング先となるcontrol</param>
    /// <returns>deviceの作成に成功したか</returns>
    public new bool InitializeApplication(Control control)
    {
        return InitializeApplication(control, false);
    }

    /// <summary>
    /// deviceを作成します。
    /// </summary>
    /// <param name="control">レンダリング先となるcontrol</param>
    /// <param name="shadowMapEnabled">シャドウマップを作成するか</param>
    /// <returns>deviceの作成に成功したか</returns>
    public new bool InitializeApplication(Control control, bool shadowMapEnabled)
    {
        if (! base.InitializeApplication(control, shadowMapEnabled))
            return false;

        sphere = Mesh.Sphere(device, 0.25f, 8, 4);
        dot_texture = TextureLoader.FromFile(device, GetDotBitmapPath());
        handle_LocalBoneSels = effect.GetParameter(null, "LocalBoneSels");

        return true;
    }

    /// <summary>
    /// メッシュ描画モード
    /// </summary>
    public enum MeshViewMode
    {
        /// <summary>
        /// トゥーン描画
        /// </summary>
        Toon,
        /// <summary>
        /// ウェイト描画
        /// </summary>
        Heat,
        /// <summary>
        /// ワイヤー描画
        /// </summary>
        Wire
    };
    /// <summary>
    /// メッシュ描画モード
    /// </summary>
    public MeshViewMode mesh_view_mode = MeshViewMode.Toon;

    /// <summary>
    /// メッシュ選択モード
    /// </summary>
    public enum MeshSelectionMode
    {
        /// <summary>
        /// 全てのメッシュを描画
        /// </summary>
        AllMeshes,
        /// <summary>
        /// 選択メッシュのみ描画
        /// </summary>
        SelectedMesh
    }
    /// <summary>
    /// メッシュ選択モード
    /// </summary>
    public MeshSelectionMode mesh_selection_mode = MeshSelectionMode.AllMeshes;

    /// <summary>
    /// 頂点選択モード
    /// </summary>
    public enum VertexSelectionMode
    {
        /// <summary>
        /// 全ての頂点を描画
        /// </summary>
        AllVertices,
        /// <summary>
        /// 表面頂点のみ描画
        /// </summary>
        CcwVertices,
        /// <summary>
        /// 頂点を描画しない
        /// </summary>
        None
    }
    /// <summary>
    /// 頂点選択モード
    /// </summary>
    public VertexSelectionMode vertex_selection_mode = VertexSelectionMode.CcwVertices;

    /// <summary>
    /// node選択モード
    /// </summary>
    public enum NodeSelectionMode
    {
        /// <summary>
        /// 全てのnodeを描画
        /// </summary>
        AllBones,
        /// <summary>
        /// nodeを描画しない
        /// </summary>
        None
    }
    /// <summary>
    /// node選択モード
    /// </summary>
    public NodeSelectionMode node_selection_mode = NodeSelectionMode.AllBones;

    /// <summary>
    /// フィギュアを描画します。
    /// </summary>
    protected override void DrawFigure()
    {
        device.SetRenderState(RenderStates.AlphaBlendEnable, true);

        device.SetRenderTarget(0, dev_surface);
        device.DepthStencilSurface = dev_zbuf;
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer | ClearFlags.Stencil, ScreenColor, 1.0f, 0);

        device.VertexDeclaration = vd;
        Figure fig;
        if (TryGetFigure(out fig))
        {
            switch (mesh_view_mode)
            {
                case MeshViewMode.Toon:
                    switch (mesh_selection_mode)
                    {
                        case MeshSelectionMode.AllMeshes:
                            {
                                foreach (TSOFile tso in fig.TSOList)
                                {
                                    tso.BeginRender();
                                    foreach (TSOMesh mesh in tso.meshes)
                                        foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                                            DrawSubMeshForToonRendering(fig, tso, sub_mesh);
                                    tso.EndRender();
                                }
                            }
                            break;
                        case MeshSelectionMode.SelectedMesh:
                            if (SelectedTSOFile != null && SelectedMesh != null)
                            {
                                {
                                    TSOFile tso = SelectedTSOFile;
                                    tso.BeginRender();
                                    foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
                                        DrawSubMeshForToonRendering(fig, tso, sub_mesh);
                                    tso.EndRender();
                                }
                            }
                            break;
                    }
                    break;
                case MeshViewMode.Heat:
                    switch (mesh_selection_mode)
                    {
                        case MeshSelectionMode.AllMeshes:
                            {
                                foreach (TSOFile tso in fig.TSOList)
                                {
                                    tso.BeginRender();
                                    foreach (TSOMesh mesh in tso.meshes)
                                        foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                                            DrawSubMeshForWeightHeating(fig, sub_mesh);
                                    tso.EndRender();
                                }
                            }
                            break;
                        case MeshSelectionMode.SelectedMesh:
                            if (SelectedTSOFile != null && SelectedMesh != null)
                            {
                                TSOFile tso = SelectedTSOFile;
                                tso.BeginRender();
                                foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
                                    DrawSubMeshForWeightHeating(fig, sub_mesh);
                                tso.EndRender();
                            }
                            break;
                    }
                    break;
                case MeshViewMode.Wire:
                    switch (mesh_selection_mode)
                    {
                        case MeshSelectionMode.AllMeshes:
                            {
                                foreach (TSOFile tso in fig.TSOList)
                                {
                                    tso.BeginRender();
                                    foreach (TSOMesh mesh in tso.meshes)
                                        foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                                            DrawSubMeshForWireFrame(fig, tso, sub_mesh);
                                    tso.EndRender();
                                }
                            }
                            break;
                        case MeshSelectionMode.SelectedMesh:
                            if (SelectedTSOFile != null && SelectedMesh != null)
                            {
                                {
                                    TSOFile tso = SelectedTSOFile;
                                    tso.BeginRender();
                                    foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
                                        DrawSubMeshForWireFrame(fig, tso, sub_mesh);
                                    tso.EndRender();
                                }
                            }
                            break;
                    }
                    break;
            }
        }
    }

    private void DrawSubMeshForToonRendering(Figure fig, TSOFile tso, TSOSubMesh sub_mesh)
    {
        device.SetRenderState(RenderStates.FillMode, (int)FillMode.Solid);
        //device.RenderState.VertexBlend = (VertexBlend)(4 - 1);
        device.SetStreamSource(0, sub_mesh.vb, 0, 52);
        device.Indices = sub_mesh.ib;

        tso.SwitchShader(sub_mesh);
        effect.SetValue(handle_LocalBoneMats, fig.ClipBoneMatrices(sub_mesh));

        int npass = effect.Begin(0);
        for (int ipass = 0; ipass < npass; ipass++)
        {
            effect.BeginPass(ipass);
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, sub_mesh.vh.Count, 0, sub_mesh.vindices.Count / 3);
            effect.EndPass();
        }
        effect.End();
    }

    private void DrawSubMeshForWeightHeating(Figure fig, TSOSubMesh sub_mesh)
    {
        device.SetRenderState(RenderStates.FillMode, (int)FillMode.Solid);
        //device.RenderState.VertexBlend = (VertexBlend)(4 - 1);
        device.SetStreamSource(0, sub_mesh.vb, 0, 52);
        device.Indices = sub_mesh.ib;

        effect.Technique = "BoneCol";
        effect.SetValue("PenColor", new Vector4(1, 1, 1, 1));
        effect.SetValue(handle_LocalBoneMats, fig.ClipBoneMatrices(sub_mesh));
        effect.SetValue(handle_LocalBoneSels, ClipBoneSelections(sub_mesh, selected_node));

        int npass = effect.Begin(0);
        for (int ipass = 0; ipass < npass; ipass++)
        {
            effect.BeginPass(ipass);
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, sub_mesh.vh.Count, 0, sub_mesh.vindices.Count / 3);
            effect.EndPass();
        }
        effect.End();
    }

    private void DrawSubMeshForWireFrame(Figure fig, TSOFile tso, TSOSubMesh sub_mesh)
    {
        device.SetRenderState(RenderStates.FillMode, (int)FillMode.WireFrame);
        //device.RenderState.VertexBlend = (VertexBlend)(4 - 1);
        device.SetStreamSource(0, sub_mesh.vb, 0, 52);
        device.Indices = sub_mesh.ib;

        tso.SwitchShader(sub_mesh);
        effect.SetValue(handle_LocalBoneMats, fig.ClipBoneMatrices(sub_mesh));

        int npass = effect.Begin(0);
        for (int ipass = 0; ipass < npass; ipass++)
        {
            effect.BeginPass(ipass);
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, sub_mesh.vh.Count, 0, sub_mesh.vindices.Count / 3);
            effect.EndPass();
        }
        effect.End();
    }

    /// <summary>
    /// ボーン選択の配列を得ます。
    /// </summary>
    /// <param name="sub_mesh">サブメッシュ</param>
    /// <param name="selected_node">選択ボーン</param>
    /// <returns>ボーン選択の配列</returns>
    static int[] ClipBoneSelections(TSOSubMesh sub_mesh, TSONode selected_node)
    {
        int[] clipped_boneSelections = new int[sub_mesh.maxPalettes];

        for (int numPalettes = 0; numPalettes < sub_mesh.maxPalettes; numPalettes++)
        {
            TSONode tso_node = sub_mesh.GetBone(numPalettes);
            clipped_boneSelections[numPalettes] = (selected_node == tso_node) ? 1 : 0;
        }
        return clipped_boneSelections;
    }

    /// <summary>
    /// シーンをレンダリングします。
    /// </summary>
    public void RenderDerived()
    {
        if (MotionEnabled)
            return;

        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (node_selection_mode == NodeSelectionMode.AllBones)
            {
                //nodeを描画する。
                DrawNodeTree(fig);
                DrawSelectedNode(fig);
            }

            if (SelectedMesh != null)
            {
                //頂点を描画する。
                foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
                {
                    DrawVertices(fig, sub_mesh);
                }
                DrawSelectedVertex(fig);
            }
        }
    }

    /// node line描画色
    public Color LineColor { get; set; }

    /// <summary>
    /// フィギュアに含まれるnode treeを描画する。
    /// </summary>
    /// <param name="fig"></param>
    void DrawNodeTree(Figure fig)
    {
        TMOFile tmo = fig.Tmo;

        Line line = new Line(device);
        foreach (TMONode node in tmo.nodes)
        {
            Vector3 p0 = GetNodePositionOnScreen(node);

            TMONode parent_node = node.parent;
            if (parent_node != null)
            {
                Vector3 p1 = GetNodePositionOnScreen(parent_node);

                Vector3 pd = p0 - p1;
                float len = Vector3.Length(pd);
                float scale = 4.0f / len;
                Vector2 p3 = new Vector2(p1.X+pd.Y*scale, p1.Y-pd.X*scale);
                Vector2 p4 = new Vector2(p1.X-pd.Y*scale, p1.Y+pd.X*scale);

                Vector2[] vertices = new Vector2[3];
                vertices[0] = new Vector2(p3.X, p3.Y);
                vertices[1] = new Vector2(p0.X, p0.Y);
                vertices[2] = new Vector2(p4.X, p4.Y);
                line.Draw(vertices, LineColor);
            }
        }
        line.Dispose();
        line = null;

        Rectangle rect = new Rectangle(0, 16, 15, 15); //node circle
        Vector3 rect_center = new Vector3(7, 7, 0);
        sprite.Begin(SpriteFlags.None);
        foreach (TMONode node in tmo.nodes)
        {
            Vector3 p0 = GetNodePositionOnScreen(node);
            sprite.Draw(dot_texture, rect, rect_center, p0, Color.White);
        }
        sprite.End();
    }

    /// 指定行列の移動変位を得ます。
    public static Vector3 GetMatrixTranslation(ref Matrix m)
    {
        return new Vector3(m.M41, m.M42, m.M43);
    }

    /// 指定行列のX軸先位置を得ます。
    public static Vector3 GetMatrixDirXTranslation(ref Matrix m, float len)
    {
        return new Vector3(m.M11 * len + m.M41, m.M12 * len + m.M42, m.M13 * len + m.M43);
    }

    /// 指定行列のY軸先位置を得ます。
    public static Vector3 GetMatrixDirYTranslation(ref Matrix m, float len)
    {
        return new Vector3(m.M21 * len + m.M41, m.M22 * len + m.M42, m.M23 * len + m.M43);
    }

    /// 指定行列のZ軸先位置を得ます。
    public static Vector3 GetMatrixDirZTranslation(ref Matrix m, float len)
    {
        return new Vector3(m.M31 * len + m.M41, m.M32 * len + m.M42, m.M33 * len + m.M43);
    }

    Vector3 GetNodePositionOnScreen(TMONode node)
    {
        Vector3 p1 = GetMatrixTranslation(ref node.combined_matrix);
        Vector3 p2 = WorldToScreen(p1);
        p2.Z = 0.0f; //表面に固定
        return p2;
    }

    Vector3 GetNodeDirXPositionOnScreen(TMONode node)
    {
        Vector3 p1 = GetMatrixDirXTranslation(ref node.combined_matrix, 1);
        Vector3 p2 = WorldToScreen(p1);
        p2.Z = 0.0f; //表面に固定
        return p2;
    }

    Vector3 GetNodeDirYPositionOnScreen(TMONode node)
    {
        Vector3 p1 = GetMatrixDirYTranslation(ref node.combined_matrix, 1);
        Vector3 p2 = WorldToScreen(p1);
        p2.Z = 0.0f; //表面に固定
        return p2;
    }

    Vector3 GetNodeDirZPositionOnScreen(TMONode node)
    {
        Vector3 p1 = GetMatrixDirZTranslation(ref node.combined_matrix, 1);
        Vector3 p2 = WorldToScreen(p1);
        p2.Z = 0.0f; //表面に固定
        return p2;
    }

    /// 選択node line描画色
    public Color SelectedLineColor { get; set; }

    /// 選択nodeを描画する。
    void DrawSelectedNode(Figure fig)
    {
        if (selected_node == null)
            return;

        TMONode bone;
        if (fig.nodemap.TryGetValue(selected_node, out bone))
        {
            Vector3 p1 = GetNodePositionOnScreen(bone);

            if (selected_node.children.Count != 0)
            {
                TMONode child_bone;
                if (fig.nodemap.TryGetValue(selected_node.children[0], out child_bone))
                {
                    Line line = new Line(device);

                    Vector3 p0 = GetNodePositionOnScreen(child_bone);

                    Vector3 pd = p0 - p1;
                    float len = Vector3.Length(pd);
                    float scale = 4.0f / len;
                    Vector2 p3 = new Vector2(p1.X + pd.Y * scale, p1.Y - pd.X * scale);
                    Vector2 p4 = new Vector2(p1.X - pd.Y * scale, p1.Y + pd.X * scale);

                    Vector2[] vertices = new Vector2[3];
                    vertices[0] = new Vector2(p3.X, p3.Y);
                    vertices[1] = new Vector2(p0.X, p0.Y);
                    vertices[2] = new Vector2(p4.X, p4.Y);
                    line.Draw(vertices, SelectedLineColor);

                    line.Dispose();
                    line = null;
                }
            }

            {
                Vector3 px = GetNodeDirXPositionOnScreen(bone);
                Vector3 py = GetNodeDirYPositionOnScreen(bone);
                Vector3 pz = GetNodeDirZPositionOnScreen(bone);

                Color line_color_x = Color.FromArgb(255, 0, 0); //R
                Color line_color_y = Color.FromArgb(0, 255, 0); //G
                Color line_color_z = Color.FromArgb(0, 0, 255); //B
                Line line = new Line(device);
                line.Width = 3;

                Vector2[] vertices = new Vector2[2];
                vertices[0] = new Vector2(p1.X, p1.Y);
                vertices[1] = new Vector2(px.X, px.Y);
                line.Draw(vertices, line_color_x);
                vertices[1] = new Vector2(py.X, py.Y);
                line.Draw(vertices, line_color_y);
                vertices[1] = new Vector2(pz.X, pz.Y);
                line.Draw(vertices, line_color_z);

                line.Dispose();
                line = null;
            }

            Rectangle rect = new Rectangle(16, 16, 15, 15); //node circle
            Vector3 rect_center = new Vector3(7, 7, 0);
            sprite.Begin(SpriteFlags.None);
            sprite.Draw(dot_texture, rect, rect_center, p1, Color.White);
            sprite.End();
        }
    }

    /// 頂点を描画する。
    void DrawVertices(Figure fig, TSOSubMesh sub_mesh)
    {
        Matrix[] clipped_boneMatrices = fig.ClipBoneMatrices(sub_mesh);

        Rectangle rect = new Rectangle(0, 0, 7, 7);//red
        Vector3 rect_center = new Vector3(3, 3, 0);

        Vector3[] view_positions = new Vector3[sub_mesh.vertices.Length];
        Vector3[] screen_positions = new Vector3[sub_mesh.vertices.Length];
        for (int i = 0; i < sub_mesh.vertices.Length; i++)
        {
            Vector3 p1 = sub_mesh.vertices[i].CalcSkindeformPosition(clipped_boneMatrices);
            view_positions[i] = Vector3.TransformCoordinate(p1, Transform_View);
            screen_positions[i] = WorldToScreen(p1);
        }
        switch (vertex_selection_mode)
        {
            case VertexSelectionMode.AllVertices:
                {
                    if (selected_vertex != null)
                    {
                        sprite.Begin(SpriteFlags.None);

                        for (int i = 0; i < sub_mesh.vertices.Length; i++)
                        {
                            if (sub_mesh.vertices[i].selected)
                                rect = new Rectangle(8, 8, 7, 7);//yellow
                            else
                                rect = new Rectangle(0, 0, 7, 7);//red

                            Vector3 p2 = screen_positions[i];
                            p2.Z = 0.0f;
                            sprite.Draw(dot_texture, rect, rect_center, p2, Color.White);
                        }
                        sprite.End();
                    }
                    else
                    {
                        sprite.Begin(SpriteFlags.None);

                        for (int i = 0; i < sub_mesh.vertices.Length; i++)
                        {
                            Vector3 p2 = screen_positions[i];
                            p2.Z = 0.0f;
                            sprite.Draw(dot_texture, rect, rect_center, p2, Color.White);
                        }
                        sprite.End();
                    }
                }
                break;
            case VertexSelectionMode.CcwVertices:
                {
                    bool[] ccws = CreateCcws(view_positions);

                    if (selected_vertex != null)
                    {
                        sprite.Begin(SpriteFlags.None);

                        for (int i = 0; i < sub_mesh.vertices.Length; i++)
                        {
                            if (!ccws[i])
                                continue;

                            if (sub_mesh.vertices[i].selected)
                                rect = new Rectangle(8, 8, 7, 7);//yellow
                            else
                                rect = new Rectangle(0, 0, 7, 7);//red

                            Vector3 p2 = screen_positions[i];
                            p2.Z = 0.0f;
                            sprite.Draw(dot_texture, rect, rect_center, p2, Color.White);
                        }
                        sprite.End();
                    }
                    else
                    {
                        sprite.Begin(SpriteFlags.None);

                        for (int i = 0; i < sub_mesh.vertices.Length; i++)
                        {
                            if (!ccws[i])
                                continue;

                            Vector3 p2 = screen_positions[i];
                            p2.Z = 0.0f;
                            sprite.Draw(dot_texture, rect, rect_center, p2, Color.White);
                        }
                        sprite.End();
                    }
                }
                break;
            case VertexSelectionMode.None:
                break;
        }

    }

    bool IsCounterClockWise(float x0, float y0, float x1, float y1, float x2, float y2)
    {
        return (x2-x0) * (y1-y0) - (y2-y0) * (x1-x0) < 0.0f;
    }

    bool IsCounterClockWise(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return IsCounterClockWise(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y);
    }

    /// 選択頂点を描画する。
    void DrawSelectedVertex(Figure fig)
    {
        if (selected_vertex == null)
            return;

        Matrix[] clipped_boneMatrices = fig.ClipBoneMatrices(SelectedSubMesh);

        Rectangle rect = new Rectangle(8, 0, 7, 7);//green
        Vector3 rect_center = new Vector3(3, 3, 0);

        switch (vertex_selection_mode)
        {
            case VertexSelectionMode.AllVertices:
            case VertexSelectionMode.CcwVertices:
                {
                    sprite.Begin(SpriteFlags.None);
                    {
                        Vector3 p1 = selected_vertex.CalcSkindeformPosition(clipped_boneMatrices);
                        Vector3 p2 = WorldToScreen(p1);
                        p2.Z = 0.0f;
                        sprite.Draw(dot_texture, rect, rect_center, p2, Color.White);
                    }
                    sprite.End();
                }
                break;
            case VertexSelectionMode.None:
                break;
        }
    }

    /// 選択ボーンに対応するウェイトを加算します。
    public void GainSkinWeight()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (SelectedMesh != null && SelectedVertex != null)
            {
                Vector3 center = SelectedVertex.CalcSkindeformPosition(fig.ClipBoneMatrices(SelectedSubMesh));
                MeshCommand mesh_command = new MeshCommand(fig, SelectedMesh, SelectedNode, weight, WeightOperation.Gain, center, radius);
                Execute(mesh_command);
            }
        }
    }

    /// 選択ボーンに対応するウェイトを減算します。
    public void ReduceSkinWeight()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (SelectedMesh != null && SelectedVertex != null)
            {
                Vector3 center = SelectedVertex.CalcSkindeformPosition(fig.ClipBoneMatrices(SelectedSubMesh));
                MeshCommand mesh_command = new MeshCommand(fig, SelectedMesh, SelectedNode, weight, WeightOperation.Reduce, center, radius);
                Execute(mesh_command);
            }
        }
    }

    /// 選択ボーンに対応するウェイトを代入します。
    public void AssignSkinWeight()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (SelectedMesh != null && SelectedVertex != null)
            {
                Vector3 center = SelectedVertex.CalcSkindeformPosition(fig.ClipBoneMatrices(SelectedSubMesh));
                MeshCommand mesh_command = new MeshCommand(fig, SelectedMesh, SelectedNode, weight, WeightOperation.Assign, center, radius);
                Execute(mesh_command);
            }
        }
    }

    /// 指定操作を実行します。
    public void Execute(ICommand command)
    {
        if (command.Execute())
        {
            if (command_id == commands.Count)
                commands.Add(command);
            else
                commands[command_id] = command;
            command_id++;
        }
    }

    float radius = 0.500f;
    /// 半径
    public float Radius
    {
        get { return radius; }
        set
        {
            radius = value;
            SelectVertices();
        }
    }

    float weight = 0.020f;
    /// 加算ウェイト値
    public float Weight
    {
        get { return weight; }
        set
        {
            weight = value;
        }
    }

    public void SelectVertices()
    {
        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (SelectedMesh != null && SelectedVertex != null)
            {
                Vector3 center = SelectedVertex.CalcSkindeformPosition(fig.ClipBoneMatrices(SelectedSubMesh));

                foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
                {
                    Matrix[] clipped_boneMatrices = fig.ClipBoneMatrices(sub_mesh);

                    for (int i = 0; i < sub_mesh.vertices.Length; i++)
                    {
                        Vertex v = sub_mesh.vertices[i];

                        //頂点間距離が半径未満なら選択する。
                        Vector3 p1 = v.CalcSkindeformPosition(clipped_boneMatrices);
                        v.selected = Vector3.LengthSq(p1 - center) - radius * radius < float.Epsilon;
                    }
                }
            }
        }
    }

    /// 操作リスト
    public List<ICommand> commands = new List<ICommand>();
    int command_id = 0;

    /// 操作を消去します。
    public void ClearCommands()
    {
        commands.Clear();
        command_id = 0;
    }

    /// ひとつ前の操作による変更を元に戻せるか。
    public bool CanUndo()
    {
        return (command_id > 0);
    }

    /// ひとつ前の操作による変更を元に戻します。
    public void Undo()
    {
        if (!CanUndo())
            return;

        command_id--;
        Undo(commands[command_id]);
    }

    /// 指定操作による変更を元に戻します。
    public void Undo(ICommand command)
    {
        command.Undo();
    }

    /// ひとつ前の操作による変更をやり直せるか。
    public bool CanRedo()
    {
        return (command_id < commands.Count);
    }

    /// ひとつ前の操作による変更をやり直します。
    public void Redo()
    {
        if (!CanRedo())
            return;

        Redo(commands[command_id]);
        command_id++;
    }

    /// 指定操作による変更をやり直します。
    public void Redo(ICommand command)
    {
        command.Redo();
    }

    /// マウスボタンを押したときに実行するハンドラ
    protected override void form_OnMouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
        case MouseButtons.Left:
            if (Control.ModifierKeys == Keys.Control)
            {
                SetLightDirection(ScreenToOrientation(e.X, e.Y));
                control.Invalidate(false);
            }
            else
                if (!MotionEnabled)
                {
                    if (!SelectVertex())
                        SelectNode();
                    control.Invalidate(false);
                }
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    /// マウスを移動したときに実行するハンドラ
    protected override void form_OnMouseMove(object sender, MouseEventArgs e)
    {
        int dx = e.X - lastScreenPoint.X;
        int dy = e.Y - lastScreenPoint.Y;

        switch (e.Button)
        {
        case MouseButtons.Left:
            if (Control.ModifierKeys == Keys.Control)
                SetLightDirection(ScreenToOrientation(e.X, e.Y));
            else
                Camera.Move(dx, -dy, 0.0f);
            control.Invalidate(false);
            break;
        case MouseButtons.Middle:
            Camera.MoveView(-dx*0.125f, dy*0.125f);
            control.Invalidate(false);
            break;
        case MouseButtons.Right:
            Camera.Move(0.0f, 0.0f, -dy*0.125f);
            control.Invalidate(false);
            break;
        }

        lastScreenPoint.X = e.X;
        lastScreenPoint.Y = e.Y;
    }

    TSOFile selected_tso_file = null;
    TSOMesh selected_mesh = null;
    TSOSubMesh selected_sub_mesh = null;
    TSONode selected_node = null;
    Vertex selected_vertex = null;

    /// 選択TSOファイル
    public TSOFile SelectedTSOFile
    {
        get { return selected_tso_file; }
        set
        {
            selected_tso_file = value;
            selected_mesh = null;
            selected_sub_mesh = null;
            selected_vertex = null;
        }
    }

    /// 選択メッシュ
    public TSOMesh SelectedMesh
    {
        get { return selected_mesh; }
        set
        {
            selected_mesh = value;
            selected_sub_mesh = null;
            selected_vertex = null;
        }
    }

    /// 選択サブメッシュ
    public TSOSubMesh SelectedSubMesh
    {
        get { return selected_sub_mesh; }
        set
        {
            selected_sub_mesh = value;
            selected_vertex = null;
        }
    }

    /// 選択ボーン
    public TSONode SelectedNode
    {
        get { return selected_node; }
        set
        {
            selected_node = value;
        }
    }

    /// 選択頂点
    public Vertex SelectedVertex
    {
        get { return selected_vertex; }
        set
        {
            selected_vertex = value;
            SelectVertices();
        }
    }

    /// <summary>
    /// node選択時に呼び出されるハンドラ
    /// </summary>
    public event EventHandler SelectedNodeChanged;

    /// nodeを選択します。
    /// returns: nodeを見つけたかどうか
    public bool SelectNode()
    {
        bool found = false;

        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (SelectedTSOFile != null)
            {
                //スクリーン座標からnodeを見つけます。
                //衝突する頂点の中で最も近い位置にあるnodeを返します。

                float x = lastScreenPoint.X;
                float y = lastScreenPoint.Y;

                int width = 5;//頂点ハンドルの幅
                float min_z = 1e12f;

                TSONode found_node = null;

                foreach (TSONode node in SelectedTSOFile.nodes)
                {
                    TMONode bone;
                    if (fig.nodemap.TryGetValue(node, out bone))
                    {
                        Vector3 p2 = GetNodePositionOnScreen(bone);
                        if (p2.X - width <= x && x <= p2.X + width && p2.Y - width <= y && y <= p2.Y + width)
                        {
                            if (p2.Z < min_z)
                            {
                                min_z = p2.Z;
                                found = true;
                                found_node = node;
                            }
                        }
                    }
                }

                if (found)
                {
                    selected_node = found_node;
                    if (SelectedNodeChanged != null)
                        SelectedNodeChanged(this, EventArgs.Empty);
                }
            }
        }
        return found;
    }

    /// <summary>
    /// 頂点選択時に呼び出されるハンドラ
    /// </summary>
    public event EventHandler SelectedVertexChanged;

    /// 頂点を選択します。
    /// returns: 頂点を見つけたかどうか
    public bool SelectVertex()
    {
        bool found = false;

        Figure fig;
        if (TryGetFigure(out fig))
        {
            if (SelectedMesh != null)
            {
                //スクリーン座標から頂点を見つけます。
                //衝突する頂点の中で最も近い位置にある頂点を返します。

                float x = lastScreenPoint.X;
                float y = lastScreenPoint.Y;

                int width = 3;//頂点ハンドルの幅
                float min_z = 1e12f;

                TSOSubMesh found_sub_mesh = null;
                Vertex found_vertex = null;

    foreach (TSOSubMesh sub_mesh in SelectedMesh.sub_meshes)
    {
        Matrix[] clipped_boneMatrices = fig.ClipBoneMatrices(sub_mesh);

        Vector3[] view_positions = new Vector3[sub_mesh.vertices.Length];
        Vector3[] screen_positions = new Vector3[sub_mesh.vertices.Length];
        for (int i = 0; i < sub_mesh.vertices.Length; i++)
        {
            Vector3 p1 = sub_mesh.vertices[i].CalcSkindeformPosition(clipped_boneMatrices);
            view_positions[i] = Vector3.TransformCoordinate(p1, Transform_View);
            screen_positions[i] = WorldToScreen(p1);
        }
        switch (vertex_selection_mode)
        {
            case VertexSelectionMode.AllVertices:
                {
                    for (int i = 0; i < sub_mesh.vertices.Length; i++)
                    {
                        Vector3 p2 = screen_positions[i];
                        if (p2.X - width <= x && x <= p2.X + width && p2.Y - width <= y && y <= p2.Y + width)
                        {
                            if (p2.Z < min_z)
                            {
                                min_z = p2.Z;
                                found = true;
                                found_sub_mesh = sub_mesh;
                                found_vertex = sub_mesh.vertices[i];
                            }
                        }
                    }
                }
                break;
            case VertexSelectionMode.CcwVertices:
                {
                    bool[] ccws = CreateCcws(view_positions);

                    for (int i = 0; i < sub_mesh.vertices.Length; i++)
                    {
                        if (!ccws[i])
                            continue;

                        Vector3 p2 = screen_positions[i];
                        if (p2.X - width <= x && x <= p2.X + width && p2.Y - width <= y && y <= p2.Y + width)
                        {
                            if (p2.Z < min_z)
                            {
                                min_z = p2.Z;
                                found = true;
                                found_sub_mesh = sub_mesh;
                                found_vertex = sub_mesh.vertices[i];
                            }
                        }
                    }
                }
                break;
            case VertexSelectionMode.None:
                break;
        }
    }

                //

                if (found)
                {
                    selected_sub_mesh = found_sub_mesh;
                    selected_vertex = found_vertex;
                    SelectVertices();
                    if (SelectedVertexChanged != null)
                        SelectedVertexChanged(this, EventArgs.Empty);
                }
            }
        }
        return found;
    }

    private bool[] CreateCcws(Vector3[] view_positions)
    {
        bool[] ccws = new bool[view_positions.Length];
        for (int i = 2; i < view_positions.Length; i++)
        {
            ccws[i] = false;
        }
        for (int i = 2; i < view_positions.Length; i++)
        {
            int a, b, c;
            if (i % 2 != 0)
            {
                a = i - 0;
                b = i - 1;
                c = i - 2;
            }
            else
            {
                a = i - 2;
                b = i - 1;
                c = i - 0;
            }
            ccws[i] = ccws[i] || IsCounterClockWise(view_positions[a], view_positions[b], view_positions[c]);
        }
        ccws[0] = ccws[2];
        ccws[1] = ccws[2];
        return ccws;
    }

    NodeCommand node_command = null;

    /// node操作を開始します。
    public void BeginNodeCommand()
    {
        if (SelectedNode == null)
            return;

        Figure fig;
        if (TryGetFigure(out fig))
        {
            Debug.Assert(fig.Tmo.nodemap != null, "fig.Tmo.nodemap should not be null");
            TMONode bone;
            if (fig.nodemap.TryGetValue(SelectedNode, out bone))
            {
                node_command = new NodeCommand(fig, bone);
            }
        }
    }

    /// nodeを操作中であるか。
    public bool HasNodeCommand()
    {
        return node_command != null;
    }

    /// node操作を終了します。
    public void EndNodeCommand()
    {
        if (node_command != null)
            Execute(node_command);

        node_command = null;
    }

    /// 選択nodeをX軸方向に移動します。
    public void TranslateXOnScreen(int dx, int dy)
    {
        if (node_command != null)
        {
            TMONode bone = node_command.node;
            {
                float len = dx * 0.005f;
                Matrix m = bone.RotationMatrix;
                Vector3 axis = new Vector3(m.M11, m.M12, m.M13);
                bone.Translation = new Vector3(axis.X * len, axis.Y * len, axis.Z * len) + bone.Translation;
            }
            node_command.fig.UpdateBoneMatricesWithoutTMOFrame();
        }
    }

    /// 選択nodeをY軸方向に移動します。
    public void TranslateYOnScreen(int dx, int dy)
    {
        if (node_command != null)
        {
            TMONode bone = node_command.node;
            {
                float len = dx * 0.005f;
                Matrix m = bone.RotationMatrix;
                Vector3 axis = new Vector3(m.M21, m.M22, m.M23);
                bone.Translation = new Vector3(axis.X * len, axis.Y * len, axis.Z * len) + bone.Translation;
            }
            node_command.fig.UpdateBoneMatricesWithoutTMOFrame();
        }
    }

    /// 選択nodeをZ軸方向に移動します。
    public void TranslateZOnScreen(int dx, int dy)
    {
        if (node_command != null)
        {
            TMONode bone = node_command.node;
            {
                float len = dx * 0.005f;
                Matrix m = bone.RotationMatrix;
                Vector3 axis = new Vector3(m.M31, m.M32, m.M33);
                bone.Translation = new Vector3(axis.X * len, axis.Y * len, axis.Z * len) + bone.Translation;
            }
            node_command.fig.UpdateBoneMatricesWithoutTMOFrame();
        }
    }

    /// 選択nodeを指定軸中心に回転します。
    public void RotateAxisOnScreen(int dx, int dy, Vector3 axis)
    {
        if (node_command != null)
        {
            TMONode bone = node_command.node;
            {
                float angle = dx * 0.005f;
                bone.Rotation = Quaternion.RotationAxis(axis, angle) * bone.Rotation;
            }
            node_command.fig.UpdateBoneMatricesWithoutTMOFrame();
        }
    }

    /// 選択nodeをX軸中心に回転します。
    public void RotateXOnScreen(int dx, int dy)
    {
        RotateAxisOnScreen(dx, dy, new Vector3(1, 0, 0));
    }

    /// 選択nodeをY軸中心に回転します。
    public void RotateYOnScreen(int dx, int dy)
    {
        RotateAxisOnScreen(dx, dy, new Vector3(0, 1, 0));
    }

    /// 選択nodeをZ軸中心に回転します。
    public void RotateZOnScreen(int dx, int dy)
    {
        RotateAxisOnScreen(dx, dy, new Vector3(0, 0, 1));
    }

    /// <summary>
    /// 内部objectを破棄します。
    /// </summary>
    public new void Dispose()
    {
        if (dot_texture != null)
            dot_texture.Dispose();
        if (sphere != null)
            sphere.Dispose();
        base.Dispose();
    }
}
}
