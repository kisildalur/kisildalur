using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Database;

namespace Kisildalur
{
	public partial class FlokkurManager : Form
    {
		public FlokkurManager()
        {
            InitializeComponent();

			this.Size = Properties.config.Default.fmSize;
            this.CenterToScreen();

            for (int I = 0; I < Main.DB.Folders.Count; I++)
            {
                TreeNode node = new TreeNode(Main.DB.Folders[I].Name);
                node.Tag = Main.DB.Folders[I].ID;
                node.Checked = Main.DB.Folders[I].Visible;
                for (int category = 0; category < Main.DB.Folders[I].Categories.Count; category++)
                {
                    TreeNode parent = new TreeNode(Main.DB.Folders[I].Categories[category].Name);
                    parent.Tag = Main.DB.Folders[I].Categories[category].ID;
                    parent.Checked = Main.DB.Folders[I].Categories[category].Visible;
                    node.Nodes.Add(parent);
                }
                _list.Nodes.Add(node);
            }

            Main.DB.Connect();
        }

        private void _closeSave_Click(object sender, EventArgs e)
        {
            for (int I = 0; I < Main.DB.Folders.Count; I++)
            {
                if (Main.DB.Folders[I].Deleted == false)
                {
                    for (int root = 0; root < _list.Nodes.Count; root++)
                    {
                        if ((int)_list.Nodes[root].Tag == Main.DB.Folders[I].ID)
                        {
                            Main.DB.Folders[I].Visible = _list.Nodes[root].Checked;
                            break;
                        }
                    }
                    Main.DB.Folders.Update(Main.DB.Folders[I]);
                    for (int category = 0; category < Main.DB.Folders[I].Categories.Count; category++)
                    {
                        if (Main.DB.Folders[I].Categories[category].Deleted == false)
                        {
                            for (int root = 0; root < _list.Nodes.Count; root++)
                            {
                                for (int parent = 0; parent < _list.Nodes[root].Nodes.Count; parent++)
                                {
                                    if ((int)_list.Nodes[root].Nodes[parent].Tag == Main.DB.Folders[I].Categories[category].ID)
                                    {
                                        Main.DB.Folders[I].Categories[category].Visible = _list.Nodes[root].Nodes[parent].Checked;
                                        root = _list.Nodes.Count;
                                        break;
                                    }
                                }
                            }
                            Main.DB.Folders[I].Categories.Update(Main.DB.Folders[I].Categories[category]);
                        }
                        else
                            Main.DB.Folders[I].Categories.Remove(Main.DB.Folders[I].Categories[category], true);
                    }
                }
                else
                    Main.DB.Folders.Remove(Main.DB.Folders[I], true);
            }
            this.Close();
        }

        private void _newRoot_Click(object sender, EventArgs e)
        {
            _text.Text = "";
            _text.ReadOnly = true;
            _save.Enabled = false;

            UITextbox ui = new UITextbox("Vinsamlegast skrifaðu inn nafnið á albúminu", "Nýtt albúm", "Búa til albúm");
            ui.ShowDialog();
            string name = ui.Input;
            ui.Dispose();

            if (name != "")
            {
                Folder album = new Folder();
                album.Name = name;
                album.Visible = false;
                Main.DB.Folders.Add(album, true);
                album.Update();

                TreeNode node = new TreeNode(name);
                node.Tag = album.ID;
                _list.Nodes.Add(node);
            }
        }

        private void _newParent_Click(object sender, EventArgs e)
        {
            _text.Text = "";
            _text.ReadOnly = true;
            _save.Enabled = false;

            UITextbox ui = new UITextbox("Vinsamlegast skrifaðu inn nafnið á flokknum", "Nýr flokkur", "Búa til flokk");
            ui.ShowDialog();
            string name = ui.Input;
            ui.Dispose();

            if (name != "")
            {
                Category cat = new Category();
                cat.Name = name;
                cat.Visible = false;

                TreeNode node = new TreeNode(name);
                if (_list.SelectedNode.Level == 1)
                {
                    Main.DB.Folders[(int)_list.SelectedNode.Parent.Tag, true].Categories.Add(cat, true);
                    node.Tag = cat.ID;
                    _list.SelectedNode.Parent.Nodes.Add(node);
                }
                else
                {
                    Main.DB.Folders[(int)_list.SelectedNode.Tag, true].Categories.Add(cat, true);
                    node.Tag = cat.ID;
                    _list.SelectedNode.Nodes.Add(node);
                }
            }
        }

        private void _edit_Click(object sender, EventArgs e)
        {
            _edit.Enabled = false;
            _save.Enabled = true;
            _text.ReadOnly = false;
            _text.Text = _list.SelectedNode.Text;
        }

        private void _save_Click(object sender, EventArgs e)
        {
            _save.Enabled = false;
            _text.ReadOnly = true;
            if (_text.Text != "")
            {
                if (_list.SelectedNode.Level == 1)
                    Main.DB.Folders[(int)_list.SelectedNode.Parent.Tag, true].Categories[(int)_list.SelectedNode.Tag, true].Name = _text.Text;
                else
                    Main.DB.Folders[(int)_list.SelectedNode.Tag, true].Name = _text.Text;

                _list.SelectedNode.Text = _text.Text;
            }
            _text.Text = "";
        }

        private void _delete_Click(object sender, EventArgs e)
        {
            _text.Text = "";
            _text.ReadOnly = true;
            _save.Enabled = false;

            if (_list.SelectedNode.Level == 1)
            {
                if (MessageBox.Show("Ertu viss um að Þú viljir eyða þessum flokki?", "Eyða flokk", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Main.DB.Folders[(int)_list.SelectedNode.Parent.Tag, true].Categories[(int)_list.SelectedNode.Tag, true].Deleted = true;

                    _list.Nodes.Remove(_list.SelectedNode);
                }
            }
            else
            {
                if (MessageBox.Show("Ertu viss um að Þú viljir eyða þessu albúmi og öllum undirflokkum?", "Eyða album", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Main.DB.Folders[(int)_list.SelectedNode.Tag, true].Deleted = true;

                    _list.Nodes.Remove(_list.SelectedNode);
                }
            }
        }

		private void FlokkurManager_SizeChanged(object sender, EventArgs e)
		{
			Properties.config.Default.fmSize = this.Size;
		}

        private void _list_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _text.ReadOnly = true;
            _text.Text = "";
            if (_list.SelectedNode != null)
            {
                _newParent.Enabled = true;
                _edit.Enabled = true;
                _save.Enabled = false;
                _delete.Enabled = true;
            }
            else
            {
                _newParent.Enabled = false;
                _edit.Enabled = false;
                _save.Enabled = false;
                _delete.Enabled = false;
            }
        }

        private void _cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FlokkurManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int I = 0; I < Main.DB.Folders.Count; I++)
            {
                Main.DB.Folders[I].Deleted = false;
                for (int category = 0; category < Main.DB.Folders[I].Categories.Count; category++)
                {
                    Main.DB.Folders[I].Categories[category].Deleted = false;
                }
            }
            Main.DB.Disconnect();
        }
    }
}
