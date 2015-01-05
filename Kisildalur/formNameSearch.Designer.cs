namespace Kisildalur
{
    partial class formNameSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formNameSearch));
            this._group1 = new System.Windows.Forms.GroupBox();
            this._name = new System.Windows.Forms.TextBox();
            this._group2 = new System.Windows.Forms.GroupBox();
            this._results = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this._cancel = new System.Windows.Forms.Button();
            this._loadKennitala = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this._statusProgress = new System.Windows.Forms.ToolStripProgressBar();
            this._statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this._worker = new System.ComponentModel.BackgroundWorker();
            this._group1.SuspendLayout();
            this._group2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _group1
            // 
            this._group1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._group1.Controls.Add(this._name);
            this._group1.Location = new System.Drawing.Point(12, 12);
            this._group1.Name = "_group1";
            this._group1.Size = new System.Drawing.Size(274, 50);
            this._group1.TabIndex = 0;
            this._group1.TabStop = false;
            this._group1.Text = "Nafn";
            // 
            // _name
            // 
            this._name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._name.Location = new System.Drawing.Point(6, 19);
            this._name.Name = "_name";
            this._name.Size = new System.Drawing.Size(262, 20);
            this._name.TabIndex = 0;
            this._name.TextChanged += new System.EventHandler(this._name_TextChanged);
            // 
            // _group2
            // 
            this._group2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._group2.Controls.Add(this._results);
            this._group2.Location = new System.Drawing.Point(12, 68);
            this._group2.Name = "_group2";
            this._group2.Size = new System.Drawing.Size(274, 162);
            this._group2.TabIndex = 1;
            this._group2.TabStop = false;
            this._group2.Text = "Niðurstöður";
            // 
            // _results
            // 
            this._results.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._results.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this._results.FullRowSelect = true;
            this._results.Location = new System.Drawing.Point(6, 19);
            this._results.Name = "_results";
            this._results.Size = new System.Drawing.Size(262, 137);
            this._results.TabIndex = 0;
            this._results.UseCompatibleStateImageBehavior = false;
            this._results.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Nafn";
            this.columnHeader1.Width = 160;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Kennitala";
            this.columnHeader2.Width = 85;
            // 
            // _cancel
            // 
            this._cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._cancel.Location = new System.Drawing.Point(12, 236);
            this._cancel.Name = "_cancel";
            this._cancel.Size = new System.Drawing.Size(93, 23);
            this._cancel.TabIndex = 2;
            this._cancel.Text = "Hætta við";
            this._cancel.UseVisualStyleBackColor = true;
            this._cancel.Click += new System.EventHandler(this._cancel_Click);
            // 
            // _loadKennitala
            // 
            this._loadKennitala.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._loadKennitala.Location = new System.Drawing.Point(193, 236);
            this._loadKennitala.Name = "_loadKennitala";
            this._loadKennitala.Size = new System.Drawing.Size(93, 23);
            this._loadKennitala.TabIndex = 3;
            this._loadKennitala.Text = "Sækja kennitölu";
            this._loadKennitala.UseVisualStyleBackColor = true;
            this._loadKennitala.Click += new System.EventHandler(this._loadKennitala_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusText,
            this._statusProgress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 262);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(298, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // _statusProgress
            // 
            this._statusProgress.Name = "_statusProgress";
            this._statusProgress.Size = new System.Drawing.Size(100, 16);
            // 
            // _statusText
            // 
            this._statusText.Name = "_statusText";
            this._statusText.Size = new System.Drawing.Size(25, 17);
            this._statusText.Text = "Idle";
            // 
            // _worker
            // 
            this._worker.WorkerReportsProgress = true;
            this._worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._worker_DoWork);
            this._worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._worker_RunWorkerCompleted);
            this._worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this._worker_ProgressChanged);
            // 
            // formNameSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 284);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this._loadKennitala);
            this.Controls.Add(this._cancel);
            this.Controls.Add(this._group2);
            this.Controls.Add(this._group1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "formNameSearch";
            this.Text = "Leita eftir nafni";
            this._group1.ResumeLayout(false);
            this._group1.PerformLayout();
            this._group2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox _group1;
        private System.Windows.Forms.TextBox _name;
        private System.Windows.Forms.GroupBox _group2;
        private System.Windows.Forms.ListView _results;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button _cancel;
        private System.Windows.Forms.Button _loadKennitala;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel _statusText;
        private System.Windows.Forms.ToolStripProgressBar _statusProgress;
        private System.ComponentModel.BackgroundWorker _worker;
    }
}