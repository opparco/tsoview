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
        TMONode node = null;
        /// 変更前の属性
        NodeAttr old_attr;
        /// 変更後の属性
        NodeAttr new_attr;

        Figure fig = null;

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
        /// node (bone)
        public TSONode bone;
        /// ウェイト値
        public float weight;
    }

    /// スキンウェイト操作
    public class SkinWeightCommand
    {
        //操作対象スキンウェイト
        MqoSkinWeight skin_weight = null;
        /// 変更前の属性
        public SkinWeightAttr old_attr;
        /// 変更後の属性
        public SkinWeightAttr new_attr;

        /// スキンウェイト操作を生成します。
        public SkinWeightCommand(MqoSkinWeight skin_weight)
        {
            this.skin_weight = skin_weight;
        }

        /// 変更を元に戻す。
        public void Undo()
        {
            skin_weight.bone = old_attr.bone;
            skin_weight.weight = old_attr.weight;
        }

        /// 変更をやり直す。
        public void Redo()
        {
            skin_weight.bone = new_attr.bone;
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
        MqoVert vertex;
        //スキンウェイト操作リスト
        List<SkinWeightCommand> skin_weight_commands = new List<SkinWeightCommand>();

        TSOFile tso = null;
        TSONode selected_node = null;
        float weight;
        WeightOperation weight_op = WeightOperation.None;

        /// 頂点操作を生成します。
        public VertexCommand(TSOFile tso, TSONode selected_node, MqoVert vertex, float weight, WeightOperation weight_op)
        {
            this.tso = tso;
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
            vertex.Update();
        }

        /// 変更をやり直す。
        public void Redo()
        {
            //前提: vertex.skin_weights の各要素は変更の前後で同じインスタンスである
            foreach (SkinWeightCommand skin_weight_command in this.skin_weight_commands)
            {
                skin_weight_command.Redo();
            }
            vertex.Update();
        }

        /// 選択ボーンに対応するウェイトを加算する。
        /// returns: ウェイトを変更したか
        public bool Execute()
        {
            foreach (MqoSkinWeight skin_weight in vertex.skin_weights)
            {
                SkinWeightCommand skin_weight_command = new SkinWeightCommand(skin_weight);
                this.skin_weight_commands.Add(skin_weight_command);
            }
            //処理前の値を記憶する。
            {
                int nskin_weight = 0;
                foreach (MqoSkinWeight skin_weight in vertex.skin_weights)
                {
                    this.skin_weight_commands[nskin_weight].old_attr.bone = skin_weight.bone;
                    this.skin_weight_commands[nskin_weight].old_attr.weight = skin_weight.weight;
                    nskin_weight++;
                }
            }

            bool updated = false;

            //選択ボーンに対応するウェイトを検索する。
            MqoSkinWeight selected_skin_weight = null;
            foreach (MqoSkinWeight skin_weight in vertex.skin_weights)
            {
                if (skin_weight.bone == selected_node)
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
                TSONode bone = selected_node;
                {
                    selected_skin_weight = vertex.skin_weights[3]; //前提: vertex.skin_weights の要素数は 4 かつ並び順はウェイト値の降順
                    selected_skin_weight.bone = bone;
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

                selected_skin_weight.weight = w1;

                Array.Sort(vertex.skin_weights);
            }
#if false
            {
                int nskin_weight = 0;
                foreach (MqoSkinWeight skin_weight in vertex.skin_weights)
                {
                    Console.WriteLine("i:{0} bone:{1} w:{2}", nskin_weight, skin_weight.bone.Name, skin_weight.weight);
                    nskin_weight++;
                }
                Console.WriteLine();
            }
#endif
            //処理後の値を記憶する。
            {
                int nskin_weight = 0;
                foreach (MqoSkinWeight skin_weight in vertex.skin_weights)
                {
                    this.skin_weight_commands[nskin_weight].new_attr.bone = skin_weight.bone;
                    this.skin_weight_commands[nskin_weight].new_attr.weight = skin_weight.weight;
                    nskin_weight++;
                }
            }
            return updated;
        }
    }

    /// 統合メッシュ操作
    public class MqoMeshCommand
    {
        //操作対象統合メッシュ
        MqoMesh mqo_mesh = null;
        //頂点操作リスト
        List<VertexCommand> vertex_commands = new List<VertexCommand>();

        TSOFile tso = null;
        TSONode selected_node = null;
        float weight;
        WeightOperation weight_op;

        /// 統合メッシュ操作を生成します。
        public MqoMeshCommand(TSOFile tso, MqoMesh mqo_mesh, TSONode selected_node, float weight, WeightOperation weight_op)
        {
            this.tso = tso;
            this.mqo_mesh = mqo_mesh;
            this.selected_node = selected_node;
            this.weight = weight;
            this.weight_op = weight_op;
        }

        /// 変更を元に戻す。
        public void Undo()
        {
            foreach (VertexCommand vertex_command in this.vertex_commands)
            {
                vertex_command.Undo();
            }
            this.mqo_mesh.WriteBuffer();
        }

        /// 変更をやり直す。
        public void Redo()
        {
            foreach (VertexCommand vertex_command in this.vertex_commands)
            {
                vertex_command.Redo();
            }
            this.mqo_mesh.WriteBuffer();
        }

        /// 選択ボーンに対応するウェイトを加算する。
        /// returns: ウェイトを変更したか
        public bool Execute()
        {
            bool updated = false;

            foreach (MqoVert v in mqo_mesh.vertices)
            {
                if (v.selected)
                {
                    VertexCommand vertex_command = new VertexCommand(tso, selected_node, v, weight * v.factor, weight_op);
                    
                    if (vertex_command.Execute())
                    {
                        v.Update();
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
        //統合メッシュ操作リスト
        List<MqoMeshCommand> mqo_mesh_commands = new List<MqoMeshCommand>();

        TSOFile tso = null;
        TSONode selected_node = null;
        float weight;
        WeightOperation weight_op;

        /// メッシュ操作を生成します。
        public MeshCommand(TSOFile tso, TSOMesh mesh, TSONode selected_node, float weight, WeightOperation weight_op)
        {
            this.tso = tso;
            this.mesh = mesh;
            this.selected_node = selected_node;
            this.weight = weight;
            this.weight_op = weight_op;
        }

        /// 変更を元に戻す。
        public void Undo()
        {
            foreach (MqoMeshCommand mqo_mesh_command in this.mqo_mesh_commands)
            {
                mqo_mesh_command.Undo();
            }
        }

        /// 変更をやり直す。
        public void Redo()
        {
            foreach (MqoMeshCommand mqo_mesh_command in this.mqo_mesh_commands)
            {
                mqo_mesh_command.Redo();
            }
        }

        /// 選択ボーンに対応するウェイトを加算する。
        /// returns: ウェイトを変更したか
        public bool Execute()
        {
            bool updated = false;

            MqoMesh mqo_mesh = MqoMesh.FromTSOMesh(mesh);
            MqoMeshCommand mqo_mesh_command = new MqoMeshCommand(tso, mqo_mesh, selected_node, weight, weight_op);

            if (mqo_mesh_command.Execute())
            {
                mqo_mesh.WriteBuffer();
                this.mqo_mesh_commands.Add(mqo_mesh_command);
                updated = true;
            }
            return updated;
        }
    }
}
