using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Entities
{
	class Order
	{
		public Order()
		{
		}

		public virtual int Id { get; private set; }
		public virtual int OrderId { get; set; }
		public virtual string Kennitala { get; set; }
		public virtual int Abyrgd { get; set; }
		public virtual User User { get; set; }
		public virtual DateTime Date { get; set; }
		public virtual DiscountType DiscountType { get; set; }
		public virtual string DiscountTest { get; set; }
		public virtual int value { get; set; }
		public virtual string Comment { get; set; }
		public virtual string PrivateComment { get; set; }
	}
}
