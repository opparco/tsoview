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
    /// 頂点操作
    public class NormalizedVertexCommand
    {
        //操作対象頂点
        MqoVert vertex;
        //スキンウェイト操作リスト
        List<SkinWeightCommand> skin_weight_commands = new List<SkinWeightCommand>();

        TSOFile tso = null;

        /// 頂点操作を生成します。
        public NormalizedVertexCommand(TSOFile tso, MqoVert vertex)
        {
            this.tso = tso;
            this.vertex = vertex;
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

        public bool NormalizedWeight()
        {
            float total_weit = 0;

            foreach (MqoSkinWeight skin_weight in vertex.skin_weights)
            {
                total_weit += skin_weight.weight;
            }

            if (total_weit == 1.0f)
                return false;

            foreach (MqoSkinWeight skin_weight in vertex.skin_weights)
            {
                skin_weight.weight /= total_weit;
            }
            return true;
        }

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

            bool updated = NormalizedWeight();

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
    public class NormalizedMqoMeshCommand
    {
        //操作対象統合メッシュ
        MqoMesh mqo_mesh = null;
        //頂点操作リスト
        List<NormalizedVertexCommand> vertex_commands = new List<NormalizedVertexCommand>();

        TSOFile tso = null;

        /// 統合メッシュ操作を生成します。
        public NormalizedMqoMeshCommand(TSOFile tso, MqoMesh mqo_mesh)
        {
            this.tso = tso;
            this.mqo_mesh = mqo_mesh;
        }

        /// 変更を元に戻す。
        public void Undo()
        {
            foreach (NormalizedVertexCommand vertex_command in this.vertex_commands)
            {
                vertex_command.Undo();
            }
            this.mqo_mesh.WriteBuffer();
        }

        /// 変更をやり直す。
        public void Redo()
        {
            foreach (NormalizedVertexCommand vertex_command in this.vertex_commands)
            {
                vertex_command.Redo();
            }
            this.mqo_mesh.WriteBuffer();
        }

        public bool Execute()
        {
            bool updated = false;

            foreach (MqoVert v in mqo_mesh.vertices)
            {
                NormalizedVertexCommand vertex_command = new NormalizedVertexCommand(tso, v);
                
                if (vertex_command.Execute())
                {
                    v.Update();
                    this.vertex_commands.Add(vertex_command);
                    updated = true;
                }
            }
            return updated;
        }
    }

    /// メッシュ操作
    public class NormalizedMeshCommand : ICommand
    {
        //操作対象メッシュ
        TSOMesh mesh = null;
        //統合メッシュ操作リスト
        List<NormalizedMqoMeshCommand> mqo_mesh_commands = new List<NormalizedMqoMeshCommand>();

        TSOFile tso = null;

        /// メッシュ操作を生成します。
        public NormalizedMeshCommand(TSOFile tso, TSOMesh mesh)
        {
            this.tso = tso;
            this.mesh = mesh;
        }

        /// 変更を元に戻す。
        public void Undo()
        {
            foreach (NormalizedMqoMeshCommand mqo_mesh_command in this.mqo_mesh_commands)
            {
                mqo_mesh_command.Undo();
            }
        }

        /// 変更をやり直す。
        public void Redo()
        {
            foreach (NormalizedMqoMeshCommand mqo_mesh_command in this.mqo_mesh_commands)
            {
                mqo_mesh_command.Redo();
            }
        }

        /// 対象メッシュのウェイトを正規化する。
        /// returns: ウェイトを変更したか
        public bool Execute()
        {
            bool updated = false;

            MqoMesh mqo_mesh = MqoMesh.FromTSOMesh(mesh);
            NormalizedMqoMeshCommand mqo_mesh_command = new NormalizedMqoMeshCommand(tso, mqo_mesh);

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
