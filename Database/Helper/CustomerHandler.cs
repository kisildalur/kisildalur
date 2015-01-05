using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using System.ComponentModel;

namespace Database
{
    /// <summary>
    /// For customer transaction with database
    /// </summary>
    public class CustomerHandler
    {
        /// <summary>
        /// Initialize a new instance of CustomerHandler
        /// </summary>
        public CustomerHandler()
        {
        }

        /// <summary>
        /// Save changes of customer data to database
        /// </summary>
        /// <param name="customer">Customer to save</param>
        public void SaveCustomerDataToDatabase(Customer customer)
        {
            OdbcCommand command;

            try
            {
                MainDatabase.GetDB.Connect();

                if (customer.Id != -1)
                {
                    command = new OdbcCommand(string.Format("UPDATE prog_customer SET kennitala = '{1}', name = '{2}', homenumber = '{3}', gsmnumber = '{4}', worknumber = '{5}', address1 = '{6}', address2 = '{7}', city = '{8}', zip = '{9}', notes = '{10}', alarmnotes = '{11}' WHERE id = {0}", 
                                                                        customer.Id,
                                                                        customer.Kennitala,
                                                                        customer.Name,
                                                                        customer.Telephone,
                                                                        customer.Gsm,
                                                                        customer.WorkPhone,
                                                                        customer.Address1,
                                                                        customer.Address2,
                                                                        customer.City,
                                                                        customer.Zip,
                                                                        customer.Notes,
                                                                        customer.AlarmNotes), MainDatabase.GetDB.MySQL);

                    command.ExecuteNonQuery();
                }
                else
                {
                    command = new OdbcCommand(string.Format("INSERT INTO prog_customer (kennitala,name,homenumber,gsmnumber,worknumber,address1,address2,city,zip,notes,alarmnotes) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')",
                                                                        customer.Kennitala,
                                                                        customer.Name,
                                                                        customer.Telephone,
                                                                        customer.Gsm,
                                                                        customer.WorkPhone,
                                                                        customer.Address1,
                                                                        customer.Address2,
                                                                        customer.City,
                                                                        customer.Zip,
                                                                        customer.Notes,
                                                                        customer.AlarmNotes), MainDatabase.GetDB.MySQL);

                    command.ExecuteNonQuery();
                    command.CommandText = "SELECT LAST_INSERT_ID()";
                    customer.AssignNewId(Convert.ToInt32(command.ExecuteScalar()));
                }
            }
            catch (Exception e)
            {
                MainDatabase.GetDB.ErrorLog("Error while saveing to database", e.Message, e.ToString());
                return;
            }
        }

        /// <summary>
        /// Retreave and save all orders from specific Customer
        /// </summary>
        /// <param name="customer">The customer to retrave orders from.</param>
        public void GetOrdersFromCustomer(Customer customer)
        {
            OrderCollection orders = customer.Orders;
            DatabaseHelper.SearchDatabaseForOrders(null, ref orders, customer.Kennitala, "", 0, 0, DateTime.MinValue, DateTime.MaxValue, new List<string>());
        }

        /// <summary>
        /// Retreave and save all orders from specific Customer
        /// </summary>
        /// <param name="worker">Worker to report prograss and statitics</param>
        /// <param name="customer">The customer to retrave orders from.</param>
        public void GetOrdersFromCustomer(BackgroundWorker worker, Customer customer)
        {
            OrderCollection orders = customer.Orders;
            DatabaseHelper.SearchDatabaseForOrders(worker, ref orders, customer.Kennitala, "", 0, 0, DateTime.MinValue, DateTime.MaxValue, new List<string>());
        }

        /// <summary>
        /// Gets the total of customer who has the name like the one specified.
        /// </summary>
        /// <param name="name">The name to search in database.</param>
        /// <returns>The number of customers containing specified name</returns>
        public int SearchTotalCustomer(string name)
        {
            try
            {
                MainDatabase.GetDB.Connect();

                OdbcCommand command = new OdbcCommand(string.Format("SELECT count(*) FROM prog_customer WHERE name LIKE '%{0}%'", name), MainDatabase.GetDB.MySQL);
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception e)
            {
                MainDatabase.GetDB.ErrorLog("Error while retreaving the number of customers with name like the one specified from database", e.Message, e.ToString());

                return -1;
            }
        }

        /// <summary>
        /// Retreave a single customer from database. If a customer is not found in the database, it will retreave a new instance of Customer containing the kennitala value.
        /// </summary>
        /// <param name="kennitala">Kennitala to search for in database. If customer is not found then a new one is created.</param>
        /// <returns>The customer data</returns>
        public Customer RetreaveCustomer(string kennitala)
        {
            try
            {
                MainDatabase.GetDB.Connect();

                OdbcCommand command = new OdbcCommand(string.Format("SELECT id,kennitala,name,homenumber,gsmnumber,worknumber,address1,address2,city,zip,notes,alarmnotes FROM prog_customer WHERE kennitala = '{0}'", kennitala), MainDatabase.GetDB.MySQL);
                OdbcDataReader results = command.ExecuteReader();

                Customer customer = null;
                while (results.Read())
                {
                    customer = new Customer(results.GetInt32(0), results.GetString(1), results.GetString(2), results.GetString(3), results.GetString(4), results.GetString(5), results.GetString(6), results.GetString(7), results.GetString(8), results.GetString(9), results.GetString(10), results.GetString(11));
					break;
                }

                if (customer == null)
                {
                    customer = new Customer();
                    customer.Kennitala = kennitala;
                }

                return customer;
            }
            catch (Exception e)
            {
                MainDatabase.GetDB.ErrorLog("Error while retreaving customer data from database", e.Message, e.ToString());

                return new Customer();
            }
        }

        /// <summary>
        /// Retreave a single customer from database
        /// </summary>
        /// <param name="id">The id of customer to retreave</param>
        /// <returns>The customer data</returns>
        public Customer RetreaveCustomer(int id)
        {
            try
            {
                MainDatabase.GetDB.Connect();

                OdbcCommand command = new OdbcCommand(string.Format("SELECT id,kennitala,name,homenumber,gsmnumber,worknumber,address1,address2,city,zip,notes,alarmnotes FROM prog_customer WHERE id = '{0}'", id), MainDatabase.GetDB.MySQL);
                OdbcDataReader results = command.ExecuteReader();

                Customer customer = null;
                while (results.Read())
                {
                    customer = new Customer(results.GetInt32(0), results.GetString(1), results.GetString(2), results.GetString(3), results.GetString(4), results.GetString(5), results.GetString(6), results.GetString(7), results.GetString(8), results.GetString(9), results.GetString(10), results.GetString(11));
                }
                if (customer == null)
                    return new Customer();

                return customer;
            }
            catch (Exception e)
            {
                MainDatabase.GetDB.ErrorLog("Error while retreaving customer data from database", e.Message, e.ToString());

                return new Customer();
            }
        }

		/// <summary>
		/// Retreave a list of all customers whose names are like the one specified.
		/// </summary>
		/// <param name="kennitala">Kennitala þess sem leitast er verið eftir</param>
		/// <param name="name">The name to search in the database.</param>
		/// <param name="collection">A refrence to the collection where all customers will be added to.</param>
        public void RetreaveCustomerCollection(string name, ref CustomerCollection collection)
        {
            DatabaseHelper.SearchDatabaseForCustomer(null, ref collection, "", name, "", "", "", "", "", "", "");
        }

		/// <summary>
		/// Retreave a list of all customers whose names or kennitala are like the one specified.
		/// </summary>
		/// <param name="kennitala">Portion of the Kennitala to search for.</param>
		/// <param name="name">The name to search in the database.</param>
		/// <param name="collection">A refrence to the collection where all customers will be added to.</param>
		/// <param name="worker">Worker that reports progress and such</param>
        public void RetreaveCustomerCollection(string kennitala, string name, ref CustomerCollection collection, BackgroundWorker worker)
        {
            DatabaseHelper.SearchDatabaseForCustomer(worker, ref collection, kennitala, name, "", "", "", "", "", "", "");
        }
    }
}
