using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using Database;

namespace Database
{
	public class OrderNumber
	{
		public static string GetNextOrderNumber
		{
			get 
			{
				try
				{
					MainDatabase.GetDB.Connect();

					OdbcCommand command = new OdbcCommand("SELECT order_id FROM prog_orders ORDER BY id DESC LIMIT 1", MainDatabase.GetDB.MySQL);
					return (Convert.ToInt32(command.ExecuteScalar()) + 1).ToString();
				}
				catch (Exception e)
				{
					return "Error";
				}
			}
		}
	}
}
