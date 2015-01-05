namespace Kisildalur.Forms
{
    partial class ButtonExpandGroup
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._expand = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _expand
            // 
            this._expand.Dock = System.Windows.Forms.DockStyle.Fill;
            this._expand.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._expand.Location = new System.Drawing.Point(0, 0);
            this._expand.Name = "_expand";
            this._expand.Size = new System.Drawing.Size(20, 19);
            this._expand.TabIndex = 0;
            this._expand.Text = ">";
            this._expand.UseVisualStyleBackColor = true;
            this._expand.Click += new System.EventHandler(this.Expand_Click);
            // 
            // ButtonExpandGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._expand);
            this.Name = "ButtonExpandGroup";
            this.Size = new System.Drawing.Size(20, 19);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _expand;
    }
}
