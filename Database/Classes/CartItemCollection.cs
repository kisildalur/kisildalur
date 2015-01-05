using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class CartItemCollection : List<CartItem>
    {
        public CartItemCollection()
            : base()
        {
        }

		public long Total
		{
			get
			{
				long total = 0;
				for (int i = 0; i < this.Count; i++)
					total += this[i].Count * this[i].Price;
				return total;
			}
		}
    }
}
