using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Kisildalur.Forms
{
    public partial class ButtonExpandGroup : UserControl
    {
        public ButtonExpandGroup()
        {
            InitializeComponent();

            _defaultHeightIsRelative = false;
        }

        private GroupBox _group;
        private int _defaultHeight;
        private bool _state;
        private bool _defaultHeightIsRelative;

        public GroupBox Group
        {
            get { return _group; }
            set { _group = value; }
        }
        public int DefaultHeight
        {
            get { return _defaultHeight; }
            set { _defaultHeight = value; }
        }
        public bool State1
        {
            get { return _state; }
            set { _state = value; }
        }

        public bool DefaultHeightIsRelative
        {
            get { return _defaultHeightIsRelative; }
            set { _defaultHeightIsRelative = value; }
        }
        private void Expand_Click(object sender, EventArgs e)
        {
            int add = 0;
            if (!_defaultHeightIsRelative)
            {
                if (_state == false)
                {
                    _group.Size = new Size(_group.Width, _defaultHeight);
                    add = _defaultHeight - 19;
                    _expand.Text = "v";
                }
                else
                {
                    _group.Size = new Size(_group.Width, 19);
                    add = 19 - _defaultHeight;
                    _expand.Text = ">";
                }
            }
            else if (this.Parent.Parent.Parent.ToString() == "System.Windows.Forms.SplitContainer")
            {
                if (_state == false)
                {
                    //_group.Size = new Size(_group.Width, ((SplitContainer)this.Parent.Parent.Parent).Panel2.Height / _defaultHeight);
                    _group.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
                    add = ((SplitContainer)this.Parent.Parent.Parent).Panel2.Height / _defaultHeight - 19;
                    _expand.Text = "v";
                }
                else
                {
                    add = 19 - this.Group.Height;
                    _group.Size = new Size(_group.Width, 19);
                    _group.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
                    _expand.Text = ">";
                }
            }

            _state = !_state;

            if (this.Parent.Parent.Parent.ToString() == "System.Windows.Forms.SplitContainer")
            {
                ((SplitContainer)this.Parent.Parent.Parent).SplitterDistance += add;
            }

            foreach (Control c in this.Parent.Controls)
            {
                if (c.ToString() == "ButtonExpandGroup")
                {
                    ButtonExpandGroup button = ((ButtonExpandGroup)c);
                    if (button == this)
                        break;

                    button.Location = new Point(button.Location.X, button.Location.Y + add);
                    button.Group.Location = new Point(button.Group.Location.X, button.Group.Location.Y + add);

                    if (button.DefaultHeightIsRelative && button.State1)
                    {
                        button.Group.Size = new Size(button.Group.Size.Width, button.Group.Size.Height - add);
                    }
                }
            }
        }

        public override string ToString()
        {
            return "ButtonExpandGroup";
        }
    }
}
