using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kisildalur.Pages
{
	/// <summary>
	/// Interaction logic for User.xaml
	/// </summary>
	public partial class User : Page
	{
		public User()
		{
			InitializeComponent();
			this.DataContextChanged += new DependencyPropertyChangedEventHandler(User_DataContextChanged);
		}

		void User_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (this.DataContext != null)
			{
				_user = (Database.User)this.DataContext;
				if (_user.ID == -1)
					textblockWarning.Visibility = Visibility.Collapsed;
                if (_popup == null)
                {
                    _popup = ((this.Tag as Frame).Parent as Border).Parent as Popup;
                    _popup.Opened += new EventHandler(_popup_Opened);
                }
			}
		}

        void _popup_Opened(object sender, EventArgs e)
        {
            if (this._user.ID == -1)
            {
                this.textboxName.Text = "";
                this.textboxPassword.Password = "";
            }
        }

		Database.User _user;
		Popup _popup;

		private void buttonSearchRight_Click(object sender, RoutedEventArgs e)
		{
			_user.Name = textboxName.Text;
			switch (comboboxPrivilegs.SelectedIndex)
			{
				case 0:
					_user.Privileges = Database.UserPrivileges.Seller;
					break;
				default:
					_user.Privileges = Database.UserPrivileges.Admin;
					break;
			}
			if (!string.IsNullOrEmpty(textboxPassword.Password))
			{
				MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
				byte[] sec = Encoding.UTF8.GetBytes(textboxPassword.Password.ToLower());
				sec = x.ComputeHash(sec);
				StringBuilder s = new StringBuilder();
				foreach (byte b in sec)
					s.Append(b.ToString("x2"));
				_user.Hash = s.ToString();
			}
			if (_user.ID != -1)
				Main.DB.Users.Update(_user);
			else
				Main.DB.Users.Add(_user, true);
			buttonSearchLeft_Click(null, null);
		}

		private void buttonSearchLeft_Click(object sender, RoutedEventArgs e)
		{
			Storyboard sizePopupClose = (Storyboard)_popup.FindResource("sizePopupClose"); // (Storyboard)FindResource("sizePopupClose");
			sizePopupClose.Begin(_popup);
		}
	}
}
