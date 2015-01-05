using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Odbc;
using System.Text;

namespace Database
{
	public class PayMethodCollection : ObservableCollection<PayMethod>
	{
		public PayMethodCollection()
			: base()
		{
		}

		public PayMethod this[int id, bool searchforid]
		{
			get
			{
				if (searchforid)
				{
					for (int I = 0; I < base.Count; I++)
						if (base[I].Id == id)
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
						if (base[I].Id == id)
						{
							base[I] = value;
							return;
						}
					return;
				}
				base[id] = value;
			}
		}

		public void Add(PayMethod item, bool save)
		{
			if (save)
			{
				try
				{
					MainDatabase.GetDB.Connect();

					OdbcCommand command = new OdbcCommand("SELECT MAX(`order`) FROM prog_paymethods", MainDatabase.GetDB.MySQL);
					int order = Convert.ToInt32(command.ExecuteScalar()) + 1;
					command.CommandText = "INSERT INTO prog_paymethods (method, `order`) VALUES ('" + item.Name + "', " + order + ")";
					command.ExecuteNonQuery();
					command.CommandText = "SELECT LAST_INSERT_ID()";
					item.Id = Convert.ToInt32(command.ExecuteScalar());
					item.Order = order;
				}
				catch (Exception e)
				{
					MainDatabase.GetDB.ErrorLog("Error while saveing to database", e.Message, e.ToString());
					return;
				}
			}
			base.Add(item);
		}

		public void Update(PayMethod item)
		{
			try
			{
				MainDatabase.GetDB.Connect();

				OdbcCommand command = new OdbcCommand("UPDATE prog_paymethods SET method = '" + item.Name + "', `order` = " + item.Order + " WHERE id = " + item.Id, MainDatabase.GetDB.MySQL);
				command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while updating to database", e.Message, e.ToString());
			}
		}

		public void Remove(PayMethod item, bool save)
		{
			if (save)
			{
				try
				{
					MainDatabase.GetDB.Connect();

					OdbcCommand command = new OdbcCommand("DELETE FROM prog_paymethods WHERE id = " + item.Id, MainDatabase.GetDB.MySQL);
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
