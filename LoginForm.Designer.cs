namespace CheckDataSystem
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnAddUser = new Button();
            btnCheckUser = new Button();
            SuspendLayout();
            // 
            // btnAddUser
            // 
            btnAddUser.Font = new Font("Microsoft JhengHei UI", 22F);
            btnAddUser.Location = new Point(149, 164);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Size = new Size(220, 111);
            btnAddUser.TabIndex = 0;
            btnAddUser.Text = "前站";
            btnAddUser.UseVisualStyleBackColor = true;
            btnAddUser.Click += btnAddUser_Click;
            // 
            // btnCheckUser
            // 
            btnCheckUser.Font = new Font("Microsoft JhengHei UI", 22F);
            btnCheckUser.Location = new Point(459, 164);
            btnCheckUser.Name = "btnCheckUser";
            btnCheckUser.Size = new Size(220, 111);
            btnCheckUser.TabIndex = 1;
            btnCheckUser.Text = "後站";
            btnCheckUser.UseVisualStyleBackColor = true;
            btnCheckUser.Click += btnCheckUser_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(825, 477);
            Controls.Add(btnCheckUser);
            Controls.Add(btnAddUser);
            Name = "LoginForm";
            Text = "LoginForm";
            ResumeLayout(false);
        }

        #endregion

        private Button btnAddUser;
        private Button btnCheckUser;
    }
}