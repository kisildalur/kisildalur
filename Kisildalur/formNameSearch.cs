using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Database;

namespace Kisildalur
{
    public partial class formNameSearch : Form
    {
        public formNameSearch()
        {
            InitializeComponent();

            this.CenterToScreen();
        }

        string name;
        string _kennitala;

        public string GetKennitala
        {
            get { return _kennitala; }
        }

        private void _loadKennitala_Click(object sender, EventArgs e)
        {
            if (_results.SelectedItems.Count == 1)
            {
                _kennitala = _results.SelectedItems[0].SubItems[1].Text.Replace("-", "");
                this.Close();
            }
        }

        private void _cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _name_TextChanged(object sender, EventArgs e)
        {
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            name = _name.Text;

            CustomerHandler handler = new CustomerHandler();
            CustomerCollection collection = new CustomerCollection();

            if (InvokeRequired)
                Invoke(new ListviewClear(ClearListView));
            int total = handler.SearchTotalCustomer(name);
            if (total < 15)
            {
                handler.RetreaveCustomerCollection("", name, ref collection, (BackgroundWorker)sender);
                foreach (Customer customer in collection)
                {
                    if (InvokeRequired)
                    Invoke(new AddToListview(AddItemToListview), new ListViewItem(new string[] {customer.Name, customer.Kennitala.Insert(6, "-")}));
                }
            }
            else
            {
                if (InvokeRequired)
                    Invoke(new AddToListview(AddItemToListview), new ListViewItem(new string[] {string.Format("Of margir með þetta nafn ({0} fundust)", total), ""}));
            }
        }

        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _statusText.Text = (string)e.UserState;
            _statusProgress.Value = e.ProgressPercentage;
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (name != _name.Text)
                ((BackgroundWorker)sender).RunWorkerAsync();
        }

        private delegate void AddToListview(ListViewItem item);
        private void AddItemToListview(ListViewItem item)
        {
            _results.Items.Add(item);
            Application.DoEvents();
        }

        private delegate void ListviewClear();
        private void ClearListView()
        {
            this._results.Items.Clear();
            Application.DoEvents();
        }	
    }
}
