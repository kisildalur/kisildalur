using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Entities
{
	class OrderItem : Item
	{
		public OrderItem()
		{
		}

		public virtual double Count { get; set; }
		public virtual ItemType ItemType { get; set; }
		public virtual DiscountType DiscountType { get; set; }
		public virtual int DiscountValue { get; set; }
		public virtual double Vsk { get; set; }
		public virtual WarrantyType WarrantyType { get; set; }
		public virtual int WarrantyValue { get; set; }
	}
}
