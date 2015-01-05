using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.ComponentModel;
using System.Data.Odbc;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Database
{
    public class MainDatabase
    {
        /// <summary>
        /// Initialice a new instance of Database
        /// </summary>
        public MainDatabase()
        {
            _folders = new FolderCollection();
            _users = new UserCollection();
            _pantanir = new OrderCollection();
            _siteOrders = new SiteOrderCollection();
			_payMethods = new PayMethodCollection();
			_thumbManager = new ThumbManager(this);

            _customerHandler = new CustomerHandler();

            GetDB = this;
        }

        private FolderCollection _folders;
        private DateTime _lastChecked;
        private UserCollection _users;
        private OrderCollection _pantanir;
        private OdbcConnection _mysql;
        private SiteOrderCollection _siteOrders;
        private PayMethodCollection _payMethods;
        private CustomerHandler _customerHandler;
		private ThumbManager _thumbManager;
        public static MainDatabase GetDB;

        private string _host;
        private string _port;
        private string _database;
        private string _user;

        /// <summary>
        /// Get CustomerHandler to retreave, edit and manipulate customer data
        /// </summary>
        public CustomerHandler CustomerHandler
        {
            get { return _customerHandler; }
        }
        /// <summary>
        /// Get folder data.
        /// </summary>
        public FolderCollection Folders
        {
            get { return _folders; }
        }
        /// <summary>
        /// Get user data.
        /// </summary>
        public UserCollection Users
        {
            get { return _users; }
        }
        /// <summary>
        /// Get order data.
        /// </summary>
        public OrderCollection Orders
        {
            get { return _pantanir; }
        }
		/// <summary>
		/// Get the thumb manager used to check for new thumbs
		/// </summary>
		public ThumbManager ThumbManager
		{
			get { return _thumbManager; }
		}
		/// <summary>
        /// Get MySQL connection item.
        /// </summary>
        public OdbcConnection MySQL
        {
            get { return _mysql; }
        }
        /// <summary>
        /// Get paymethods list.
        /// </summary>
        public PayMethodCollection PayMethods
        {
            get { return _payMethods; }
        }
        /// <summary>
        /// Get site order list.
        /// </summary>
        public SiteOrderCollection SiteOrders
        {
            get { return _siteOrders; }
        }
        /// <summary>
        /// Gets the total number of all items in the database
        /// </summary>
        /// <returns></returns>
        public int GetTotalNumberOfItems()
        {
            int total = 0;
            foreach (Category category in GetCategories())
                total += category.Items.Count;
            return total;
        }
        /// <summary>
        /// Enumerate through folder and return each category found.
        /// </summary>
        /// <returns>Each and every category.</returns>
		public IEnumerable<Category> GetCategories()
		{
			foreach (Folder folder in _folders)
			{
				foreach (Category cat in folder.Categories)
				{
					cat.AlbumId = folder.ID;
					yield return cat;
				}
			}
		}

        /// <summary>
        /// Enumerate through each and every category and return each product item found.
        /// </summary>
        /// <returns>Each and every product item.</returns>
		public IEnumerable<Item> GetItems()
		{
			foreach(Category cat in GetCategories())
			{
				foreach (Item item in cat.Items)
					yield return item;
			}
		}

        /// <summary>
        /// Enumerate through collection in a search for a specific item
        /// </summary>
        /// <param name="itemId">The id of the item you are searching for</param>
        /// <returns>The item or null if not found</returns>
        public Item GetItem(int itemId)
        {
            foreach (Item item in GetItems())
            {
                if (item.ID == itemId)
                    return item;
            }
            return null;
        }

        public void ErrorLog(string message, string header, string all)
        {
            try
            {
                StreamWriter write = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\error.log", true);
                write.WriteLine("\n-----------------------------\n\n");
                write.WriteLine(all);
                write.Flush();
                write.Close();

                if (message != "")
                    System.Windows.Forms.MessageBox.Show(message + ":\n\n\t" + header);
                else
                    System.Windows.Forms.MessageBox.Show("Unknown error occured:\n\n\t" + header);
            }
            catch
            {
                if (message != "")
                    System.Windows.Forms.MessageBox.Show(message + " (This error was NOT logged):\n\n\t" + all);
                else
                    System.Windows.Forms.MessageBox.Show("Unknown error occured (This error was NOT logged):\n\n\t" + all);
            }
        }

		private delegate void delegateClearCollections();
		private void ClearCollecions()
		{
			_folders.Clear();
			_users.Clear();
			_siteOrders.Clear();
			_payMethods.Clear();
			_pantanir.Clear();
		}

		delegate void delegateLoadData(OdbcDataReader results);
		private void LoadDataFolders(OdbcDataReader results)
		{
			_folders.Add(new Folder(results.GetInt32(0), results.GetString(1), results.GetInt32(2) == 1 ? true : false));
		}

		private void LoadDataCategories(OdbcDataReader results)
		{
			_folders[results.GetInt32(0), true].Categories.Add(new Category(results.GetInt32(1), results.GetString(2), results.GetInt32(3) == 1 ? true : false));
		}

		private void LoadDataUsers(OdbcDataReader results)
		{
			_users.Add(new User(results.GetInt32(0), results.GetString(1), results.GetString(2), results.GetString(3)));
		}
		private void LoadDataProperties(OdbcDataReader results)
		{
			foreach (Category cat in GetCategories())
			{
				if (cat.ID == results.GetInt32(0))
				{
					PropertyGroup propertyGroup = new PropertyGroup(results.GetInt32(1), results.GetString(2));
					OdbcCommand subCommand = new OdbcCommand(string.Format("SELECT id,name,description FROM property WHERE fk_propertygroup = {0}", propertyGroup.Id), _mysql);
					OdbcDataReader subReader = subCommand.ExecuteReader();
					while (subReader.Read())
					{
						propertyGroup.Properties.Add(new Property(subReader.GetInt32(0), subReader.GetString(1), subReader.GetString(2)));
					}
					cat.PropertyGroups.Add(propertyGroup);
					break;
				}
			}
		}
		private void LoadDataItems(OdbcDataReader results)
		{
			for (int I = 0; I < _folders.Count; I++)
			{
				if (_folders[I].Categories[results.GetInt32(0), true] != null)
				{
					_folders[I].Categories[results.GetInt32(0), true].Items.Add(new Item(
															results.GetInt32(1),
															results.GetString(2),
															results.GetString(3),
															results.GetString(4),
															results.GetString(5),
															results.GetString(6),
															results.GetInt32(7),
															results.GetInt32(8),
															(results.GetInt32(9) == 1 ? true : false),
															(results.GetInt32(10) == 1 ? true : false),
															results.GetInt32(11)));

					//Item item = _folders[I].Categories[results.GetInt32(0), true].Items[_folders[I].Categories[results.GetInt32(0), true].Items.Count - 1];
					break;
				}
			}
		}
		private void LoadDataSubitems(OdbcDataReader results)
		{
			foreach (Item item in MainDatabase.GetDB.GetItems())
			{
				if (item.ID == results.GetInt32(0))
				{
					FindAndRetreaveSubitemsFromDatabase(item);
					break;
				}
			}
		}
		private void LoadDataPaymethods(OdbcDataReader results)
		{
			_payMethods.Add(new PayMethod(results.GetInt32(0), results.GetString(1), results.GetInt32(2)));
		}

		private delegate void delegateCreateTemporaryAdminUser();
		private void CreateTemporaryAdminUser()
		{
			_users.Add(new User(0, "Admin", "21232f297a57a5a743894a0e4a801fc3", "admin"));
		}
		/// <summary>
        /// Clear all data and retreave a fresh set of all data from the database.
        /// </summary>
        /// <param name="worker">Background worker used to return status updates.</param>
        public bool LoadFromODBC(BackgroundWorker worker, Dispatcher dispatcher, string host, string port, string database, string user)
        {
			dispatcher.Invoke(new delegateClearCollections(ClearCollecions));

			_host = host;
			_port = port;
			_database = database;
			_user = user;

			try
			{
				string connection = string.Format("{0};{1};{2};{3};{4};{5};{6}",
									"DRIVER={MySQL ODBC 3.51 Driver}",
									string.Format("SERVER={0}", _host),
									string.Format("PORT={0}", _port),
									string.Format("DATABASE={0}", _database),
									string.Format("UID={0}", _user),
									"PASSWORD=tncm2s1",
									"OPTION=3");

				_mysql = new OdbcConnection(connection);
				worker.ReportProgress(0, new WorkerReportHandler("Connecting to mysql server please wait...", 0, 100));
				_mysql.Open();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while connecting to website", e.Message, e.ToString());

				dispatcher.Invoke(new delegateCreateTemporaryAdminUser(CreateTemporaryAdminUser));

				worker.ReportProgress(0, new WorkerReportHandler("Error connecting to Database", 0, 100));
				return false;
			}


			OdbcCommand command = new OdbcCommand();
			OdbcDataReader results;

			try
			{
				command = new OdbcCommand("SELECT id,name,visible FROM folder", _mysql);
				worker.ReportProgress(0, new WorkerReportHandler("Connected: receaving data (folders)", 0, 0));
				results = command.ExecuteReader();
				while (results.Read())
				{
					dispatcher.Invoke(new delegateLoadData(LoadDataFolders), results);
				}
				results.Close();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while loading folders", e.Message, e.ToString());
			}

			try
			{
				command = new OdbcCommand("SELECT fk_folder,id,name,visible FROM category", _mysql);
				worker.ReportProgress(0, new WorkerReportHandler("Connected: receaving data (categories)", 20, 100));
				results = command.ExecuteReader();
				while (results.Read())
				{
					dispatcher.Invoke(new delegateLoadData(LoadDataCategories), results);
				}
				results.Close();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while loading Categories", e.Message, e.ToString());
			}

			try
			{
				command.CommandText = "SELECT id,name,`password`,`privileges` FROM prog_users";
				worker.ReportProgress(0, new WorkerReportHandler("Connected: reading data (users)", 40, 100));
				results = command.ExecuteReader();
				while (results.Read())
				{
					try
					{
						dispatcher.Invoke(new delegateLoadData(LoadDataUsers), results);
					}
					catch (Exception e)
					{
					}
				}
				results.Close();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while loading users", e.Message, e.ToString());
			}

			try
			{
				command.CommandText = "SELECT fk_category,id,name FROM propertygroup";
				worker.ReportProgress(0, new WorkerReportHandler("Connected: receaving data (propertygroup)", 20, 100));
				results = command.ExecuteReader();
				while (results.Read())
				{
					dispatcher.Invoke(new delegateLoadData(LoadDataProperties), results);
				}
				results.Close();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while loading Categories", e.Message, e.ToString());
			}

			try
			{
				command.CommandText = "SELECT fk_category,id,barcode,prog_id,name,subtitle,description,stock,price,visible,calculate_price,fk_album FROM product";
				worker.ReportProgress(0, new WorkerReportHandler("Connected: receaving data (items)", 60, 100));
				results = command.ExecuteReader();
				while (results.Read())
				{
					dispatcher.Invoke(new delegateLoadData(LoadDataItems), results);
				}
				results.Close();

				//command.CommandText = "SELECT fk_category,id,prog_id,name,subtitle,description,stock,price,visible,fk_album FROM product,product_group WHERE fk_product = id GROUP BY id";
				command.CommandText = "SELECT product.id FROM product,product_group WHERE fk_product = id GROUP BY id";
				worker.ReportProgress(0, new WorkerReportHandler("Connected: reading data (subitems)", 70, 100));
				results = command.ExecuteReader();
				while (results.Read())
				{
					dispatcher.Invoke(new delegateLoadData(LoadDataSubitems), results);
				}
				results.Close();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while loading items", e.Message, e.ToString());
			}

			try
			{
				command.CommandText = "SELECT id,method,`order` FROM prog_paymethods ORDER BY `order`";
				worker.ReportProgress(0, new WorkerReportHandler("Connected: reading data (pay methods)", 85, 100));
				results = command.ExecuteReader();
				while (results.Read())
				{
					dispatcher.Invoke(new delegateLoadData(LoadDataPaymethods), results);
				}
				results.Close();
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while loading pay methods", e.Message, e.ToString());
			}

			try
			{
				worker.ReportProgress(0, new WorkerReportHandler("Connected: reading site orders", 95, 100));
				CheckForSiteOrders(dispatcher);

				Disconnect();
				worker.ReportProgress(0, new WorkerReportHandler("Finished", 0, 100));
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while loading site orders", e.Message, e.ToString());
			}

			_lastChecked = DateTime.Now;
			return true;
        }

        /// <summary>
        /// Connect to database and update all changes found since last update.
        /// Used to refresh item stock and such from the database.
        /// Returns whether any changes were found.
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="worker">Background worker used to retreave status update</param>
		public bool Update(BackgroundWorker worker, ref List<int> updateList, Dispatcher dispatcher)
		{
			bool anyChanges = false;

			try
			{
				if (Connect())
				{
					worker.ReportProgress(0, new WorkerReportHandler("Checking for updates", 0, 100));

					OdbcCommand command = new OdbcCommand(string.Format("SELECT fk_category,id,barcode,prog_id,name,subtitle,description,stock,price,visible,calculate_price,fk_album FROM product WHERE lastedited >= '{0}-{1}-{2} {3:00}:{4:00}:{5:00}'",
															  _lastChecked.Year,
															  _lastChecked.Month,
															  _lastChecked.Day,
															  _lastChecked.Hour,
															  _lastChecked.Minute - 5,
															  _lastChecked.Second), _mysql);

					OdbcDataReader results = command.ExecuteReader();

					while (results.Read())
					{
						//Breytingar hafa orðið á kerfinu, láta vita af því.
						worker.ReportProgress(0, new WorkerReportHandler("Retreaving changes", 0, 100));
						anyChanges = true;

						dispatcher.Invoke(new delegateLoadData(UpdateItemFromResult), results);
					}
					results.Close();

					CheckForSiteOrders(dispatcher);

					Disconnect();
					_lastChecked = DateTime.Now;
				}
			}
			catch (Exception e)
			{
				MainDatabase.GetDB.ErrorLog("Error while update-ing items", e.Message, e.ToString());
			}
			worker.ReportProgress(0, new WorkerReportHandler("Finished", 0, 100));

			return anyChanges;
		}

		private void CheckForSiteOrders(Dispatcher dispatcher)
		{
			OdbcDataReader dataReader = null;
			OdbcCommand command = new OdbcCommand("", this._mysql);
			for (int i = 0; i < this._siteOrders.Count; i++)
			{
				command.CommandText = string.Format("SELECT `order`.id FROM `order` WHERE `order`.id = {0} AND `order`.stage = 3", this._siteOrders[i].Id);
				dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					dispatcher.Invoke(new delegateLoadData(RemoveOrders), dataReader);
				}
				dataReader.Close();
			}

			command.CommandText = "SELECT `order`.`id`, user.kennitala, user.name, user.realname, `order`.`stage` , `order`.`date` , `order`.`shipping` , `order`.`payment` FROM `order`, user WHERE user.id = `order`.fk_user AND `order`.stage < 3 ORDER BY `order`.date DESC";
			dataReader = command.ExecuteReader();

			while (dataReader.Read())
			{
				dispatcher.Invoke(new delegateLoadData(ReadOrders), dataReader);
			}
			dataReader.Close();
		}
		private void UpdateItemFromResult(OdbcDataReader results)
        {
            //Sækja gömlu vöruna út úr gagnagrunninum
            Item oldItem = GetItem(results.GetInt32(1));

            //Sækja nýju vöruna út úr minninu
            Item newItem = new Item(results.GetInt32(1),
									results.GetString(2),
                                    results.GetString(3),
                                    results.GetString(4),
                                    results.GetString(5),
                                    results.GetString(6),
                                    results.GetInt32(7),
                                    results.GetInt32(8),
                                    (results.GetInt32(9) == 1 ? true : false),
                                    (results.GetInt32(10) == 1 ? true : false),
                                    results.GetInt32(11));

            //Ef gamla varan finnst ekki, þá er varan ný
            if (oldItem == null)
            {
                oldItem = new Item();
                oldItem.AssignNewId(newItem.ID);
                oldItem.Album = newItem.Album;
            }

            oldItem.Name = newItem.Name;
            oldItem.Price = newItem.Price;
            oldItem.ProductID = newItem.ProductID;
            oldItem.Description = newItem.Description;
            oldItem.Sub = newItem.Sub;
            oldItem.Stock = newItem.Stock;
            oldItem.Visible = newItem.Visible;
            oldItem.CalculatePrice = newItem.CalculatePrice;
            oldItem.SubProducts.Clear();

            FindAndRetreaveSubitemsFromDatabase(oldItem);
        }

		private void RemoveOrders(OdbcDataReader results)
		{
			this._siteOrders.Remove(this._siteOrders[results.GetInt32(0), true]);
		}

		private void ReadOrders(OdbcDataReader results)
		{

			SiteOrder order = this._siteOrders[results.GetInt32(0), true];
			bool craeted = false;
			if (order == null)
				order = new SiteOrder();

			if (order.Id == -1)
				craeted = true;

			order.Id = results.GetInt32(0);
			order.Kennitala = results.GetString(1);
			order.Username = results.GetString(2);
			order.Name = results.GetString(3);
			order.ParseStage(results.GetInt32(4));
			order.Date = DateTime.Parse(results.GetString(5));
			order.ParseJSonShipping(results.GetString(6));
			order.ParseJSonPaymethod(results.GetString(7));

			if (craeted)
			{
				RetraeveAllSiteOrderItems(order);
				this._siteOrders.Add(order);
			}
		}

		private void RetraeveAllSiteOrderItems(SiteOrder siteOrder)
		{
			OdbcCommand command = new OdbcCommand(string.Format("SELECT orderitem.id, orderitem.fk_id, product.prog_id, orderitem.name, orderitem.price, orderitem.`count` FROM orderitem, product WHERE product.id = orderitem.fk_id AND orderitem.fk_order = {0}", siteOrder.Id), this._mysql);
			OdbcDataReader results = command.ExecuteReader();
			siteOrder.Items.Clear();
			while (results.Read())
			{
				siteOrder.Items.Add(new CartItem(results.GetInt32(0), results.GetInt32(1), results.GetString(2), results.GetString(3), results.GetInt64(4), results.GetInt32(5)));
			}
		}

        private void FindAndRetreaveSubitemsFromDatabase(Item item)
		{
			OdbcCommand subCommand = new OdbcCommand(string.Format("SELECT fk_extra,groupid,count FROM product_group WHERE fk_product = {0} ",
																				item.ID), _mysql);
			OdbcDataReader subResults = subCommand.ExecuteReader();
			while (subResults.Read())
			{
				foreach (Item subItem in MainDatabase.GetDB.GetItems())
				{
					if (subItem.ID == subResults.GetInt32(0))
					{
						OrderItem subOrderItem = subItem.ConvertToOrderItem();
						subOrderItem.Id = subResults.GetInt32(1);
						subOrderItem.Count = subResults.GetInt32(2);
						item.SubProducts.Add(subOrderItem);
						break;
					}
				}
			}
		}
		private delegate void delegateCollectionsClear();
		private void CollectionsClear()
		{
		}

        #region Load from sqlite
        /*public void Load()
        {
			SQLite sql;
			try
			{
                sql = new SQLite("gagnagrunnur.db");
			}
			catch (Exception e)
			{
				MessageBox.Show("Error while opening database:\n\n\t" + e.ToString());
				return;
			}


			string temp;

			//--------------------------------------------------------------------------------//
			//---------------------------Read from Flokkur------------------------------------//
			//--------------------------------------------------------------------------------//
			try
			{
                AssociatedRowList flokkar = sql.QueryAssoc("SELECT * FROM Flokkur");
                Flokkur f = new Flokkur();
                for (int I = 0; I < flokkar.Count; I++)
                {
                    if (flokkar[I].TryGetValue("id", out temp))
                        f.ID = Convert.ToInt32(temp);
                    if (flokkar[I].TryGetValue("Name", out temp))
                        f.Name = temp;

                    _directories.Add(f);
                    f = new Flokkur();
                }
			}
			catch (Exception e)
			{
				try
				{
					sql.Query("CREATE TABLE Flokkur (id INTEGER PRIMARY KEY, Name TEXT)");
				}
				catch (Exception err)
				{
					MessageBox.Show("Error while creating table Flokkur in database:\n\n\t" + err.Message + "\n\nError triggering the creation command:\n\n\t" + e.ToString());
					return;
				}
			}

			//--------------------------------------------------------------------------------//
			//----------------------------Read from Items------------------------------------//
			//--------------------------------------------------------------------------------//
			try
			{
                AssociatedRowList hlutir = sql.QueryAssoc("SELECT * FROM Items");
                Hlut i = new Hlut();
                for (int I = 0; I < hlutir.Count; I++)
                {
                    if (hlutir[I].TryGetValue("id", out temp))
                        i.ID = Convert.ToInt32(temp);
                    if (hlutir[I].TryGetValue("ItemId", out temp))
                        i.ProductID = temp;
                    if (hlutir[I].TryGetValue("Name", out temp))
                        i.Name = temp;
                    if (hlutir[I].TryGetValue("Price", out temp))
                        i.Price = Convert.ToInt64(temp);
                    if (hlutir[I].TryGetValue("Stock", out temp))
                        i.Stock = Convert.ToInt32(temp);
                    if (hlutir[I].TryGetValue("FlokkurId", out temp))
                    {
                        for (int A = 0; A < _directories.Count; A++)
                        {
                            if (_directories[A].ID == Convert.ToInt32(temp))
                                _directories[A].Items.Add(i);
                        }
                    }
                    i = new Hlut();
                }
			}
			catch (Exception e)
			{
				try
				{
					sql.Query("CREATE TABLE Items (id INTEGER PRIMARY KEY, ItemId TEXT, Name TEXT, Price NUMERIC, Stock NUMERIC, FlokkurId NUMERIC)");
				}
				catch (Exception err)
				{
					MessageBox.Show("Error while creating table Flokkur in database:\n\n\t" + err.Message + "\n\nError triggering the creation command:\n\n\t" + e.ToString());
					return;
				}
			}

			//--------------------------------------------------------------------------------//
			//----------------------------Read from Users------------------------------------//
			//--------------------------------------------------------------------------------//
			try
			{
                AssociatedRowList users = sql.QueryAssoc("SELECT * FROM Users");
                User u = new User();
                for (int I = 0; I < users.Count; I++)
                {
                    if (users[I].TryGetValue("id", out temp))
                        u.ID = Convert.ToInt32(temp);
                    if (users[I].TryGetValue("Name", out temp))
                        u.Name = temp;
                    if (users[I].TryGetValue("Hash", out temp))
                        u.Hash = temp;

                    _users.Add(u);
                    u = new User();
                }
			}
			catch (Exception e)
			{
				try
				{
					sql.Query("CREATE TABLE Users (id INTEGER PRIMARY KEY, Name TEXT, Hash BLOB)");
					sql.Query("INSERT INTO Users (Name, Hash) VALUES ('admin', '21232f297a57a5a743894a0e4a801fc3')");
					User u = new User();
					u.ID = sql.LastInsertID();
					u.Name = "admin";
					u.Hash = "21232f297a57a5a743894a0e4a801fc3";
					_users.Add(u);
				}
				catch (Exception err)
				{
					MessageBox.Show("Error while creating table Flokkur in database:\n\n\t" + err.Message + "\n\nError triggering the creation command:\n\n\t" + e.ToString());
					return;
				}
			}

			//--------------------------------------------------------------------------------//
			//---------------------------Read from Pantanir-----------------------------------//
			//--------------------------------------------------------------------------------//
			try
			{
				sql.QueryAssoc("SELECT * FROM Pantanir");
			}
			catch (Exception e)
			{
				try
				{
					sql.Query("CREATE TABLE Pantanir (id INTEGER PRIMARY KEY, Name TEXT, Kennitala TEXT, PayMethod TEXT, Items NUMERIC, Abyrgd NUMERIC)");
				}
				catch (Exception err)
				{
					MessageBox.Show("Error while creating table Flokkur in database:\n\n\t" + err.Message + "\n\nError triggering the creation command:\n\n\t" + e.ToString());
					return;
				}
			}

			//--------------------------------------------------------------------------------//
			//------------------------Read from Pantanir_items--------------------------------//
			//--------------------------------------------------------------------------------//
			try
			{
				sql.QueryAssoc("SELECT * FROM Pantanir_items");
			}
			catch (Exception e)
			{
				try
				{
					sql.Query("CREATE TABLE Pantanir_items (ItemId NUMERIC, PontunId NUMERIC)");
				}
				catch (Exception err)
				{
					MessageBox.Show("Error while creating table Flokkur in database:\n\n\t" + err.Message + "\n\nError triggering the creation command:\n\n\t" + e.ToString());
					return;
				}
			}

			//--------------------------------------------------------------------------------//
			//---------------------------Read from PontunItems--------------------------------//
			//--------------------------------------------------------------------------------//
			try
			{
				sql.QueryAssoc("SELECT * FROM PontunItems");
			}
			catch (Exception e)
			{
				try
				{
					sql.Query("CREATE TABLE PontunItems (id INTEGER PRIMARY KEY, Vorunr TEXT, Lysing TEXT, Verd TEXT, Magn TEXT, Samtals TEXT)");
				}
				catch (Exception err)
				{
					MessageBox.Show("Error while creating table Flokkur in database:\n\n\t" + err.Message + "\n\nError triggering the creation command:\n\n\t" + e.ToString());
					return;
				}
			}

			//--------------------------------------------------------------------------------//
			//----------------------------Read from PayMethods--------------------------------//
			//--------------------------------------------------------------------------------//
			try
			{
				sql.QueryAssoc("SELECT * FROM PayMethods");
			}
			catch (Exception e)
			{
				try
				{
					sql.Query("CREATE TABLE PayMethods (id INTEGER PRIMARY KEY, Name TEXT)");
					sql.Query("INSERT INTO PayMethods (Name) VALUES ('Staðgreitt')");
					sql.Query("INSERT INTO PayMethods (Name) VALUES ('Póstkrafa')");
				}
				catch (Exception err)
				{
					MessageBox.Show("Error while creating table Flokkur in database:\n\n\t" + err.Message + "\n\nError triggering the creation command:\n\n\t" + e.ToString());
					return;
				}
			}
        }*/
        #endregion

        /// <summary>
        /// Open and connect to database. Reopen lost connection if connections gets closed or broken.
        /// </summary>
        public bool Connect()
        {
            try
            {
                if (_mysql.State != System.Data.ConnectionState.Open)
                    _mysql.Close();
                else
                    return true;

                _mysql.Open();
                return true;
            }
            catch (Exception e)
            {
                MainDatabase.GetDB.ErrorLog("Error while connecting", e.Message, e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Disconnect from database to prevent the connection from becoming broken or lost.
        /// </summary>
        public void Disconnect()
        {
            try
            {
				if (_mysql.State != System.Data.ConnectionState.Closed)
					_mysql.Close();
            }
            catch (Exception e)
            {
                MainDatabase.GetDB.ErrorLog("Error while disconnecting", e.Message, e.ToString());
            }
        }

        /// <summary>
        /// Get or set the host address to the mysql server
        /// </summary>
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        /// <summary>
        /// Get or set the port of the mysql server
        /// </summary>
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Get or set the database of the mysql server
        /// </summary>
        public string Database1
        {
            get { return _database; }
            set { _database = value; }
        }

        /// <summary>
        /// Get or set the user of the mysql server
        /// </summary>
        public string User
        {
            get { return _user; }
            set { _user = value; }
        }
    }

    

    

    

    

    

    

    

    

    
}
