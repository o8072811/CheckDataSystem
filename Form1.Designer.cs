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
            lblResult = new Label();
            txtInput = new TextBox();
            rtbLog = new RichTextBox();
            btnAddData = new TextBox();
            btnCheck = new Button();
            button1 = new Button();
            SuspendLayout();
            // 
            // lblResult
            // 
            lblResult.Font = new Font("Microsoft JhengHei UI", 22F);
            lblResult.Location = new Point(527, 9);
            lblResult.Name = "lblResult";
            lblResult.Size = new Size(314, 125);
            lblResult.TabIndex = 0;
            lblResult.Text = "label1";
            // 
            // txtInput
            // 
            txtInput.Font = new Font("Microsoft JhengHei UI", 22F);
            txtInput.Location = new Point(192, 120);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(329, 63);
            txtInput.TabIndex = 1;
            txtInput.Text = "ABCDEFGHIJKL";
            // 
            // rtbLog
            // 
            rtbLog.Location = new Point(428, 284);
            rtbLog.Name = "rtbLog";
            rtbLog.Size = new Size(496, 253);
            rtbLog.TabIndex = 2;
            rtbLog.Text = "";
            // 
            // btnAddData
            // 
            btnAddData.Font = new Font("Microsoft JhengHei UI", 22F);
            btnAddData.Location = new Point(861, 120);
            btnAddData.Name = "btnAddData";
            btnAddData.Size = new Size(329, 63);
            btnAddData.TabIndex = 3;
            btnAddData.Text = "ABCDEFGHIJKL";
            // 
            // btnCheck
            // 
            btnCheck.Location = new Point(265, 202);
            btnCheck.Name = "btnCheck";
            btnCheck.Size = new Size(166, 76);
            btnCheck.TabIndex = 4;
            btnCheck.Text = "查詢";
            btnCheck.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(953, 202);
            button1.Name = "button1";
            button1.Size = new Size(166, 76);
            button1.TabIndex = 5;
            button1.Text = "錄入";
            button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1257, 601);
            Controls.Add(button1);
            Controls.Add(btnCheck);
            Controls.Add(btnAddData);
            Controls.Add(rtbLog);
            Controls.Add(txtInput);
            Controls.Add(lblResult);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblResult;
        private TextBox txtInput;
        private RichTextBox rtbLog;
        private TextBox btnAddData;
        private Button btnCheck;
        private Button button1;
    }
}
