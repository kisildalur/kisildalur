using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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
using System.Windows.Forms;
using Database;

namespace Kisildalur
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

			DB = new Database.MainDatabase();
			_siteOrdersNew = new SiteOrderCollection();
			_siteOrdersConfirmed = new SiteOrderCollection();
            _reportHandler = new WorkerReportHandler("Connecting in: 1", 0, 100);
            this.DataContext = _reportHandler;
			this.listNewOrders.ItemsSource = _siteOrdersNew;
			this.listConfirmedOrders.ItemsSource = _siteOrdersConfirmed;

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
            _backgroundWorker.DoWork += new DoWorkEventHandler(_worker_Thread);
			_backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_backgroundWorker_RunWorkerCompleted);
            _backgroundWorker.WorkerReportsProgress = true;

            Timer timer = new Timer();
            timer.Tick += new EventHandler(timer_runWorker);
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Start();
        }

        BackgroundWorker _backgroundWorker;
        WorkerReportHandler _reportHandler;
		public static Database.MainDatabase DB;
		SiteOrderCollection _siteOrdersNew;
		SiteOrderCollection _siteOrdersConfirmed;

        private void buttonNewOrder_Click(object sender, RoutedEventArgs e)
        {
            int usrId;
            if (Login(out usrId))
                ShowWindow(new NewOrder(usrId), false);
        }

        /*private void buttonNewOffer_Click(object sender, RoutedEventArgs e)
        {
            int usrId;
            if (Login(out usrId))
                ShowForm(new Pontun(usrId, false), false);
        }*/

        private void buttonDatabase_Click(object sender, RoutedEventArgs e)
        {
            int usrId;
            if (Login(out usrId))
                ShowWindow(new Vorugeymsla(), true);
        }

        private void buttonSettings_Click(object sender, RoutedEventArgs e)
        {
            int usrId;
            if (Login(out usrId))
				//ShowForm(new A_History(usrId), true);
                ShowWindow(new Settings(usrId), true);
        }

        private void buttonHistory_Click(object sender, RoutedEventArgs e)
        {
            int usrId;
            if (Login(out usrId))
                ShowForm(new A_History(usrId), true);
                //ShowWindow(new History(), false);
        }

        private void ShowForm(System.Windows.Forms.Form form, bool showAsDialog)
        {
            if (showAsDialog)
                form.ShowDialog();
            else
                form.Show();
        }

		private void ShowWindow(Window window, bool showAsDialog)
		{
			//window.Owner = this;
			if (showAsDialog)
				window.ShowDialog();
			else
				window.Show();
		}

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool Login(out int id)
        {
            id = 0;
            Main_Login l = new Main_Login();

            l.ShowDialog();
            if (l._pass.Text == "")
                return false;

            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] sec = Encoding.UTF8.GetBytes(l._pass.Text.ToLower());
            sec = x.ComputeHash(sec);
            StringBuilder s = new StringBuilder();
            foreach (byte b in sec)
                s.Append(b.ToString("x2"));
            string pass = s.ToString();

            for (int I = 0; I < DB.Users.Count; I++)
                if (DB.Users[I].Hash == pass)
                {
                    id = DB.Users[I].ID;
                    return true;
                }
            System.Windows.MessageBox.Show("Password incorrect", "Login failed");
            return false;
        }

        private void timer_runWorker(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
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
                        Properties.config.Default.mysql_port,
                        Properties.config.Default.mysql_database,
                        Properties.config.Default.mysql_user))
                    DB.ThumbManager.DownloadThumbs((BackgroundWorker)sender, ref updateList);
            }
        }

		void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_siteOrdersNew.Clear();
			_siteOrdersConfirmed.Clear();
			for (int i = 0; i < DB.SiteOrders.Count; i++)
			{
				if (DB.SiteOrders[i].Stage == SiteOrderStage.New)
					_siteOrdersNew.Add(DB.SiteOrders[i]);
				else
					_siteOrdersConfirmed.Add(DB.SiteOrders[i]);
			}
		}

        private void refreshDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (!_backgroundWorker.IsBusy)
                _backgroundWorker.RunWorkerAsync("update");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_backgroundWorker.IsBusy)
                _backgroundWorker.RunWorkerAsync();
        }

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			Properties.config.Default.Save();
		}

		private void WebSiteOrder_ViewOrderClick(object sender, RoutedEventArgs e)
		{
			int usrId;
			if (Login(out usrId))
				ShowWindow(new NewOrder(usrId, (sender as FrameworkElement).DataContext as SiteOrder), false);
		}
    }
}