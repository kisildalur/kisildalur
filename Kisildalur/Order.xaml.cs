using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Drawing.Printing;
using Database;

namespace Kisildalur
{
    /// <summary>
    /// Interaction logic for NewOrder.xaml
    /// </summary>
    public partial class NewOrder : Window
    {
		Order _order;
		PrintDocument _printDocument;
		OfferPrinterHelper _printerHelper;
		OrderItem _selectedItem;

        public NewOrder()
        {
			_selectedItem = null;
            InitializeComponent();
			textboxBarcode.Focus();

            _printDocument = new PrintDocument();
            _printDocument.PrintPage += new PrintPageEventHandler(_printDocument_PrintPage);
            _printerHelper = new OfferPrinterHelper(
                Properties.config.Default.header1,
                Properties.config.Default.header2);
        }

		public NewOrder(int userid)
			: this()
		{
			_order = new Order();
			_order.PrintTwoCopies = true;
			_order.UserID = userid;
			_order.Items.Add(new OrderItem(0, "", "", "", 0, 0, "", -1, "", new Warranty(), new Discount()));
			this.DataContext = _order;
			_printerHelper.Order = _order;
		}

		public NewOrder(int userid, SiteOrder siteOrder)
			: this(userid)
		{
			_order.SiteOrder = siteOrder;
		}

		void _printDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			_printerHelper.PrintPage(e);
		}

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
			this.Close();
        }

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (this.WindowState == WindowState.Normal)
			{
				Properties.config.Default.pSizeWidth = this.Width;
				Properties.config.Default.pSizeHeight = this.Height;
			}
			Properties.config.Default.Save();
		}

		private void listViewItemContent_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!string.IsNullOrEmpty(_order.Items[_order.Items.Count - 1].Vorunr) ||
				!string.IsNullOrEmpty(_order.Items[_order.Items.Count - 1].Name) ||
				!string.IsNullOrEmpty(_order.Items[_order.Items.Count - 1].SubName) ||
				_order.Items[_order.Items.Count - 1].Count != 0 ||
				_order.Items[_order.Items.Count - 1].Price != 0)
				_order.Items.Add(new OrderItem(0, "", "", "", 0, 0, "", -1, "", new Warranty(), new Discount()));
		}

		private void removeItemFromList_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			object data = button.DataContext;
			_order.Items.Remove((OrderItem)data);
			if (_order.Items.Count == 0)
				_order.Items.Add(new OrderItem(0, "", "", "", 0, 0, "", -1, "", new Warranty(), new Discount()));
			else if (!string.IsNullOrEmpty(_order.Items[_order.Items.Count - 1].Vorunr) ||
				!string.IsNullOrEmpty(_order.Items[_order.Items.Count - 1].Name) ||
				!string.IsNullOrEmpty(_order.Items[_order.Items.Count - 1].SubName) ||
				_order.Items[_order.Items.Count - 1].Count != 0 ||
				_order.Items[_order.Items.Count - 1].Price != 0)
				_order.Items.Add(new OrderItem(0, "", "", "", 0, 0, "", -1, "", new Warranty(), new Discount()));
		}

		private void clearItemFromList_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			OrderItem data = (OrderItem)button.DataContext;
			int index = _order.Items.IndexOf(data);
			_order.Items.Remove(data);
			_order.Items.Insert(index, new OrderItem(0, "", "", "", 0, 0, "", -1, "", new Warranty(), new Discount()));
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			OrderAddItem addItem = new OrderAddItem();
            addItem.OnItemAdd += new OnItemAdd(addItem_OnItemAdd);
			addItem.ShowDialog();
		}

        void addItem_OnItemAdd(object sender, OrderItem h)
        {
			foreach (OrderItem item in _order.Items)
				if (item.Compare(h))
				{
					item.Count++;
					return;
				}
            _order.Items.Insert(_order.Items.Count - 1, h);
        }

		private void printAsOrder_Click(object sender, RoutedEventArgs e)
		{
			this._order.Items.RemoveAt(this._order.Items.Count - 1);
			OrderFinish finishOrder = new OrderFinish(this._order);
            finishOrder.Owner = this;
			finishOrder.ShowDialog();
			_order.Items.Add(new OrderItem(0, "", "", "", 0, 0, "", -1, "", new Warranty(), new Discount()));
		}

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Kisildalur.Properties.config.Default.pSplitWidth = (int)((sender as GridSplitter).Parent as Grid).ColumnDefinitions[2].Width.Value;
        }

        private void printAsOffer_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            dialog.MinPage = 1;
            dialog.MaxPage = 2;
            dialog.UserPageRangeEnabled = true;
            if (dialog.ShowDialog() == true)
            {
                _order.IsOffer = true;

                try
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
                    _printerHelper.ResetHelper();
                    _printDocument.Print();
                }
                catch (Exception err)
                {
                    Main.DB.ErrorLog("", err.Message, err.ToString());
                }
            }
        }

		private void Barcode_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && textboxBarcode.Text != "")
			{
				foreach (Item i in Main.DB.GetItems())
				{
					if (i.Barcode == textboxBarcode.Text)
					{
						addItem_OnItemAdd(this, i.ConvertToOrderItem());
						textboxBarcode.Text = "";
						return;
					}
				}
				OrderAddItem addItem = new OrderAddItem();
				addItem.Barcode = textboxBarcode.Text;
				textboxBarcode.Text = "";
				textboxBarcode.Text = "";
				addItem.OnItemAdd += new OnItemAdd(addItem_OnItemAdd);
				addItem.ShowDialog();
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!string.IsNullOrEmpty(((sender as TextBox).DataContext as OrderItem).Serials[((sender as TextBox).DataContext as OrderItem).Serials.Count - 1].Serial))
				((sender as TextBox).DataContext as OrderItem).Serials.Add(new ItemSerial());
		}

		private void addSubItemToProduct_Click(object sender, RoutedEventArgs e)
		{
			_selectedItem = ((sender as Button).Tag as OrderItem);
			OrderAddItem addItem = new OrderAddItem();
			addItem.OnItemAdd += new OnItemAdd(addToSubItem_ItemAdd);
			addItem.ShowDialog();
		}

		void addToSubItem_ItemAdd(object sender, OrderItem h)
		{
			_selectedItem.SubItems.Add(h);
		}

		private void removeSubItemFromProduct_Click(object sender, RoutedEventArgs e)
		{
			((sender as Button).Tag as OrderItem).SubItems.Remove(((sender as Button).DataContext as OrderItem));
			if (((sender as Button).Tag as OrderItem).SubItems.Count == 0)
				((sender as Button).Tag as OrderItem).ContainsSubitems = false;
		}

		private void buttonExtraOrderItem_Click(object sender, RoutedEventArgs e)
		{
			if (!((sender as Button).Tag as Popup).IsOpen)
			{
				((sender as Button).Tag as Popup).IsOpen = true;

				Storyboard sizePopupOpen = (Storyboard)((sender as Button).Tag as Popup).FindResource("sizePopupOpen"); // (Storyboard)FindResource("sizePopupClose");
				sizePopupOpen.Begin((sender as Button).Tag as Popup);
			}
		}
    }

    public enum EyesColors
    {
        brown,
        blue,
        black,
        green,
        red,
    }

    public class DiscountItem : INotifyPropertyChanged
    {
        public DiscountItem()
        {
        }

        string _name;
        DiscountType _type;
        bool _enabled;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name") );
            }
        }
        public DiscountType DiscountType
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DiscountType"));
            }
        }
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Enabled"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

    }
}
