using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Entities
{
	class Customer
	{
		public Customer()
		{
		}

		public virtual int Id { get; private set; }
		public virtual string Kennitala { get; set; }
		public virtual string Name { get; set; }
		public virtual string HomeNumber { get; set; }
		public virtual string GSMNumber { get; set; }
		public virtual string WorkNumber { get; set; }
		public virtual string Address1 { get; set; }
		public virtual string Address2 { get; set; }
		public virtual string City { get; set; }
		public virtual string Zip { get; set; }
		public virtual string Notes { get; set; }
		public virtual string AlarmNotes { get; set; }
	}
}
