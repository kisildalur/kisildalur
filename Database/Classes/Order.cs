using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace Database
{
	public class Order : INotifyPropertyChanged
	{
		public static string SQLSelect = "prog_orders.id, prog_orders.order_id, prog_orders.kennitala, prog_orders.payment, prog_orders.abyrgd, prog_orders.fk_employ, prog_orders.date, prog_orders.discounttype, prog_orders.discounttext, prog_orders.discountvalue, prog_orders.notes, prog_orders.comment";
		private int _id;
		private bool _hidePrice;
		private bool _printTwoCopies;
		private bool _isOffer;
		private Discount _globalDiscount;
		private string _kennitala;
		private string _payMethod;
		private string _comment;
		private string _notes;
		private int _orderNumber;
		private int _abyrgd;
		private int _userID;
		private SiteOrder _siteOrder;
		private DateTime _date;
		private OrderItemCollection _items;
		private OrderPaymentCollection _payment;
		private Customer _customer;
		public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initialise a new instance of Order.
        /// </summary>
		public Order()
		{
            _date = DateTime.Now;
			_items = new OrderItemCollection(this);
            _payment = new OrderPaymentCollection();
			_globalDiscount = new Discount();
			_globalDiscount.PropertyChanged += new PropertyChangedEventHandler(_globalDiscount_PropertyChanged);
			_siteOrder = null;
			_id = -1;
			_abyrgd = 2;
			_kennitala = "";
			_payMethod = "";
			_hidePrice = false;
            _customer = new Customer();

			_items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_items_CollectionChanged);
			_payment.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_payment_CollectionChanged);
		}

		public Order(SiteOrder siteOrder)
			: this()
		{
			this.SiteOrder = siteOrder;
		}

        public Order(int id, int orderNumber, string kennitala, string paymethod, int yearWarranty, int employId, DateTime dateTime, Discount discount)
			: this()
        {
            _id = id;
            _orderNumber = orderNumber;
            _kennitala = kennitala;
            _payMethod = paymethod;
            _abyrgd = yearWarranty;
            _userID = employId;
            _date = dateTime;
            _globalDiscount = discount;
        }

        /// <summary>
        /// Get or set the product id. This value does generally not need to be changed.
        /// </summary>
		public int ID
		{
			get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ID"));
            }
		}

        /// <summary>
        /// Get or set whether or not to display the price of each individual product.
        /// </summary>
		public bool HidePrice
		{
			get { return _hidePrice; }
            set
            {
                _hidePrice = value;
                OnPropertyChanged(new PropertyChangedEventArgs("HidePrice"));
            }
		}

        /// <summary>
        /// Get or set whether two copies should be printed. Only used when printing an offer.
        /// </summary>
        public bool PrintTwoCopies
        {
            get { return _printTwoCopies; }
            set
            {
                _printTwoCopies = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PrintTwoCopies"));
            }
        }

        public bool IsOffer
        {
            get { return _isOffer; }
            set
            {
                _isOffer = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsOffer"));
            }
        }
        /// <summary>
        /// Get or set the global discount. This discount applies to all products.
        /// </summary>
        public Discount GlobalDiscount
        {
            get { return _globalDiscount; }
            set
            {
                _globalDiscount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Discount"));
            }
        }

        /// <summary>
        /// Get or set the kennitala of the client.
        /// </summary>
		public string Kennitala
		{
			get { return _kennitala; }
            set
            {
                _kennitala = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Kennitala"));
            }
		}

        /// <summary>
        /// Get or set the paymethod used to pay this order (deprecated).
        /// </summary>
		public string PayMethod
		{
			get { return _payMethod; }
            set
            {
                _payMethod = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PayMethod"));
            }
		}

		public string Comment
		{
			get { return _comment; }
			set
			{
				_comment = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Notes"));
			}
		}
		public string Notes
		{
			get { return _notes; }
			set
			{
				_notes = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Notes"));
			}
		}
		/// <summary>
        /// Get or set the number id of an order. This value can be found on the paper.
        /// </summary>
		public int OrderNumber
		{
			get { return _orderNumber; }
            set
            {
                _orderNumber = value;
                OnPropertyChanged(new PropertyChangedEventArgs("OrderNumber"));
            }
		}

        /// <summary>
        /// Get or set the general warranty for all items.
        /// This does not happen for products which already contain a specified warranty time.
        /// </summary>
		public int Abyrgd
		{
			get { return _abyrgd; }
            set
            {
                _abyrgd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Abyrgd"));
            }
		}

        /// <summary>
        /// Get or set the id of the seller.
        /// </summary>
		public int UserID
		{
			get { return _userID; }
            set
            {
                _userID = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UserID"));
            }
		}

        /// <summary>
        /// Get or set the date of order.
        /// </summary>
		public DateTime Date
		{
			get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Date"));
            }
		}

        /// <summary>
        /// Get the item list.
        /// </summary>
		public OrderItemCollection Items
		{
			get { return _items; }
		}

        /// <summary>
        /// Get the order payment list.
        /// </summary>
        public OrderPaymentCollection Payment
        {
            get { return _payment; }
        }
        /// <summary>
        /// Get or set the customer for this order.
        /// </summary>
        public Customer Customer
        {
            get { return _customer; }
			set
			{
				_customer = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Customer"));
			}
        }

		public SiteOrder SiteOrder
		{
			get { return _siteOrder; }
			set
			{
				_siteOrder = value;
				if (_siteOrder != null)
				{
					_kennitala = _siteOrder.Kennitala;
					_customer.Kennitala = _kennitala;

					for (int i = 0; i < _siteOrder.Items.Count; i++)
						this.Items.Insert(this.Items.Count - 1, new OrderItem(_siteOrder.Items[i]));
				}
				else
					this.Items.Clear();
			}
		}
		public long TotalWithoutVSK
		{
			get
			{
				return _items.Total() - _items.TotalVsk(ItemVsk.items_240) - _items.TotalVsk(ItemVsk.books_7);
			}
		}
		public long TotalVSK
		{
			get { return _items.TotalVsk(ItemVsk.items_240) + _items.TotalVsk(ItemVsk.books_7); }
		}

        public long Total
        {
            get { return _items.Total(); }
        }

		void _globalDiscount_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			item_PropertyChanged(null, null);
		}

		public long TotalUnpaid
		{
			get
			{
				long total = Total;
				long totalPayment = 0;
				foreach (OrderPayment payment in _payment)
					totalPayment += payment.Amount;

				return total - totalPayment;
			}
		}

		void _payment_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					foreach (OrderPayment paymentItem in e.NewItems)
						paymentItem.PropertyChanged += new PropertyChangedEventHandler(paymentItem_PropertyChanged);
					break;

				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					foreach (OrderPayment paymentItem in e.OldItems)
						paymentItem.PropertyChanged -= new PropertyChangedEventHandler(paymentItem_PropertyChanged);
					break;
			}

			paymentItem_PropertyChanged(null, null);
		}

		void paymentItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(new PropertyChangedEventArgs("TotalUnpaid"));
		}

		void _items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					foreach (OrderItem item in e.NewItems)
					{
						item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
						foreach (OrderItem subItem in item.SubItems)
						{
							subItem.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
						}
					}
					break;

				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					foreach (OrderItem item in e.OldItems)
					{
						item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
						foreach (OrderItem subItem in item.SubItems)
						{
							subItem.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
						}
					}
					break;
			}

			item_PropertyChanged(null, null);
		}

		void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e != null)
			{
				if (e.PropertyName != "Price" &&
					e.PropertyName != "Count")
					return;
			}
			OnPropertyChanged(new PropertyChangedEventArgs("TotalWithoutVSK"));
			OnPropertyChanged(new PropertyChangedEventArgs("TotalVSK"));
			OnPropertyChanged(new PropertyChangedEventArgs("Total"));
			OnPropertyChanged(new PropertyChangedEventArgs("TotalUnpaid"));
		}


		public static Order ReadOrderFromDataReader(OdbcDataReader reader)
		{
			Order order = new Order(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5), DateTime.Parse(reader.GetString(6)), new Discount(reader.GetString(7), reader.GetInt32(9), reader.GetString(8)));
			order.Notes = reader.GetString(10);
			order.Comment = reader.GetString(11);
			OdbcCommand subItem = new OdbcCommand(string.Format("SELECT id, productnumber, name, subtitle, price, count, itemtype, fk_item, vsktype, warrantytype, warrantyyear, discounttype, discountvalue, contains_subitem FROM prog_orderitem WHERE fk_order = {0}", order.ID), MainDatabase.GetDB.MySQL);
			OdbcDataReader itemReader = subItem.ExecuteReader();
			while (itemReader.Read())
			{
				order.Items.Add(new OrderItem(itemReader.GetInt32(0), itemReader.GetString(1), itemReader.GetString(2), itemReader.GetString(3), itemReader.GetInt64(4), itemReader.GetDouble(5), itemReader.GetString(6), itemReader.GetInt32(7), itemReader.GetString(8), new Warranty(itemReader.GetString(9), itemReader.GetInt32(10)), new Discount(itemReader.GetString(11), itemReader.GetInt32(12), "")));
				if (itemReader.GetInt32(13) == 1)
				{
					OdbcCommand insideItem = new OdbcCommand(string.Format("SELECT id, productnumber, name, subtitle, price, count, itemtype, fk_item, vsktype, warrantytype, warrantyyear, discounttype, discountvalue, contains_subitem FROM prog_orderitem WHERE fk_subItem = {0}", order.Items[order.Items.Count - 1].Id), MainDatabase.GetDB.MySQL);
					OdbcDataReader subItemReader = insideItem.ExecuteReader();
					while (subItemReader.Read())
					{
						order.Items[order.Items.Count - 1].SubItems.Add(new OrderItem(subItemReader.GetInt32(0), subItemReader.GetString(1), subItemReader.GetString(2), subItemReader.GetString(3), subItemReader.GetInt64(4), subItemReader.GetDouble(5), subItemReader.GetString(6), subItemReader.GetInt32(7), subItemReader.GetString(8), new Warranty(subItemReader.GetString(9), subItemReader.GetInt32(10)), new Discount(subItemReader.GetString(11), subItemReader.GetInt32(12), "")));
					}
				}
			}

			OdbcCommand subPayment = new OdbcCommand(string.Format("SELECT id, name, amount FROM prog_orderspay WHERE fk_order = {0}", order.ID), MainDatabase.GetDB.MySQL);
			OdbcDataReader paymentReader = subPayment.ExecuteReader();
			while (paymentReader.Read())
			{
				order.Payment.Add(new OrderPayment(paymentReader.GetInt32(0), paymentReader.GetString(1), paymentReader.GetInt64(2)));
			}

			return order;
		}

        public void SaveOrderToDatabase(int orderId)
        {
            foreach (OrderItem orderItem in this._items)
                if (orderItem.ItemId != -1)
                {
                    Item item = MainDatabase.GetDB.GetItem(orderItem.ItemId);
                    if (item != null)
                        if (item.Stock > 0 &&
                            item.ProductID == orderItem.Vorunr &&
                            item.Name == orderItem.Name &&
                            item.Sub == orderItem.SubName)
                        {
                            item.SaveChanges(Convert.ToInt32(orderItem.Count));
                            break;
                        }
                }

            this._date = DateTime.Now;
            this._orderNumber = orderId;
            MainDatabase.GetDB.Orders.Add(this, true);
        }

		public int GetNumberOfItems()
		{
			int countItems = 0;

			foreach (OrderItem item in this.Items)
				if (!string.IsNullOrEmpty(item.Vorunr))
					countItems++;

			return countItems;
		}

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
