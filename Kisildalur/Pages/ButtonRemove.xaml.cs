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
    /// Interaction logic for ButtonSave.xaml
    /// </summary>
    public partial class ButtonRemove : UserControl
    {
        public ButtonRemove()
        {
            InitializeComponent();
        }

		public event EventHandler RemoveItem;

        private void frameRemove_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender as Frame).HasContent)
            {
                Pages.PopupConfirmateDelete page = new Kisildalur.Pages.PopupConfirmateDelete();
				page.RemoveItem += new EventHandler(page_RemoveItem);
                page.Tag = (sender as Frame);
                page.DataContext = (sender as Frame).DataContext;

                (sender as Frame).Navigate(page);
            }
        }

		void page_RemoveItem(object sender, EventArgs e)
		{
			if (RemoveItem != null)
				RemoveItem(sender, e);
		}

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            (((sender as Button).Parent as Grid).Children[0] as Popup).IsOpen = true;
        }
    }
}
