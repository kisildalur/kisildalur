using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data.Odbc;


namespace Database
{
	public class CategoryCollection : ObservableCollection<Category>
	{
        public CategoryCollection(int directoryId) : base() { _directoryId = directoryId; }
		public Category this[int id, bool searchforid]
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

        int _directoryId;

		public void Add(Category item, bool save)
		{
			if (save)
			{
				try
				{
                    MainDatabase.GetDB.Connect();

                    OdbcCommand command = new OdbcCommand("INSERT INTO category (name,fk_folder) VALUES ('" + item.Name + "', " + _directoryId + ")", MainDatabase.GetDB.MySQL);
                    command.ExecuteNonQuery();
                    command.CommandText = "SELECT LAST_INSERT_ID()";
                    item.ID = Convert.ToInt32(command.ExecuteScalar());
				}
				catch (Exception e)
				{
					MainDatabase.GetDB.ErrorLog("Error while saveing to database", e.Message, e.ToString());
					return;
				}
			}
			base.Add(item);
		}
		public void Update(Category item)
		{
			try
			{
                MainDatabase.GetDB.Connect();

                OdbcCommand command = new OdbcCommand("UPDATE category SET name = '" + item.Name + "', visible = " + (item.Visible ? 1 : 0) + " WHERE id = " + item.ID, MainDatabase.GetDB.MySQL);
                command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while updating to database", e.Message, e.ToString());
			}
		}
		public void Remove(Category item, bool save)
		{
			if (save)
			{
				try
				{
                    MainDatabase.GetDB.Connect();

                    OdbcCommand command = new OdbcCommand("DELETE FROM category WHERE id = " + item.ID, MainDatabase.GetDB.MySQL);
                    command.ExecuteNonQuery();
                    command.CommandText = "DELETE FROM product WHERE fk_category = " + item.ID;
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
