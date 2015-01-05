using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data.Odbc;
using System.Windows;


namespace Database
{
	public class OrderItemCollection : ObservableCollection<OrderItem>
    {
        public OrderItemCollection(Order parent)
            : base()
        {
			_parent = parent;
        }

		private Order _parent;

		/// <summary>
		/// Enumerate through each OrderItem and calculate total price of every item with discount.
		/// </summary>
		/// <returns>Total of all items with discount.</returns>
		public long Total()
		{
			return Total(true);
		}

		/// <summary>
		/// Enumerate through each OrderItem and calculate total price of every item.
		/// You can specifie whether to apply global discount or not.
		/// </summary>
		/// <param name="withDiscount">Specifie whether or not to calculate total with global discount or not.</param>
		/// <returns>Total of all items.</returns>
		public long Total(bool withDiscount)
		{
			return Total(withDiscount, 0, 0);
		}


		/// <summary>
		/// Enumerate through each OrderItem and calculate total price of every item.
		/// You can specifie whether to apply global discount or not.
		/// </summary>
		/// <param name="withDiscount">Specifie whether or not to calculate total with global discount or not.</param>
		/// <param name="index">Index of a specified item to add discount.</param>
		/// <param name="addDiscount">the amount of discount to apply to a specified item.</param>
		/// <returns>Total of all items.</returns>
		public long Total(bool withDiscount, int index, long addDiscount)
		{
			long total = 0;
			foreach (OrderItem item in this)
			{
				long itemTotal = item.TotalPrice; // CalculateSingleItem(item);

				if (_parent != null)
					if (_parent.GlobalDiscount.Type == DiscountType.Percent && withDiscount)
						itemTotal = Convert.ToInt64(itemTotal * ((100 - _parent.GlobalDiscount.PercentDiscount) / 100.0));
				total += itemTotal;
			}

			double krDiscount;
			if (_parent != null)
				if (_parent.GlobalDiscount.Type == DiscountType.Coin && withDiscount && total != 0)
				{
					krDiscount = Convert.ToInt64(_parent.GlobalDiscount.CoinDiscount) / Convert.ToDouble(total);
					total = 0;

					for (int itemIndex = 0; itemIndex < this.Count; itemIndex++)
					{
						OrderItem item = this[itemIndex];
						long itemTotal = CalculateSingleItem(item);

						itemTotal = Convert.ToInt64(itemTotal - itemTotal * krDiscount);
						if (itemIndex == index)
							itemTotal += addDiscount;
						total += itemTotal;
					}

					long totalWithoutDiscount = Total(false);
					if ((total + _parent.GlobalDiscount.CoinDiscount) != totalWithoutDiscount && (totalWithoutDiscount - _parent.GlobalDiscount.CoinDiscount) > 0)
					{
						return AddDiscountToHighestItemTotal(totalWithoutDiscount - (total + _parent.GlobalDiscount.CoinDiscount));
					}
				}

			return total;
		}

		public long TotalVsk(ItemVsk type)
		{
			return TotalVsk(type, 0, 0);
		}

		private long TotalVsk(ItemVsk type, int index, long addDiscount)
		{
			long vsk = 0;
			long total = Total(false);
			if (total != 0)
			{
				long total2 = 0;
				decimal krDiscount = 0;
				if (_parent != null)
					krDiscount = Convert.ToDecimal(_parent.GlobalDiscount.CoinDiscount) / Convert.ToDecimal(total);

				for (int itemIndex = 0; itemIndex < this.Count; itemIndex++)
				{
					OrderItem item = this[itemIndex];

					long itemTotal = CalculateSingleItem(item);

					if (_parent != null)
					{
						if (_parent.GlobalDiscount.Type == DiscountType.Percent)
							itemTotal = Convert.ToInt64(itemTotal * ((100 - _parent.GlobalDiscount.PercentDiscount) / 100.0));
						else if (_parent.GlobalDiscount.Type == DiscountType.Coin)
							itemTotal = Convert.ToInt64(itemTotal - itemTotal * krDiscount);
					}

					if (itemIndex == index)
						itemTotal += addDiscount;

					total2 += itemTotal;

					if (item.Vsk == type)
					{
						switch (item.Vsk)
						{
							case ItemVsk.items_240:
                                if (_parent != null)
                                {
                                    if (_parent.Date.Year < 2010)
                                    {
                                        vsk += Convert.ToInt64(itemTotal * (1 - (1 / 1.245)));
                                        break;
                                    }
                                    if (_parent.Date.Year < 2015)
                                    {
                                        vsk += Convert.ToInt64(itemTotal * (1 - (1 / 1.255)));
                                        break;
                                    }
                                }
								vsk += Convert.ToInt64(itemTotal * (1 - (1 / 1.240)));
								break;

							case ItemVsk.books_7:
								vsk += Convert.ToInt64(itemTotal * 0.065420);
								break;
						}
					}
				}

				if (_parent != null)
					if ((total2 + _parent.GlobalDiscount.CoinDiscount) != total && _parent.GlobalDiscount.Type == DiscountType.Coin && (total - _parent.GlobalDiscount.CoinDiscount) > 0)
					{
						return AddDiscountToHighestItemVSK(type, total - (total2 + _parent.GlobalDiscount.CoinDiscount));
					}
			}

			return vsk;
		}

		/// <summary>
		/// Process each item in current list and add any leftover discount to the most expensive item.
		/// This is so each kr discount gets calculated.
		/// Return the total after the correct amount has been added.
		/// </summary>
		/// <param name="type">Type of VSK to retreave.</param>
		/// <param name="leftOfDiscount">The total left of discount to be added.</param>
		private long AddDiscountToHighestItemTotal(long leftOfDiscount)
		{
			int index = 0;
			long price = 0;
			for (int itemIndex = 0; itemIndex < this.Count; itemIndex++)
			{
				long totalItem = CalculateSingleItem(this[itemIndex]);
				if (totalItem > price)
				{
					index = itemIndex;
					price = totalItem;
				}
			}

			return Total(true, index, leftOfDiscount);
		}

		/// <summary>
		/// Process each item in current list and add any leftover discount to the most expensive item.
		/// This is so each kr discount gets calculated.
		/// Return the total VSK after the correct amount has been added.
		/// </summary>
		/// <param name="type">Type of VSK to retreave.</param>
		/// <param name="leftOfDiscount">The total left of discount to be added.</param>
		private long AddDiscountToHighestItemVSK(ItemVsk type, long leftOfDiscount)
		{
			int index = 0;
			long price = 0;
			for (int itemIndex = 0; itemIndex < this.Count; itemIndex++)
			{
				long totalItem = CalculateSingleItem(this[itemIndex]);
				if (totalItem > price)
				{
					index = itemIndex;
					price = totalItem;
				}
			}

			return TotalVsk(type, index, leftOfDiscount);
		}

		private long CalculateSingleItem(OrderItem item)
		{
			long itemTotal = Convert.ToInt64(item.Price * item.Count);
			if (item.Discount.Type != DiscountType.None)
				switch (item.Discount.Type)
				{
					case DiscountType.Coin:
						itemTotal -= item.Discount.CoinDiscount;
						break;

					case DiscountType.Percent:
						itemTotal = Convert.ToInt64(itemTotal * ((100 - item.Discount.PercentDiscount) / 100.0));
						break;
				}

			return itemTotal;
		}
    }
}
