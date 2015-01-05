using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Kisildalur
{
    public partial class UIListOptions : Form
    {
        /// <summary>
        /// Initialise a new instance of ListOptions with specified perimeters
        /// </summary>
        /// <param name="okText">The text of the OK Button</param>
        /// <param name="header">The text on the header</param>
        public UIListOptions(string okText, string header)
        {
            InitializeComponent();

			this.Size = Properties.config.Default.loSize;
            this.CenterToScreen();

            this._ok.Text = okText;
            this.Text = header;
        }

        /// <summary>
        /// Add a new item in the list options
        /// </summary>
        /// <param name="item">The items to be added in the list</param>
        public void _AddList(TreeNode node)
        {
            this._options.Nodes.Add(node);
        }

        /// <summary>
        /// Return the specified value the user selected
        /// </summary>
        public TreeNode _SelectedValue
        {
            get { return _options.SelectedNode; }
        }

        private void _ok_Click(object sender, EventArgs e)
        {
            if (_options.SelectedNode != null)
            {
                if (_options.SelectedNode.Level == 1)
                    this.Close();
                else
                    MessageBox.Show("Must be a category, not folder.", "Choose a category");
            }
            else
                MessageBox.Show("Must choose a category", "Choose a category");
        }

        private void _cancel_Click(object sender, EventArgs e)
        {
            _options.SelectedNode = null;
            this.Close();
        }

		private void ListOptions_SizeChanged(object sender, EventArgs e)
		{
			Properties.config.Default.loSize = this.Size;
		}
    }
}