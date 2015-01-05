namespace Kisildalur.Forms
{
	partial class TextboxKennitala
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
			this._input = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// _input
			// 
			this._input.Dock = System.Windows.Forms.DockStyle.Fill;
			this._input.Location = new System.Drawing.Point(0, 0);
			this._input.Name = "_input";
			this._input.Size = new System.Drawing.Size(96, 20);
			this._input.TabIndex = 0;
			this._input.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._input_KeyPress);
			// 
			// TextboxKennitala
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._input);
			this.MaximumSize = new System.Drawing.Size(9999, 20);
			this.MinimumSize = new System.Drawing.Size(1, 20);
			this.Name = "TextboxKennitala";
			this.Size = new System.Drawing.Size(96, 20);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _input;
	}
}
