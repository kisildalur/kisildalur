namespace Kisildalur
{
    partial class Main_Login
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_Login));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._pass = new System.Windows.Forms.TextBox();
			this._enter = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this._pass);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(356, 52);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Password";
			// 
			// _pass
			// 
			this._pass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._pass.Location = new System.Drawing.Point(16, 19);
			this._pass.Name = "_pass";
			this._pass.PasswordChar = ' ';
			this._pass.Size = new System.Drawing.Size(325, 20);
			this._pass.TabIndex = 0;
			// 
			// _enter
			// 
			this._enter.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this._enter.Location = new System.Drawing.Point(150, 71);
			this._enter.Name = "_enter";
			this._enter.Size = new System.Drawing.Size(75, 23);
			this._enter.TabIndex = 1;
			this._enter.Text = "Login";
			this._enter.UseVisualStyleBackColor = true;
			this._enter.Click += new System.EventHandler(this._enter_Click);
			// 
			// Main_Login
			// 
			this.AcceptButton = this._enter;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(380, 106);
			this.Controls.Add(this._enter);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Main_Login";
			this.Text = "Login";
			this.SizeChanged += new System.EventHandler(this.Login_SizeChanged);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button _enter;
        public System.Windows.Forms.TextBox _pass;
    }
}