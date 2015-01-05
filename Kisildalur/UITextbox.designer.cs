partial class UITextbox
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UITextbox));
        this._label = new System.Windows.Forms.Label();
        this._textbox = new System.Windows.Forms.TextBox();
        this._button = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // _label
        // 
        this._label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this._label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._label.Location = new System.Drawing.Point(12, 9);
        this._label.Name = "_label";
        this._label.Size = new System.Drawing.Size(380, 17);
        this._label.TabIndex = 0;
        this._label.Text = "label1";
        // 
        // _textbox
        // 
        this._textbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this._textbox.Location = new System.Drawing.Point(54, 37);
        this._textbox.Name = "_textbox";
        this._textbox.Size = new System.Drawing.Size(294, 20);
        this._textbox.TabIndex = 1;
        // 
        // _button
        // 
        this._button.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this._button.Location = new System.Drawing.Point(151, 63);
        this._button.Name = "_button";
        this._button.Size = new System.Drawing.Size(98, 23);
        this._button.TabIndex = 2;
        this._button.Text = "button1";
        this._button.UseVisualStyleBackColor = true;
        this._button.Click += new System.EventHandler(this._button_Click);
        // 
        // UITextbox
        // 
        this.AcceptButton = this._button;
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(404, 98);
        this.Controls.Add(this._button);
        this.Controls.Add(this._textbox);
        this.Controls.Add(this._label);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.Name = "UITextbox";
        this.Text = "UITextbox";
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label _label;
    private System.Windows.Forms.TextBox _textbox;
    private System.Windows.Forms.Button _button;
}