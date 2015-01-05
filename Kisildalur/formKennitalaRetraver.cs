using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Database;

namespace Kisildalur
{
    public partial class formKennitalaRetraver : Form
    {
        public formKennitalaRetraver()
        {
            InitializeComponent();

            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
        }

        public void Run()
        {
            _worker.RunWorkerAsync();
        }

        Customer _customer;

        public Customer Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (((string)e.UserState).Contains("Error"))
            {
                _status.Text = "Error";
                MessageBox.Show((string)e.UserState);
            }
            else
                _status.Text = (string)e.UserState;

            if (_status.Text == "Complete")
            {
                this.Close();
            }
            
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            try
            {

                worker.ReportProgress(0, "Initializing");

                PostSubmitter post = new PostSubmitter();
                post.Url = "http://www2.glitnir.is/tkaup/Main.asp";
                post.PostItems.Add("tbKennitala", Customer.Kennitala);
                post.PostItems.Add("state", "1");
                post.Type = PostSubmitter.PostTypeEnum.Post;
                worker.ReportProgress(0, "Connecting");
                string result = post.Post();

                string regularExpressFinder = "<input type=\"text\" name=\"(?<name>[^\"]*)\" value=\"(?<value>[^\"]*)\"[^\\/]*/>";
                //                              <input type="text" name="tbHeimili" value="Gilsbakka 6" size="40" />

                worker.ReportProgress(0, "Retreaving values");

                Regex findAllValues = new Regex(regularExpressFinder);

                if (findAllValues.IsMatch(result))
                {
                    MatchCollection collection = findAllValues.Matches(result);

                    foreach (Match m in collection)
                    {
                        for (int I = 0; I < m.Groups["name"].Captures.Count; I++)
                        {
                            switch (m.Groups["name"].Captures[0].Value)
                            {
                                case "tbNafn":
                                    _customer.Name = m.Groups["value"].Captures[0].Value;
                                    break;

                                case "tbHeimili":
                                    _customer.Address1 = m.Groups["value"].Captures[0].Value;
                                    break;

                                case "tbPostnumer":
                                    _customer.Zip = m.Groups["value"].Captures[0].Value;
                                    break;

                                case "tbSveitarfelag":
                                    _customer.City = m.Groups["value"].Captures[0].Value;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                worker.ReportProgress(0, string.Format("Error {0}", error.ToString()));
            }
            finally
            {
                worker.ReportProgress(0, "Complete");
            }

        }
    }
}
