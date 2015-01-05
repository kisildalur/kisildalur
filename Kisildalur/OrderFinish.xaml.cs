using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Threading;
using System.Windows.Interop;
using Database;

namespace Kisildalur
{
	/// <summary>
	/// Interaction logic for OrderFinish.xaml
	/// </summary>
	public partial class OrderFinish : Window
	{
		public OrderFinish()
		{
			InitializeComponent();
			_previousNumber = orderId.Text;
			_customerSearchCollection = new CustomerCollection();
			_worker = new BackgroundWorker();
			_worker.WorkerReportsProgress = true;
			_worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
			_worker.DoWork += new DoWorkEventHandler(worker_DoWork);
			_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
			_printDocument = new PrintDocument();
			_printDocument.PrintPage += new PrintPageEventHandler(_printDocument_PrintPage);
			_printHelper = new OrderPrinterHelper(
				Properties.config.Default.header1,
				Properties.config.Default.header2);
		}

		void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			textBlockSearchStatus.Text = e.UserState as string;
		}

		void _printDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			_printHelper.PrintPage(e);
		}

		public OrderFinish(Order order) : this()
		{
			_order = order;
			_printHelper.Order = _order;
			if (_order.Payment.Count > 0)
			{
				if (_order.Payment[0].Name != "" && _order.Payment.Count < 2)
					_order.Payment[0].Amount = _order.Total;
				else if (_order.Payment.Count == 2)
					if (_order.Payment[0].Name != "" && string.IsNullOrEmpty(_order.Payment[1].Name))
						_order.Payment[0].Amount = _order.Total;
			}
			this.DataContext = _order;
		}

		Order _order;
		string _previousNumber;
        BackgroundWorker _worker;
		CustomerCollection _customerSearchCollection;
		string _searchKennitala;
		string _searchName;
		OrderPrinterHelper _printHelper;
		PrintDocument _printDocument;
		

		private void retreaveCustomerInformation_Click(object sender, RoutedEventArgs e)
		{
            if (!popupSearch.IsOpen)
            {
                popupSearch.IsOpen = true;

                Storyboard sizePopupOpen = (Storyboard)popupSearch.FindResource("sizePopupOpen"); // (Storyboard)FindResource("sizePopupClose");
                sizePopupOpen.Begin(this);

                if (!_worker.IsBusy)
                {
                    ClearVisibilityPopup();

                    _customerSearchCollection.Clear();

                    textBlockSearchStatus.Visibility = Visibility.Visible;
                    buttonSearchFill.Visibility = Visibility.Visible;

                    if (!string.IsNullOrEmpty(textBoxKennitala.Text))
                    {
						progressSearch.Visibility = Visibility.Visible;
						textBlockSearchStatus.Text = "Leita að kennitölu í gagnagrunni.";
						_searchKennitala = textBoxKennitala.Text;
						if (!_worker.IsBusy)
							_worker.RunWorkerAsync("kennitala");

                        /*_customerSearchCollection.Add(Main.DB.CustomerHandler.RetreaveCustomer(textBoxKennitala.Text.Replace("-", "")));

                        if (_customerSearchCollection[0].Id == -1 || (_customerSearchCollection[0].Id != -1 && _customerSearchCollection[0].Name == ""))
                        {
                            progressSearch.Visibility = Visibility.Visible;
                            textBlockSearchStatus.Text = "Sæki upplýsingar af kennitölu.";
                            _worker.RunWorkerAsync("kennitala");
                        }
                        else
                        {
                            popupSearch.IsOpen = false;
                            _order.Customer = _customerSearchCollection[0];
                            textBoxKennitala.Text = _order.Customer.Kennitala.Insert(6, "-");
                            textBoxNafn.Text = _order.Customer.Name;
                        }*/
                    }
                    else if (!string.IsNullOrEmpty(textBoxNafn.Text))
                    {
                        progressSearch.Visibility = Visibility.Visible;
                        textBlockSearchStatus.Text = "Leita að nafninu í gagnagrunninum.";
                        _searchName = textBoxNafn.Text;
						if (!_worker.IsBusy)
							_worker.RunWorkerAsync("names");
                    }
                    else
                        textBlockSearchStatus.Text = "Þú verður að skrifa annaðhvort nafn eða kennitölu áður en leitin getur byrjað.";
                }
            }
		}

        private void ClearVisibilityPopup()
        {
            progressSearch.Visibility = Visibility.Hidden;
            buttonSearchFill.Visibility = Visibility.Hidden;
            buttonSearchLeft.Visibility = Visibility.Hidden;
            buttonSearchRight.Visibility = Visibility.Hidden;
            stackCustomerInfo.Visibility = Visibility.Hidden;
            textBlockSearchStatus.Visibility = Visibility.Hidden;
            listSearchCustomers.Visibility = Visibility.Hidden;
        }

		private void popupSearch_Closed(object sender, EventArgs e)
		{
			Storyboard sizePopupClose = (Storyboard)popupSearch.FindResource("sizePopupClose"); // (Storyboard)FindResource("sizePopupClose");
			sizePopupClose.Begin(this); 
		}

		private void buttonSearchLeft_Click(object sender, RoutedEventArgs e)
		{
			popupSearch_Closed(null, null);
		}

		private void buttonSearchFill_Click(object sender, RoutedEventArgs e)
		{
			popupSearch_Closed(null, null);
		}
		
		void AskConfirmationOnCustomer(Customer customer)
		{
            ClearVisibilityPopup();
			buttonSearchLeft.Visibility = Visibility.Visible;
			buttonSearchRight.Visibility = Visibility.Visible;
			stackCustomerInfo.Visibility = Visibility.Visible;

			buttonSearchRight.Content = "Velja";
			popupSearch.DataContext = customer;

			textBlockKennitala.Text = string.Format("Kt. {0}", customer.Kennitala.Insert(6, "-"));
			textBlockName.Text = customer.Name;
			textBlockAddress.Text = string.Format("{0} {1}", customer.Address1, customer.Address2);
			textBlockCity.Text = string.Format("{0} {1}", customer.Zip, customer.City);
		}

		private delegate void delegateSearchForCustomers(BackgroundWorker worker);
		private void SearchForCustomersByName(BackgroundWorker worker)
		{
			CustomerHandler handler = new CustomerHandler();
			handler.RetreaveCustomerCollection("", _searchName, ref _customerSearchCollection, worker);
		}

		private void SearchForCustomersByKennitala(BackgroundWorker worker)
		{
			CustomerHandler handler = new CustomerHandler();
			handler.RetreaveCustomerCollection("", _searchName, ref _customerSearchCollection, worker);
		}

		private void buttonSearchRight_Click(object sender, RoutedEventArgs e)
		{
			if (listSearchCustomers.Visibility == Visibility.Visible)
			{
				if (listSearchCustomers.SelectedItem != null)
				{
					popupSearch.DataContext = listSearchCustomers.SelectedItem as Customer;
					listSearchCustomers.Visibility = Visibility.Hidden;
				}
			}
            if (popupSearch.DataContext is Customer)
            {
                _order.Customer = popupSearch.DataContext as Customer;
				if (Convert.ToInt32(_order.Customer.Kennitala.Substring(0, 2)) > 31)
					_order.Abyrgd = 1;
                Main.DB.CustomerHandler.SaveCustomerDataToDatabase(_order.Customer);
                textBoxKennitala.Text = _order.Customer.Kennitala.Insert(6, "-");
                textBoxNafn.Text = _order.Customer.Name;
                popupSearch_Closed(null, null);
            }
		}

		private void editCustomerInformation_Click(object sender, RoutedEventArgs e)
		{
			ViewerCustomer viewer = new ViewerCustomer(_order.Customer);
			viewer.ShowDialog();
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			System.Threading.Thread.Sleep(500);
			switch ((string)e.Argument)
			{
				case "names":
					CustomerHandler handler = new CustomerHandler();
					int total = handler.SearchTotalCustomer(_searchName);

					CustomerCollection customrResult = new CustomerCollection();

					if (total < 25)
					{
						handler.RetreaveCustomerCollection("", _searchName, ref customrResult, null);
					}
					else
					{
						for (int i = 0; i < 26; i++)
							customrResult.Add(new Customer());
					}
					e.Result = customrResult;
					return;

				case "kennitala":
					CustomerCollection customers = new CustomerCollection();
					try
					{
						(sender as BackgroundWorker).ReportProgress(0, "Leita að kennitölu í gagnagrunni.");

						Main.DB.CustomerHandler.RetreaveCustomerCollection(_searchKennitala.Replace("-", ""), "", ref customers, null);

						if (customers.Count == 1)
						{
							if (customers[0].Id == -1 || (customers[0].Id != -1 && customers[0].Name == ""))
							{
								(sender as BackgroundWorker).ReportProgress(0, "Sæki upplýsingar af þjóðskrá.");
							}
							else
							{
								e.Result = customers[0];
								return;
							}
						}
						else if (customers.Count > 1)
						{
							e.Result = customers;
							return;
						}
						else if (customers.Count == 0)
						{
							if (_searchKennitala.Length == 10)
								(sender as BackgroundWorker).ReportProgress(0, "Sæki upplýsingar af þjóðskrá.");
							else
							{
								(sender as BackgroundWorker).ReportProgress(0, "Kennitalan er of löng eða og stutt til að hægt sé að leita í þjóðskrá.");
								return;
							}
						}
					}
					catch (Exception error)
					{
						(sender as BackgroundWorker).ReportProgress(0, string.Format("Villa við tengingu á gagnagrunni\n{0}.", error.Message));
						e.Result = true;
						return;
					}
					try
					{
						Customer customer = new Customer();
						customer.Kennitala = _searchKennitala;

						PostSubmitter post = new PostSubmitter();
						post.Url = "http://www2.glitnir.is/tkaup/Main.asp";
						post.PostItems.Add("tbKennitala", customer.Kennitala);
						post.PostItems.Add("state", "1");
						post.Type = PostSubmitter.PostTypeEnum.Post;
						string result = post.Post();

						string regularExpressFinder = "<input type=\"text\" name=\"(?<name>[^\"]*)\" value=\"(?<value>[^\"]*)\"[^\\/]*/>";

						Regex findAllValues = new Regex(regularExpressFinder);

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
											customer.Name = m.Groups["value"].Captures[0].Value;
											break;

										case "tbHeimili":
											customer.Address1 = m.Groups["value"].Captures[0].Value;
											break;

										case "tbPostnumer":
											customer.Zip = m.Groups["value"].Captures[0].Value;
											break;

										case "tbSveitarfelag":
											customer.City = m.Groups["value"].Captures[0].Value;
											break;
									}
								}
							}
							e.Result = customer;
							return;
						}
					}
					catch (Exception error)
					{
						(sender as BackgroundWorker).ReportProgress(0, string.Format("Villa við tengingu á þjóðskrá\n{0}.", error.Message));
						e.Result = true;
					}
					return;
			}
		}

		void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Result is Customer)
			{
				Customer customer = e.Result as Customer;
				if (customer.Id == -1)
					AskConfirmationOnCustomer(customer);
				else
				{
					_order.Customer = customer;
					if (Convert.ToInt32(_order.Customer.Kennitala.Substring(0, 2)) > 31)
						_order.Abyrgd = 1;
					Main.DB.CustomerHandler.SaveCustomerDataToDatabase(_order.Customer);
					textBoxKennitala.Text = _order.Customer.Kennitala.Insert(6, "-");
					textBoxNafn.Text = _order.Customer.Name;
					popupSearch.IsOpen = false;
				}
				return;
			}
			else if (e.Result is CustomerCollection)
			{
				progressSearch.Visibility = Visibility.Hidden;

				CustomerCollection customers = e.Result as CustomerCollection;
				if (customers.Count > 25)
				{
					progressSearch.Visibility = Visibility.Hidden;
					string search = "";
					if (textBoxNafn.Text != "")
						search = textBoxNafn.Text;
					else
						search = textBoxKennitala.Text;
					textBlockSearchStatus.Text = string.Format("Of margar niðurstöður fundust með þessu nafni. {0} manneskjur fundust með leitarstrenginum '{1}'.", customers.Count, search);
					return;
				}
				else if (customers.Count == 1)
				{
					AskConfirmationOnCustomer(customers[0]);
					return;
				}
				else if (customers.Count > 1)
				{
					listSearchCustomers.ItemsSource = customers;
					ClearVisibilityPopup();
					buttonSearchLeft.Visibility = Visibility.Visible;
					buttonSearchRight.Visibility = Visibility.Visible;
					listSearchCustomers.Visibility = Visibility.Visible;
					buttonSearchRight.Content = "Velja";
					return;
				}
			}
			else if (e.Result is bool)
				return;
			textBlockSearchStatus.Text = string.Format("Engin manneskja fannst.");
		}

		private void paymentMethod_Loaded(object sender, RoutedEventArgs e)
		{
			ComboBox box = sender as ComboBox;
			box.ItemsSource = Main.DB.PayMethods;
		}

		private void paymentMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OrderPayment payment = (OrderPayment)((ComboBox)sender).DataContext;
			if (payment.Amount == 0)
				payment.Amount = _order.TotalUnpaid;
		}

		private void textBoxTotalUnpaid_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (_order.Payment.Count > 0)
			{
				if (_order.Payment[_order.Payment.Count - 1].Name != "")
					_order.Payment.Add(new OrderPayment(-1, "", 0));
			}
			else
				_order.Payment.Add(new OrderPayment(-1, "", 0));
		}

		private void buttonExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void buttonPrint_Click(object sender, RoutedEventArgs e)
		{
			if (_order.TotalUnpaid != 0)
			{
				Storyboard gridUnpaidAnimate = (Storyboard)gridTotalUnpaid.FindResource("gridUnpaidAnimate"); // (Storyboard)FindResource("sizePopupClose");
				gridUnpaidAnimate.Begin(this); 
			}
			else
			{
				if (string.IsNullOrEmpty(_order.Customer.Kennitala))
				{
					_order.Customer.Kennitala = "";
				}
				else if (!string.IsNullOrEmpty(_order.Customer.AlarmNotes))
				{
					if (MessageBox.Show(string.Format("Athugið, viðskiptavinurinn er með mikilvægar upplýsingar sem þarf að staðfesta áður en hægt er að keyra pöntunina í gegn:\n\n\t{0}", _order.Customer.AlarmNotes), "Mikilvægar upplýsingar vantar staðfestingu á", MessageBoxButton.YesNo) == MessageBoxResult.No)
						return;
				}
				try
				{
					int number = 0;
					int oldNumber = 0;
					if (orderId.Text != _previousNumber)
						_order.OrderNumber = Convert.ToInt32(orderId.Text);
					else
					{
						string newNumber = Database.OrderNumber.GetNextOrderNumber;
						if (!int.TryParse(newNumber, out number))
							number = -1;
						if (!int.TryParse(_previousNumber, out oldNumber))
							oldNumber = -1;
						if (number == -1 && oldNumber == -1)
						{
							MessageBox.Show("Pöntun vantar númer. Vinsamlegast athugið að það sé rétt númer á pöntuninni");
							return;
						}
						else if (number != -1)
							_order.OrderNumber = number;
						else
							_order.OrderNumber = oldNumber;
					}
                    _order.PrintTwoCopies = false;
					_order.Date = DateTime.Now;
					_printDocument.Print();
				}
				catch (Exception err)
				{
					Main.DB.ErrorLog("", err.Message, err.ToString());
                    return;
				}

                SaveOrder();
			}
		}

        private void orderId_LostFocus(object sender, RoutedEventArgs e)
        {
            orderId.IsReadOnly = true;
        }

        private void buttonEditOrderId_Click(object sender, RoutedEventArgs e)
        {
            orderId.IsReadOnly = false;
            orderId.Focus();
            orderId.SelectAll();
        }

        private void SaveOrder()
        {
            try
            {
                foreach (OrderItem orderItem in _order.Items)
                {
                    if (orderItem.ItemId != -1)
                    {
                        Item item = Main.DB.GetItem(orderItem.ItemId);
                        if (item != null)
                        {
                            if (item.Name != orderItem.Name ||
                                item.Sub != orderItem.SubName ||
                                (item.Price != orderItem.Price && orderItem.Price != 0))
                            {
                                if (ConfirmUpdateDatabase(item, orderItem))
                                {
                                    item.Price = orderItem.Price;
                                    item.Name = orderItem.Name;
                                    item.Sub = orderItem.SubName;
                                    item.SaveChanges();
                                }
                            }
                        }
                    }
                    else if (orderItem.Vorunr != "")
                    {
                        if (ConfirmAddToDatabase(orderItem))
                        {
                            FormSelectCategory selectCategory = new FormSelectCategory();
                            selectCategory.Title = "Veldu flokk";
                            selectCategory.ShowDialog();

                            if (selectCategory.SelectedCategory != null)
                            {
                                Item item = new Item();
                                item.ProductID = orderItem.Vorunr;
								item.Barcode = orderItem.Barcode;
                                item.Name = orderItem.Name;
                                item.Sub = orderItem.SubName;
                                item.Price = orderItem.Price;
                                item.Stock = 1;
                                selectCategory.SelectedCategory.Items.Add(item, true, selectCategory.SelectedCategory.ID);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Main.DB.ErrorLog("", err.Message, err.ToString());
            }

            try
            {
                _order.SaveOrderToDatabase(_order.OrderNumber);
            }
            catch (Exception err)
            {
                Main.DB.ErrorLog("", err.Message, err.ToString());
            }

            
            Properties.config.Default.order_id++;

            this.Close();
            this.Owner.Close();
        }

        private static bool ConfirmUpdateDatabase(Item item, OrderItem orderItem)
        {
            StringBuilder builder = new StringBuilder("Viltu uppfæra eftirfarandi í gagnagrunni.\n\n");
            builder.AppendFormat("Vörunr: {0}\n", item.ProductID);
            if (item.Name != orderItem.Name)
                builder.AppendFormat("Name: {0} -> {1}\n", item.Name, orderItem.Name);
            else
                builder.AppendFormat("Name: {0}\n", item.Name);

            if (item.Sub != orderItem.SubName)
                builder.AppendFormat("Sublýsing: {0} -> {1}\n", item.Sub, orderItem.SubName);
            else
                builder.AppendFormat("Sublýsing: {0}\n", item.Sub);

            if (item.Price != orderItem.Price)
                builder.AppendFormat("Verð: {0} -> {1}\n", item.Price, orderItem.Price);
            else
                builder.AppendFormat("Verð: {0}\n", item.Price);

            return MessageBox.Show(builder.ToString(),
                       "Uppfæra verð", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private static bool ConfirmUpdateDatabase(Item item, string newName, string newSub, long newPrice)
        {
            return MessageBox.Show(String.Format("Viltu uppfæra eftirfarandi upplýsingar í gagnagrunninn.\n\nVörunr: {0}\nNafn: {1} - {2}\nSublýsing: {3} -> {4}\nVerð: {5} -> {6}",
                                       item.ProductID,
                                       item.Name,
                                       newName,
                                       item.Sub,
                                       newSub,
                                       item.Price,
                                       newPrice),
                       "Uppfæra verð", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private static bool ConfirmAddToDatabase(OrderItem item)
        {
            return MessageBox.Show(String.Format("Eftirfarandi vara fannst ekki í gagnagrunni, viltu bæta því við?\n\nVörunr: {0}\nLýsing: {1}\nVerð: {2}",
                                       item.Vorunr,
                                       item.Name,
                                       item.Price),
                        "Bæta í gagnagrunni", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
	}
}
