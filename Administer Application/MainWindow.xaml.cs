using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Database;

namespace Administer_Application
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _mainWidthBinding = BindingOperations.GetBinding(mainColumnOpen, Grid.WidthProperty);
            mainColumnOpen.Width = 0;
            UserId = -1;
			DB = new Database.Database();
			_reportHandler = new WorkerReportHandler("Connecting in: 1", 0, 100);
			this.DataContext = DB;
			statusText.DataContext = _reportHandler;
			progressBar.DataContext = _reportHandler;

			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
			_backgroundWorker.DoWork += new DoWorkEventHandler(_worker_Thread);
			_backgroundWorker.WorkerReportsProgress = true;

			var timer = new System.Windows.Forms.Timer();
			timer.Tick += new EventHandler(timer_runWorker);
			timer.Interval = 1000;
			timer.Enabled = true;
			timer.Start();

			_pagesItemStorage = new Administer_Application.ObjectPages.ItemViewer();
        }

        Binding _mainWidthBinding;
		BackgroundWorker _backgroundWorker;
		WorkerReportHandler _reportHandler;
		public static Database.Database DB;
        public static int UserId;

		ObjectPages.ItemViewer _pagesItemStorage;

		private void timer_runWorker(object sender, EventArgs e)
		{
			var timer = (System.Windows.Forms.Timer)sender;
			if (timer.Interval == 1000)
			{
				_reportHandler.Status = "Starting connection";

				timer.Interval = 1000 * 60 * 10;
				if (!_backgroundWorker.IsBusy)
					_backgroundWorker.RunWorkerAsync("");
			}
			else if (!_backgroundWorker.IsBusy)
				_backgroundWorker.RunWorkerAsync("update");
		}

		void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			WorkerReportHandler handler = (WorkerReportHandler)e.UserState;
			_reportHandler.Status = handler.Status;
			_reportHandler.Value = handler.Value;
			_reportHandler.MaxValue = handler.MaxValue;
		}

		void _worker_Thread(object sender, DoWorkEventArgs e)
		{
			List<int> updateList = new List<int>();
			if ((string)e.Argument == "update")
			{
				if (DB.Update((BackgroundWorker)sender, ref updateList, Dispatcher))
					DB.ThumbManager.DownloadThumbs((BackgroundWorker)sender, ref updateList);
			}
			else
			{
				if (DB.LoadFromODBC((BackgroundWorker)sender,
                        Dispatcher,
						Properties.config.Default.mysql_host,
						"3307" /*Properties.config.Default.mysql_port*/,
						Properties.config.Default.mysql_database,
						Properties.config.Default.mysql_user))
					DB.ThumbManager.DownloadThumbs((BackgroundWorker)sender, ref updateList);
			}
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard columnOpen = (Storyboard)gridClose.FindResource("gridOpenAnimation"); // (Storyboard)FindResource("sizePopupClose");
            columnOpen.Completed += new EventHandler(columnOpen_Completed);
            columnOpen.Begin(this);
        }

        void columnOpen_Completed(object sender, EventArgs e)
        {
            if (!BindingOperations.IsDataBound(mainColumnOpen, Grid.WidthProperty))
                BindingOperations.SetBinding(mainColumnOpen, Grid.WidthProperty, _mainWidthBinding);

			loginWindow.Visibility = Visibility.Collapsed;
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (passwordTextbox.Password != "")
            {
                MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
                byte[] sec = Encoding.UTF8.GetBytes(passwordTextbox.Password);
                sec = x.ComputeHash(sec);
                StringBuilder s = new StringBuilder();
                foreach (byte b in sec)
                    s.Append(b.ToString("x2"));
                string pass = s.ToString();

                for (int I = 0; I < DB.Users.Count; I++)
                    if (DB.Users[I].Hash == pass)
                    {
                        UserId = DB.Users[I].ID;
                        statusUser.Text = string.Format("{0} logged in", DB.Users[I].Name);

                        Storyboard columnOpen = (Storyboard)gridClose.FindResource("gridOpenAnimation"); // (Storyboard)FindResource("sizePopupClose");
                        columnOpen.Completed += new EventHandler(columnOpen_Completed);
                        columnOpen.Begin(this);
                        return;
                    }
                System.Windows.MessageBox.Show("Password incorrect", "Login failed");
            }
        }

        private void radioButtonItems_Checked(object sender, RoutedEventArgs e)
        {
            mainPage.Navigate(_pagesItemStorage);
        }

		private void mainWindow_Closing(object sender, CancelEventArgs e)
		{
			Properties.config.Default.Save();
		}
    }
}
