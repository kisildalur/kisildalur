using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Database
{
    /// <summary>
    /// Customer data retreaved from the customer database
    /// </summary>
	public class Customer : INotifyPropertyChanged
    {
        /// <summary>
        /// Initliaze a new istance of Customer with zero arguments
        /// </summary>
        public Customer()
        {
            _id = -1;
            _name = "";
            _orders = new OrderCollection();
        }

        /// <summary>
        /// Initialize a new instance of Customer with specified attributes
        /// </summary>
        /// <param name="id">The id of the object to reference with the database</param>
        /// <param name="kennitala">Kennitala of the customer</param>
        /// <param name="name">Name of the customer</param>
        /// <param name="telephone">Customer's Telephone number</param>
        /// <param name="gsm">Customer's GSM number</param>
        /// <param name="workPhone">Customer's work phone</param>
        /// <param name="address1">Home Address (1) of customer</param>
        /// <param name="address2">Home Address (2) of customer</param>
        /// <param name="city">Name of the city customer lives in</param>
        /// <param name="zip">The zip code. This has to be 3 letters.</param>
        /// <param name="notes">Notes about the customer</param>
        /// <param name="alarmNotes">Alarm notes about the customer.
        /// If this is note empty then an alarm will be triggered everytime an order is being created on this kennitala</param>
        public Customer(int id, 
                        string kennitala, 
                        string name, 
                        string telephone, 
                        string gsm, 
                        string workPhone, 
                        string address1, 
                        string address2, 
                        string city, 
                        string zip, 
                        string notes, 
                        string alarmNotes)
        {
            _id = id;
            _kennitala = kennitala;
            _name = name;
            _telephone = telephone;
            _gsm = gsm;
            _workPhone = workPhone;
            _address1 = address1;
            _address2 = address2;
            _city = city;
            _zip = zip;
            _notes = notes;
            _alarmNotes = alarmNotes;
            _orders = new OrderCollection();
        }

        private int _id;
        private string _kennitala = "";
        private string _name = "";

		private string _telephone = "";
		private string _gsm = "";
		private string _workPhone = "";

		private string _address1 = "";
		private string _address2 = "";
		private string _city = "";
		private string _zip = "";

		private string _notes = "";
		private string _alarmNotes = "";

		public event PropertyChangedEventHandler PropertyChanged;
        OrderCollection _orders;

        //Deprecated
        private int _totalOrders;

        /// <summary>
        /// Get the id of customer
        /// </summary>
        public int Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Get or set the kennitala of customer
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
        /// Get or set the name of customer
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
        /// Get or set the telephone of customer
        /// </summary>
        public string Telephone
        {
            get { return _telephone; }
			set
			{
				_telephone = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Telephone"));
			}
        }

        /// <summary>
        /// Get or set the gsm of customer
        /// </summary>
        public string Gsm
        {
            get { return _gsm; }
			set
			{
				_gsm = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Gsm"));
			}
        }

        /// <summary>
        /// Get or set the work phone of customer
        /// </summary>
        public string WorkPhone
        {
            get { return _workPhone; }
			set
			{
				_workPhone = value;
				OnPropertyChanged(new PropertyChangedEventArgs("WorkPhone"));
			}
        }

        /// <summary>
        /// Get or set the Address1 of customer
        /// </summary>
        public string Address1
        {
            get { return _address1; }
			set
			{
				_address1 = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Address1"));
			}
        }

        /// <summary>
        /// Get or set the Address2 of customer
        /// </summary>
        public string Address2
        {
            get { return _address2; }
			set
			{
				_address2 = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Address2"));
			}
        }

        /// <summary>
        /// Get or set the city of customer
        /// </summary>
        public string City
        {
            get { return _city; }
			set
			{
				_city = value;
				OnPropertyChanged(new PropertyChangedEventArgs("City"));
			}
        }

        /// <summary>
        /// Get or set the zip of customer
        /// </summary>
        public string Zip
        {
            get { return _zip; }
			set
			{
				_zip = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Zip"));
			}
        }

        /// <summary>
        /// Get or set the Notes
        /// </summary>
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
        /// Get or set the AlarmNotes
        /// </summary>
        public string AlarmNotes
        {
            get { return _alarmNotes; }
			set
			{
				_alarmNotes = value;
				OnPropertyChanged(new PropertyChangedEventArgs("AlarmNotes"));
			}
        }

        /// <summary>
        /// Assign a new id for a product.
        /// </summary>
        /// <param name="productId">the new id for a product.</param>
        public void AssignNewId(int productId)
        {
            _id = productId;
			OnPropertyChanged(new PropertyChangedEventArgs("Id"));
        }

        /// <summary>
        /// Deprecated: Gets or sets the total number of orders
        /// </summary>
        public int TotalOrders
        {
            get { return _totalOrders; }
            set { _totalOrders = value; }
        }

        /// <summary>
        /// Get all orders from this customer.
        /// </summary>
        public OrderCollection Orders
        {
            get
            {
                return _orders;
            }
        }

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
    }
}
