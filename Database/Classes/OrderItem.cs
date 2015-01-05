using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Database
{
	public enum ItemType { FromDatabase, UserMade}
	public enum ItemVsk { items_240, books_7 , other_0 }

	public class OrderItem :INotifyPropertyChanged
	{
		private int _id;
		private int _itemId;
		private string _barcode;
		private string _vorunr;
		private string _lysing;
		private string _subLysing;
		private long _verd;
		private double _count;
		private bool _calculatePrice = false;
		private bool _containsSerial;
		private bool _containsSubitems;
		private ItemType _type;
		private ItemVsk _vsk;
		private Warranty _warranty;
		private Discount _discount;
		private ItemSerialCollection _serials;
		private OrderItemCollection _subItems;
		private OrderItem _parent;
		public event PropertyChangedEventHandler PropertyChanged;

		public OrderItem()
		{
			_warranty = new Warranty();
			_discount = new Discount();
			_itemId = -1;
			_type = ItemType.UserMade;
			_vorunr = "";
			_lysing = "";
            _subLysing = "";
			_containsSerial = false;
			_subItems = new OrderItemCollection(null);
			_serials = new ItemSerialCollection();
			_serials.Add(new ItemSerial());
			_discount.PropertyChanged += new PropertyChangedEventHandler(_discount_PropertyChanged);

            _subItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_subItems_CollectionChanged);
		}

		void _discount_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(new PropertyChangedEventArgs("Price"));
			OnPropertyChanged(new PropertyChangedEventArgs("TotalPrice"));
		}

		public OrderItem(CartItem cartItem)
			: this()
		{
			Item origItem = MainDatabase.GetDB.GetItem(cartItem.ItemId);
			if (origItem != null)
			{
				this.Barcode = origItem.Barcode;
				this.Name = origItem.Name;
				this.SubName = origItem.Sub;
				this.Vorunr = origItem.ProductID;
				this.Type = ItemType.FromDatabase;
				this.ItemId = origItem.ID;
				this.Price = origItem.Price;
				this.Count = cartItem.Count;
				this.CalculatePrice = origItem.CalculatePrice;
				if (origItem.SubProducts.Count > 0)
				{
					_containsSubitems = true;
					for (int i = 0; i < origItem.SubProducts.Count; i++)
						this.SubItems.Add(origItem.SubProducts[i]);
				}
			}
			else
			{
				this._vorunr = cartItem.ProductID;
				this._lysing = cartItem.Name;
				this._verd = cartItem.Price;
				this._count = cartItem.Count;
				this._itemId = cartItem.ItemId;
			}
		}

        public OrderItem(int id, string vorunr, 
                        string description, 
                        string subDescription,
                        long price, 
                        double count,
                        string type,
                        int itemId,
                        string vsk, 
                        Warranty warranty, 
                        Discount discount)
			: this()
		{
			_id = id;
			_vorunr = vorunr;
			_lysing = description;

			if (_lysing.Contains("[") && _lysing.Contains("]"))
			{
				subDescription = _lysing.Remove(0, _lysing.IndexOf("[") + 1);
				subDescription = subDescription.Remove(subDescription.IndexOf("]"));
				_subLysing = subDescription;
			}
			else
				_subLysing = subDescription;
			_verd = price;
			_count = count;
			switch (type)
			{
				case "FromDatabase":
					_type = ItemType.FromDatabase;
					break;

				default:
					_type = ItemType.UserMade;
					break;
			}
			switch (vsk)
			{
				case "None":
					_vsk = ItemVsk.other_0;
					break;

				case "Books":
					_vsk = ItemVsk.books_7;
					break;

				default:
					_vsk = ItemVsk.items_240;
					break;
			}

			_itemId = itemId;
			_warranty = warranty;
			_discount = discount;
			_discount.PropertyChanged += new PropertyChangedEventHandler(_discount_PropertyChanged);
		}

        void _subItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e != null)
            {
                if (e.PropertyName != "Price" &&
                    e.PropertyName != "Count")
                    return;
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Price"));
            OnPropertyChanged(new PropertyChangedEventArgs("TotalPrice"));
        }

        public bool Compare(OrderItem item)
        {
            return (this.ItemId == item.ItemId &&
                    this.Vorunr == item.Vorunr &&
                    this.Name == item.Name &&
                    this.SubName == item.SubName &&
                    this.Price == this.Price);
        }

        public long TotalPrice
        {
            get
            {
                long total = Convert.ToInt64(this.Price * _count);

                if (this._discount.Type != DiscountType.None)
                    switch (this._discount.Type)
                    {
                        case DiscountType.Coin:
                            total -= this._discount.CoinDiscount;
                            break;

                        case DiscountType.Percent:
                            total = Convert.ToInt64(total * ((100 - this._discount.PercentDiscount) / 100.0));
                            break;
                    }
                return total;
            }
        }

		/// <summary>
		/// Get or set the Id of the orderitem
		/// </summary>
		public int Id
		{
			get { return _id; }
			set
			{
				_id = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Id"));
			}
		}

        public bool CalculatePrice
        {
            get { return _calculatePrice; }
			set
			{
				_calculatePrice = value;
				OnPropertyChanged(new PropertyChangedEventArgs("CalculatePrice"));
				OnPropertyChanged(new PropertyChangedEventArgs("Price"));
				OnPropertyChanged(new PropertyChangedEventArgs("TotalPrice"));
			}
        }
		public bool ContainsSerial
		{
			get { return _containsSerial; }
			set
			{
				_containsSerial = value;
				OnPropertyChanged(new PropertyChangedEventArgs("ContainsSerial"));
			}
		}
		public bool ContainsSubitems
		{
			get { return _containsSubitems; }
			set
			{
				_containsSubitems = value;
				OnPropertyChanged(new PropertyChangedEventArgs("ContainsSubitems"));
			}
		}
		public OrderItem Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}
		public int ItemId
		{
			get { return _itemId; }
			set
			{
				_itemId = value;
				OnPropertyChanged(new PropertyChangedEventArgs("ItemId"));
			}
		}
		public string Barcode
		{
			get { return _barcode; }
			set
			{
				_barcode = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Barcode"));
			}
		}
		public string Vorunr
		{
			get { return _vorunr; }
			set
			{
				_vorunr = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Vörunr"));
			}
		}
		public string Name
		{
			get { return _lysing; }
			set
			{
				_lysing = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Name"));
			}
		}
        public string SubName
        {
            get { return _subLysing; }
			set
			{
				_subLysing = value;
				OnPropertyChanged(new PropertyChangedEventArgs("SubName"));
			}
        }
        public long Price
        {
			get
            {
                if (_calculatePrice)
                {
                    long total = 0;
                    foreach (OrderItem subItem in _subItems)
                        total += Convert.ToInt64(subItem.Price * subItem.Count);

                    return total;
                }
                else
                    return _verd;
            }
			set
			{
				_verd = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Price"));
                OnPropertyChanged(new PropertyChangedEventArgs("TotalPrice"));
			}
		}
		public double Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("TotalPrice"));
			}
		}
		public ItemType Type
		{
			get { return _type; }
			set
			{
				_type = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Type"));
			}
		}
		public ItemVsk Vsk
		{
			get { return _vsk; }
			set
			{
				_vsk = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Vsk"));
			}
		}
		public Warranty Warranty
		{
			get { return _warranty; }
		}
		public Discount Discount
		{
			get { return _discount; }
		}

		public OrderItemCollection SubItems
		{
			get { return _subItems; }
		}
		public ItemSerialCollection Serials
		{
			get { return _serials; }
		}
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		public OrderItem Clone()
		{
			OrderItem cloned = new OrderItem(_id, _vorunr, _lysing, _subLysing, _verd, _count, _type.ToString(), _itemId, _vsk.ToString(), _warranty, _discount);
			cloned._parent = this._parent;
			cloned._containsSubitems = this._containsSubitems;
			cloned._containsSerial = this._containsSerial;
			cloned._calculatePrice = this._calculatePrice;
			cloned._barcode = this._barcode;
			foreach (OrderItem subItem in _subItems)
				cloned._subItems.Add(subItem.Clone());
			return cloned;
		}
	}
}