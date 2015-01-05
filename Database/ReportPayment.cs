using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class ReportPayment
    {
        public ReportPayment()
        {
        }

        public ReportPayment(string name, int numberUsed, long total)
        {
            _name = name;
            _numberUsed = numberUsed;
            _total = total;
        }

        private string _name;
        private int _numberUsed;
        private long _total;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int NumberUsed
        {
            get { return _numberUsed; }
            set { _numberUsed = value; }
        }
        public long Total
        {
            get { return _total; }
            set { _total = value; }
        }
    }
}
