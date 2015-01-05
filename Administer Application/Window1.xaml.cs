using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Database;

namespace Administer_Application
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

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
        }

		BackgroundWorker _backgroundWorker;
		WorkerReportHandler _reportHandler;
		public static Database.Database DB;

		private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch (Convert.ToInt32(((ListViewItem)e.AddedItems[0]).Tag))
			{
				case 0:
					mainPage.Navigate(new ObjectPages.FolderCollectionPage(DB.Folders));
					break;
			}
		}

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
				if (DB.Update((BackgroundWorker)sender, ref updateList))
					DB.ThumbManager.DownloadThumbs((BackgroundWorker)sender, ref updateList);
			}
			else
			{
				if (DB.LoadFromODBC((BackgroundWorker)sender,
						Properties.config.Default.mysql_host,
						Properties.config.Default.mysql_port,
						Properties.config.Default.mysql_database,
						Properties.config.Default.mysql_user))
					DB.ThumbManager.DownloadThumbs((BackgroundWorker)sender, ref updateList);
			}
		}
    }
}
