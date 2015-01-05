using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data.Odbc;

namespace Database
{
    public class DatabaseHelper
    {
        public static void RetreaveOrdersByDate(BackgroundWorker worker, DateTime date, ref OrderCollection orders)
        {
            orders.Clear();
            worker.ReportProgress(0, "Connecting");
            MainDatabase.GetDB.Connect();

            OdbcCommand command = new OdbcCommand();
            OdbcDataReader results;

            try
            {
                command = new OdbcCommand(string.Format("SELECT {2} FROM prog_orders WHERE date > '{0}' AND date < '{1}' ORDER BY date ASC", string.Format("{0}-{1}-{2} 00:00:00", date.Year, date.Month, date.Day), string.Format("{0}-{1}-{2} 23:59:59", date.Year, date.Month, date.Day), Order.SQLSelect), MainDatabase.GetDB.MySQL);
                worker.ReportProgress(25, "Connected: reading data");
                results = command.ExecuteReader();
                while (results.Read())
                {
                    orders.Add(Order.ReadOrderFromDataReader(results));
                }
                worker.ReportProgress(0, "Idle");
                results.Close();
            }
            catch (Exception e)
            {
                worker.ReportProgress(0, "Error occured");
                MainDatabase.GetDB.ErrorLog("Error while loading orders by date", e.Message, e.ToString());
            }
        }

		public static void SearchDatabaseForOrders(BackgroundWorker worker, ref OrderCollection orders, string kennitala, string noteId, long minimumTotal, long maxTotal, DateTime minimumDate, DateTime maximumDate, List<string> itemNames)
		{
			orders.Clear();
			if (worker != null)
				worker.ReportProgress(0, "Connecting");

			MainDatabase.GetDB.Connect();

			OdbcCommand command = new OdbcCommand();
			OdbcDataReader results;

			try
			{
                string commandString = string.Format("SELECT {0} FROM prog_orders", Order.SQLSelect);
				string where = "", from = "";
				if (kennitala != "")
				{
					if (where == "")
						where += string.Format(" WHERE kennitala LIKE '%{0}%'", kennitala.Replace("*", "%"));
					else
                        where += string.Format(" AND kennitala LIKE '%{0}%'", kennitala.Replace("*", "%"));
				}
				if (noteId != "")
				{
					if (where == "")
						where += string.Format(" WHERE order_id = '{0}'", noteId);
					else
						where += string.Format(" AND order_id = '{0}'", noteId);
				}
				if (minimumDate != null)
				{
					if (where == "")
                        where += string.Format(" WHERE `date` >= '{0}'", string.Format("{0}-{1:00}-{2:00} 00:00:00", minimumDate.Year, minimumDate.Month, minimumDate.Day));
					else
                        where += string.Format(" AND `date` >= '{0}'", string.Format("{0}-{1:00}-{2:00} 00:00:00", minimumDate.Year, minimumDate.Month, minimumDate.Day));
				}
				if (maximumDate != null)
				{
					if (where == "")
                        where += string.Format(" WHERE `date` <= '{0}'", string.Format("{0}-{1:00}-{2:00} 23:59:59", maximumDate.Year, maximumDate.Month, maximumDate.Day));
					else
                        where += string.Format(" AND `date` <= '{0}'", string.Format("{0}-{1:00}-{2:00} 23:59:59", maximumDate.Year, maximumDate.Month, maximumDate.Day));
				}
                if (itemNames.Count > 0)
                {
                    from += ", prog_orderitem";
                    if (where == "")
                        where += " WHERE prog_orders.id = prog_orderitem.fk_order";
                    else
                        where += " AND prog_orders.id = prog_orderitem.fk_order";

                    foreach (string searchName in itemNames)
                    {
                        where += string.Format(" AND prog_orderitem.name LIKE '%{0}%'", searchName);
                    }

                    where += " GROUP BY prog_orders.id";
                }
                command = new OdbcCommand(string.Format("{0}{1}{2}{3}", commandString, from, where, " ORDER BY prog_orders.`date`"), MainDatabase.GetDB.MySQL);

				if (worker != null)
					worker.ReportProgress(25, "Connected: reading data");

				results = command.ExecuteReader();
				while (results.Read())
				{
                    Order order = Order.ReadOrderFromDataReader(results);

					if (minimumTotal != 0 || maxTotal != 0)
					{
						long total = order.Items.Total();
						if (maxTotal == 0)
							maxTotal = long.MaxValue;

						if (total >= minimumTotal && total <= maxTotal)
							orders.Add(order);
					}
					else
						orders.Add(order);
				}

				if (worker != null)
					worker.ReportProgress(0, "Idle");
				results.Close();
			}
			catch (Exception e)
			{
				if (worker != null)
					worker.ReportProgress(0, "Idle");
				MainDatabase.GetDB.ErrorLog("Error while loading orders by date", e.Message, e.ToString());
			}
		}

        public static void SearchDatabaseForCustomer(BackgroundWorker worker, ref CustomerCollection customers, string kennitala, string name, string address1, string address2, string zip, string city, string homeNumber, string gsm, string workNumber)
		{
            customers.Clear();
            if (worker != null)
				if (worker.WorkerReportsProgress)
					worker.ReportProgress(0, "Connecting");
			MainDatabase.GetDB.Connect();

			OdbcCommand command = null;
			OdbcDataReader results;

			try
			{
                string commandString = "SELECT prog_customer.kennitala FROM prog_customer WHERE 1 = 1";
				string where = "";
                if (kennitala != "")
                    where += string.Format(" AND prog_customer.kennitala LIKE '%{0}%'", kennitala);
                if (name != "")
                    where += string.Format(" AND prog_customer.name LIKE '%{0}%'", name);

                if (address1 != "")
                    where += string.Format(" AND prog_customer.address1 LIKE '%{0}%'", address1);
                if (address2 != "")
                    where += string.Format(" AND prog_customer.address2 LIKE '%{0}%'", address2);
                if (zip != "")
                    where += string.Format(" AND prog_customer.zip LIKE '%{0}%'", zip);
                if (city != "")
                    where += string.Format(" AND prog_customer.city LIKE '%{0}%'", city);
                if (homeNumber != "")
                    where += string.Format(" AND prog_customer.homenumber LIKE '%{0}%'", homeNumber);
                if (gsm != "")
                    where += string.Format(" AND prog_customer.gsmnumber LIKE '%{0}%'", gsm);
                if (workNumber != "")
                    where += string.Format(" AND prog_customer.worknumber LIKE '%{0}%'", workNumber);

				command = new OdbcCommand(string.Format("{0}{1} GROUP BY prog_customer.kennitala" , commandString, where), MainDatabase.GetDB.MySQL);

                if (worker != null)
					if (worker.WorkerReportsProgress)
						worker.ReportProgress(25, "Connected: reading data");

				results = command.ExecuteReader();
                CustomerHandler handler = new CustomerHandler();
				while (results.Read())
				{
                    customers.Add(handler.RetreaveCustomer(results.GetString(0)));
					customers[customers.Count - 1].TotalOrders = Convert.ToInt32(new OdbcCommand("SELECT COUNT(*) FROM prog_orders WHERE kennitala = '" + customers[customers.Count - 1].Kennitala + "'", MainDatabase.GetDB.MySQL).ExecuteScalar());
				}
                if (worker != null)
					if (worker.WorkerReportsProgress)
						worker.ReportProgress(0, "Idle");
				results.Close();
			}
			catch (Exception e)
			{
                if (worker != null)
					if (worker.WorkerReportsProgress)
						worker.ReportProgress(0, "Error occured");
				MainDatabase.GetDB.ErrorLog("Error while loading orders by date", e.Message, e.ToString());
			}
		}
    }
}
