using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

using System.Data.Odbc;

namespace Database
{
	public class Item : INotifyPropertyChanged
	{
        /// <summary>
        /// Initialice a new instance of product item.
        /// </summary>
		public Item()
		{
			_subProducts = new ItemSubCollection(-1);
			_subProducts.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_subProducts_CollectionChanged);
		}

        /// <summary>
        /// Initialice a new instance of product item with specified values
        /// </summary>
        /// <param name="id">Id of the product used to identify the product in database.</param>
        /// <param name="productId">Id of the product.</param>
        /// <param name="name">Name of product.</param>
        /// <param name="sub">Sub text of a product.</param>
        /// <param name="description">Description of a product.</param>
        /// <param name="stock">Product stock available.</param>
        /// <param name="price">Price of the product.</param>
        /// <param name="visible">Specify whether the item is visible on site or not.</param>
        /// <param name="album">Id of the album</param>
        public Item(int id, string barcode, string productId, string name, string sub, string description, int stock, Int64 price, bool visible, bool calculatePrice, int album)
			: this()
        {
            _id = id;
			_barcode = barcode;
            _name = name;
            _sub = sub;
            _description = description;
            _productId = productId;
            _stock = stock;
            _price = price;
            _visible = visible;
            _album = album;
            _calculatePrice = calculatePrice;

			_subProducts.AssignNewProductId(_id);
        }

        private string _productId;
		private string _barcode;
        private string _name;
        private string _sub;
        private string _description;
		private Int64 _price;
		private int _stock;
		private int _id;
        private bool _visible = false;
        private bool _calculatePrice = false;
        private int _album;
        private ItemSubCollection _subProducts;
		public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Get or set the product id.
        /// </summary>
        public string ProductID
        {
            get { return _productId; }
			set
			{
				_productId = value;
				OnPropertyChanged(new PropertyChangedEventArgs("ProductID"));
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
		/// <summary>
        /// Get or set the price of a product.
        /// </summary>
        public Int64 Price
        {
			get
			{
				if (_calculatePrice)
				{
					long total = 0;
					foreach (OrderItem subItem in _subProducts)
						total += subItem.TotalPrice;

					return total;
				}
				else
					return _price;
			}
			set
			{
				_price = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Price"));
			}
        }
        /// <summary>
        /// Get or set the total stock of an item.
        /// </summary>
        public int Stock
        {
            get { return _stock; }
			set
			{
				_stock = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Stock"));
			}
        }
        /// <summary>
        /// Get the id of an object from the database. 
        /// Note: This value should never be changed.
        /// </summary>
        public int ID
        {
            get { return _id; }
        }
        /// <summary>
        /// Get or set whether product should be visible on the website or not.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
			set
			{
				_visible = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Visible"));
			}
        }

        /// <summary>
        /// Get or set whether product price should be calculated or not.
        /// </summary>
        public bool CalculatePrice
        {
            get { return _calculatePrice; }
			set
			{
				_calculatePrice = value;
				OnPropertyChanged(new PropertyChangedEventArgs("CalculatePrice"));
			}
        }

        /// <summary>
        /// Get or set the album id.
        /// </summary>
        public int Album
        {
            get { return _album; }
            set { _album = value; }
        }
        /// <summary>
        /// Get or set the name or header of a product.
        /// </summary>
        public string Name
        {
            get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Name"));
			}
        }
        /// <summary>
        /// Get or set the sub text of a product.
        /// </summary>
        public string Sub
        {
            get { return _sub; }
			set
			{
				_sub = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Sub"));
			}
        }
        /// <summary>
        /// Get or set the description of a product.
        /// </summary>
        public string Description
        {
            get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Description"));
			}
        }

        /// <summary>
        /// Assign a new id for a product.
        /// </summary>
        /// <param name="productId">the new id for a product.</param>
        public void AssignNewId(int productId)
        {
            _id = productId;
			_subProducts.AssignNewProductId(_id);
			OnPropertyChanged(new PropertyChangedEventArgs("ID"));
        }

        /// <summary>
        /// Get the list of subproducts in current item
        /// </summary>
		public ItemSubCollection SubProducts
        {
            get { return _subProducts; }
			set
			{
				_subProducts = value;
				OnPropertyChanged(new PropertyChangedEventArgs("SubProducts"));
			}
        }

        /// <summary>
        /// Save changes to an object into the database. Saves every information including the stock.
        /// </summary>
		public void SaveChanges()
		{
			try
			{
				MainDatabase.GetDB.Connect();

				string query = String.Format("UPDATE product SET barcode = '{0}', name = '{1}', subtitle = '{2}', description = '{3}', prog_id = '{4}', price = {5}, stock = {6}, visible = {7} WHERE id = {8}", 
								   Barcode,
                                   Name, 
                                   Sub, 
                                   Description, 
                                   ProductID,
                                   Price, 
                                   Stock, 
                                   (Visible ? "1" : "0"), 
                                   ID);
				OdbcCommand command = new OdbcCommand(query, MainDatabase.GetDB.MySQL);
				command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while updating to database", e.Message, e.ToString());
			}
		}

        /// <summary>
        /// Save stock changes to specified object.
        /// </summary>
        /// <param name="stockChange">The number to minus from the original stock. 
        /// Note: This is not the total stock but how much the program should minus from the stock from the database.</param>
		public void SaveChanges(int stockChange)
		{
			try
			{
				MainDatabase.GetDB.Connect();

				string query = String.Format("UPDATE product SET stock = stock - {0} WHERE id = {1}", 
                                   stockChange, 
                                   ID);
				OdbcCommand command = new OdbcCommand(query, MainDatabase.GetDB.MySQL);
				command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while updating to database", e.Message, e.ToString());
			}
		}

        /// <summary>
        /// Get the id of the primary image for this product.
        /// </summary>
        /// <returns>integer containing the id of the image of this product</returns>
        public int GetPrimaryImageIndex()
        {
            MainDatabase.GetDB.Connect();

            try
            {
                OdbcCommand command = new OdbcCommand(string.Format("select id from image where fk_album = {0}", this._album), MainDatabase.GetDB.MySQL);
                object result = command.ExecuteScalar();
                if (result != null)
                    return ((int)result);
            }
            catch (Exception e)
            {
                MainDatabase.GetDB.ErrorLog("Error while retreaving id for the primary image", e.Message, e.ToString());
            }
            return -1;
        }

		public OrderItem ConvertToOrderItem()
		{
			OrderItem orderItem = new OrderItem();
			orderItem.Barcode = this._barcode;
			orderItem.Name = this._name;
			orderItem.SubName = this._sub;
			orderItem.Vorunr = this._productId;
			orderItem.Type = ItemType.FromDatabase;
			orderItem.ItemId = this._id;
			orderItem.Price = this._price;
			orderItem.Count = 1;
            orderItem.CalculatePrice = this._calculatePrice;

			if (_subProducts.Count > 0)
			{
				orderItem.ContainsSubitems = true;
				foreach (OrderItem subItem in _subProducts)
				{
					OrderItem add = subItem.Clone();
					add.Parent = orderItem;
					orderItem.SubItems.Add(add);
				}
			}

			return orderItem;
		}

		public Item GetItem
		{
			get { return this; }
		}

		void _subProducts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(new PropertyChangedEventArgs("GetItem"));
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
			if (e.PropertyName != "GetItem")
				OnPropertyChanged(new PropertyChangedEventArgs("GetItem"));
		}
    }
}
