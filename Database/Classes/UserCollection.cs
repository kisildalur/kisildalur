using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using System.Data.Odbc;

namespace Database
{
	public class UserCollection : ObservableCollection<User>
	{
		public UserCollection() : base() { }
		public User this[int id, bool searchforid]
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

		public void Add(User item, bool save)
		{
			if (save)
			{
				try
				{
                    MainDatabase.GetDB.Connect();

                    OdbcCommand command = new OdbcCommand("INSERT INTO prog_users (password, name) VALUES ('" + item.Hash + "', '" + item.Name + "')", MainDatabase.GetDB.MySQL);
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
		public void Update(User item)
		{
			try
			{
                MainDatabase.GetDB.Connect();

                OdbcCommand command = new OdbcCommand("UPDATE prog_users SET password = '" + item.Hash + "', name = '" + item.Name + "' WHERE id = " + item.ID, MainDatabase.GetDB.MySQL);
                command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while updating to database", e.Message, e.ToString());
			}
		}
		public void Remove(User item, bool save)
		{
			if (save)
			{
				try
				{
                    MainDatabase.GetDB.Connect();

                    OdbcCommand command = new OdbcCommand("DELETE FROM prog_users WHERE id = " + item.ID, MainDatabase.GetDB.MySQL);
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
