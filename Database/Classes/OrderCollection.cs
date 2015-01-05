using System;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Globalization;

namespace Database
{
	public class OrderCollection : ObservableCollection<Order>
	{
		public OrderCollection() : base() { }
		public Order this[int id, bool searchforid]
		{
			get
			{
				if (searchforid)
				{
					for (int I = 0; I < base.Count; I++)
						if (base[I].ID == id)
							return base[I];
					return null;
				}
				return base[id];
			}
			set
			{
				if (searchforid)
				{
					for (int I = 0; I < base.Count; I++)
						if (base[I].ID == id)
						{
							base[I] = value;
							return;
						}
					return;
				}
				base[id] = value;
			}
		}

		public void Add(Order item, bool save)
		{
			if (save)
			{
				try
				{
                    MainDatabase.GetDB.Connect();
					string globalDiscountType = "", globalDiscountValue = "0";

					switch (item.GlobalDiscount.Type)
					{
						case DiscountType.None:
							globalDiscountType = "None";
							break;
						case DiscountType.Coin:
							globalDiscountType = "Coin";
							globalDiscountValue = item.GlobalDiscount.CoinDiscount.ToString();
							break;
						case DiscountType.Percent:
							globalDiscountType = "Percent";
							globalDiscountValue = item.GlobalDiscount.PercentDiscount.ToString();
							break;
					}

					OdbcCommand command = new OdbcCommand(string.Format("INSERT INTO prog_orders (order_id, kennitala, payment, abyrgd, fk_employ, discounttype, discounttext, discountvalue, notes, comment, date) VALUES ({0}, '{1}', '{2}', {3}, {4}, '{5}', '{6}', {7}, '{8}', '{9}', '{10}')", 
                                                              item.OrderNumber, 
                                                              item.Customer.Kennitala.Replace("-", ""), 
                                                              item.PayMethod, 
                                                              item.Abyrgd, 
                                                              item.UserID,
															  globalDiscountType,
                                                              item.GlobalDiscount.Text,
															  globalDiscountValue,
															  item.Notes,
															  item.Comment,
                                                              string.Format("{0}:{1}:{2} {3}:{4}:{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)), MainDatabase.GetDB.MySQL);
                    command.ExecuteNonQuery();
                    command.CommandText = "SELECT LAST_INSERT_ID()";
                    item.ID = Convert.ToInt32(command.ExecuteScalar());

                    for (int index = 0; index < item.Items.Count; index++)
					{
						AddOrderItemToDatabase(item, command, item.Items[index], 0);

						for (int sub = 0; sub < item.Items[index].SubItems.Count; sub++)
						{
							AddOrderItemToDatabase(item, command, item.Items[index].SubItems[sub], item.Items[index].Id);
						}
					}

                    foreach (OrderPayment payment in item.Payment)
                    {
						if (payment.Amount != 0 && item.Total != 0)
						{
							command.CommandText = string.Format("INSERT INTO prog_orderspay (fk_order, name, amount) VALUES ({0}, '{1}', {2})", item.ID, payment.Name, payment.Amount);
							command.ExecuteNonQuery();
						}
                    }

					if (item.SiteOrder != null)
						if (item.SiteOrder.Id > 0)
						{
							command.CommandText = string.Format("UPDATE `order` SET `order`.stage = 3 WHERE id = {0}", item.SiteOrder.Id);
							command.ExecuteNonQuery();
						}
				}
				catch (Exception e)
				{
					MainDatabase.GetDB.ErrorLog("Error while saveing to database", e.Message, e.ToString());
					return;
				}
			}
			//base.Add(item);
		}
		private static void AddOrderItemToDatabase(Order item, OdbcCommand command, OrderItem orderItem, int subItem)
		{
			string itemType = "", vskType = "", warrantyType = "", discountType = "", discountValue = "0";
			if (orderItem.Type == ItemType.UserMade)
				itemType = "FromUser";
			else
				itemType = "FromDatabase";

			if (orderItem.Vsk == ItemVsk.items_240)
				vskType = "Items";
			else if (orderItem.Vsk == ItemVsk.books_7)
				vskType = "Books";
			else
				vskType = "None";

			if (orderItem.Warranty.Type == WarrantyType.Default)
				warrantyType = "Default";
			else if (orderItem.Warranty.Type == WarrantyType.SpecificYears)
				warrantyType = "SpecificYears";
			else
				warrantyType = "Lifetime";

			if (orderItem.Discount.Type == DiscountType.None)
				discountType = "None";
			else if (orderItem.Discount.Type == DiscountType.Coin)
			{
				discountType = "Coin";
				discountValue = orderItem.Discount.CoinDiscount.ToString();
			}
			else
			{
				discountType = "Percent";
				discountValue = orderItem.Discount.PercentDiscount.ToString();
			}

			command.CommandText = string.Format("INSERT INTO prog_orderitem (fk_order, contains_subitem, fk_subItem, productnumber, name, subtitle, price, count, itemtype, fk_item, vsktype, warrantytype, warrantyyear, discounttype, discountvalue) VALUES ({0}, {1}, {2}, '{3}', '{4}', '{5}', {6}, '{7}', '{8}', {9}, '{10}', '{11}', {12}, '{13}', {14})",
				subItem != 0 ? 0 : item.ID, orderItem.ContainsSubitems ? "1" : "0", subItem, orderItem.Vorunr, orderItem.Name, orderItem.SubName, orderItem.Price.ToString(), orderItem.Count.ToString(new CultureInfo("en-US")), itemType, orderItem.ItemId, vskType, warrantyType, orderItem.Warranty.Years, discountType, discountValue);
			command.ExecuteNonQuery();
			command.CommandText = "SELECT LAST_INSERT_ID()";
			orderItem.Id = Convert.ToInt32(command.ExecuteScalar());
		}
		public void Update(Order item)
		{
			/*try
			{
				SQLite sql = new SQLite("gagnagrunnur.db");
				sql.Query("UPDATE Users SET Hash = \'" + item.Hash + "\', Name = \'" + item.Name + "\' WHERE id = " + item.ID);
			}
			catch (Exception e)
			{
				MessageBox.Show("Error while updating to database:\n\n\t" + e.ToString());
			}*/
		}
		public void Remove(Order item, bool save)
		{
			/*if (save)
			{
				try
				{
					SQLite sql = new SQLite("gagnagrunnur.db");
					sql.Query("DELETE FROM Users WHERE id = " + item.ID);
				}
				catch (Exception e)
				{
					MessageBox.Show("Error while deleteing from database:\n\n\t" + e.ToString());
				}
			}*/
			base.Remove(item);
		}
	}
}
