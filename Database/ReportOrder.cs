using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class ReportOrder
    {
        public ReportOrder()
        {
        }

        public ReportOrder(string orderNumber, string numberItems, string kennitala, string totalVsk0, string totalVsk7, string totalVsk245, string total)
        {
            _orderNumber = orderNumber;
            _numberItems = numberItems;
            _kennitala = kennitala;
            _totalVsk0 = totalVsk0;
            _totalVsk7 = totalVsk7;
            _totalVsk245 = totalVsk245;
            _total = total;
        }

        private string _orderNumber;
        private string _numberItems;
        private string _kennitala;
        private string _totalVsk0;
        private string _totalVsk7;
        private string _totalVsk245;
        private string _total;
        public string OrderNumber
        {
            get { return _orderNumber; }
            set { _orderNumber = value; }
        }
        public string NumberItems
        {
            get { return _numberItems; }
            set { _numberItems = value; }
        }
        public string Kennitala
        {
            get { return _kennitala; }
            set { _kennitala = value; }
        }
        public string TotalVsk0
        {
            get { return _totalVsk0; }
            set { _totalVsk0 = value; }
        }
        public string TotalVsk7
        {
            get { return _totalVsk7; }
            set { _totalVsk7 = value; }
        }
        public string TotalVsk245
        {
            get { return _totalVsk245; }
            set { _totalVsk245 = value; }
        }
        public string Total
        {
            get { return _total; }
            set { _total = value; }
        }
    }
}
