using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Database
{
	public enum DiscountType { None, Coin, Percent}

	public class Discount : INotifyPropertyChanged
	{
		public Discount()
		{
			_type = DiscountType.None;
            _coinDiscount = 0;
            _percentDiscount = 0;
            _text = "Afsláttur";
		}

        public Discount(string type, int value, string text)
        {
            switch (type)
            {
                case "Percent":
                    _type = DiscountType.Percent;
                    _percentDiscount = value;
                    break;

                case "Coin":
                    _type = DiscountType.Coin;
                    _coinDiscount = value;
                    break;

                default:
                    _type = DiscountType.None;
                    break;
            }
            _text = text;
        }

		private DiscountType _type;
		private long _coinDiscount;
		private int _percentDiscount;
        private string _text;
        public event PropertyChangedEventHandler PropertyChanged;

		public long CoinDiscount
		{
			get { return _coinDiscount; }
            set
            {
                _coinDiscount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CoinDiscount"));
            }
		}

		public int PercentDiscount
		{
			get { return _percentDiscount; }
            set
            {
                _percentDiscount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PercenttDiscount"));
            }
		}

		public DiscountType Type
		{
			get 
			{
				return _type;
			}
            set
            {
                _type = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Type"));
            }
		}

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Text"));
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
