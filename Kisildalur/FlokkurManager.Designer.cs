namespace Kisildalur
{
    partial class FlokkurManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlokkurManager));
            this._group3 = new System.Windows.Forms.GroupBox();
            this._text = new System.Windows.Forms.TextBox();
            this._save = new System.Windows.Forms.Button();
            this._closeSave = new System.Windows.Forms.Button();
            this._change = new System.Windows.Forms.Button();
            this._newRoot = new System.Windows.Forms.Button();
            this._delete = new System.Windows.Forms.Button();
            this._list = new System.Windows.Forms.TreeView();
            this._group2 = new System.Windows.Forms.GroupBox();
            this._edit = new System.Windows.Forms.Button();
            this._group1 = new System.Windows.Forms.GroupBox();
            this._newParent = new System.Windows.Forms.Button();
            this._cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this._group3.SuspendLayout();
            this._group2.SuspendLayout();
            this._group1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _group3
            // 
            this._group3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._group3.Controls.Add(this._text);
            this._group3.Location = new System.Drawing.Point(12, 264);
            this._group3.Name = "_group3";
            this._group3.Size = new System.Drawing.Size(405, 54);
            this._group3.TabIndex = 1;
            this._group3.TabStop = false;
            this._group3.Text = "Nafn";
            // 
            // _text
            // 
            this._text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._text.Location = new System.Drawing.Point(16, 19);
            this._text.Name = "_text";
            this._text.ReadOnly = true;
            this._text.Size = new System.Drawing.Size(368, 20);
            this._text.TabIndex = 0;
            // 
            // _save
            // 
            this._save.Enabled = false;
            this._save.Location = new System.Drawing.Point(6, 19);
            this._save.Name = "_save";
            this._save.Size = new System.Drawing.Size(102, 23);
            this._save.TabIndex = 2;
            this._save.Text = "Vista breytingar";
            this._save.UseVisualStyleBackColor = true;
            this._save.Click += new System.EventHandler(this._save_Click);
            // 
            // _closeSave
            // 
            this._closeSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._closeSave.Location = new System.Drawing.Point(309, 324);
            this._closeSave.Name = "_closeSave";
            this._closeSave.Size = new System.Drawing.Size(108, 23);
            this._closeSave.TabIndex = 3;
            this._closeSave.Text = "Vista og loka";
            this._closeSave.UseVisualStyleBackColor = true;
            this._closeSave.Click += new System.EventHandler(this._closeSave_Click);
            // 
            // _change
            // 
            this._change.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._change.Enabled = false;
            this._change.Location = new System.Drawing.Point(271, 41);
            this._change.Name = "_change";
            this._change.Size = new System.Drawing.Size(75, 23);
            this._change.TabIndex = 4;
            this._change.Text = "Breyta";
            this._change.UseVisualStyleBackColor = true;
            // 
            // _newRoot
            // 
            this._newRoot.Location = new System.Drawing.Point(6, 19);
            this._newRoot.Name = "_newRoot";
            this._newRoot.Size = new System.Drawing.Size(102, 23);
            this._newRoot.TabIndex = 5;
            this._newRoot.Text = "Nýr folder (rót)";
            this._newRoot.UseVisualStyleBackColor = true;
            this._newRoot.Click += new System.EventHandler(this._newRoot_Click);
            // 
            // _delete
            // 
            this._delete.Enabled = false;
            this._delete.Location = new System.Drawing.Point(6, 78);
            this._delete.Name = "_delete";
            this._delete.Size = new System.Drawing.Size(102, 23);
            this._delete.TabIndex = 7;
            this._delete.Text = "Eyða";
            this._delete.UseVisualStyleBackColor = true;
            this._delete.Click += new System.EventHandler(this._delete_Click);
            // 
            // _list
            // 
            this._list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._list.CheckBoxes = true;
            this._list.Location = new System.Drawing.Point(12, 12);
            this._list.Name = "_list";
            this._list.Size = new System.Drawing.Size(285, 210);
            this._list.TabIndex = 8;
            this._list.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._list_AfterSelect);
            // 
            // _group2
            // 
            this._group2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._group2.Controls.Add(this._edit);
            this._group2.Controls.Add(this._save);
            this._group2.Controls.Add(this._delete);
            this._group2.Location = new System.Drawing.Point(303, 97);
            this._group2.Name = "_group2";
            this._group2.Size = new System.Drawing.Size(114, 113);
            this._group2.TabIndex = 9;
            this._group2.TabStop = false;
            this._group2.Text = "Breyta";
            // 
            // _edit
            // 
            this._edit.Enabled = false;
            this._edit.Location = new System.Drawing.Point(6, 49);
            this._edit.Name = "_edit";
            this._edit.Size = new System.Drawing.Size(102, 23);
            this._edit.TabIndex = 8;
            this._edit.Text = "Breyta";
            this._edit.UseVisualStyleBackColor = true;
            this._edit.Click += new System.EventHandler(this._edit_Click);
            // 
            // _group1
            // 
            this._group1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._group1.Controls.Add(this._newParent);
            this._group1.Controls.Add(this._newRoot);
            this._group1.Location = new System.Drawing.Point(303, 12);
            this._group1.Name = "_group1";
            this._group1.Size = new System.Drawing.Size(114, 79);
            this._group1.TabIndex = 10;
            this._group1.TabStop = false;
            this._group1.Text = "Bæta";
            // 
            // _newParent
            // 
            this._newParent.Enabled = false;
            this._newParent.Location = new System.Drawing.Point(6, 49);
            this._newParent.Name = "_newParent";
            this._newParent.Size = new System.Drawing.Size(102, 23);
            this._newParent.TabIndex = 6;
            this._newParent.Text = "Nýr flokkur";
            this._newParent.UseVisualStyleBackColor = true;
            this._newParent.Click += new System.EventHandler(this._newParent_Click);
            // 
            // _cancel
            // 
            this._cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._cancel.Location = new System.Drawing.Point(12, 324);
            this._cancel.Name = "_cancel";
            this._cancel.Size = new System.Drawing.Size(108, 23);
            this._cancel.TabIndex = 11;
            this._cancel.Text = "Hætta við";
            this._cancel.UseVisualStyleBackColor = true;
            this._cancel.Click += new System.EventHandler(this._cancel_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(9, 225);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(288, 35);
            this.label1.TabIndex = 12;
            this.label1.Text = "Kassinn fyrir framan nöfnin segja til um hvort flokkurinn eða folderið er sjánleg" +
                "t á vefsíðunni";
            // 
            // FlokkurManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 359);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._cancel);
            this.Controls.Add(this._closeSave);
            this.Controls.Add(this._group1);
            this.Controls.Add(this._group2);
            this.Controls.Add(this._list);
            this.Controls.Add(this._group3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(200, 271);
            this.Name = "FlokkurManager";
            this.Text = "Flokkar";
            this.SizeChanged += new System.EventHandler(this.FlokkurManager_SizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FlokkurManager_FormClosing);
            this._group3.ResumeLayout(false);
            this._group3.PerformLayout();
            this._group2.ResumeLayout(false);
            this._group1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _group3;
        private System.Windows.Forms.TextBox _text;
        private System.Windows.Forms.Button _save;
        private System.Windows.Forms.Button _closeSave;
        private System.Windows.Forms.Button _change;
        private System.Windows.Forms.Button _newRoot;
        private System.Windows.Forms.Button _delete;
        private System.Windows.Forms.TreeView _list;
        private System.Windows.Forms.GroupBox _group2;
        private System.Windows.Forms.Button _edit;
        private System.Windows.Forms.GroupBox _group1;
        private System.Windows.Forms.Button _newParent;
        private System.Windows.Forms.Button _cancel;
        private System.Windows.Forms.Label label1;
    }
}