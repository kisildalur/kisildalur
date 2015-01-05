using System;
using System.Collections.Generic;
using System.Text;
using LitJson;

namespace Database
{
    public enum SiteOrderStage { New = 1, Confirmed = 2, Finished = 3}
    public enum SiteOrderShipping { PickedOnTheSpot = 0, Shipped = 1}
	public enum SiteOrderPaymethod { PayiedOnTheSpot = 0, MoneyTransfer = 1, Card = 2, Cleared = 3 }

    public class SiteOrder
    {
        public SiteOrder()
        {
			_id = -1;
			_shippingAddress = "";
            _items = new CartItemCollection();
        }

        public SiteOrder(int id, string kennitala, string username, string name, int userid, SiteOrderStage stage, DateTime time, string jsonShipping, string jsonPaymethod)
			: this()
        {
            _id = id;
			_kennitala = kennitala;
            _username = username;
            _name = name;
            _userId = userid;
            _stage = stage;
            _date = time;

			ParseJSonShipping(jsonShipping);
			ParseJSonPaymethod(jsonPaymethod);
        }

        int _id;
		string _kennitala;
        string _username;
        string _name;
		string _shippingAddress;
        int _userId;
        SiteOrderStage _stage;
        SiteOrderShipping _shipping;
		SiteOrderPaymethod _paymethod;
        CartItemCollection _items;
        DateTime _date;

		public void ParseStage(int stage)
		{
			if (stage == 1)
				_stage = SiteOrderStage.New;
			else if (stage == 2)
				_stage = SiteOrderStage.Confirmed;
			else
				_stage = SiteOrderStage.Finished;
		}

		public void ParseJSonPaymethod(string jsonPaymethod)
		{
			JsonReader reader = new JsonReader(jsonPaymethod);

			// The Read() method returns false when there's nothing else to read
			while (reader.Read())
			{
				if (reader.Token.ToString() == "String")
				{
					switch (reader.Value.ToString())
					{
						case "1":
							_paymethod = SiteOrderPaymethod.PayiedOnTheSpot;
							break;

						case "2":
							_paymethod = SiteOrderPaymethod.Card;
							break;

						case "3":
							_paymethod = SiteOrderPaymethod.MoneyTransfer;
							break;

						default:
							_paymethod = SiteOrderPaymethod.Cleared;
							break;
					}
					break;
				}
			}
		}

		public void ParseJSonShipping(string jsonShipping)
		{
			JsonReader reader = new JsonReader(jsonShipping);

			string propertyName = "";
			// The Read() method returns false when there's nothing else to read
			while (reader.Read())
			{
				if (reader.Token.ToString() == "String")
				{
					if (propertyName != "method")
					{
						if (_shippingAddress != "")
							_shippingAddress += "\n";
						_shippingAddress += string.Format("{0}{1}: {2}",
							propertyName.ToUpper()[0],
							propertyName.Remove(0, 1),
							reader.Value.ToString());
					}
					else
						switch (reader.Value.ToString())
						{
							case "2":
								_shipping = SiteOrderShipping.Shipped;
								break;

							default:
								_shipping = SiteOrderShipping.PickedOnTheSpot;
								break;
						}
				}
				else if (reader.Token.ToString() == "PropertyName")
					propertyName = reader.Value.ToString();

				if (reader.Token.ToString() == "String" && _shipping == SiteOrderShipping.PickedOnTheSpot)
				{
					_shippingAddress = "Pöntunin verður sótt út í búð.";
					break;
				}
			}
		}

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

		public string Kennitala
		{
			get { return _kennitala; }
			set { _kennitala = value; }
		}
		public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public SiteOrderStage Stage
        {
            get { return _stage; }
            set { _stage = value; }
        }

        public CartItemCollection Items
        {
            get { return _items; }
        }

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

		public SiteOrderPaymethod Paymethod
		{
			get { return _paymethod; }
			set { _paymethod = value; }
		}
		public SiteOrderShipping Shipping
        {
            get { return _shipping; }
            set { _shipping = value; }
        }
		public string ShippingAddress
		{
			get { return _shippingAddress; }
			set { _shippingAddress = value; }
		}
	}
}
