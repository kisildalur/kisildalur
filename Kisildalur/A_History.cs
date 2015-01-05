using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using Database;

namespace Kisildalur
{
	public partial class A_History : Form
	{
		public A_History(int userId)
		{
			InitializeComponent();

            _ordersDate.Value = DateTime.Now;
            _orders = new OrderCollection();
			_searchOrders = new OrderCollection();
            _searchCustomers = new CustomerCollection();
            _reportOrders = new OrderCollection();
            _reportMonthlyOrders = new OrderCollection();
            _userId = userId;
            this.Size = Properties.config.Default.historySize;

            this._orderList.Columns[0].Width = Properties.config.Default.historyListWidth0;
            this._orderList.Columns[1].Width = Properties.config.Default.historyListWidth1;
            this._orderList.Columns[2].Width = Properties.config.Default.historyListWidth2;
            this._orderList.Columns[3].Width = Properties.config.Default.historyListWidth3;
            this._orderList.Columns[4].Width = Properties.config.Default.historyListWidth4;
            this._orderList.Columns[5].Width = Properties.config.Default.historyListWidth5;

            this._SearchList.Columns[0].Width = Properties.config.Default.historySearchWidth0;
            this._SearchList.Columns[1].Width = Properties.config.Default.historySearchWidth1;
            this._SearchList.Columns[2].Width = Properties.config.Default.historySearchWidth2;
            this._SearchList.Columns[3].Width = Properties.config.Default.historySearchWidth3;
            this._SearchList.Columns[4].Width = Properties.config.Default.historySearchWidth4;
            this._SearchList.Columns[5].Width = Properties.config.Default.historySearchWidth5;

            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
            _worker.WorkerReportsProgress = true;

            this.CenterToScreen();

            Timer t = new Timer();
            t.Tick += new EventHandler(t_Tick);
            t.Interval = 500;
            t.Start();

			InitializeSearchTab();


            ListViewSorter l = new ListViewSorter(_searchCustomerList, new ListViewSorter.Comparer[] { ListViewSorter.CompareStrings, ListViewSorter.CompareStrings, ListViewSorter.CompareNumbers, ListViewSorter.CompareStrings });

            _printingDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(_printDocument_PrintPage);
            _printingDocument.BeginPrint += new PrintEventHandler(_printDocument_BeginPrint);
            _printingDocument.QueryPageSettings += new QueryPageSettingsEventHandler(_printDocument_QueryPageSettings);
            _printingHelper = new DayPrinterHelper();

            _monthPrinterHelper = new MonthPrinterHelper(Properties.config.Default.header1, Properties.config.Default.header2);
            _printMonthReport.PrintPage += new PrintPageEventHandler(_printMonthReport_PrintPage);
        }

        void _printMonthReport_PrintPage(object sender, PrintPageEventArgs e)
        {
            _monthPrinterHelper.PrintPage(e);
        }

        void t_Tick(object sender, EventArgs e)
        {
            if (!_worker.IsBusy)
            {
                ((Timer)sender).Stop();
                if (_tabControl.SelectedIndex == 0)
                    _worker.RunWorkerAsync("orders");
                else if (_tabControl.SelectedIndex == 3)
                    _worker.RunWorkerAsync("reportDate");
            }
        }

        OrderCollection _orders;
		OrderCollection _searchOrders;
        OrderCollection _reportOrders;
        OrderCollection _reportMonthlyOrders;
        CustomerCollection _searchCustomers;
        DayPrinterHelper _printingHelper;
        MonthPrinterHelper _monthPrinterHelper;
        int _userId;

        #region Orders

        private void InitializeOrders()
        {
            ColumnsResize(0);
        }

        private void _ordersDate_ValueChanged(object sender, EventArgs e)
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync("orders");
        }

        private void _ordersViewOrder_Click(object sender, EventArgs e)
        {
            if (_orderList.SelectedItems.Count == 1)
            {
                ListViewItem item = _orderList.SelectedItems[0];
                for (int I = 0; I < _orders.Count; I++)
                {
                    if (_orders[I].ID == (int)item.Tag)
                    {
						ViewerOrder viewer = new ViewerOrder(_orders[I]);
						viewer.ShowDialog();
                        break;
                    }
                }
            }
        }

        private void LoadOrdersByDate()
        {
            long total = 0;
            DatabaseHelper.RetreaveOrdersByDate(_worker, _ordersDate.Value, ref _orders);
            if (InvokeRequired)
                Invoke(new ListviewClear(ClearListView), 1);

            if (_orders.Count == 0)
            {
                Invoke(new AddToListview(AddItemToListview), new ListViewItem("No orders found"), 1);
                Invoke(new SetTotal(TotalSet), total, 0);
                return;
            }

            foreach (Order order in _orders)
            {
                string kennitala = "";
                if (order.Kennitala != "0000000000")
                    kennitala = order.Kennitala;

                ListViewItem item = new ListViewItem(new string[] {
                        string.Format("{0:00}:{1:00}", order.Date.Hour, order.Date.Minute),
                        order.OrderNumber.ToString(),
                        kennitala.ToString(),
                        order.GetNumberOfItems().ToString(),
                        order.Payment.GetString(),
                        order.Items.Total().ToString()});
                total += order.Items.Total();
                item.Tag = order.ID;
                if (InvokeRequired)
                    Invoke(new AddToListview(AddItemToListview), item, 1);
            }
            if (InvokeRequired)
                Invoke(new SetTotal(TotalSet), total, 0);

            if (InvokeRequired)
                Invoke(new ResizeColumns(ColumnsResize), 0);
        }
        private void _orderList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_orderList.SelectedItems.Count == 1)
                _ordersViewOrder.Enabled = true;
            else
                _ordersViewOrder.Enabled = false;
        }

		private void _orderList_DoubleClick(object sender, EventArgs e)
		{
			_ordersViewOrder_Click(null, null);
		}

        #endregion

		#region SearchOrders

		public void InitializeSearchTab()
		{
			_searchDateMore.Value = DateTime.Parse("01-01-2008");

            _searchCategory.Items.Clear();
            foreach (Folder folder in Main.DB.Folders)
            {
                foreach (Category cat in folder.Categories)
                {
                    _searchCategory.Items.Add(new ComboboxItem(cat.ID, string.Format("{0} - {1}", folder.Name, cat.Name)));
                }
            }
            _searchProduct.Items.Clear();
		}

		private void _searchButton_Click(object sender, EventArgs e)
		{
			if (!_worker.IsBusy)
				_worker.RunWorkerAsync("searchOrders");
		}

        private void _searchCustomer_Click(object sender, EventArgs e)
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync("searchOrders");
        }

		private void _SearchList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_SearchList.SelectedItems.Count == 1)
				_searchViewOrder.Enabled = true;
			else
				_searchViewOrder.Enabled = false;
		}

		private void _SearchList_DoubleClick(object sender, EventArgs e)
		{
			_searchViewOrder_Click(null, null);
		}

		private void _searchViewOrder_Click(object sender, EventArgs e)
		{
			ListViewItem item = _SearchList.SelectedItems[0];
			for (int I = 0; I < _searchOrders.Count; I++)
			{
				if (_searchOrders[I].ID == (int)item.Tag)
				{
					ViewerOrder viewer = new ViewerOrder(_searchOrders[I]);
					viewer.ShowDialog();
					break;
				}
			}
		}

        private void _searchListProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_searchListProducts.SelectedItems.Count > 0)
                _searchProductRemove.Enabled = true;
            else
                _searchProductRemove.Enabled = false;
        }

        private void _searchProductRemove_Click(object sender, EventArgs e)
        {
            if (_searchListProducts.SelectedItems.Count > 0)
                _searchListProducts.Items.Remove(_searchListProducts.SelectedItem);
        }

        private void _searchProductAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_searchProduct.Text))
            {
                for (int i = 0; i < _searchListProducts.Items.Count; i++)
                {
                    if ((string)_searchListProducts.Items[i] == _searchProduct.Text)
                        return;
                }
                _searchListProducts.Items.Add(_searchProduct.Text);
            }
        }

        private void SearchOrdersFromDatabase()
        {
            long minimumTotal = 0, maximumtotal = 0;
            long.TryParse(_searchTotalMore.Text, out minimumTotal);
            long.TryParse(_searchTotalLess.Text, out maximumtotal);
            List<string> listNames = new List<string>();
            foreach (string name in _searchListProducts.Items)
            {
                listNames.Add(name);
            }
            DatabaseHelper.SearchDatabaseForOrders(_worker, ref _searchOrders, _searchKennitala.Text, _searchId.Text, minimumTotal, maximumtotal, _searchDateMore.Value, _searchDateLess.Value, listNames);
            if (InvokeRequired)
                Invoke(new ListviewClear(ClearListView), 0);

            if (_searchOrders.Count == 0)
            {
                Invoke(new AddToListview(AddItemToListview), new ListViewItem("No orders found"), 0);
                return;
            }

            foreach (Order order in _searchOrders)
            {
                string kennitala = "";
                if (order.Kennitala != "0000000000")
                    kennitala = order.Kennitala;
                string payment = "";
                foreach (OrderPayment method in order.Payment)
                {
                    if (payment != "")
                        payment += string.Format(", {0}", method.Name);
                    else
                        payment = method.Name;
                }
                ListViewItem item = new ListViewItem(new string[] { string.Format("{2:00}-{3:00}-{4} {0:00}:{1:00}", order.Date.Hour, order.Date.Minute, order.Date.Day, order.Date.Month, order.Date.Year),
                                                                    order.OrderNumber.ToString(),
                                                                    kennitala.ToString(),
                                                                    order.GetNumberOfItems().ToString(),
                                                                    payment,
                                                                    order.Items.Total().ToString()});
                item.Tag = order.ID;
                if (InvokeRequired)
                    Invoke(new AddToListview(AddItemToListview), item, 0);
            }

            if (InvokeRequired)
                Invoke(new ResizeColumns(ColumnsResize), 1);
        }

        private void _searchCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_searchCategory.SelectedIndex != -1)
            {
                int id = ((ComboboxItem)_searchCategory.Items[_searchCategory.SelectedIndex]).Id;
                foreach (Category cat in Main.DB.GetCategories())
                {
                    if (cat.ID == id)
                    {
                        foreach (Item item in cat.Items)
                        {
                            _searchProduct.Items.Add(item.Name);
                        }
                        return;
                    }
                }
            }
        }

		#endregion

        #region SearchCustomer

        private void _searchCustomer_Click_1(object sender, EventArgs e)
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync("searchCustomer");
        }

        private void SearchCustomerFromDatabase()
        {
            if (InvokeRequired)
                Invoke(new ListviewClear(ClearListView), 2);

            DatabaseHelper.SearchDatabaseForCustomer(_worker, ref _searchCustomers, _searchCustomerKennitala.Text, _searchCustomerName.Text, _searchCustomerAddress1.Text, _searchCustomerAddress2.Text, _searchCustomerZIP.Text, _searchCustomerCity.Text, _searchCustomerTelephone1.Text, _searchCustomerTelephone2.Text, _searchCustomerTelephone3.Text);

            if (_searchCustomers.Count == 0)
            {
                Invoke(new AddToListview(AddItemToListview), new ListViewItem("No customers found"), 2);
                return;
            }

            foreach (Customer customer in _searchCustomers)
            {
                string address = "";
                if (customer.Address1 != "")
                    address += customer.Address1;
                if (customer.Address2 != "")
                    address += " " + customer.Address2;
                if (customer.Zip != "")
                    address += " - " + customer.Zip;
                if (customer.City != "")
                    address += ", " + customer.City;
                ListViewItem item = new ListViewItem(new string[] {customer.Kennitala,
	                                                             customer.Name,
	                                                             customer.TotalOrders.ToString(),
	                                                             address});
                item.Tag = customer;
                if (InvokeRequired)
                    Invoke(new AddToListview(AddItemToListview), item, 2);
            }
        }
        private void _searchCustomerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_searchCustomerList.SelectedItems.Count == 1)
                _viewCustomer.Enabled = true;
            else
                _viewCustomer.Enabled = false;
        }

        private void _addCustomer_Click(object sender, EventArgs e)
        {
			ViewerCustomer customerEditor = new ViewerCustomer();
			customerEditor.ShowDialog();
        }

        private void _viewCustomer_Click(object sender, EventArgs e)
        {
            if (_searchCustomerList.SelectedItems.Count == 1)
            {
				ViewerCustomer customerEditor = new ViewerCustomer((Customer)_searchCustomerList.SelectedItems[0].Tag);
				customerEditor.ShowDialog();
            }
        }

        private void _search_KeyPressEnter(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
                _searchButton_Click(null, null);
        }

        #endregion

        #region DailyReport

        private void PrepareReport()
        {
            DatabaseHelper.RetreaveOrdersByDate(_worker, _reportDate.Value, ref _reportOrders);

            if (InvokeRequired)
                Invoke(new ListviewClear(ClearListView), 3);
            if (InvokeRequired)
                Invoke(new ListviewClear(ClearListView), 4);

            long totalPayment = 0;
            long totalPrice = 0;
            OrderPaymentCollection paymentCollection = new OrderPaymentCollection();
            foreach (Order order in _reportOrders)
            {
                string kennitala = "";
                if (order.Kennitala != "0000000000")
                    kennitala = order.Kennitala;
                string paymethod = "";
                foreach (OrderPayment method in order.Payment)
                {
                    if (paymethod != "")
                        paymethod += string.Format(", {0}", method.Name);
                    else
                        paymethod = method.Name;
                }
                long orderTotalVsk = order.Items.TotalVsk(ItemVsk.items_240) + order.Items.TotalVsk(ItemVsk.books_7);
                long orderTotal = order.Items.Total();
                totalPrice += orderTotal;
                ListViewItem item = new ListViewItem(new string[] { string.Format("{0:00}:{1:00}", order.Date.Hour, order.Date.Minute),
                                                                    order.OrderNumber.ToString(),
                                                                    kennitala.ToString(),
                                                                    order.GetNumberOfItems().ToString(),
                                                                    paymethod,
                                                                    orderTotalVsk.ToString(),
                                                                    orderTotal.ToString()});
                item.Tag = order.ID;
                if (InvokeRequired)
                    Invoke(new AddToListview(AddItemToListview), item, 3);

                foreach (OrderPayment payment in order.Payment)
                {
                    _paymentSearch = payment.Name;
                    if (paymentCollection.Find(payment.Name) == null)
                    {
                        paymentCollection.Add(new OrderPayment(1, payment.Name, payment.Amount));
                    }
                    else
                    {
                        OrderPayment method = paymentCollection.Find(payment.Name);
                        method.SetId(method.Id + 1);
                        method.Amount += payment.Amount;
                    }
                }
            }

            foreach (OrderPayment payment in paymentCollection)
            {
                ListViewItem item = new ListViewItem(new string[] { payment.Name,
                                                                    payment.Id.ToString(),
                                                                    payment.Amount.ToString()});
                if (InvokeRequired)
                    Invoke(new AddToListview(AddItemToListview), item, 4);

                totalPayment += payment.Amount;
            }

            if (InvokeRequired)
                Invoke(new ResizeColumns(ColumnsResize), 2);
            if (InvokeRequired)
                Invoke(new ResizeColumns(ColumnsResize), 3);

            if (InvokeRequired)
                Invoke(new SetTotal(TotalSet), totalPrice, 1);
            if (InvokeRequired)
                Invoke(new SetTotal(TotalSet), totalPayment, 2);
        }

        private static string _paymentSearch;
        private static bool FindPaymentMatch(OrderPayment p)
        {
            return (p.Name == _paymentSearch);
        }


        private void RunReports()
        {
            Application.DoEvents();
        }

        private delegate void RefreshReports();

        private void _tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (_tabControl.SelectedIndex)
            {
                case 0:
                    InitializeOrders();
                    break;

                case 3:
                    if (!_worker.IsBusy)
                        _worker.RunWorkerAsync("reportDate");
                    break;
                case 4:
                    _monthDateFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    break;
            }
        }

        private void _reportDate_ValueChanged(object sender, EventArgs e)
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync("reportDate");
        }

        private void _printReport_Click(object sender, EventArgs e)
        {
            if (_reportOrders.Count > 0)
            {
                if (_printingDialog.ShowDialog() == DialogResult.OK)
                {
                    _printingDocument.PrinterSettings = _printingDialog.PrinterSettings;
                    _printingHelper.ResetHelper(_reportOrders, _userId);

                    try
                    {
                        _printingDocument.Print();
                    }
                    catch (Exception err)
                    {
                        Main.DB.ErrorLog("Error while printing document", err.Message, err.ToString());
                    }
                }
            }
            else
                MessageBox.Show("Uppgjörið verður að innihalda að minnsta kosti eina færslu áður en hægt er að prenta það út.");
        }

        #endregion

        void _printDocument_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            return;
        }

        void _printDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            return;
        }

        void _printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            _printingHelper.PrintPage(e);
        }

        void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
                _statusLabel.Text = (string)e.UserState;
            _progressBar.Value = e.ProgressPercentage;
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch ((string)e.Argument)
            {
                case "orders":
                    LoadOrdersByDate();
                    break;

                case "searchOrders":
                    SearchOrdersFromDatabase();
                    break;

                case "searchCustomer":
                    SearchCustomerFromDatabase();
                    break;

                case "reportDate":
                    PrepareReport();
                    break;

                case "reportMonth":
                    PrepareMonthlyReport();
                    break;
            }
        }

        private delegate void AddToListview(ListViewItem item, int id);
		private void AddItemToListview(ListViewItem item, int id)
		{
            switch (id)
            {
                case 0:
                    _SearchList.Items.Add(item);
                    break;
                case 1:
                    _orderList.Items.Add(item);
                    break;
                case 2:
                    _searchCustomerList.Items.Add(item);
                    break;
                case 3:
                    _reportListOrders.Items.Add(item);
                    break;
                case 4:
                    _reportListPayment.Items.Add(item);
                    break;
                case 5:
                    _monthListDays.Items.Add(item);
                    break;
                case 6:
                    _monthListPayments.Items.Add(item);
                    break;
            }
                
			Application.DoEvents();
		}

        private delegate void ListviewClear(int id);
		private void ClearListView(int id)
		{
            switch (id)
            {
                case 0:
                    _SearchList.Items.Clear();
                    break;
                case 1:
                    _orderList.Items.Clear();
                    break;
                case 2:
                    _searchCustomerList.Items.Clear();
                    break;
                case 3:
                    _reportListOrders.Items.Clear();
                    break;
                case 4:
                    _reportListPayment.Items.Clear();
                    break;
                case 5:
                    _monthListDays.Items.Clear();
                    break;
                case 6:
                    _monthListPayments.Items.Clear();
                    break;
            }
			Application.DoEvents();
		}		

        private delegate void SetTotal(long total, int id);
        private void TotalSet(long total, int id)
        {
            switch (id)
            {
                case 0:
                    _total.Text = string.Format("{0:#,0}", total);
                    break;

                case 1:
                    _reportTotalOrder.Text = string.Format("{0:#,0}", total);
                    break;

                case 2:
                    _reportTotalPayment.Text = string.Format("{0:#,0}", total);
                    break;

                case 3:
                    _monthTotal.Text = string.Format("{0:#,0}", total);
                    break;

                case 4:
                    _monthTotalPayment.Text = string.Format("{0:#,0}", total);
                    break;

                case 5:
                    _monthTotalVsk.Text = string.Format("{0:#,0}", total);
                    break;
            }
        }

        enum ResizeMethod { AddHundred, DivideByTwo, None}
        private delegate void ResizeColumns(int id);
        private void ColumnsResize(int id)
        {
            int size1 = 0, size2 = 0;
            ResizeMethod method = ResizeMethod.None;
            ListView list = null;

            switch (id)
            {
                case 0:
                    list = _orderList;
                    method = ResizeMethod.AddHundred;
                    break;

                case 1:
                    list = _SearchList;
                    method = ResizeMethod.AddHundred;
                    break;

                case 2:
                    list = _reportListOrders;
                    method = ResizeMethod.DivideByTwo;
                    break;

                case 3:
                    list = _reportListPayment;
                    method = ResizeMethod.None;
                    break;

                case 4:
                    list = _monthListDays;
                    method = ResizeMethod.DivideByTwo;
                    break;

                case 5:
                    list = _monthListPayments;
                    method = ResizeMethod.DivideByTwo;
                    break;
            }

            foreach (ColumnHeader header in list.Columns)
            {
                header.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                size1 = header.Width;
                header.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                size2 = header.Width;

                if (size1 > size2)
                    header.Width = size1 + 20;
                else
                    header.Width = size2 + 20;
            }
            list.Columns[list.Columns.Count - 1].Width -= 20;
            size1 -= 20;
            switch (method)
            {
                case ResizeMethod.AddHundred:
                    int after = size1 - 50;
                    list.Columns[list.Columns.Count - 1].Width -= after;
                    list.Columns[list.Columns.Count - 2].Width += after;
                    break;

                case ResizeMethod.DivideByTwo:
                    int total = list.Columns[list.Columns.Count - 1].Width + list.Columns[list.Columns.Count - 2].Width;
                    total = total / 2;
                    list.Columns[list.Columns.Count - 1].Width = total;
                    list.Columns[list.Columns.Count - 2].Width = total;
                    break;
            }
            
        }

        private void A_History_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.DB.Disconnect();
            Properties.config.Default.Save();
        }

		private void _close_Click(object sender, EventArgs e)
		{
			this.Close();
		}

        private void A_History_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
                Properties.config.Default.historySize = this.Size;
        }

        private void _orderList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            Properties.config.Default.historyListWidth0 = this._orderList.Columns[0].Width;
            Properties.config.Default.historyListWidth1 = this._orderList.Columns[1].Width;
            Properties.config.Default.historyListWidth2 = this._orderList.Columns[2].Width;
            Properties.config.Default.historyListWidth3 = this._orderList.Columns[3].Width;
            Properties.config.Default.historyListWidth4 = this._orderList.Columns[4].Width;
            Properties.config.Default.historyListWidth5 = this._orderList.Columns[5].Width;
        }

        private void _SearchList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            Properties.config.Default.historySearchWidth0 = this._SearchList.Columns[0].Width;
            Properties.config.Default.historySearchWidth1 = this._SearchList.Columns[1].Width;
            Properties.config.Default.historySearchWidth2 = this._SearchList.Columns[2].Width;
            Properties.config.Default.historySearchWidth3 = this._SearchList.Columns[3].Width;
            Properties.config.Default.historySearchWidth4 = this._SearchList.Columns[4].Width;
            Properties.config.Default.historySearchWidth5 = this._SearchList.Columns[5].Width;
        }

        private void A_History_ResizeEnd(object sender, EventArgs e)
        {
            ColumnsResize(0);
            ColumnsResize(1);
            ColumnsResize(2);
            ColumnsResize(3);
        }

        #region ReportMonth

        private void PrepareMonthlyReport()
        {
            DatabaseHelper.SearchDatabaseForOrders(_worker, ref _reportMonthlyOrders, "", "", long.MinValue, long.MaxValue, _monthDateFrom.Value, _monthDateTo.Value, new List<string>());


            if (InvokeRequired)
                Invoke(new ListviewClear(ClearListView), 5);
            if (InvokeRequired)
                Invoke(new ListviewClear(ClearListView), 6);

            long totalPayment = 0;
            long totalPriceVsk = 0;
            long totalPrice = 0;

            long totalDay = 0, totalDayVsk = 0;
            int totalOrders = 0;

            OrderPaymentCollection paymentCollection = new OrderPaymentCollection();

            foreach (PayMethod subMethod in Main.DB.PayMethods)
                paymentCollection.Add(new OrderPayment(0, subMethod.Name, 0));

            DateTime currDate = _monthDateFrom.Value;
            foreach (Order order in _reportMonthlyOrders)
            {
                if (currDate.Day != order.Date.Day || currDate.Month != order.Date.Month || currDate.Year != order.Date.Year)
                {
                    if (totalDay != 0)
                    {
                        ListViewItem item = new ListViewItem(new string[] { currDate.ToLongDateString(),
                                                                    string.Format("{0:#,0}", totalOrders),
                                                                    string.Format("{0:#,0}", totalDayVsk),
                                                                    string.Format("{0:#,0}", totalDay)});
                        if (InvokeRequired)
                            Invoke(new AddToListview(AddItemToListview), item, 5);

                        totalDay = 0;
                        totalDayVsk = 0;
                        totalOrders = 0;
                    }

                    currDate = order.Date;
                }

                totalDayVsk += order.Items.TotalVsk(ItemVsk.items_240) + order.Items.TotalVsk(ItemVsk.books_7);
                totalPriceVsk += order.Items.TotalVsk(ItemVsk.items_240) + order.Items.TotalVsk(ItemVsk.books_7);
                totalDay += order.Items.Total();
                totalPrice += order.Items.Total();

                foreach (OrderPayment payment in order.Payment)
                {
                    _paymentSearch = payment.Name;
                    if (paymentCollection.Find(payment.Name) == null)
                    {
                        paymentCollection.Add(new OrderPayment(1, payment.Name, payment.Amount));
                    }
                    else
                    {
                        OrderPayment method = paymentCollection.Find(payment.Name);
                        method.SetId(method.Id + 1);
                        method.Amount += payment.Amount;
                    }
                }
                totalOrders++;
            }

            ListViewItem lastItem = new ListViewItem(new string[] { currDate.ToLongDateString(),
                                                                    string.Format("{0:#,0}", totalOrders),
                                                                    string.Format("{0:#,0}", totalDayVsk),
                                                                    string.Format("{0:#,0}", totalDay)});
            if (InvokeRequired)
                Invoke(new AddToListview(AddItemToListview), lastItem, 5);

            foreach (OrderPayment payment in paymentCollection)
            {
                if (payment.Amount != 0)
                {
					ListViewItem item = null;
					if (_reportMonthlyOrders[0].Date.Year >= 2015)
						item = new ListViewItem(new string[] { payment.Name,
									payment.Id.ToString(),
									string.Format("{0:#,0}", payment.Amount * (1 - (1 / 1.240))),
									string.Format("{0:#,0}", payment.Amount)});
                    else if (_reportMonthlyOrders[0].Date.Year >= 2010)
                        item = new ListViewItem(new string[] { payment.Name,
									payment.Id.ToString(),
									string.Format("{0:#,0}", payment.Amount * (1 - (1 / 1.255))),
									string.Format("{0:#,0}", payment.Amount)});
					else if (_reportMonthlyOrders[_reportMonthlyOrders.Count - 1].Date.Year < 2010)
						item = new ListViewItem(new string[] { payment.Name,
									payment.Id.ToString(),
									string.Format("{0:#,0}", payment.Amount * (1 - (1 / 1.245))),
									string.Format("{0:#,0}", payment.Amount)});
					else
						item = new ListViewItem(new string[] { payment.Name,
									payment.Id.ToString(),
									"",
									string.Format("{0:#,0}", payment.Amount)});
                    if (InvokeRequired)
                        Invoke(new AddToListview(AddItemToListview), item, 6);

                    totalPayment += payment.Amount;
                }
            }

            if (InvokeRequired)
                Invoke(new ResizeColumns(ColumnsResize), 4);
            if (InvokeRequired)
                Invoke(new ResizeColumns(ColumnsResize), 5);

            if (InvokeRequired)
                Invoke(new SetTotal(TotalSet), totalPrice, 3);
            if (InvokeRequired)
                Invoke(new SetTotal(TotalSet), totalPayment, 4);
            if (InvokeRequired)
                Invoke(new SetTotal(TotalSet), totalPriceVsk, 5);
        }

        private void _monthDateFrom_ValueChanged(object sender, EventArgs e)
        {
            DateTime selected = _monthDateFrom.Value;
            if (selected.Year == DateTime.Now.Year && selected.Month == DateTime.Now.Month)
            {
                _monthDateTo.Value = new DateTime(selected.Year, selected.Month, DateTime.Now.Day);
            }
            else
            {
                _monthDateTo.Value = new DateTime(selected.Year, selected.Month, DateTime.DaysInMonth(selected.Year, selected.Month));
            }
        }

        private void _monthButtonRefresh_Click(object sender, EventArgs e)
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync("reportMonth");
        }

        #endregion  

        private void _monthPrint_Click(object sender, EventArgs e)
        {
            if (this._monthListDays.Items.Count > 0)
            {
                if (_printingDialog.ShowDialog() == DialogResult.OK)
                {
                    _printMonthReport.PrinterSettings = _printingDialog.PrinterSettings;
                    _monthPrinterHelper.DateFrom = _monthDateFrom.Value;
                    _monthPrinterHelper.DateTo = _monthDateTo.Value;
                    _monthPrinterHelper.OrderList = _reportMonthlyOrders;
                    _monthPrinterHelper.UserId = _userId;
                    try
                    {
                        _printMonthReport.Print();
                    }
                    catch (Exception err)
                    {
                        Main.DB.ErrorLog("Error while printing document", err.Message, err.ToString());
                    }
                }
            }
            else
                MessageBox.Show("Uppgjörið verður að innihalda að minnsta kosti eina færslu áður en hægt er að prenta það út.");
        }
    }

    class ComboboxItem
    {
        public ComboboxItem()
        {
        }

        public ComboboxItem(int id, string content)
        {
            _id = id;
            _content = content;
        }

        int _id;
        string _content;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public override string ToString()
        {
            return _content;
        }
    }
}