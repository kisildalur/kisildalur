using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Database;

namespace Kisildalur
{
	/// <summary>
	/// Interaction logic for ViewerCustomer.xaml
	/// </summary>
	public partial class ViewerCustomer : Window
	{
		private System.Drawing.Printing.PrintDocument _printReport;
		private CustomerPrinterHelper _printerHelper;
		private BackgroundWorker _worker;
		private OrderCollection _orders;
		private string _kennitala;

		public ViewerCustomer()
		{
			InitializeComponent();
			_printReport = new System.Drawing.Printing.PrintDocument();
			_printerHelper = new CustomerPrinterHelper(Properties.config.Default.header1, Properties.config.Default.header2);
			_worker = new BackgroundWorker();
			_orders = new OrderCollection();
			_worker.WorkerReportsProgress = false;
			_worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
			_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
			_printReport.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(_printReport_PrintPage);

			this.DataContext = new Customer();
		}

		public ViewerCustomer(Customer customer)
			: this()
		{
			this.DataContext = customer;
		}

		private void buttonClose_Click(object sender, RoutedEventArgs e)
		{
			Properties.config.Default.Save();
			this.Close();
		}

		private void buttonSaveClose_Click(object sender, RoutedEventArgs e)
		{
			if (this.DataContext != null)
			{
				Customer customer = this.DataContext as Customer;
				customer.Kennitala = this.textboxKennitala.Text;
				customer.Name = this.textboxName.Text;
				customer.Telephone = this.textboxTelephone.Text;
				customer.Gsm = this.textboxGsm.Text;
				customer.WorkPhone = this.textboxWorkPhone.Text;
				customer.Address1 = this.textboxAddress1.Text;
				customer.Address2 = this.textboxAddress2.Text;
				customer.City = this.textboxCity.Text;
				customer.Zip = this.textboxZip.Text;
				customer.Notes = this.textboxNotes.Text;
				customer.AlarmNotes = this.textboxAlarmNotes.Text;

				CustomerHandler handler = new CustomerHandler();
				handler.SaveCustomerDataToDatabase(customer);
			}
			buttonClose_Click(null, null);
		}

		void _worker_DoWork(object sender, DoWorkEventArgs e)
		{
			switch (e.Argument as string)
			{
				case "kennitala":
					try
					{
						PostSubmitter post = new PostSubmitter();
						post.Url = "http://www2.glitnir.is/tkaup/Main.asp";
						post.PostItems.Add("tbKennitala", _kennitala.Replace("-", ""));
						post.PostItems.Add("state", "1");
						post.Type = PostSubmitter.PostTypeEnum.Post;
						string result = post.Post();

						string regularExpressFinder = "<input type=\"text\" name=\"(?<name>[^\"]*)\" value=\"(?<value>[^\"]*)\"[^\\/]*/>";
						//                              <input type="text" name="tbHeimili" value="Gilsbakka 6" size="40" /

						Regex findAllValues = new Regex(regularExpressFinder);

						Customer searchCustomer = new Customer();

						if (findAllValues.IsMatch(result))
						{
							MatchCollection collection = findAllValues.Matches(result);

							foreach (Match m in collection)
							{
								for (int I = 0; I < m.Groups["name"].Captures.Count; I++)
								{
									switch (m.Groups["name"].Captures[0].Value)
									{
										case "tbNafn":
											searchCustomer.Name = m.Groups["value"].Captures[0].Value;
											break;

										case "tbHeimili":
											searchCustomer.Address1 = m.Groups["value"].Captures[0].Value;
											break;

										case "tbPostnumer":
											searchCustomer.Zip = m.Groups["value"].Captures[0].Value;
											break;

										case "tbSveitarfelag":
											searchCustomer.City = m.Groups["value"].Captures[0].Value;
											break;
									}
								}
							}
						}
						e.Result = searchCustomer;
					}
					catch (Exception error)
					{
						e.Result = error.Message;
					}
					break;

				case "orders":
					OrderCollection orders = new OrderCollection();
					DatabaseHelper.SearchDatabaseForOrders(null, ref orders, _kennitala, "", long.MinValue, long.MaxValue, DateTime.MinValue, DateTime.MaxValue, new List<string>());
					e.Result = orders;
					break;
			}
		}

		void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Result is string)
			{
				statusText.Text = e.Result as string;
			}
			else if (e.Result is Customer)
			{
				textboxName.Text = (e.Result as Customer).Name;
				textboxAddress1.Text = (e.Result as Customer).Address1;
				textboxZip.Text = (e.Result as Customer).Zip;
				textboxCity.Text = (e.Result as Customer).City;
			}
			else if (e.Result is OrderCollection)
			{
				Customer customer = this.DataContext as Customer;
				customer.Orders.Clear();
				foreach (Order order in e.Result as OrderCollection)
					customer.Orders.Add(order);
			}
			statusProgress.IsIndeterminate = false;
		}

		private void buttonUpdate_Click(object sender, RoutedEventArgs e)
		{
			tabcontrolMain.SelectedIndex = 0;

			if (!KennitalaIsLegal())
				return;

			statusText.Text = "Tengist þjóðskrá";
			statusProgress.IsIndeterminate = true;
			_kennitala = textboxKennitala.Text;
			if (!_worker.IsBusy)
				_worker.RunWorkerAsync("kennitala");
			else
				MessageBox.Show("Vinnsluvélin er upptekin, vinsamlegast reynið aftur seinna (íttirðu kannski 2 á takkann?).");
		}

		private void tabcontrolMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (tabcontrolMain.SelectedIndex == 1 && (this.DataContext as Customer).Orders.Count < 1)
			{
				if (!KennitalaIsLegal())
					return;

				statusProgress.IsIndeterminate = true;
				_kennitala = textboxKennitala.Text;
				_worker.RunWorkerAsync("orders");
			}
		}
		private bool KennitalaIsLegal()
		{
			if (!(textboxKennitala.Text.Length == 10 || textboxKennitala.Text.Length == 11))
			{
				statusText.Text = "Ólögleg kennitala";
				return false;
			}
			Regex regularExpression = new Regex("[0-9]{6}[-]?[0-9]{4}");
			if (!regularExpression.IsMatch(textboxKennitala.Text))
			{
				statusText.Text = "Kennitala verður að vera lögleg. Ekki skiptir máli hvort mínusinn sé eða ekki.";
				return false;
			}
			return true;
		}

		private void listviewOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			object temp = e.OriginalSource;
		}

		private void listviewOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (listviewOrders.SelectedItem != null)
			{
				ViewerOrder viewer = new ViewerOrder(listviewOrders.SelectedItem as Order);
				viewer.ShowDialog();
			}
		}

		private void PrintReport_Click(object sender, RoutedEventArgs e)
		{
			PrintDialog dialog = new PrintDialog();
			if ((this.DataContext as Customer).Orders.Count > 0)
			{
				_printerHelper.Customer = (this.DataContext as Customer);
				_printerHelper.OrderList = (this.DataContext as Customer).Orders;
				try
				{
					_printReport.Print();
				}
				catch (Exception err)
				{
					Main.DB.ErrorLog("Error while printing document", err.Message, err.ToString());
				}
			}
			else
				MessageBox.Show("Viðskiptavinur verður að hafa keypt að minnsta kosti einu sinni áður en hægt er að prenta skýrsluna út.");
		}

		void _printReport_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			_printerHelper.PrintPage(e);
		}

		private void buttonExport_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
			dialog.FileName = (this.DataContext as Customer).Name + " skýrsla (" + DateTime.Now.Year + ")";
			dialog.DefaultExt = ".ods";
			dialog.Filter = "OpenDocument Spreadsheet (*.ods)|*.ods";
			Nullable<bool> result = dialog.ShowDialog();
			if (result == true)
			{
				DocumentReport reporter = new DocumentReport(this.DataContext as Customer);
				try
				{
					reporter.Generate(dialog.FileName);
				}
				catch (Exception err)
				{
					MessageBox.Show("Error occured while generating report: \n\n\t" + err.Message, "Error generating report", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}
	}
}
