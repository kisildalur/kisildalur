using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Database;

namespace Kisildalur
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings(int usrId)
        {
            InitializeComponent();
			_userId = usrId;
			_previewDocument = new PrintDocument();
			_previewDocument.PrintPage += new PrintPageEventHandler(_previewDocument_PrintPage);
			_helper = new OrderPrinterHelper(Properties.config.Default.header1, Properties.config.Default.header2);
			_helper.Order = new Order();
			_helper.UserId = _userId;
			printPreview.Document = _previewDocument;
			printPreview.Zoom = 0.8;
			_userId = usrId;
        }

		OrderPrinterHelper _helper;
		PrintDocument _previewDocument;
		int _userId;

		void _previewDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			_helper.PrintHeaderOnly(e);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Properties.config.Default.Save();
		}

		private void buttonZoomIn_Click(object sender, RoutedEventArgs e)
		{
			printPreview.Zoom += 0.1;
		}

		private void buttonZoomOut_Click(object sender, RoutedEventArgs e)
		{
			printPreview.Zoom -= 0.1;
		}

		private void buttonZoomRefresh_Click(object sender, RoutedEventArgs e)
		{
			_helper = new OrderPrinterHelper(Properties.config.Default.header1, Properties.config.Default.header2);
			_helper.Order = new Order();
			_helper.UserId = _userId;
			printPreview.InvalidatePreview();
		}

		private void pageUserList_Loaded(object sender, RoutedEventArgs e)
		{
			if (!(sender as Frame).HasContent)
			{
				(sender as Frame).Navigate(new Kisildalur.Pages.UserCollectionEditor());
			}
		}

		private void pagePaymentList_Loaded(object sender, RoutedEventArgs e)
		{
			if (!(sender as Frame).HasContent)
			{
				(sender as Frame).Navigate(new Kisildalur.Pages.PaymentCollectionEditor());
			}
		}

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
