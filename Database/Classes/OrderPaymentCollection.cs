using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Odbc;
using System.Text;

namespace Database
{
    public class OrderPaymentCollection : ObservableCollection<OrderPayment>
    {
        public OrderPaymentCollection()
            : base()
        {
        }

        public string GetString()
        {
            string payment = "";
            foreach (OrderPayment method in this)
            {
                if (payment != "")
                    payment += string.Format(", {0}", method.Name);
                else
                    payment = method.Name;
            }
            return payment;
        }

		public OrderPayment Find(string name)
		{
			foreach (OrderPayment payment in this)
				if (payment.Name == name)
					return payment;
			return null;
		}
    }
}
