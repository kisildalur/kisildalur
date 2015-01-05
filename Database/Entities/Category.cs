using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Entities
{
	public class Category
	{
		public Category()
		{
		}

		public virtual int Id {get; private set;  }
		public virtual string Name { get; set; }
		public virtual bool Visible { get; set; }
		public virtual Folder Folder { get; set; }
		public virtual List<Product> Products { get; set; }
	}
}
