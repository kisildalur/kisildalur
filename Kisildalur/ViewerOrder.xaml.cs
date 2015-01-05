using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing.Printing;
using Database;

namespace Kisildalur
{
	/// <summary>
	/// Interaction logic for ViewerOrder.xaml
	/// </summary>
	public partial class ViewerOrder : Window
	{
		public ViewerOrder()
		{
			InitializeComponent();

            _printDocument = new PrintDocument();
            _printDocument.PrintPage += new PrintPageEventHandler(_printDocument_PrintPage);
            _printHelper = new OrderPrinterHelper(
                Properties.config.Default.header1,
                Properties.config.Default.header2);
		}

        void _printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            _printHelper.PrintPage(e);
        }

        OrderPrinterHelper _printHelper;
        PrintDocument _printDocument;

		public ViewerOrder(Order order)
			: this()
		{
			if (order.Customer.Id == -1)
			{
				CustomerHandler handler = new CustomerHandler();
				order.Customer = handler.RetreaveCustomer(order.Kennitala);
			}
			this.DataContext = order;
            this._printHelper.Order = order;
		}

		private void editCustomerInformation_Click(object sender, RoutedEventArgs e)
		{
			if (this.DataContext is Order)
			{
				ViewerCustomer viewer = new ViewerCustomer((this.DataContext as Order).Customer);
				viewer.ShowDialog();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

        private void buttonPrint_Click(object sender, RoutedEventArgs e)
        {
			if (_printHelper != null)
			{
				PrintDialog dialog = new PrintDialog();
				dialog.MinPage = 1;
				dialog.MaxPage = 3;
				dialog.UserPageRangeEnabled = true;
				if (dialog.ShowDialog() == true)
				{
					switch (dialog.PageRangeSelection)
					{
						case PageRangeSelection.AllPages:
							_printDocument.PrinterSettings.PrintRange = PrintRange.AllPages;
							break;
						case PageRangeSelection.UserPages:
							_printDocument.PrinterSettings.FromPage = dialog.PageRange.PageFrom;
							_printDocument.PrinterSettings.ToPage = dialog.PageRange.PageTo;
							_printDocument.PrinterSettings.PrintRange = PrintRange.SomePages;
							break;
					}
					_printHelper.ResetHelper();

					try
					{
						_printDocument.Print();
					}
					catch (Exception err)
					{
						Main.DB.ErrorLog("Error while printing document", err.Message, err.ToString());
					}
				}
			}
			else
			{
				System.Windows.MessageBox.Show("Printer helper has not been initialised.", "Printer not initialised");
			}
        }
	}
}
