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

        static WeightViewer()
        {
            Figure.SliderMatrixEnabled = false;
        }

        /// <summary>
        /// viewerを生成します。
        /// </summary>
        public WeightViewer()
        {
            this.Rendering += delegate ()
            {
                RenderDerived();
            };
            TSONodeLineColor = Color.FromArgb(100, 100, 230); //from MikuMikuDance
            SelectedTSONodeLineColor = Color.FromArgb(255, 0, 0); //red
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
            if (!base.InitializeApplication(control))
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

            if (node_selection_mode == NodeSelectionMode.AllBones)
            {
                //nodeを描画する。
                DrawNodeTree();
                DrawSelectedNode();
            }
        }

        bool HiddenNode(TSONode node)
        {
            Vector3 position = node.GetWorldPosition();
            Vector3 view_position = Vector3.TransformCoordinate(position, Transform_View);
            return view_position.Z > 0.0f;
        }

        Vector3 GetNodePositionOnScreen(TSONode node)
        {
            Vector3 p1 = node.GetWorldPosition();
            Vector3 p2 = WorldToScreen(p1);
            return p2;
        }

        /// node line描画色
        public Color TSONodeLineColor { get; set; }

        /// <summary>
        /// フィギュアに含まれるnode treeを描画する。
        /// </summary>
        void DrawNodeTree()
        {
            TSOFile tso = selected_tso_file;

            if (tso == null)
                return;

            Line line = new Line(device);
            foreach (TSONode node in tso.nodes)
            {
                if (HiddenNode(node))
                    continue;
                Vector3 p0 = GetNodePositionOnScreen(node);
                p0.Z = 0.0f;

                TSONode parent_node = node.parent;
                if (parent_node != null)
                {
                    if (HiddenNode(parent_node))
                        continue;
                    Vector3 p1 = GetNodePositionOnScreen(parent_node);
                    p1.Z = 0.0f;

                    Vector3 pd = p0 - p1;
                    float len = Vector3.Length(pd);
                    float scale = 4.0f / len;
                    Vector2 p3 = new Vector2(p1.X + pd.Y * scale, p1.Y - pd.X * scale);
                    Vector2 p4 = new Vector2(p1.X - pd.Y * scale, p1.Y + pd.X * scale);

                    Vector2[] vertices = new Vector2[3];
                    vertices[0] = new Vector2(p3.X, p3.Y);
                    vertices[1] = new Vector2(p0.X, p0.Y);
                    vertices[2] = new Vector2(p4.X, p4.Y);
                    line.Draw(vertices, TSONodeLineColor);
                }
            }
            line.Dispose();
            line = null;

            Rectangle rect = new Rectangle(0, 16, 15, 15); //node circle
            Vector3 rect_center = new Vector3(7, 7, 0);
            sprite.Begin(SpriteFlags.None);
            foreach (TSONode node in tso.nodes)
            {
                if (HiddenNode(node))
                    continue;
                Vector3 p0 = GetNodePositionOnScreen(node);
                p0.Z = 0.0f;
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
        static Vector3 GetMatrixDirX(ref Matrix m)
        {
            return new Vector3(m.M11, m.M12, m.M13);
        }

        /// 指定行列のY軸先位置を得ます。
        static Vector3 GetMatrixDirY(ref Matrix m)
        {
            return new Vector3(m.M21, m.M22, m.M23);
        }

        /// 指定行列のZ軸先位置を得ます。
        static Vector3 GetMatrixDirZ(ref Matrix m)
        {
            return new Vector3(m.M31, m.M32, m.M33);
        }

        /// 選択node line描画色
        public Color SelectedTSONodeLineColor { get; set; }

        /// 選択nodeを描画する。
        void DrawSelectedNode()
        {
            if (selected_node == null)
                return;

            if (HiddenNode(selected_node))
                return;
            Vector3 p1 = GetNodePositionOnScreen(selected_node);
            p1.Z = 0.0f;

            if (selected_node.children.Count != 0)
            {
                Line line = new Line(device);

                Vector3 p0 = GetNodePositionOnScreen(selected_node.children[0]);
                p0.Z = 0.0f;

                Vector3 pd = p0 - p1;
                float len = Vector3.Length(pd);
                float scale = 4.0f / len;
                Vector2 p3 = new Vector2(p1.X + pd.Y * scale, p1.Y - pd.X * scale);
                Vector2 p4 = new Vector2(p1.X - pd.Y * scale, p1.Y + pd.X * scale);

                Vector2[] vertices = new Vector2[3];
                vertices[0] = new Vector2(p3.X, p3.Y);
                vertices[1] = new Vector2(p0.X, p0.Y);
                vertices[2] = new Vector2(p4.X, p4.Y);
                line.Draw(vertices, SelectedTSONodeLineColor);

                line.Dispose();
                line = null;
            }

            DrawNodeAxis(selected_node);

            Rectangle rect = new Rectangle(16, 16, 15, 15); //node circle
            Vector3 rect_center = new Vector3(7, 7, 0);
            sprite.Begin(SpriteFlags.None);
            sprite.Draw(dot_texture, rect, rect_center, p1, Color.White);
            sprite.End();
        }

        void DrawNodeAxis(TSONode node)
        {
            Vector3 p1 = GetNodePositionOnScreen(node);
            p1.Z = 0.0f;

            Vector3 position = node.GetWorldPosition();
            Matrix m = node.GetWorldCoordinate();
            Vector3 dirx = GetMatrixDirX(ref m);
            Vector3 diry = GetMatrixDirY(ref m);
            Vector3 dirz = GetMatrixDirZ(ref m);

            Vector3 view_position_dirx = Vector3.TransformCoordinate(position + dirx, Transform_View);
            Vector3 view_position_diry = Vector3.TransformCoordinate(position + diry, Transform_View);
            Vector3 view_position_dirz = Vector3.TransformCoordinate(position + dirz, Transform_View);

            bool hidden_dirx = view_position_dirx.Z > 0.0f;
            bool hidden_diry = view_position_diry.Z > 0.0f;
            bool hidden_dirz = view_position_dirz.Z > 0.0f;

            Line line = new Line(device);
            line.Width = 3;

            Vector2[] vertices = new Vector2[2];
            vertices[0] = new Vector2(p1.X, p1.Y);
            if (!hidden_dirx)
            {
                Vector3 px = WorldToScreen(position + dirx);
                vertices[1] = new Vector2(px.X, px.Y);
                line.Draw(vertices, Color.FromArgb(255, 0, 0));//R
            }
            if (!hidden_diry)
            {
                Vector3 py = WorldToScreen(position + diry);
                vertices[1] = new Vector2(py.X, py.Y);
                line.Draw(vertices, Color.FromArgb(0, 255, 0));//G
            }
            if (!hidden_dirz)
            {
                Vector3 pz = WorldToScreen(position + dirz);
                vertices[1] = new Vector2(pz.X, pz.Y);
                line.Draw(vertices, Color.FromArgb(0, 0, 255));//B
            }

            line.Dispose();
            line = null;
        }

        /// 操作リスト
        public List<ICommand> commands = new List<ICommand>();
        int command_id = 0;

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
                    Camera.MoveView(-dx * 0.125f, dy * 0.125f);
                    control.Invalidate(false);
                    break;
                case MouseButtons.Right:
                    Camera.Move(0.0f, 0.0f, -dy * 0.125f);
                    control.Invalidate(false);
                    break;
            }

            lastScreenPoint.X = e.X;
            lastScreenPoint.Y = e.Y;
        }

        TSOFile selected_tso_file = null;
        TSONode selected_node = null;

        /// 選択TSOファイル
        public TSOFile SelectedTSOFile
        {
            get { return selected_tso_file; }
            set
            {
                selected_tso_file = value;
                selected_node = null;
                if (SelectedNodeChanged != null)
                    SelectedNodeChanged(this, EventArgs.Empty);
            }
        }

        /// 選択ボーン
        public TSONode SelectedNode
        {
            get { return selected_node; }
            set
            {
                selected_node = value;
                if (SelectedNodeChanged != null)
                    SelectedNodeChanged(this, EventArgs.Empty);
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
                    Vector3 p2 = GetNodePositionOnScreen(node);
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

                if (found)
                {
                    SelectedNode = found_node;
                }
            }
            return found;
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
                TSONode node = SelectedNode;
                node_command = new NodeCommand(fig, node);
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

        /// 選択nodeを指定軸方向に移動します。
        public void TranslateAxisOnScreen(int dx, int dy, Vector3 axis)
        {
            TSONode node = SelectedNode;

            if (node == null)
                return;

            axis = Vector3.TransformNormal(axis, node.RotationMatrix);

            float len = dx * 0.005f;
            node.Translation = new Vector3(axis.X * len, axis.Y * len, axis.Z * len) + node.Translation;
        }

        /// 選択nodeをX軸方向に移動します。
        public void TranslateXOnScreen(int dx, int dy)
        {
            TranslateAxisOnScreen(dx, dy, new Vector3(1, 0, 0));
        }

        /// 選択nodeをY軸方向に移動します。
        public void TranslateYOnScreen(int dx, int dy)
        {
            TranslateAxisOnScreen(dx, dy, new Vector3(0, 1, 0));
        }

        /// 選択nodeをZ軸方向に移動します。
        public void TranslateZOnScreen(int dx, int dy)
        {
            TranslateAxisOnScreen(dx, dy, new Vector3(0, 0, 1));
        }

        /// 選択nodeを指定軸中心に回転します。
        public void RotateAxisOnScreen(int dx, int dy, Vector3 axis)
        {
            TSONode node = SelectedNode;

            if (node == null)
                return;

            float angle = dx * 0.005f;
            node.Rotation = Quaternion.RotationAxis(axis, angle) * node.Rotation;
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
