namespace 视觉检测系统
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.相机采图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.本地测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加载文件夹ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加载单个文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加载程序ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建程序ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.相机采图ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.本地测试ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.加载已有程序ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.标定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.九点标定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.日志ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开程序ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存图片ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存方案ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加载方案ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.存图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cogRecordDisplayStart = new Cognex.VisionPro.CogRecordDisplay();
            this.panel5 = new System.Windows.Forms.Panel();
            this.cogRecordDisplayEnd = new Cognex.VisionPro.CogRecordDisplay();
            this.panel6 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.检测_button = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplayStart)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplayEnd)).BeginInit();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.839779F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.16022F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(922, 607);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(916, 42);
            this.panel1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.相机采图ToolStripMenuItem,
            this.本地测试ToolStripMenuItem,
            this.加载程序ToolStripMenuItem,
            this.标定ToolStripMenuItem,
            this.日志ToolStripMenuItem,
            this.打开程序ToolStripMenuItem,
            this.保存图片ToolStripMenuItem,
            this.存图ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(916, 42);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 相机采图ToolStripMenuItem
            // 
            this.相机采图ToolStripMenuItem.Name = "相机采图ToolStripMenuItem";
            this.相机采图ToolStripMenuItem.Size = new System.Drawing.Size(98, 38);
            this.相机采图ToolStripMenuItem.Text = "相机采图";
            // 
            // 本地测试ToolStripMenuItem
            // 
            this.本地测试ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.加载文件夹ToolStripMenuItem,
            this.加载单个文件ToolStripMenuItem});
            this.本地测试ToolStripMenuItem.Name = "本地测试ToolStripMenuItem";
            this.本地测试ToolStripMenuItem.Size = new System.Drawing.Size(98, 38);
            this.本地测试ToolStripMenuItem.Text = "本地测试";
            // 
            // 加载文件夹ToolStripMenuItem
            // 
            this.加载文件夹ToolStripMenuItem.Name = "加载文件夹ToolStripMenuItem";
            this.加载文件夹ToolStripMenuItem.Size = new System.Drawing.Size(218, 34);
            this.加载文件夹ToolStripMenuItem.Text = "加载文件夹";
            this.加载文件夹ToolStripMenuItem.Click += new System.EventHandler(this.加载文件夹ToolStripMenuItem_Click);
            // 
            // 加载单个文件ToolStripMenuItem
            // 
            this.加载单个文件ToolStripMenuItem.Name = "加载单个文件ToolStripMenuItem";
            this.加载单个文件ToolStripMenuItem.Size = new System.Drawing.Size(218, 34);
            this.加载单个文件ToolStripMenuItem.Text = "加载单个文件";
            // 
            // 加载程序ToolStripMenuItem
            // 
            this.加载程序ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建程序ToolStripMenuItem,
            this.加载已有程序ToolStripMenuItem});
            this.加载程序ToolStripMenuItem.Name = "加载程序ToolStripMenuItem";
            this.加载程序ToolStripMenuItem.Size = new System.Drawing.Size(98, 38);
            this.加载程序ToolStripMenuItem.Text = "加载程序";
            this.加载程序ToolStripMenuItem.Click += new System.EventHandler(this.加载程序ToolStripMenuItem_Click);
            // 
            // 新建程序ToolStripMenuItem
            // 
            this.新建程序ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.相机采图ToolStripMenuItem1,
            this.本地测试ToolStripMenuItem1});
            this.新建程序ToolStripMenuItem.Name = "新建程序ToolStripMenuItem";
            this.新建程序ToolStripMenuItem.Size = new System.Drawing.Size(218, 34);
            this.新建程序ToolStripMenuItem.Text = "新建程序";
            this.新建程序ToolStripMenuItem.Click += new System.EventHandler(this.新建程序ToolStripMenuItem_Click);
            // 
            // 相机采图ToolStripMenuItem1
            // 
            this.相机采图ToolStripMenuItem1.Name = "相机采图ToolStripMenuItem1";
            this.相机采图ToolStripMenuItem1.Size = new System.Drawing.Size(182, 34);
            this.相机采图ToolStripMenuItem1.Text = "相机采图";
            // 
            // 本地测试ToolStripMenuItem1
            // 
            this.本地测试ToolStripMenuItem1.Name = "本地测试ToolStripMenuItem1";
            this.本地测试ToolStripMenuItem1.Size = new System.Drawing.Size(182, 34);
            this.本地测试ToolStripMenuItem1.Text = "本地测试";
            this.本地测试ToolStripMenuItem1.Click += new System.EventHandler(this.本地测试ToolStripMenuItem1_Click);
            // 
            // 加载已有程序ToolStripMenuItem
            // 
            this.加载已有程序ToolStripMenuItem.Name = "加载已有程序ToolStripMenuItem";
            this.加载已有程序ToolStripMenuItem.Size = new System.Drawing.Size(218, 34);
            this.加载已有程序ToolStripMenuItem.Text = "加载已有程序";
            this.加载已有程序ToolStripMenuItem.Click += new System.EventHandler(this.加载已有程序ToolStripMenuItem_Click);
            // 
            // 标定ToolStripMenuItem
            // 
            this.标定ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.九点标定ToolStripMenuItem});
            this.标定ToolStripMenuItem.Name = "标定ToolStripMenuItem";
            this.标定ToolStripMenuItem.Size = new System.Drawing.Size(62, 38);
            this.标定ToolStripMenuItem.Text = "标定";
            // 
            // 九点标定ToolStripMenuItem
            // 
            this.九点标定ToolStripMenuItem.Name = "九点标定ToolStripMenuItem";
            this.九点标定ToolStripMenuItem.Size = new System.Drawing.Size(182, 34);
            this.九点标定ToolStripMenuItem.Text = "九点标定";
            // 
            // 日志ToolStripMenuItem
            // 
            this.日志ToolStripMenuItem.Name = "日志ToolStripMenuItem";
            this.日志ToolStripMenuItem.Size = new System.Drawing.Size(62, 38);
            this.日志ToolStripMenuItem.Text = "日志";
            this.日志ToolStripMenuItem.Click += new System.EventHandler(this.日志ToolStripMenuItem_Click);
            // 
            // 打开程序ToolStripMenuItem
            // 
            this.打开程序ToolStripMenuItem.Name = "打开程序ToolStripMenuItem";
            this.打开程序ToolStripMenuItem.Size = new System.Drawing.Size(98, 38);
            this.打开程序ToolStripMenuItem.Text = "打开程序";
            this.打开程序ToolStripMenuItem.Click += new System.EventHandler(this.打开程序ToolStripMenuItem_Click);
            // 
            // 保存图片ToolStripMenuItem
            // 
            this.保存图片ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.保存方案ToolStripMenuItem,
            this.加载方案ToolStripMenuItem});
            this.保存图片ToolStripMenuItem.Name = "保存图片ToolStripMenuItem";
            this.保存图片ToolStripMenuItem.Size = new System.Drawing.Size(62, 38);
            this.保存图片ToolStripMenuItem.Text = "方案";
            // 
            // 保存方案ToolStripMenuItem
            // 
            this.保存方案ToolStripMenuItem.Name = "保存方案ToolStripMenuItem";
            this.保存方案ToolStripMenuItem.Size = new System.Drawing.Size(182, 34);
            this.保存方案ToolStripMenuItem.Text = "保存方案";
            this.保存方案ToolStripMenuItem.Click += new System.EventHandler(this.保存方案ToolStripMenuItem_Click);
            // 
            // 加载方案ToolStripMenuItem
            // 
            this.加载方案ToolStripMenuItem.Name = "加载方案ToolStripMenuItem";
            this.加载方案ToolStripMenuItem.Size = new System.Drawing.Size(182, 34);
            this.加载方案ToolStripMenuItem.Text = "加载方案";
            this.加载方案ToolStripMenuItem.Click += new System.EventHandler(this.加载方案ToolStripMenuItem_Click);
            // 
            // 存图ToolStripMenuItem
            // 
            this.存图ToolStripMenuItem.Name = "存图ToolStripMenuItem";
            this.存图ToolStripMenuItem.Size = new System.Drawing.Size(62, 38);
            this.存图ToolStripMenuItem.Text = "设置";
            this.存图ToolStripMenuItem.Click += new System.EventHandler(this.设置ToolStripMenuItem_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 51);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(916, 489);
            this.panel2.TabIndex = 1;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.tableLayoutPanel2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(916, 489);
            this.panel4.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel5, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(916, 489);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.cogRecordDisplayStart);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(452, 483);
            this.panel3.TabIndex = 0;
            // 
            // cogRecordDisplayStart
            // 
            this.cogRecordDisplayStart.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplayStart.ColorMapLowerRoiLimit = 0D;
            this.cogRecordDisplayStart.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecordDisplayStart.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplayStart.ColorMapUpperRoiLimit = 1D;
            this.cogRecordDisplayStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogRecordDisplayStart.DoubleTapZoomCycleLength = 2;
            this.cogRecordDisplayStart.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecordDisplayStart.Location = new System.Drawing.Point(0, 0);
            this.cogRecordDisplayStart.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecordDisplayStart.MouseWheelSensitivity = 1D;
            this.cogRecordDisplayStart.Name = "cogRecordDisplayStart";
            this.cogRecordDisplayStart.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecordDisplayStart.OcxState")));
            this.cogRecordDisplayStart.Size = new System.Drawing.Size(452, 483);
            this.cogRecordDisplayStart.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.cogRecordDisplayEnd);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(461, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(452, 483);
            this.panel5.TabIndex = 1;
            // 
            // cogRecordDisplayEnd
            // 
            this.cogRecordDisplayEnd.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplayEnd.ColorMapLowerRoiLimit = 0D;
            this.cogRecordDisplayEnd.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecordDisplayEnd.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplayEnd.ColorMapUpperRoiLimit = 1D;
            this.cogRecordDisplayEnd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogRecordDisplayEnd.DoubleTapZoomCycleLength = 2;
            this.cogRecordDisplayEnd.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecordDisplayEnd.Location = new System.Drawing.Point(0, 0);
            this.cogRecordDisplayEnd.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecordDisplayEnd.MouseWheelSensitivity = 1D;
            this.cogRecordDisplayEnd.Name = "cogRecordDisplayEnd";
            this.cogRecordDisplayEnd.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecordDisplayEnd.OcxState")));
            this.cogRecordDisplayEnd.Size = new System.Drawing.Size(452, 483);
            this.cogRecordDisplayEnd.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.button4);
            this.panel6.Controls.Add(this.检测_button);
            this.panel6.Controls.Add(this.button2);
            this.panel6.Controls.Add(this.button1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 546);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(916, 58);
            this.panel6.TabIndex = 2;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(534, 6);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(129, 43);
            this.button4.TabIndex = 3;
            this.button4.Text = "全部检测";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // 检测_button
            // 
            this.检测_button.Location = new System.Drawing.Point(368, 6);
            this.检测_button.Name = "检测_button";
            this.检测_button.Size = new System.Drawing.Size(129, 43);
            this.检测_button.TabIndex = 2;
            this.检测_button.Text = "检测";
            this.检测_button.UseVisualStyleBackColor = true;
            this.检测_button.Click += new System.EventHandler(this.检测_button_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(188, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(129, 43);
            this.button2.TabIndex = 1;
            this.button2.Text = "下一张";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(19, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(129, 43);
            this.button1.TabIndex = 0;
            this.button1.Text = "上一张";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 607);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplayStart)).EndInit();
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplayEnd)).EndInit();
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 相机采图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 本地测试ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加载程序ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 标定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 九点标定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加载文件夹ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加载单个文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 日志ToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private Cognex.VisionPro.CogRecordDisplay cogRecordDisplayStart;
        private Cognex.VisionPro.CogRecordDisplay cogRecordDisplayEnd;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button 检测_button;
        private System.Windows.Forms.ToolStripMenuItem 新建程序ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加载已有程序ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 相机采图ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 本地测试ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 打开程序ToolStripMenuItem;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ToolStripMenuItem 保存图片ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存方案ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加载方案ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 存图ToolStripMenuItem;
    }
}

