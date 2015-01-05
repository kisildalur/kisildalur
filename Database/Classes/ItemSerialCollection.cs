using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Database
{
	public class ItemSerialCollection : ObservableCollection<ItemSerial>
	{
		public ItemSerialCollection()
			: base()
		{
		}
	}
}
