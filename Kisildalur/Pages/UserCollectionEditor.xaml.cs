using System;
using System.Collections.Generic;
using System.Linq;
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
	/// Interaction logic for UserCollectionEditor.xaml
	/// </summary>
	public partial class UserCollectionEditor : Page
	{
		public UserCollectionEditor()
		{
			InitializeComponent();
			this.listUsers.ItemsSource = Main.DB.Users;
		}

		private void mainPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!(sender as Frame).HasContent)
			{
				Pages.User page = new Kisildalur.Pages.User();
				page.Tag = (sender as Frame);
				page.DataContext = (sender as Frame).DataContext;

				(sender as Frame).Navigate(page);
			}
		}

		private void buttonEditUser_Click(object sender, RoutedEventArgs e)
		{
			(((sender as Button).Parent as StackPanel).Children[0] as Popup).IsOpen = true;
		}

		private void buttonAddUser_Click(object sender, RoutedEventArgs e)
		{
            (_newFrame.Content as Pages.User).DataContext = new Database.User();
			(((sender as Button).Parent as StackPanel).Children[1] as Popup).IsOpen = true;
		}

        Frame _newFrame;

		private void mainAddPage_Loaded(object sender, RoutedEventArgs e)
		{
            _newFrame = sender as Frame;
			if (!(sender as Frame).HasContent)
			{
				Pages.User page = new Kisildalur.Pages.User();
				page.Tag = (sender as Frame);
				page.DataContext = new Database.User();

				(sender as Frame).Navigate(page);
			}
		}

        private void frameRemoveUser_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender as Frame).HasContent)
            {
                Pages.PopupConfirmateDelete page = new Kisildalur.Pages.PopupConfirmateDelete();
                page.Tag = (sender as Frame);
                page.DataContext = (sender as Frame).DataContext;

                (sender as Frame).Navigate(page);
            }
        }

        private void buttonRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            (((sender as Button).Parent as StackPanel).Children[2] as Popup).IsOpen = true;
        }
	}
}
