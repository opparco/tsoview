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
        TSONode node = null;
        /// 変更前の属性
        NodeAttr old_attr;
        /// 変更後の属性
        NodeAttr new_attr;

        Figure fig = null;

        /// node操作を生成します。
        public NodeCommand(Figure fig, TSONode node)
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
}
