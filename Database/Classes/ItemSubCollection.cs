using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace Database
{
	public class ItemSubCollection : ObservableCollection<OrderItem>
	{
		public ItemSubCollection(int parentProductId)
			: base()
		{
			_parentProductId = parentProductId;
		}

		private int _parentProductId;

		public void AssignNewProductId(int id)
		{
			_parentProductId = id;
		}

		public void Add(OrderItem item, bool save)
		{
			if (save)
			{
				try
				{
					MainDatabase.GetDB.Connect();

					OdbcCommand command = new OdbcCommand(
						string.Format("INSERT INTO product_group (fk_product, fk_extra, count) VALUES ({0}, {1}, {2})",
						_parentProductId, item.ItemId, item.Count), MainDatabase.GetDB.MySQL);

					command.ExecuteNonQuery();
					command.CommandText = "SELECT LAST_INSERT_ID()";
					item.Id = Convert.ToInt32(command.ExecuteScalar());
				}
				catch (Exception e)
				{
					MainDatabase.GetDB.ErrorLog("Error while saving to database", e.Message, e.ToString());
					return;
				}
			}
			base.Add(item);
		}

		public void Remove(OrderItem item, bool save)
		{
			if (save)
			{
				try
				{
					MainDatabase.GetDB.Connect();

					OdbcCommand command = new OdbcCommand(
						string.Format("DELETE FROM product_group WHERE groupid = {0}", item.Id), MainDatabase.GetDB.MySQL);

					command.ExecuteNonQuery();
				}
				catch (Exception e)
				{
					MainDatabase.GetDB.ErrorLog("Error while deleting from database", e.Message, e.ToString());
					return;
				}
			}
			base.Remove(item);
		}
	}
}
