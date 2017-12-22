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
    /// 統合メッシュ操作
    public class SmoothMqoMeshCommand
    {
        //操作対象統合メッシュ
        MqoMesh mqo_mesh = null;
        //頂点操作リスト
        List<VertexCommand> vertex_commands = new List<VertexCommand>();

        TSOFile tso = null;
        TSONode selected_node = null;

        float[] weits;
        float[] new_weits;

        /// 統合メッシュ操作を生成します。
        public SmoothMqoMeshCommand(TSOFile tso, MqoMesh mqo_mesh, TSONode selected_node)
        {
            this.tso = tso;
            this.mqo_mesh = mqo_mesh;
            this.selected_node = selected_node;

            weits = new float[mqo_mesh.vertices.Length];
            new_weits = new float[mqo_mesh.vertices.Length];
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

        //選択ボーンに対応するウェイトを検索して保持する。
        public void FindWeight(MqoVert v)
        {
            foreach (MqoSkinWeight skin_weight in v.skin_weights)
            {
                if (skin_weight.bone == selected_node)
                {
                    weits[v.Index] = skin_weight.weight;
                    break;
                }
            }
        }

        //選択ボーンに対応するウェイトを補間して保持する。
        public void SmoothWeight(MqoVert v)
        {
            float total_weit = 0;
            float total = 0;

            foreach (MqoEdge edge in mqo_mesh.graph.edges[v.Index])
            {
                float weit = weits[edge.To];
                float factor = 1.0f / edge.Cost;
                total_weit += weit * factor;
                total += factor;
            }

            float new_weit = total_weit / total;
            float cur_weit = weits[v.Index];
            new_weits[v.Index] = ( new_weit + cur_weit ) * 0.5f;
        }

        public bool Execute()
        {
            foreach (MqoVert v in mqo_mesh.vertices)
            {
                FindWeight(v);
            }
            foreach (MqoVert v in mqo_mesh.vertices)
            {
                if (v.selected)
                    SmoothWeight(v);
            }
            bool updated = false;

            foreach (MqoVert v in mqo_mesh.vertices)
            {
                if (v.selected)
                {
                    VertexCommand vertex_command = new VertexCommand(tso, selected_node, v, new_weits[v.Index], WeightOperation.Assign);
                    
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
    public class SmoothMeshCommand : ICommand
    {
        //操作対象メッシュ
        TSOMesh mesh = null;
        //統合メッシュ操作リスト
        List<SmoothMqoMeshCommand> mqo_mesh_commands = new List<SmoothMqoMeshCommand>();

        TSOFile tso = null;
        TSONode selected_node = null;

        /// メッシュ操作を生成します。
        public SmoothMeshCommand(TSOFile tso, TSOMesh mesh, TSONode selected_node)
        {
            this.tso = tso;
            this.mesh = mesh;
            this.selected_node = selected_node;
        }

        /// 変更を元に戻す。
        public void Undo()
        {
            foreach (SmoothMqoMeshCommand mqo_mesh_command in this.mqo_mesh_commands)
            {
                mqo_mesh_command.Undo();
            }
        }

        /// 変更をやり直す。
        public void Redo()
        {
            foreach (SmoothMqoMeshCommand mqo_mesh_command in this.mqo_mesh_commands)
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
            SmoothMqoMeshCommand mqo_mesh_command = new SmoothMqoMeshCommand(tso, mqo_mesh, selected_node);

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
