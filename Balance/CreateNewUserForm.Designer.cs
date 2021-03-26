
namespace Balance
{
    partial class CreateNewUserForm
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.credentials = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.LoginBox = new System.Windows.Forms.TextBox();
            this.RegisterBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.PasswordBox1 = new System.Windows.Forms.TextBox();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.PasswordBox2 = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.credentials.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 114);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(457, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // status
            // 
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(0, 17);
            // 
            // credentials
            // 
            this.credentials.Controls.Add(this.label1);
            this.credentials.Controls.Add(this.LoginBox);
            this.credentials.Controls.Add(this.RegisterBtn);
            this.credentials.Controls.Add(this.label2);
            this.credentials.Controls.Add(this.PasswordBox1);
            this.credentials.Controls.Add(this.CancelBtn);
            this.credentials.Controls.Add(this.label3);
            this.credentials.Controls.Add(this.PasswordBox2);
            this.credentials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.credentials.Location = new System.Drawing.Point(0, 0);
            this.credentials.Name = "credentials";
            this.credentials.Size = new System.Drawing.Size(457, 114);
            this.credentials.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.label1.MinimumSize = new System.Drawing.Size(150, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Login:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LoginBox
            // 
            this.LoginBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LoginBox.Location = new System.Drawing.Point(159, 6);
            this.LoginBox.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.LoginBox.MaxLength = 32;
            this.LoginBox.Name = "LoginBox";
            this.LoginBox.Size = new System.Drawing.Size(200, 26);
            this.LoginBox.TabIndex = 1;
            this.LoginBox.TextChanged += new System.EventHandler(this.textChanged);
            // 
            // RegisterBtn
            // 
            this.RegisterBtn.Enabled = false;
            this.RegisterBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.RegisterBtn.Location = new System.Drawing.Point(365, 3);
            this.RegisterBtn.Name = "RegisterBtn";
            this.RegisterBtn.Size = new System.Drawing.Size(84, 32);
            this.RegisterBtn.TabIndex = 7;
            this.RegisterBtn.Text = "Register";
            this.RegisterBtn.UseVisualStyleBackColor = true;
            this.RegisterBtn.Click += new System.EventHandler(this.RegisterBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(3, 44);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.label2.MinimumSize = new System.Drawing.Size(150, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Enter Password:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PasswordBox1
            // 
            this.PasswordBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PasswordBox1.Location = new System.Drawing.Point(159, 44);
            this.PasswordBox1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.PasswordBox1.MaxLength = 32;
            this.PasswordBox1.Name = "PasswordBox1";
            this.PasswordBox1.PasswordChar = '*';
            this.PasswordBox1.Size = new System.Drawing.Size(200, 26);
            this.PasswordBox1.TabIndex = 3;
            this.PasswordBox1.TextChanged += new System.EventHandler(this.textChanged);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CancelBtn.Location = new System.Drawing.Point(365, 41);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(84, 32);
            this.CancelBtn.TabIndex = 6;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(3, 79);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.MinimumSize = new System.Drawing.Size(150, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 26);
            this.label3.TabIndex = 4;
            this.label3.Text = "ReEnter Password:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PasswordBox2
            // 
            this.PasswordBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PasswordBox2.Location = new System.Drawing.Point(159, 79);
            this.PasswordBox2.MaxLength = 32;
            this.PasswordBox2.Name = "PasswordBox2";
            this.PasswordBox2.PasswordChar = '*';
            this.PasswordBox2.Size = new System.Drawing.Size(200, 26);
            this.PasswordBox2.TabIndex = 5;
            this.PasswordBox2.TextChanged += new System.EventHandler(this.textChanged);
            // 
            // CreateNewUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(457, 136);
            this.ControlBox = false;
            this.Controls.Add(this.credentials);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "CreateNewUserForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create New User";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.credentials.ResumeLayout(false);
            this.credentials.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.FlowLayoutPanel credentials;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox LoginBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PasswordBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox PasswordBox2;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button RegisterBtn;
    }
}

