namespace CheckDataSystem
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lblResult = new Label();
            rtbLog = new RichTextBox();
            tabControl1 = new TabControl();
            tabAdd = new TabPage();
            btnAdd = new Button();
            txtInputAdd = new TextBox();
            tabCheck = new TabPage();
            txtInputCheck = new TextBox();
            btnCheck = new Button();
            tabView = new TabPage();
            lblTotalCount = new Label();
            btnCancel = new Button();
            btnRefresh = new Button();
            dgvData = new DataGridView();
            menuStrip1 = new MenuStrip();
            檔案ToolStripMenuItem = new ToolStripMenuItem();
            tsmi_SetFolderPath = new ToolStripMenuItem();
            tmrAutoRefresh = new System.Windows.Forms.Timer(components);
            tabControl1.SuspendLayout();
            tabAdd.SuspendLayout();
            tabCheck.SuspendLayout();
            tabView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvData).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // lblResult
            // 
            lblResult.Font = new Font("Microsoft JhengHei UI", 22F);
            lblResult.Location = new Point(454, 9);
            lblResult.Name = "lblResult";
            lblResult.Size = new Size(314, 125);
            lblResult.TabIndex = 0;
            lblResult.Text = "label1";
            // 
            // rtbLog
            // 
            rtbLog.Location = new Point(711, 220);
            rtbLog.Name = "rtbLog";
            rtbLog.Size = new Size(392, 274);
            rtbLog.TabIndex = 2;
            rtbLog.Text = "";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabAdd);
            tabControl1.Controls.Add(tabCheck);
            tabControl1.Controls.Add(tabView);
            tabControl1.Font = new Font("Microsoft JhengHei UI", 12F);
            tabControl1.Location = new Point(10, 137);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(658, 475);
            tabControl1.TabIndex = 6;
            // 
            // tabAdd
            // 
            tabAdd.Controls.Add(btnAdd);
            tabAdd.Controls.Add(txtInputAdd);
            tabAdd.Font = new Font("Microsoft JhengHei UI", 22F);
            tabAdd.Location = new Point(4, 39);
            tabAdd.Name = "tabAdd";
            tabAdd.Padding = new Padding(3);
            tabAdd.Size = new Size(650, 432);
            tabAdd.TabIndex = 0;
            tabAdd.Text = "上游";
            tabAdd.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(209, 216);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(166, 76);
            btnAdd.TabIndex = 5;
            btnAdd.Text = "錄入";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // txtInputAdd
            // 
            txtInputAdd.Font = new Font("Microsoft JhengHei UI", 22F);
            txtInputAdd.Location = new Point(125, 111);
            txtInputAdd.Name = "txtInputAdd";
            txtInputAdd.Size = new Size(329, 63);
            txtInputAdd.TabIndex = 3;
            // 
            // tabCheck
            // 
            tabCheck.Controls.Add(txtInputCheck);
            tabCheck.Controls.Add(btnCheck);
            tabCheck.Location = new Point(4, 39);
            tabCheck.Name = "tabCheck";
            tabCheck.Padding = new Padding(3);
            tabCheck.Size = new Size(650, 432);
            tabCheck.TabIndex = 1;
            tabCheck.Text = "下游";
            tabCheck.UseVisualStyleBackColor = true;
            // 
            // txtInputCheck
            // 
            txtInputCheck.Font = new Font("Microsoft JhengHei UI", 22F);
            txtInputCheck.Location = new Point(111, 79);
            txtInputCheck.Name = "txtInputCheck";
            txtInputCheck.Size = new Size(329, 63);
            txtInputCheck.TabIndex = 1;
            // 
            // btnCheck
            // 
            btnCheck.Location = new Point(187, 193);
            btnCheck.Name = "btnCheck";
            btnCheck.Size = new Size(166, 76);
            btnCheck.TabIndex = 4;
            btnCheck.Text = "查詢";
            btnCheck.UseVisualStyleBackColor = true;
            // 
            // tabView
            // 
            tabView.Controls.Add(lblTotalCount);
            tabView.Controls.Add(btnCancel);
            tabView.Controls.Add(btnRefresh);
            tabView.Controls.Add(dgvData);
            tabView.Location = new Point(4, 39);
            tabView.Name = "tabView";
            tabView.Size = new Size(650, 432);
            tabView.TabIndex = 2;
            tabView.Text = "資料庫總覽";
            tabView.UseVisualStyleBackColor = true;
            tabView.Click += tabView_Click;
            // 
            // lblTotalCount
            // 
            lblTotalCount.AutoSize = true;
            lblTotalCount.Font = new Font("Microsoft JhengHei UI", 10F);
            lblTotalCount.Location = new Point(527, 61);
            lblTotalCount.Name = "lblTotalCount";
            lblTotalCount.Size = new Size(52, 25);
            lblTotalCount.TabIndex = 3;
            lblTotalCount.Text = "數量";
            // 
            // btnCancel
            // 
            btnCancel.Font = new Font("Microsoft JhengHei UI", 10F);
            btnCancel.Location = new Point(511, 228);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(103, 64);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "清除資料";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            btnRefresh.Font = new Font("Microsoft JhengHei UI", 10F);
            btnRefresh.Location = new Point(511, 128);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(103, 64);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "更新";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // dgvData
            // 
            dgvData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvData.Location = new Point(19, 18);
            dgvData.Name = "dgvData";
            dgvData.RowHeadersWidth = 62;
            dgvData.Size = new Size(471, 372);
            dgvData.TabIndex = 0;
            dgvData.CellContentClick += dgvData_CellContentClick;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 檔案ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1259, 31);
            menuStrip1.TabIndex = 7;
            menuStrip1.Text = "menuStrip1";
            // 
            // 檔案ToolStripMenuItem
            // 
            檔案ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmi_SetFolderPath });
            檔案ToolStripMenuItem.Name = "檔案ToolStripMenuItem";
            檔案ToolStripMenuItem.Size = new Size(62, 27);
            檔案ToolStripMenuItem.Text = "檔案";
            // 
            // tsmi_SetFolderPath
            // 
            tsmi_SetFolderPath.Name = "tsmi_SetFolderPath";
            tsmi_SetFolderPath.Size = new Size(236, 34);
            tsmi_SetFolderPath.Text = "設定資料夾路徑";
            tsmi_SetFolderPath.Click += tsmi_SetFolderPath_Click;
            // 
            // tmrAutoRefresh
            // 
            tmrAutoRefresh.Enabled = true;
            tmrAutoRefresh.Interval = 2000;
            tmrAutoRefresh.Tick += tmrAutoRefresh_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1259, 695);
            Controls.Add(tabControl1);
            Controls.Add(rtbLog);
            Controls.Add(lblResult);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            tabControl1.ResumeLayout(false);
            tabAdd.ResumeLayout(false);
            tabAdd.PerformLayout();
            tabCheck.ResumeLayout(false);
            tabCheck.PerformLayout();
            tabView.ResumeLayout(false);
            tabView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvData).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblResult;
        private RichTextBox rtbLog;
        private TabControl tabControl1;
        private TabPage tabAdd;
        private TextBox txtInputCheck;
        private Button btnCheck;
        private TabPage tabCheck;
        private TextBox txtInputAdd;
        private Button btnAdd;
        private TabPage tabView;
        private DataGridView dgvData;
        private Button btnRefresh;
        private Button btnCancel;
        private Label lblTotalCount;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 檔案ToolStripMenuItem;
        private ToolStripMenuItem tsmi_SetFolderPath;
        private System.Windows.Forms.Timer tmrAutoRefresh;
    }
}
