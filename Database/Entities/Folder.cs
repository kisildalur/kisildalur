using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Database.Entities
{
	public class Folder : INotifyPropertyChanged
	{
		string _name;
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public Folder()
		{
			Categories = new List<Category>();
		}

		public virtual int Id { get; private set; }
		public virtual string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				PropertyChanged(this, new PropertyChangedEventArgs("Name"));
			}
		}
		public virtual bool Visible { get; set; }
		public virtual List<Category> Categories { get; set; }
	}
}
