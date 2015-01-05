using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Database
{
	public class PayMethod : INotifyPropertyChanged
	{
		public PayMethod()
		{
			_id = -1;
		}

		public PayMethod(int id, string name, int order)
		{
			_id = id;
			_name = name;
			_order = order;
		}

		int _id;
		string _name;
		int _order;
		public event PropertyChangedEventHandler PropertyChanged;

		public int Id
		{
			get { return _id; }
			set
			{
				_id = value;
			}
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
		public int Order
		{
			get { return _order; }
			set
			{
				_order = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Order"));
			}
		}

		public override string ToString()
		{
			return _name;
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}
}
