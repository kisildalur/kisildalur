using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Entities
{
	public class Product
	{
		public Product()
		{
		}

		public virtual int Id { get; private set; }
		public virtual string Name { get; set; }
		public virtual string Subtitle { get; set; }
		public virtual long Price { get; set; }
		public virtual int Stock { get; set; }
		public virtual string ProductId { get; set; }
		public virtual string Barcode { get; set; }
		public virtual DateTime LastEdited { get; set; }
		public virtual bool CalculatePrice { get; set; }
		public virtual bool EnforceSerial { get; set; }
		public virtual Category Category { get; set; }
	}
}
