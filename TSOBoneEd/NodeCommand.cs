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
    /// ����������܂��B
    public interface ICommand
    {
        /// ���ɖ߂��B
        void Undo();

        /// ��蒼���B
        void Redo();

        /// ���s����B
        bool Execute();
    }

    /// node����
    public struct NodeAttr
    {
        /// ��]
        public Quaternion rotation;
        /// �ړ�
        public Vector3 translation;
    }

    /// node����
    public class NodeCommand : ICommand
    {
        //����Ώ�node
        TSONode node = null;
        /// �ύX�O�̑���
        NodeAttr old_attr;
        /// �ύX��̑���
        NodeAttr new_attr;

        Figure fig = null;

        /// node����𐶐����܂��B
        public NodeCommand(Figure fig, TSONode node)
        {
            this.fig = fig;
            this.node = node;
            this.old_attr.rotation = node.Rotation;
            this.old_attr.translation = node.Translation;
        }

        /// ���ɖ߂��B
        public void Undo()
        {
            node.Rotation = old_attr.rotation;
            node.Translation = old_attr.translation;
            fig.UpdateBoneMatricesWithoutTMOFrame();
        }

        /// ��蒼���B
        public void Redo()
        {
            node.Rotation = new_attr.rotation;
            node.Translation = new_attr.translation;
            fig.UpdateBoneMatricesWithoutTMOFrame();
        }

        /// ���s����B
        public bool Execute()
        {
            this.new_attr.rotation = node.Rotation;
            this.new_attr.translation = node.Translation;
            bool updated = old_attr.rotation != new_attr.rotation || old_attr.translation != new_attr.translation;
            return updated;
        }
    }
}
