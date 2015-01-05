using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using System.Data.Odbc;

namespace Database
{
	public class FolderCollection : ObservableCollection<Folder>
    {
        public FolderCollection() : base() { }

		public Folder this[int id, bool searchforid]
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

		public void Add(Folder item, bool save)
		{
			if (save)
			{
				try
				{
                    MainDatabase.GetDB.Connect();

                    OdbcCommand command = new OdbcCommand("INSERT INTO folder (name) VALUES ('" + item.Name + "')", MainDatabase.GetDB.MySQL);
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
		public void Update(Folder item)
		{
			try
			{
                MainDatabase.GetDB.Connect();

                OdbcCommand command = new OdbcCommand("UPDATE folder SET name = '" + item.Name + "', visible = " + (item.Visible ? 1 : 0) + " WHERE id = " + item.ID, MainDatabase.GetDB.MySQL);
                command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while updating to database", e.Message, e.ToString());
			}
		}
		public void Remove(Folder item, bool save)
		{
			if (save)
			{
				try
				{
                    MainDatabase.GetDB.Connect();
                    
                    OdbcCommand command = new OdbcCommand("DELETE FROM folder WHERE id = " + item.ID, MainDatabase.GetDB.MySQL);
                    command.ExecuteNonQuery();
                    command.CommandText = "SELECT id FROM category WHERE fk_folder = " + item.ID;
                    OdbcDataReader results = command.ExecuteReader();
                    while (results.Read())
                    {
                        OdbcCommand remove = new OdbcCommand("DELETE FROM product WHERE fk_category = " + results.GetInt32(0), MainDatabase.GetDB.MySQL);
                        remove.ExecuteNonQuery();
                    }
                    results.Close();
                    command.CommandText = "DELETE FROM category WHERE fk_folder = " + item.ID;
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
