using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Database
{
	public class ItemSerial : INotifyPropertyChanged
	{
		public ItemSerial()
		{
			_id = -1;
		}

		int _id;
		string _serial;
		public event PropertyChangedEventHandler PropertyChanged;

		public int Id
		{
			get { return _id; }
			set
			{
				_id = value;
			}
		}
		public string Serial
		{
			get { return _serial; }
			set
			{
				_serial = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Serial"));
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}
}
