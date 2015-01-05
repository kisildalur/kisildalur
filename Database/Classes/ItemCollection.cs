using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Odbc;
using System.Text;


namespace Database
{
	public class ItemCollection : ObservableCollection<Item>
	{
		public ItemCollection() : base() { }
		public Item this[int id, bool searchforid]
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
		public void Add(Item item, bool save, int categoryId)
		{
			if (save)
			{
				try
				{
                    MainDatabase.GetDB.Connect();

					OdbcCommand command = new OdbcCommand(
						string.Format("INSERT INTO album (name) VALUES ('{0}')",
						item.Name), MainDatabase.GetDB.MySQL);
					command.ExecuteNonQuery();
					command.CommandText = "SELECT LAST_INSERT_ID()";
					int temp = Convert.ToInt32(command.ExecuteScalar());

					command.CommandText = string.Format("INSERT INTO product (barcode, name, subtitle, description, prog_id, prog_name, price, stock, fk_category, fk_album, visible) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, {7}, {8}, {9}, {10})",
														item.Barcode,
														item.Name,
														item.Sub,
														item.Description,
														item.ProductID,
														item.Name,
														item.Price,
														item.Stock,
														categoryId,
														temp,
														item.Visible ? "1" : "0");

                    command.ExecuteNonQuery();
                    command.CommandText = "SELECT LAST_INSERT_ID()";
                    item.AssignNewId(Convert.ToInt32(command.ExecuteScalar()));
                    item.Album = temp;
				}
				catch (Exception e)
				{
					MainDatabase.GetDB.ErrorLog("Error while saveing to database", e.Message, e.ToString());
					return;
				}
			}
			base.Add(item);
		}
		public void Update(Item item)
		{
			item.SaveChanges();
		}
        public void Move(Item item, int categoryId)
        {
            try
            {
                MainDatabase.GetDB.Connect();

                OdbcCommand command = new OdbcCommand("UPDATE product SET fk_category = " + categoryId + " WHERE id = " + item.ID, MainDatabase.GetDB.MySQL);
                command.ExecuteNonQuery();
				command.CommandText = "DELETE FROM product_property WHERE fk_product = " + item.ID;
				command.ExecuteNonQuery();
                this.Remove(item);
				foreach (Category cat in MainDatabase.GetDB.GetCategories())
				{
					if (cat.ID == categoryId)
					{
						cat.Items.Add(item);
						break;
					}
				}
            }
            catch (Exception e)
            {
                MainDatabase.GetDB.ErrorLog("Error while moving item", e.Message, e.ToString());
            }
        }
		public void Remove(Item item, bool save)
		{
			if (save)
			{
				try
				{
                    MainDatabase.GetDB.Connect();

                    OdbcCommand command = new OdbcCommand("DELETE FROM product WHERE id = " + item.ID, MainDatabase.GetDB.MySQL);
                    command.ExecuteNonQuery();
					command.CommandText = "DELETE FROM album WHERE id = " + item.Album;
					command.ExecuteNonQuery();
					command.CommandText = "DELETE FROM product_property WHERE fk_product = " + item.ID;
					command.ExecuteNonQuery();
					command.CommandText = "DELETE FROM product_group WHERE fk_product = " + item.ID;
					command.ExecuteNonQuery();
					command.CommandText = "UPDATE prog_orderitem SET fk_item = -1 WHERE fk_item = " + item.ID;
					command.ExecuteNonQuery();
				}
				catch (Exception e)
				{
					MainDatabase.GetDB.ErrorLog("Error while deleteing from database", e.Message, e.ToString());
				}
			}
			base.Remove(item);
		}
	}
}
