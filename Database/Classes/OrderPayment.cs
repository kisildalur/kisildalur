using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Database
{
	public class OrderPayment : INotifyPropertyChanged
    {
        public OrderPayment()
        {
            _id = -1;
        }

        public OrderPayment(int id, string name, long amount)
        {
            _id = id;
            _name = name;
            _amount = amount;
        }

        int _id;
        string _name;
		long _amount;
		public event PropertyChangedEventHandler PropertyChanged;

        public int Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Name"));
			}
        }
        public long Amount
        {
            get { return _amount; }
			set
			{
				_amount = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Amount"));
			}
        }

        public void SetId(int id)
        {
            _id = id;
			OnPropertyChanged(new PropertyChangedEventArgs("Id"));
        }

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
    }
}
