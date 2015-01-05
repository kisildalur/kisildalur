using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using Database;

namespace Kisildalur
{
	public partial class Settings_Old : Form
	{
		public Settings_Old()
		{
			InitializeComponent();

            _header1.Text = Properties.config.Default.header1;
            _header2.Text = Properties.config.Default.header2;
            _header3.Text = Properties.config.Default.header3;
            _header4.Text = Properties.config.Default.header4;
            _header5.Text = Properties.config.Default.header5;
            _header6.Text = Properties.config.Default.header6;
			_nextOrderNumber.Text = Properties.config.Default.order_id.ToString();

			this.Size = Properties.config.Default.settSize;
			this.splitContainer1.SplitterDistance = Properties.config.Default.settHeight;
			this.splitContainer2.SplitterDistance = Properties.config.Default.settWidth;
			this.splitContainer3.SplitterDistance = Properties.config.Default.settSplitTextbox;

            this.CenterToScreen();

            this._connectionHost.Text = Properties.config.Default.mysql_host;
            this._connectionPort.Text = Properties.config.Default.mysql_port;
            this._connectionUsername.Text = Properties.config.Default.mysql_user;
            this._connectionDatabase.Text = Properties.config.Default.mysql_database;

            Main.DB.Connect();
		}

        private void _refreshPreview_Click(object sender, EventArgs e)
        {
            _viewPreview.InvalidatePreview();
        }

        private void _zoomIn_Click(object sender, EventArgs e)
        {
            _viewPreview.Zoom = _viewPreview.Zoom + 0.1;
        }

        private void _zoomOut_Click(object sender, EventArgs e)
        {
            _viewPreview.Zoom = _viewPreview.Zoom - 0.1;
        }

        private void _preview_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            return;
        }

        private void _preview_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Rectangle r = new Rectangle(50, 50, e.PageBounds.Width - 125, e.PageBounds.Height - 100);
            int x = r.Width;
            int y = r.Height;
            Graphics g = e.Graphics;
            g.DrawImage(Image.FromFile("logo.png"), new Rectangle(r.X, r.Y, Convert.ToInt32(x * 0.47), Convert.ToInt32(y * 0.07632)));
            g.DrawString(_header1.Text + "\n" + _header2.Text + "\n" + _header3.Text, new Font("Arial", 10), Brushes.Black, r.X + x * 0.48f, r.Y);
            g.DrawString(_header4.Text + "\n" + _header5.Text + "\n" + _header6.Text, new Font("Arial", 10), Brushes.Black, r.X + x * 0.7435135f, r.Y);
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.DB.Disconnect();

			Properties.config.Default.settSize = this.Size;
			Properties.config.Default.settHeight = splitContainer1.SplitterDistance;
			Properties.config.Default.settWidth = splitContainer2.SplitterDistance;
			Properties.config.Default.settSplitTextbox = splitContainer3.SplitterDistance;
        }

		private void _save_Click(object sender, EventArgs e)
		{
			Properties.config.Default.header1 = _header1.Text;
			Properties.config.Default.header2 = _header2.Text;
			Properties.config.Default.header3 = _header3.Text;
			Properties.config.Default.header4 = _header4.Text;
			Properties.config.Default.header5 = _header5.Text;
			Properties.config.Default.header6 = _header6.Text;

            Properties.config.Default.mysql_host = this._connectionHost.Text;
            Properties.config.Default.mysql_port = this._connectionPort.Text;
            Properties.config.Default.mysql_user = this._connectionUsername.Text;
            Properties.config.Default.mysql_database = this._connectionDatabase.Text;

			Properties.config.Default.Save();

			this.Close();
		}

		private void _editOrderNumber_Click(object sender, EventArgs e)
		{
			UITextbox textbox = new UITextbox("Vinsamlegast skrifaðu inn næsta pöntunarnúmer eins og kemur fram á blaði hjá prentaranum", "Næsta pöntunarnúmer", "Vista pöntunarnúmer");
			textbox.ShowDialog();
			string temp = textbox.Input;
			if (temp != "")
			{
				int id = -1;
				if (int.TryParse(temp, out id))
				{
					Properties.config.Default.order_id = id;
					_nextOrderNumber.Text = id.ToString();
					Properties.config.Default.Save();
				}
				else
					MessageBox.Show("Invalid number", "Error");
			}
			textbox.Dispose();
		}

		private void _tabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_tabs.SelectedIndex == 1)
			{
				_userList.Items.Clear();

				_pass.Text = "";
				_name.Text = "";
				_pass.ReadOnly = true;
				_name.ReadOnly = true;
				_saveUser.Enabled = false;
				_deleteUser.Enabled = false;
				_changeUser.Enabled = false;

				foreach (User user in Main.DB.Users)
				{
					ListViewItem item = new ListViewItem(user.Name);
					item.Tag = user.ID;
					_userList.Items.Add(item);
				}

				_methodList.Items.Clear();

				_method.Text = "";
				_method.ReadOnly = true;
				_savePaymethod.Enabled = false;
				_deletePaymethod.Enabled = false;
				_changePaymethod.Enabled = false;

				foreach (PayMethod method in Main.DB.PayMethods)
				{
					ListViewItem item = new ListViewItem(method.Name);
					item.Tag = method.Id;
					_methodList.Items.Add(item);
				}
			}
		}

		#region Users

		private void _newUser_Click(object sender, EventArgs e)
		{
			_pass.Text = "";
			_name.Text = "";
			_pass.ReadOnly = true;
			_name.ReadOnly = true;
			_saveUser.Enabled = false;

			UITextbox ui = new UITextbox("Vinsamlegast skrifaðu inn nafnið á notendanum", "Nýr Notandi", "Búa til notenda");
			ui.ShowDialog();
			string name = ui.Input;
			ui.Dispose();

			if (name != "")
			{

				User f = new User();
				f.Name = name;
				Main.DB.Users.Add(f, true);

				ListViewItem l = new ListViewItem(name);
				l.Tag = f.ID;
				_userList.Items.Add(l);
			}
		}

		private void _saveUser_Click(object sender, EventArgs e)
		{
			_pass.ReadOnly = true;
			_name.ReadOnly = true;
			if (_pass.Text != "" && _name.Text != "")
			{
				MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
				byte[] sec = Encoding.UTF8.GetBytes(_pass.Text);
				sec = x.ComputeHash(sec);
				StringBuilder s = new StringBuilder();
				foreach (byte b in sec)
					s.Append(b.ToString("x2"));

				int id = (int)_userList.SelectedItems[0].Tag;
				Main.DB.Users[id, true].Name = _name.Text;
				Main.DB.Users[id, true].Hash = s.ToString();
				Main.DB.Users.Update(Main.DB.Users[id, true]);

				_userList.Items.Remove(_userList.SelectedItems[0]);
				ListViewItem l = new ListViewItem(Main.DB.Users[id, true].Name);
				l.Tag = Main.DB.Users[id, true].ID;
				_userList.Items.Add(l);
			}
			_pass.Text = "";
			_name.Text = "";
		}

		private void _deleteUser_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Ertu viss um að þú viljir eyða þessum user?", "Eyða user", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				int id = (int)_userList.SelectedItems[0].Tag;
				Main.DB.Users.Remove(Main.DB.Users[id, true], true);

				_userList.Items.Remove(_userList.SelectedItems[0]);
			}
		}

		private void _userList_SelectedIndexChanged(object sender, EventArgs e)
		{
			_pass.Text = "";
			_name.Text = "";
			_pass.ReadOnly = true;
			_name.ReadOnly = true;
			_saveUser.Enabled = false;
			_deleteUser.Enabled = false;
			if (_userList.SelectedItems.Count == 1)
			{
				_changeUser.Enabled = true;
				_deleteUser.Enabled = true;
			}
			else
				_changeUser.Enabled = false;
		}

		private void _changeUser_Click(object sender, EventArgs e)
		{
			_changeUser.Enabled = false;
			_saveUser.Enabled = true;
			_pass.ReadOnly = false;
			_name.ReadOnly = false;
			_name.Text = _userList.SelectedItems[0].Text;
		} 

		#endregion

		#region PayMethod

		private void _newPaymethod_Click(object sender, EventArgs e)
		{
			_method.Text = "";
			_method.ReadOnly = true;
			_savePaymethod.Enabled = false;

			UITextbox ui = new UITextbox("Vinsamlegast skrifaðu inn greiðsluaðferðina", "Ný greiðsluaðferð", "Búa til greiðsluaðferð");
			ui.ShowDialog();
			string name = ui.Input;
			ui.Dispose();

			if (name != "")
			{
				PayMethod method = new PayMethod();
				method.Name = name;
				Main.DB.PayMethods.Add(method, true);

				ListViewItem l = new ListViewItem(name);
				l.Tag = method.Id;
				_methodList.Items.Add(l);
			}
		}

		private void _changePaymethod_Click(object sender, EventArgs e)
		{
			_changePaymethod.Enabled = false;
			_savePaymethod.Enabled = true;
			_method.ReadOnly = false;
			_method.Text = _methodList.SelectedItems[0].Text;
		}

		private void _savePaymethod_Click(object sender, EventArgs e)
		{
			_method.ReadOnly = true;
			if (_method.Text != "")
			{
				int id = (int)_methodList.SelectedItems[0].Tag;
				Main.DB.PayMethods[id, true].Name = _method.Text;
				Main.DB.PayMethods.Update(Main.DB.PayMethods[id, true]);

				int index = 0;
				for (int i = 0; i < _methodList.Items.Count; i++)
					if ((int)_methodList.Items[i].Tag == (int)_methodList.SelectedItems[0].Tag)
					{
						index = i;
						break;
					}
				_methodList.Items.Remove(_methodList.SelectedItems[0]);
				ListViewItem l = new ListViewItem(Main.DB.PayMethods[id, true].Name);
				l.Tag = Main.DB.PayMethods[id, true].Id;
				_methodList.Items.Insert(index, l);
			}
			_method.Text = "";
		}

		private void _deletePaymethod_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Ertu viss um að þú viljir eyða þessari greiðslumáta?", "Eyða greiðslumáta", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				int id = (int)_methodList.SelectedItems[0].Tag;
				Main.DB.PayMethods.Remove(Main.DB.PayMethods[id, true], true);

				_methodList.Items.Remove(_methodList.SelectedItems[0]);
			}
		}

		private void _methodList_SelectedIndexChanged(object sender, EventArgs e)
		{
			_method.Text = "";
			_method.ReadOnly = true;
			_savePaymethod.Enabled = false;
			_deletePaymethod.Enabled = false;
			if (_methodList.SelectedItems.Count == 1)
			{
				_changePaymethod.Enabled = true;
				_deletePaymethod.Enabled = true;
			}
			else
				_changePaymethod.Enabled = false;
		}

		private void _moveUp_Click(object sender, EventArgs e)
		{
			if (_methodList.SelectedItems.Count == 1)
			{
				PayMethod method = Main.DB.PayMethods[(int)_methodList.SelectedItems[0].Tag, true];
				PayMethod nextMethod = null;

				foreach (PayMethod payMethod in Main.DB.PayMethods)
				{
					if (payMethod.Order < method.Order)
					{
						if (nextMethod != null)
						{
							if (nextMethod.Order < payMethod.Order)
								nextMethod = payMethod;
						}
						else
							nextMethod = payMethod;
					}
				}

				if (nextMethod != null)
				{
					int temp = nextMethod.Order;
					nextMethod.Order = method.Order;
					method.Order = temp;

					Main.DB.PayMethods.Update(method);
					Main.DB.PayMethods.Update(nextMethod);


					int index = 0;
					for (int i = 0; i < Main.DB.PayMethods.Count; i++)
						if (Main.DB.PayMethods[i].Id == method.Id)
						{
							index = i;
							break;
						}
					if (index > 0)
					{
						PayMethod tempMethod = new PayMethod(method.Id, method.Name, method.Order);
						Main.DB.PayMethods.Remove(method);
						Main.DB.PayMethods.Insert(index - 1, tempMethod);

						index = 0;
						for (int i = 0; i < _methodList.Items.Count; i++)
							if ((int)_methodList.Items[i].Tag == (int)_methodList.SelectedItems[0].Tag)
							{
								index = i;
								break;
							}

						ListViewItem item = (ListViewItem)_methodList.SelectedItems[0].Clone();
						_methodList.Items.Remove(_methodList.SelectedItems[0]);
						_methodList.Items.Insert(index - 1, item);
						_methodList.Items[index - 1].Selected = true;
					}
				}
			}
		}

		private void _moveDown_Click(object sender, EventArgs e)
		{
			if (_methodList.SelectedItems.Count == 1)
			{
				PayMethod method = Main.DB.PayMethods[(int)_methodList.SelectedItems[0].Tag, true];
				PayMethod nextMethod = null;

				foreach (PayMethod payMethod in Main.DB.PayMethods)
				{
					if (payMethod.Order > method.Order)
					{
						if (nextMethod != null)
						{
							if (nextMethod.Order > payMethod.Order)
								nextMethod = payMethod;
						}
						else
							nextMethod = payMethod;
					}
				}

				if (nextMethod != null)
				{
					int temp = nextMethod.Order;
					nextMethod.Order = method.Order;
					method.Order = temp;

					Main.DB.PayMethods.Update(method);
					Main.DB.PayMethods.Update(nextMethod);


					int index = 0;
					for (int i = 0; i < Main.DB.PayMethods.Count; i++)
						if (Main.DB.PayMethods[i].Id == method.Id)
						{
							index = i;
							break;
						}
					if (index < (Main.DB.PayMethods.Count - 1))
					{
						PayMethod tempMethod = new PayMethod(method.Id, method.Name, method.Order);
						Main.DB.PayMethods.Remove(method);
						Main.DB.PayMethods.Insert(index + 1, tempMethod);

						index = 0;
						for (int i = 0; i < _methodList.Items.Count; i++)
							if ((int)_methodList.Items[i].Tag == (int)_methodList.SelectedItems[0].Tag)
							{
								index = i;
								break;
							}


						ListViewItem item = (ListViewItem)_methodList.SelectedItems[0].Clone();
						_methodList.Items.Remove(_methodList.SelectedItems[0]);
						_methodList.Items.Insert(index + 1, item);
						_methodList.Items[index + 1].Selected = true;
					}
				}
			}
		}

		#endregion

		private void _cancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
