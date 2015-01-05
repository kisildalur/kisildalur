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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kisildalur.Pages
{
	/// <summary>
	/// Interaction logic for PaymentCollectionEditor.xaml
	/// </summary>
	public partial class PaymentCollectionEditor : Page
	{
		public PaymentCollectionEditor()
		{
			InitializeComponent();
			this.listPaymethods.ItemsSource = Main.DB.PayMethods;
		}

		private void framePaymethod_Loaded(object sender, RoutedEventArgs e)
		{
			if (!(sender as Frame).HasContent)
			{
                _newFrame = sender as Frame;
				Pages.Payment page = new Kisildalur.Pages.Payment();
				page.Tag = (sender as Frame);
				page.DataContext = new Database.PayMethod();

				(sender as Frame).Navigate(page);
			}
		}

		private void buttonEditMethod_Click(object sender, RoutedEventArgs e)
		{
			(((sender as Button).Parent as StackPanel).Children[0] as Popup).IsOpen = true;
		}

		private void buttonAddMethod_Click(object sender, RoutedEventArgs e)
		{
            (_newFrame.Content as Payment).DataContext = new Database.PayMethod();
			(((sender as Button).Parent as StackPanel).Children[1] as Popup).IsOpen = true;
		}

        Frame _newFrame;

		private void frameEditPaymethod_Loaded(object sender, RoutedEventArgs e)
		{
			if (!(sender as Frame).HasContent)
			{
				Pages.Payment page = new Kisildalur.Pages.Payment();
				page.Tag = (sender as Frame);
				page.DataContext = (sender as Frame).DataContext;

				(sender as Frame).Navigate(page);
			}
		}

        private void buttonUppMethod_Click(object sender, RoutedEventArgs e)
        {
            int tempIndex = Main.DB.PayMethods.IndexOf((Database.PayMethod)listPaymethods.SelectedValue);
            MoveItem(tempIndex, tempIndex - 1);
        }

        private void buttonDownMethod_Click(object sender, RoutedEventArgs e)
        {
            int tempIndex = Main.DB.PayMethods.IndexOf((Database.PayMethod)listPaymethods.SelectedValue);
            MoveItem(tempIndex, tempIndex + 1);
        }

        private void MoveItem(int indexMethod, int indexReplaceMethod)
        {
            Database.PayMethod method = Main.DB.PayMethods[indexMethod];
            Database.PayMethod oldUppMethod = Main.DB.PayMethods[indexReplaceMethod];

            int temp = oldUppMethod.Order;
            oldUppMethod.Order = method.Order;
            method.Order = temp;

            Main.DB.PayMethods.Update(oldUppMethod);
            Main.DB.PayMethods.Update(method);

            Main.DB.PayMethods.RemoveAt(indexReplaceMethod);
            Main.DB.PayMethods.Insert(indexMethod, oldUppMethod);

            listPaymethods.SelectedValue = null;
            listPaymethods.SelectedValue = method;
        }
	}
}
