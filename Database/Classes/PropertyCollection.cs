using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Database
{
	public class PropertyCollection : ObservableCollection<Property>
	{
		public PropertyCollection()
			: base()
		{
		}
	}
}
