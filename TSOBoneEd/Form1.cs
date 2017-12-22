using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOBoneEd
{
    public partial class Form1 : Form
    {
        public WeightViewer viewer = null;

        string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";

        public Form1(TSOConfig tso_config, string[] args)
        {
            InitializeComponent();
            this.ClientSize = tso_config.ClientSize;

            this.viewer = new WeightViewer();
            viewer.ScreenColor = tso_config.ScreenColor;

            if (viewer.InitializeApplication(this))
            {
                viewer.FigureEvent += delegate(object sender, EventArgs e)
                {
                    Figure fig;
                    if (viewer.TryGetFigure(out fig))
                    {
                        AssignTSOFiles(fig);
                    }
                    else
                    {
                        viewer.ClearCommands();
                    }
                };
                viewer.SelectedNodeChanged += delegate(object sender, EventArgs e)
                {
                    UpdateSelectedNodeControls();
                };
                foreach (string arg in args)
                    viewer.LoadAnyFile(arg, true);
                if (viewer.FigureList.Count == 0)
                    viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);

                //this.timer1.Enabled = true;
            }
        }

        public void UpdateSelectedNodeControls()
        {
            TSONode node = viewer.SelectedNode;

            if (node == null)
            {
                ClearNodeControls();
                return;
            }

            lbNodeName.Text = node.Name;

            Vector3 local = node.Translation;
            UpdateNodeLocalPos(local);

            Vector3 world = node.GetWorldPosition();
            UpdateNodePos(world);

            local = Vector3.TransformCoordinate(world, node.offset_matrix);
            UpdateNodeSub(local);
        }

        void ClearNodeControls()
        {
            lbNodeName.Text = "node name";

            edNodeLocalPosX.Text = "";
            edNodeLocalPosY.Text = "";
            edNodeLocalPosZ.Text = "";

            edNodePosX.Text = "";
            edNodePosY.Text = "";
            edNodePosZ.Text = "";

            lbNodeSubX.Text = "x";
            lbNodeSubY.Text = "y";
            lbNodeSubZ.Text = "z";
        }

        public void UpdateSelectedNodePos()
        {
            TSONode node = viewer.SelectedNode;

            Vector3 world = node.GetWorldPosition();
            UpdateNodePos(world);
        }

        void UpdateNodePos(Vector3 world)
        {
            edNodePosX.Text = string.Format("{0:F3}", world.X);
            edNodePosY.Text = string.Format("{0:F3}", world.Y);
            edNodePosZ.Text = string.Format("{0:F3}", world.Z);
        }

        public void UpdateSelectedNodeSub()
        {
            TSONode node = viewer.SelectedNode;

            Vector3 world = node.GetWorldPosition();

            Vector3 local = Vector3.TransformCoordinate(world, node.offset_matrix);
            UpdateNodeSub(local);
        }

        void UpdateNodeSub(Vector3 local)
        {
            lbNodeSubX.Text = string.Format("{0:F3}", local.X);
            lbNodeSubY.Text = string.Format("{0:F3}", local.Y);
            lbNodeSubZ.Text = string.Format("{0:F3}", local.Z);
        }

        public void UpdateSelectedNodeLocalPos()
        {
            TSONode node = viewer.SelectedNode;

            Vector3 local = node.Translation;
            UpdateNodeLocalPos(local);
        }

        void UpdateNodeLocalPos(Vector3 local)
        {
            edNodeLocalPosX.Text = string.Format("{0:F3}", local.X);
            edNodeLocalPosY.Text = string.Format("{0:F3}", local.Y);
            edNodeLocalPosZ.Text = string.Format("{0:F3}", local.Z);
        }

        void AssignTSOFiles(Figure fig)
        {
            lvTSOFiles.BeginUpdate();
            lvTSOFiles.Items.Clear();
            for (int i = 0; i < fig.TSOList.Count; i++)
            {
                TSOFile tso = fig.TSOList[i];
                ListViewItem li = new ListViewItem(tso.FileName ?? "TSO #" + i.ToString());
                li.Tag = tso;
                lvTSOFiles.Items.Add(li);
            }
            lvTSOFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvTSOFiles.EndUpdate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewer.FrameMove();
            viewer.Render();
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if ((e.KeyState & 8) == 8)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (string src in (string[])e.Data.GetData(DataFormats.FileDrop))
                    viewer.LoadAnyFile(src, (e.KeyState & 8) == 8);
                Invalidate(false);
            }
        }

        private void lvTSOFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvTSOFiles.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvTSOFiles.SelectedItems[0];
            TSOFile tso = li.Tag as TSOFile;

            viewer.SelectedTSOFile = tso;
            Invalidate(false);
        }

        private void SaveFigure()
        {
            if (lvTSOFiles.SelectedIndices.Count == 0)
                return;

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                int index = lvTSOFiles.SelectedIndices[0];
                TSOFile tso = fig.TSOList[index];
                dialog.FileName = tso.FileName;
                dialog.Filter = "tso files|*.tso";
                dialog.FilterIndex = 0;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string dest_file = dialog.FileName;
                    string extension = Path.GetExtension(dest_file);
                    if (extension == ".tso")
                    {
                        tso.Save(dest_file);
                    }
                }
            }
        }

        private void editUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Undo();
            UpdateSelectedNodeControls();
            Invalidate(false);
        }

        private void editRedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Redo();
            UpdateSelectedNodeControls();
            Invalidate(false);
        }
        
        private void editResetPoseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                fig.UpdateBoneMatrices(true);
            }
            Invalidate(false);
        }

        private void fileSaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFigure();
        }

        private void fileExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void fileNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.ClearFigureList();
            Invalidate(false);
        }

        private void fileOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFigure();
            Invalidate(false);
        }

        private void LoadFigure()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "png files|*.png|tso files|*.tso";
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string source_file = dialog.FileName;
                viewer.LoadAnyFile(source_file);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            viewer.FrameMove();
            viewer.Render();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        private void cameraResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Camera.Reset();
            viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);
            Invalidate(false);
        }

        private void cameraSelectedVertexToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void cameraSelectedBoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TSONode node = viewer.SelectedNode;

            if (node == null)
                return;

            viewer.Camera.Center = node.GetWorldPosition();
            viewer.Camera.ResetTranslation();
            Invalidate(false);
        }

        private void toonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.mesh_view_mode = WeightViewer.MeshViewMode.Toon;
            Invalidate(false);
        }

        private void heatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.mesh_view_mode = WeightViewer.MeshViewMode.Heat;
            Invalidate(false);
        }

        private void wireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.mesh_view_mode = WeightViewer.MeshViewMode.Wire;
            Invalidate(false);
        }

        private void meshAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.mesh_selection_mode = WeightViewer.MeshSelectionMode.AllMeshes;
            Invalidate(false);
        }

        private void meshSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.mesh_selection_mode = WeightViewer.MeshSelectionMode.SelectedMesh;
            Invalidate(false);
        }

        private void vertexAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            viewer.vertex_selection_mode = WeightViewer.VertexSelectionMode.AllVertices;
            Invalidate(false);
        }

        private void vertexCcwToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.vertex_selection_mode = WeightViewer.VertexSelectionMode.CcwVertices;
            Invalidate(false);
        }

        private void vertexNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.vertex_selection_mode = WeightViewer.VertexSelectionMode.None;
            Invalidate(false);
        }

        private void boneAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.node_selection_mode = WeightViewer.NodeSelectionMode.AllBones;
            Invalidate(false);
        }

        private void boneNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.node_selection_mode = WeightViewer.NodeSelectionMode.None;
            Invalidate(false);
        }

        private void cameraXaxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float angle = Geometry.DegreeToRadian(90.0f);
            viewer.Camera.SetRotationEuler(0.0f, +angle, 0.0f);
            Invalidate(false);
        }

        private void cameraYaxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float angle = Geometry.DegreeToRadian(90.0f);
            viewer.Camera.SetRotationEuler(-angle, 0.0f, 0.0f);
            Invalidate(false);
        }

        private void cameraZaxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float angle = Geometry.DegreeToRadian(90.0f);
            viewer.Camera.SetRotationEuler(0.0f, 0.0f, 0.0f);
            Invalidate(false);
        }

        private void caminvXaxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float angle = Geometry.DegreeToRadian(90.0f);
            viewer.Camera.SetRotationEuler(0.0f, -angle, 0.0f);
            Invalidate(false);
        }

        private void caminvYaxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float angle = Geometry.DegreeToRadian(90.0f);
            viewer.Camera.SetRotationEuler(+angle, 0.0f, 0.0f);
            Invalidate(false);
        }

        private void caminvZaxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float angle = Geometry.DegreeToRadian(180.0f);
            viewer.Camera.SetRotationEuler(0.0f, +angle, 0.0f);
            Invalidate(false);
        }

        private void btnPosUpdate_Click(object sender, EventArgs e)
        {
            TSONode node = viewer.SelectedNode;

            if (node == null)
                return;

            float x1 = float.Parse(edNodePosX.Text);
            float y1 = float.Parse(edNodePosY.Text);
            float z1 = float.Parse(edNodePosZ.Text);

            Vector3 p1 = new Vector3(x1, y1, z1);

            viewer.BeginNodeCommand();

            Matrix m = Matrix.Identity;
            if (node.parent != null)
                m = Matrix.Invert(node.parent.GetWorldCoordinate());
            Vector3 local = Vector3.TransformCoordinate(p1, m);
            node.Translation = local;

            viewer.EndNodeCommand();

            UpdateSelectedNodeLocalPos();
            UpdateSelectedNodeSub();
            Invalidate(false);
        }

        private void btnLocalPosUpdate_Click(object sender, EventArgs e)
        {
            TSONode node = viewer.SelectedNode;

            if (node == null)
                return;

            float x1 = float.Parse(edNodeLocalPosX.Text);
            float y1 = float.Parse(edNodeLocalPosY.Text);
            float z1 = float.Parse(edNodeLocalPosZ.Text);

            Vector3 p1 = new Vector3(x1, y1, z1);

            viewer.BeginNodeCommand();

            node.Translation = p1;

            viewer.EndNodeCommand();

            UpdateSelectedNodePos();
            UpdateSelectedNodeSub();
            Invalidate(false);
        }
    }
}
