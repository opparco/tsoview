namespace TSOBoneEd
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                viewer.Dispose();
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.fileSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.filePrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filePreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.fileExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editUndoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editRedoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editResetPoseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraSelectedVertexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraSelectedBoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meshAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meshSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vertexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vertexAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.vertexCcwToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vertexNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boneAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boneNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraXaxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraYaxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraZaxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.helpVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbTSOFiles = new System.Windows.Forms.Label();
            this.lvTSOFiles = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPosUpdate = new System.Windows.Forms.Button();
            this.lbNodeSubZ = new System.Windows.Forms.Label();
            this.lbNodeSubY = new System.Windows.Forms.Label();
            this.lbNodeSubX = new System.Windows.Forms.Label();
            this.edNodePosZ = new System.Windows.Forms.TextBox();
            this.edNodePosY = new System.Windows.Forms.TextBox();
            this.edNodePosX = new System.Windows.Forms.TextBox();
            this.lbNodeName = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.caminvXaxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.caminvYaxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.caminvZaxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLocalPosUpdate = new System.Windows.Forms.Button();
            this.edNodeLocalPosZ = new System.Windows.Forms.TextBox();
            this.edNodeLocalPosY = new System.Windows.Forms.TextBox();
            this.edNodeLocalPosX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.EditToolStripMenuItem,
            this.ViewToolStripMenuItem,
            this.HelpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 26);
            this.menuStrip1.TabIndex = 20;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileNewToolStripMenuItem,
            this.fileOpenToolStripMenuItem,
            this.toolStripSeparator,
            this.fileSaveToolStripMenuItem,
            this.fileSaveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.filePrintToolStripMenuItem,
            this.filePreviewToolStripMenuItem,
            this.toolStripSeparator2,
            this.fileExitToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(85, 22);
            this.FileToolStripMenuItem.Text = "ファイル(&F)";
            // 
            // fileNewToolStripMenuItem
            // 
            this.fileNewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fileNewToolStripMenuItem.Image")));
            this.fileNewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileNewToolStripMenuItem.Name = "fileNewToolStripMenuItem";
            this.fileNewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.fileNewToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.fileNewToolStripMenuItem.Text = "新規作成(&N)";
            this.fileNewToolStripMenuItem.Click += new System.EventHandler(this.fileNewToolStripMenuItem_Click);
            // 
            // fileOpenToolStripMenuItem
            // 
            this.fileOpenToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fileOpenToolStripMenuItem.Image")));
            this.fileOpenToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileOpenToolStripMenuItem.Name = "fileOpenToolStripMenuItem";
            this.fileOpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.fileOpenToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.fileOpenToolStripMenuItem.Text = "開く(&O)";
            this.fileOpenToolStripMenuItem.Click += new System.EventHandler(this.fileOpenToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(198, 6);
            // 
            // fileSaveToolStripMenuItem
            // 
            this.fileSaveToolStripMenuItem.Enabled = false;
            this.fileSaveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fileSaveToolStripMenuItem.Image")));
            this.fileSaveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileSaveToolStripMenuItem.Name = "fileSaveToolStripMenuItem";
            this.fileSaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.fileSaveToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.fileSaveToolStripMenuItem.Text = "上書き保存(&S)";
            // 
            // fileSaveAsToolStripMenuItem
            // 
            this.fileSaveAsToolStripMenuItem.Name = "fileSaveAsToolStripMenuItem";
            this.fileSaveAsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.fileSaveAsToolStripMenuItem.Text = "名前を付けて保存(&A)";
            this.fileSaveAsToolStripMenuItem.Click += new System.EventHandler(this.fileSaveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(198, 6);
            // 
            // filePrintToolStripMenuItem
            // 
            this.filePrintToolStripMenuItem.Enabled = false;
            this.filePrintToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("filePrintToolStripMenuItem.Image")));
            this.filePrintToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filePrintToolStripMenuItem.Name = "filePrintToolStripMenuItem";
            this.filePrintToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.filePrintToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.filePrintToolStripMenuItem.Text = "印刷(&P)";
            // 
            // filePreviewToolStripMenuItem
            // 
            this.filePreviewToolStripMenuItem.Enabled = false;
            this.filePreviewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("filePreviewToolStripMenuItem.Image")));
            this.filePreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filePreviewToolStripMenuItem.Name = "filePreviewToolStripMenuItem";
            this.filePreviewToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.filePreviewToolStripMenuItem.Text = "印刷プレビュー(&V)";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(198, 6);
            // 
            // fileExitToolStripMenuItem
            // 
            this.fileExitToolStripMenuItem.Name = "fileExitToolStripMenuItem";
            this.fileExitToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.fileExitToolStripMenuItem.Text = "終了(&X)";
            this.fileExitToolStripMenuItem.Click += new System.EventHandler(this.fileExitToolStripMenuItem_Click);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editUndoToolStripMenuItem,
            this.editRedoToolStripMenuItem,
            this.editResetPoseToolStripMenuItem});
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(61, 22);
            this.EditToolStripMenuItem.Text = "編集(&E)";
            // 
            // editUndoToolStripMenuItem
            // 
            this.editUndoToolStripMenuItem.Name = "editUndoToolStripMenuItem";
            this.editUndoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.editUndoToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.editUndoToolStripMenuItem.Text = "元に戻す(&U)";
            this.editUndoToolStripMenuItem.Click += new System.EventHandler(this.editUndoToolStripMenuItem_Click);
            // 
            // editRedoToolStripMenuItem
            // 
            this.editRedoToolStripMenuItem.Name = "editRedoToolStripMenuItem";
            this.editRedoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.editRedoToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.editRedoToolStripMenuItem.Text = "やり直し(&R)";
            this.editRedoToolStripMenuItem.Click += new System.EventHandler(this.editRedoToolStripMenuItem_Click);
            // 
            // editResetPoseToolStripMenuItem
            // 
            this.editResetPoseToolStripMenuItem.Name = "editResetPoseToolStripMenuItem";
            this.editResetPoseToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.editResetPoseToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.editResetPoseToolStripMenuItem.Text = "ポーズをリセット(&P)";
            this.editResetPoseToolStripMenuItem.Click += new System.EventHandler(this.editResetPoseToolStripMenuItem_Click);
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modeToolStripMenuItem,
            this.cameraCenterToolStripMenuItem,
            this.meshToolStripMenuItem,
            this.vertexToolStripMenuItem,
            this.boneToolStripMenuItem,
            this.cameraToolStripMenuItem});
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(62, 22);
            this.ViewToolStripMenuItem.Text = "表示(&V)";
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toonToolStripMenuItem,
            this.heatToolStripMenuItem,
            this.wireToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.modeToolStripMenuItem.Text = "表示形式(&S)";
            // 
            // toonToolStripMenuItem
            // 
            this.toonToolStripMenuItem.Name = "toonToolStripMenuItem";
            this.toonToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D1)));
            this.toonToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.toonToolStripMenuItem.Text = "トゥーン(&T)";
            this.toonToolStripMenuItem.Click += new System.EventHandler(this.toonToolStripMenuItem_Click);
            // 
            // heatToolStripMenuItem
            // 
            this.heatToolStripMenuItem.Name = "heatToolStripMenuItem";
            this.heatToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D2)));
            this.heatToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.heatToolStripMenuItem.Text = "ウェイト(&H)";
            this.heatToolStripMenuItem.Click += new System.EventHandler(this.heatToolStripMenuItem_Click);
            // 
            // wireToolStripMenuItem
            // 
            this.wireToolStripMenuItem.Name = "wireToolStripMenuItem";
            this.wireToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D3)));
            this.wireToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.wireToolStripMenuItem.Text = "ワイヤー(&W)";
            this.wireToolStripMenuItem.Click += new System.EventHandler(this.wireToolStripMenuItem_Click);
            // 
            // cameraCenterToolStripMenuItem
            // 
            this.cameraCenterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraResetToolStripMenuItem,
            this.cameraSelectedVertexToolStripMenuItem,
            this.cameraSelectedBoneToolStripMenuItem});
            this.cameraCenterToolStripMenuItem.Name = "cameraCenterToolStripMenuItem";
            this.cameraCenterToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.cameraCenterToolStripMenuItem.Text = "視点の回転中心(&C)";
            // 
            // cameraResetToolStripMenuItem
            // 
            this.cameraResetToolStripMenuItem.Name = "cameraResetToolStripMenuItem";
            this.cameraResetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.cameraResetToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.cameraResetToolStripMenuItem.Text = "初期位置(&R)";
            this.cameraResetToolStripMenuItem.Click += new System.EventHandler(this.cameraResetToolStripMenuItem_Click);
            // 
            // cameraSelectedVertexToolStripMenuItem
            // 
            this.cameraSelectedVertexToolStripMenuItem.Name = "cameraSelectedVertexToolStripMenuItem";
            this.cameraSelectedVertexToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.cameraSelectedVertexToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.cameraSelectedVertexToolStripMenuItem.Text = "選択頂点(&V)";
            this.cameraSelectedVertexToolStripMenuItem.Click += new System.EventHandler(this.cameraSelectedVertexToolStripMenuItem_Click);
            // 
            // cameraSelectedBoneToolStripMenuItem
            // 
            this.cameraSelectedBoneToolStripMenuItem.Name = "cameraSelectedBoneToolStripMenuItem";
            this.cameraSelectedBoneToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
            this.cameraSelectedBoneToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.cameraSelectedBoneToolStripMenuItem.Text = "選択ボーン(&B)";
            this.cameraSelectedBoneToolStripMenuItem.Click += new System.EventHandler(this.cameraSelectedBoneToolStripMenuItem_Click);
            // 
            // meshToolStripMenuItem
            // 
            this.meshToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.meshAllToolStripMenuItem,
            this.meshSelectedToolStripMenuItem});
            this.meshToolStripMenuItem.Name = "meshToolStripMenuItem";
            this.meshToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.meshToolStripMenuItem.Text = "メッシュ(&M)";
            // 
            // meshAllToolStripMenuItem
            // 
            this.meshAllToolStripMenuItem.Name = "meshAllToolStripMenuItem";
            this.meshAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.meshAllToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.meshAllToolStripMenuItem.Text = "全てのメッシュを表示(&A)";
            this.meshAllToolStripMenuItem.Click += new System.EventHandler(this.meshAllToolStripMenuItem_Click);
            // 
            // meshSelectedToolStripMenuItem
            // 
            this.meshSelectedToolStripMenuItem.Name = "meshSelectedToolStripMenuItem";
            this.meshSelectedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.meshSelectedToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.meshSelectedToolStripMenuItem.Text = "選択メッシュのみ表示(&S)";
            this.meshSelectedToolStripMenuItem.Click += new System.EventHandler(this.meshSelectedToolStripMenuItem_Click);
            // 
            // vertexToolStripMenuItem
            // 
            this.vertexToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vertexAllToolStripMenuItem1,
            this.vertexCcwToolStripMenuItem,
            this.vertexNoneToolStripMenuItem});
            this.vertexToolStripMenuItem.Name = "vertexToolStripMenuItem";
            this.vertexToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.vertexToolStripMenuItem.Text = "頂点(&V)";
            // 
            // vertexAllToolStripMenuItem1
            // 
            this.vertexAllToolStripMenuItem1.Name = "vertexAllToolStripMenuItem1";
            this.vertexAllToolStripMenuItem1.Size = new System.Drawing.Size(190, 22);
            this.vertexAllToolStripMenuItem1.Text = "全ての頂点を表示(&A)";
            this.vertexAllToolStripMenuItem1.Click += new System.EventHandler(this.vertexAllToolStripMenuItem1_Click);
            // 
            // vertexCcwToolStripMenuItem
            // 
            this.vertexCcwToolStripMenuItem.Name = "vertexCcwToolStripMenuItem";
            this.vertexCcwToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.vertexCcwToolStripMenuItem.Text = "表面頂点のみ表示(&S)";
            this.vertexCcwToolStripMenuItem.Click += new System.EventHandler(this.vertexCcwToolStripMenuItem_Click);
            // 
            // vertexNoneToolStripMenuItem
            // 
            this.vertexNoneToolStripMenuItem.Name = "vertexNoneToolStripMenuItem";
            this.vertexNoneToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.vertexNoneToolStripMenuItem.Text = "なし(&N)";
            this.vertexNoneToolStripMenuItem.Click += new System.EventHandler(this.vertexNoneToolStripMenuItem_Click);
            // 
            // boneToolStripMenuItem
            // 
            this.boneToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.boneAllToolStripMenuItem,
            this.boneNoneToolStripMenuItem});
            this.boneToolStripMenuItem.Name = "boneToolStripMenuItem";
            this.boneToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.boneToolStripMenuItem.Text = "ボーン(&B)";
            // 
            // boneAllToolStripMenuItem
            // 
            this.boneAllToolStripMenuItem.Name = "boneAllToolStripMenuItem";
            this.boneAllToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.boneAllToolStripMenuItem.Text = "全てのボーンを表示(&A)";
            this.boneAllToolStripMenuItem.Click += new System.EventHandler(this.boneAllToolStripMenuItem_Click);
            // 
            // boneNoneToolStripMenuItem
            // 
            this.boneNoneToolStripMenuItem.Name = "boneNoneToolStripMenuItem";
            this.boneNoneToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.boneNoneToolStripMenuItem.Text = "なし(&N)";
            this.boneNoneToolStripMenuItem.Click += new System.EventHandler(this.boneNoneToolStripMenuItem_Click);
            // 
            // cameraToolStripMenuItem
            // 
            this.cameraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraXaxisToolStripMenuItem,
            this.cameraYaxisToolStripMenuItem,
            this.cameraZaxisToolStripMenuItem,
            this.caminvXaxisToolStripMenuItem,
            this.caminvYaxisToolStripMenuItem,
            this.caminvZaxisToolStripMenuItem});
            this.cameraToolStripMenuItem.Name = "cameraToolStripMenuItem";
            this.cameraToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.cameraToolStripMenuItem.Text = "カメラ(&C)";
            // 
            // cameraXaxisToolStripMenuItem
            // 
            this.cameraXaxisToolStripMenuItem.Name = "cameraXaxisToolStripMenuItem";
            this.cameraXaxisToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.cameraXaxisToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.cameraXaxisToolStripMenuItem.Text = "X";
            this.cameraXaxisToolStripMenuItem.Click += new System.EventHandler(this.cameraXaxisToolStripMenuItem_Click);
            // 
            // cameraYaxisToolStripMenuItem
            // 
            this.cameraYaxisToolStripMenuItem.Name = "cameraYaxisToolStripMenuItem";
            this.cameraYaxisToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.cameraYaxisToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.cameraYaxisToolStripMenuItem.Text = "Y";
            this.cameraYaxisToolStripMenuItem.Click += new System.EventHandler(this.cameraYaxisToolStripMenuItem_Click);
            // 
            // cameraZaxisToolStripMenuItem
            // 
            this.cameraZaxisToolStripMenuItem.Name = "cameraZaxisToolStripMenuItem";
            this.cameraZaxisToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.cameraZaxisToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.cameraZaxisToolStripMenuItem.Text = "Z";
            this.cameraZaxisToolStripMenuItem.Click += new System.EventHandler(this.cameraZaxisToolStripMenuItem_Click);
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpContentToolStripMenuItem,
            this.helpIndexToolStripMenuItem,
            this.helpSearchToolStripMenuItem,
            this.toolStripSeparator5,
            this.helpVersionToolStripMenuItem});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(75, 22);
            this.HelpToolStripMenuItem.Text = "ヘルプ(&H)";
            // 
            // helpContentToolStripMenuItem
            // 
            this.helpContentToolStripMenuItem.Enabled = false;
            this.helpContentToolStripMenuItem.Name = "helpContentToolStripMenuItem";
            this.helpContentToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.helpContentToolStripMenuItem.Text = "内容(&C)";
            // 
            // helpIndexToolStripMenuItem
            // 
            this.helpIndexToolStripMenuItem.Enabled = false;
            this.helpIndexToolStripMenuItem.Name = "helpIndexToolStripMenuItem";
            this.helpIndexToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.helpIndexToolStripMenuItem.Text = "インデックス(&I)";
            // 
            // helpSearchToolStripMenuItem
            // 
            this.helpSearchToolStripMenuItem.Enabled = false;
            this.helpSearchToolStripMenuItem.Name = "helpSearchToolStripMenuItem";
            this.helpSearchToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.helpSearchToolStripMenuItem.Text = "検索(&S)";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(187, 6);
            // 
            // helpVersionToolStripMenuItem
            // 
            this.helpVersionToolStripMenuItem.Enabled = false;
            this.helpVersionToolStripMenuItem.Name = "helpVersionToolStripMenuItem";
            this.helpVersionToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.helpVersionToolStripMenuItem.Text = "バージョン情報(&A)...";
            // 
            // lbTSOFiles
            // 
            this.lbTSOFiles.Location = new System.Drawing.Point(12, 1);
            this.lbTSOFiles.Name = "lbTSOFiles";
            this.lbTSOFiles.Size = new System.Drawing.Size(120, 12);
            this.lbTSOFiles.TabIndex = 0;
            this.lbTSOFiles.Text = "TSOファイル";
            // 
            // lvTSOFiles
            // 
            this.lvTSOFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6});
            this.lvTSOFiles.FullRowSelect = true;
            this.lvTSOFiles.GridLines = true;
            this.lvTSOFiles.HideSelection = false;
            this.lvTSOFiles.Location = new System.Drawing.Point(14, 16);
            this.lvTSOFiles.MultiSelect = false;
            this.lvTSOFiles.Name = "lvTSOFiles";
            this.lvTSOFiles.Size = new System.Drawing.Size(174, 120);
            this.lvTSOFiles.TabIndex = 1;
            this.lvTSOFiles.UseCompatibleStateImageBehavior = false;
            this.lvTSOFiles.View = System.Windows.Forms.View.Details;
            this.lvTSOFiles.SelectedIndexChanged += new System.EventHandler(this.lvTSOFiles_SelectedIndexChanged);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "名前";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnLocalPosUpdate);
            this.panel1.Controls.Add(this.edNodeLocalPosZ);
            this.panel1.Controls.Add(this.edNodeLocalPosY);
            this.panel1.Controls.Add(this.edNodeLocalPosX);
            this.panel1.Controls.Add(this.btnPosUpdate);
            this.panel1.Controls.Add(this.lbNodeSubZ);
            this.panel1.Controls.Add(this.lbNodeSubY);
            this.panel1.Controls.Add(this.lbNodeSubX);
            this.panel1.Controls.Add(this.edNodePosZ);
            this.panel1.Controls.Add(this.edNodePosY);
            this.panel1.Controls.Add(this.edNodePosX);
            this.panel1.Controls.Add(this.lbNodeName);
            this.panel1.Controls.Add(this.lvTSOFiles);
            this.panel1.Controls.Add(this.lbTSOFiles);
            this.panel1.Location = new System.Drawing.Point(808, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 681);
            this.panel1.TabIndex = 22;
            // 
            // btnPosUpdate
            // 
            this.btnPosUpdate.Location = new System.Drawing.Point(14, 236);
            this.btnPosUpdate.Name = "btnPosUpdate";
            this.btnPosUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnPosUpdate.TabIndex = 9;
            this.btnPosUpdate.Text = "変更";
            this.btnPosUpdate.UseVisualStyleBackColor = true;
            this.btnPosUpdate.Click += new System.EventHandler(this.btnPosUpdate_Click);
            // 
            // lbNodeSubZ
            // 
            this.lbNodeSubZ.AutoSize = true;
            this.lbNodeSubZ.Location = new System.Drawing.Point(95, 214);
            this.lbNodeSubZ.Name = "lbNodeSubZ";
            this.lbNodeSubZ.Size = new System.Drawing.Size(10, 12);
            this.lbNodeSubZ.TabIndex = 8;
            this.lbNodeSubZ.Text = "z";
            // 
            // lbNodeSubY
            // 
            this.lbNodeSubY.AutoSize = true;
            this.lbNodeSubY.Location = new System.Drawing.Point(95, 188);
            this.lbNodeSubY.Name = "lbNodeSubY";
            this.lbNodeSubY.Size = new System.Drawing.Size(11, 12);
            this.lbNodeSubY.TabIndex = 7;
            this.lbNodeSubY.Text = "y";
            // 
            // lbNodeSubX
            // 
            this.lbNodeSubX.AutoSize = true;
            this.lbNodeSubX.Location = new System.Drawing.Point(95, 163);
            this.lbNodeSubX.Name = "lbNodeSubX";
            this.lbNodeSubX.Size = new System.Drawing.Size(11, 12);
            this.lbNodeSubX.TabIndex = 6;
            this.lbNodeSubX.Text = "x";
            // 
            // edNodePosZ
            // 
            this.edNodePosZ.Location = new System.Drawing.Point(14, 211);
            this.edNodePosZ.Name = "edNodePosZ";
            this.edNodePosZ.Size = new System.Drawing.Size(75, 19);
            this.edNodePosZ.TabIndex = 5;
            // 
            // edNodePosY
            // 
            this.edNodePosY.Location = new System.Drawing.Point(14, 185);
            this.edNodePosY.Name = "edNodePosY";
            this.edNodePosY.Size = new System.Drawing.Size(75, 19);
            this.edNodePosY.TabIndex = 4;
            // 
            // edNodePosX
            // 
            this.edNodePosX.Location = new System.Drawing.Point(14, 160);
            this.edNodePosX.Name = "edNodePosX";
            this.edNodePosX.Size = new System.Drawing.Size(75, 19);
            this.edNodePosX.TabIndex = 3;
            // 
            // lbNodeName
            // 
            this.lbNodeName.AutoSize = true;
            this.lbNodeName.Location = new System.Drawing.Point(14, 143);
            this.lbNodeName.Name = "lbNodeName";
            this.lbNodeName.Size = new System.Drawing.Size(60, 12);
            this.lbNodeName.TabIndex = 2;
            this.lbNodeName.Text = "node name";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 709);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip1.TabIndex = 23;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // caminvXaxisToolStripMenuItem
            // 
            this.caminvXaxisToolStripMenuItem.Name = "caminvXaxisToolStripMenuItem";
            this.caminvXaxisToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F1)));
            this.caminvXaxisToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.caminvXaxisToolStripMenuItem.Text = "inv X";
            this.caminvXaxisToolStripMenuItem.Click += new System.EventHandler(this.caminvXaxisToolStripMenuItem_Click);
            // 
            // caminvYaxisToolStripMenuItem
            // 
            this.caminvYaxisToolStripMenuItem.Name = "caminvYaxisToolStripMenuItem";
            this.caminvYaxisToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.caminvYaxisToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.caminvYaxisToolStripMenuItem.Text = "inv Y";
            this.caminvYaxisToolStripMenuItem.Click += new System.EventHandler(this.caminvYaxisToolStripMenuItem_Click);
            // 
            // caminvZaxisToolStripMenuItem
            // 
            this.caminvZaxisToolStripMenuItem.Name = "caminvZaxisToolStripMenuItem";
            this.caminvZaxisToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.caminvZaxisToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.caminvZaxisToolStripMenuItem.Text = "inv Z";
            this.caminvZaxisToolStripMenuItem.Click += new System.EventHandler(this.caminvZaxisToolStripMenuItem_Click);
            // 
            // btnLocalPosUpdate
            // 
            this.btnLocalPosUpdate.Location = new System.Drawing.Point(14, 353);
            this.btnLocalPosUpdate.Name = "btnLocalPosUpdate";
            this.btnLocalPosUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnLocalPosUpdate.TabIndex = 13;
            this.btnLocalPosUpdate.Text = "変更";
            this.btnLocalPosUpdate.UseVisualStyleBackColor = true;
            this.btnLocalPosUpdate.Click += new System.EventHandler(this.btnLocalPosUpdate_Click);
            // 
            // edNodeLocalPosZ
            // 
            this.edNodeLocalPosZ.Location = new System.Drawing.Point(14, 328);
            this.edNodeLocalPosZ.Name = "edNodeLocalPosZ";
            this.edNodeLocalPosZ.Size = new System.Drawing.Size(75, 19);
            this.edNodeLocalPosZ.TabIndex = 12;
            // 
            // edNodeLocalPosY
            // 
            this.edNodeLocalPosY.Location = new System.Drawing.Point(14, 302);
            this.edNodeLocalPosY.Name = "edNodeLocalPosY";
            this.edNodeLocalPosY.Size = new System.Drawing.Size(75, 19);
            this.edNodeLocalPosY.TabIndex = 11;
            // 
            // edNodeLocalPosX
            // 
            this.edNodeLocalPosX.Location = new System.Drawing.Point(14, 277);
            this.edNodeLocalPosX.Name = "edNodeLocalPosX";
            this.edNodeLocalPosX.Size = new System.Drawing.Size(75, 19);
            this.edNodeLocalPosX.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 262);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "local";
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 731);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "TSOBoneEd";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.Form1_DragOver);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileNewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileOpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem fileSaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileSaveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem filePrintToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filePreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem fileExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editUndoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editRedoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbTSOFiles;
        private System.Windows.Forms.ListView lvTSOFiles;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem cameraCenterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraSelectedVertexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraSelectedBoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wireToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vertexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meshAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vertexAllToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem vertexCcwToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vertexNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meshSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpContentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem helpVersionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boneAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boneNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editResetPoseToolStripMenuItem;
        private System.Windows.Forms.TextBox edNodePosZ;
        private System.Windows.Forms.TextBox edNodePosY;
        private System.Windows.Forms.TextBox edNodePosX;
        private System.Windows.Forms.Label lbNodeName;
        private System.Windows.Forms.Label lbNodeSubX;
        private System.Windows.Forms.Label lbNodeSubZ;
        private System.Windows.Forms.Label lbNodeSubY;
        private System.Windows.Forms.Button btnPosUpdate;
        private System.Windows.Forms.ToolStripMenuItem cameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraXaxisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraYaxisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraZaxisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem caminvXaxisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem caminvYaxisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem caminvZaxisToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLocalPosUpdate;
        private System.Windows.Forms.TextBox edNodeLocalPosZ;
        private System.Windows.Forms.TextBox edNodeLocalPosY;
        private System.Windows.Forms.TextBox edNodeLocalPosX;
    }
}

