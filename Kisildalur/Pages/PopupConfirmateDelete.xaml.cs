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

using Timer = System.Windows.Forms.Timer;

namespace Kisildalur.Pages
{
    

    /// <summary>
    /// Interaction logic for PopupConfirmateDelete.xaml
    /// </summary>
    public partial class PopupConfirmateDelete : Page
    {
        public PopupConfirmateDelete()
        {
            InitializeComponent();
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(Payment_DataContextChanged);

            if (timer == null)
            {
                PopupConfirmateDelete.timer = new Timer();
                PopupConfirmateDelete.timer.Tick += new EventHandler(timer_Tick);
                PopupConfirmateDelete.timer.Interval = 1000 * 60 * 5;
            }
        }

        void Payment_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                _popup = (this.Tag as Frame).Parent as Popup;
                _popup.Opened += new EventHandler(_popup_Opened);
                this.DataContextChanged -= new DependencyPropertyChangedEventHandler(Payment_DataContextChanged);
            }
        }

        void _popup_Opened(object sender, EventArgs e)
        {
            this.textboxPassword.Password = "";
            if (PopupConfirmateDelete.timer.Enabled)
            {
                textblockWarning1.Visibility = Visibility.Collapsed;
                textboxPassword.Visibility = Visibility.Collapsed;
            }
        }


        Popup _popup;
		public event EventHandler RemoveItem;

        void timer_Tick(object sender, EventArgs e)
        {
            (sender as Timer).Enabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sizePopupClose = (Storyboard)_popup.FindResource("sizePopupClose"); // (Storyboard)FindResource("sizePopupClose");
            sizePopupClose.Begin(_popup);
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (textboxPassword.Visibility == Visibility.Collapsed)
            {
                Remove();

                Button_Click(null, null);
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(textboxPassword.Password))
                {
                    MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
                    byte[] sec = Encoding.UTF8.GetBytes(textboxPassword.Password);
                    sec = x.ComputeHash(sec);
                    StringBuilder s = new StringBuilder();
                    foreach (byte b in sec)
                        s.Append(b.ToString("x2"));

                    string hash = s.ToString();

                    foreach (Database.User user in Main.DB.Users)
                    {
                        if (user.Hash == hash)
                        {
                            if (user.Privileges == Database.UserPrivileges.Admin)
                            {
                                Remove();
                                PopupConfirmateDelete.timer.Enabled = true;
                                PopupConfirmateDelete.timer.Start();
                            }
                            else
                                MessageBox.Show("Villa, þú verður að vera admin til að geta eytt hlutum.", "Óleifileg aðgerð");

                            Button_Click(null, null);
                            return;
                        }
                    }
                    MessageBox.Show("Villa, vitlaust password.", "Rangur innsláttur");
                    Button_Click(null, null);
                    return;
                }
                else
                {
                    MessageBox.Show("Villa, þú verður að skrifa inn passwordið þitt", "Enginn insláttur");
                    Button_Click(null, null);
                    return;
                }
            }
        }

        private void Remove()
        {
            if (this.DataContext is Database.User)
                Main.DB.Users.Remove(this.DataContext as Database.User, true);
            else if (this.DataContext is Database.PayMethod)
                Main.DB.PayMethods.Remove(this.DataContext as Database.PayMethod, true);
			else if (this.DataContext is Database.Item)
			{
				foreach (Database.Category cat in Database.MainDatabase.GetDB.GetCategories())
				{
					if (cat.Items[(this.DataContext as Database.Item).ID, true] != null)
					{
						cat.Items.Remove(this.DataContext as Database.Item, true);
						break;
					}
				}
			}

			if (RemoveItem != null)
				RemoveItem(this.DataContext, new EventArgs());
        }

        public static Timer timer;
    }
}
