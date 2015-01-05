using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;

namespace Database
{
    public class MonthPrinterHelper : PrinterHelper
    {
        /// <summary>
		/// Initialize a new instance of MonthPrinterHelper
		/// </summary>
		/// <param name="order"></param>
        public MonthPrinterHelper(string header1, string header2)
            : base(header1, header2)
        {
            _page = 0;
        }

        OrderCollection _orders;
        DateTime _dateFrom;
        DateTime _dateTo;
       

        public OrderCollection OrderList
        {
            get { return _orders; }
            set { _orders = value; }
        }

        public DateTime DateFrom
        {
            get { return _dateFrom; }
            set { _dateFrom = value; }
        }

        public DateTime DateTo
        {
            get { return _dateTo; }
            set { _dateTo = value; }
        }

        /// <summary>
		/// Print a single page. Run this call every time PrintPage is called in PrintingDocument.
		/// V. 1.0: Original release.
		/// </summary>
		/// <param name="e">Print Page events. Needed for printing.</param>
        public void PrintPage(PrintPageEventArgs e)
        {
            //Check for printing range.
            if (!this.PrinterStartRangeOk(e))
            {
                e.Cancel = true;
                return;
            }

            this.PrepareForPrinting(e);

            float tableHeadY = _pageBounds.Y + _pageHeight * 0.016592f * 9 + 4;

            PrintHeader();

            base.PrintPage();
            if (_page == 0)
            {
                _page++;
                e.HasMorePages = true;
            }
        }

        private new void PrepareForPrinting(PrintPageEventArgs e)
        {
            base.PrepareForPrinting(e);

            if (this.PrintingTable == null)
            {
                float[] colsX = new float[] { _pageBounds.X, 
				_pageBounds.X + _pageWidth * 0.265135132f,
                _pageBounds.X + _pageWidth * 0.328918915f,
                _pageBounds.X + _pageWidth * 0.55261261f,
				_pageBounds.X + _pageWidth * 0.776306305f,
				_pageBounds.Right };

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Far;
                this.PrintingTable = new PrintingTable(_pageBounds.Y + _pageHeight * 0.016592f * 9 + 4,
                    new PrintingTableHeader("Dagur:", colsX[0], colsX[1] - colsX[0], new StringFormat()),
                    new PrintingTableHeader("Fjöldi:", colsX[1], colsX[2] - colsX[1], new StringFormat()),
                    new PrintingTableHeader("Vaskur:", colsX[2], colsX[3] - colsX[2], stringFormat),
					new PrintingTableHeader("Án vask:", colsX[3], colsX[4] - colsX[3], stringFormat),
                    new PrintingTableHeader("Samtals:", colsX[4], colsX[5] - colsX[4], stringFormat));

                DateTime currDate = DateTime.MinValue;

                long totalPriceVsk = 0;
                long totalPrice = 0;

                long totalDay = 0, totalDayVsk = 0;
                int totalOrders = 0;

                foreach (Order order in _orders)
                {
                    if (currDate.Day != order.Date.Day || currDate.Month != order.Date.Month || currDate.Year != order.Date.Year)
                    {
                        if (totalDay != 0)
                        {
                            this.PrintingTable.Add(new PrintingTableEntry(currDate.ToLongDateString(),
                                                                        totalOrders.ToString(),
                                                                        string.Format("{0:#,0}", totalDayVsk),
																		string.Format("{0:#,0}", totalDay - totalDayVsk),
                                                                        string.Format("{0:#,0}", totalDay)));

                            totalDay = 0;
                            totalDayVsk = 0;
                            totalOrders = 0;
                        }

                        currDate = order.Date;
                    }

                    totalDayVsk += order.Items.TotalVsk(ItemVsk.items_240) + order.Items.TotalVsk(ItemVsk.books_7);
                    totalPriceVsk += order.Items.TotalVsk(ItemVsk.items_240) + order.Items.TotalVsk(ItemVsk.books_7);
                    totalDay += order.Items.Total();
                    totalPrice += order.Items.Total();

                    totalOrders++;
                }

                if (totalDay != 0)
                {
                    this.PrintingTable.Add(new PrintingTableEntry(currDate.ToLongDateString(),
                                            totalOrders.ToString(),
											string.Format("{0:#,0}", totalDayVsk),
											string.Format("{0:#,0}", totalDay - totalDayVsk),
                                            string.Format("{0:#,0}", totalDay)));
                }

                this.PrintingTable.Add(new PrintingTableEntry("", "", "", ""));
                this.PrintingTable.Add(new PrintingTableEntry("", "", string.Format("{0:#,0}", totalPriceVsk),
                                                                        string.Format("{0:#,0}", totalPrice)));
            }
            else if (_page == 1)
            {
                float[] colsX = new float[] { _pageBounds.X, 
				_pageBounds.X + _pageWidth * 0.201351349f,
                _pageBounds.X + _pageWidth * 0.265135132f,
                _pageBounds.X + _pageWidth * 0.632567566f,
				_pageBounds.Right };

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Far;
                this.PrintingTable = new PrintingTable(_pageBounds.Y + _pageHeight * 0.016592f * 9 + 4,
                    new PrintingTableHeader("Greiðslumáti:", colsX[0], colsX[1] - colsX[0], new StringFormat()),
                    new PrintingTableHeader("Fjöldi:", colsX[1], colsX[2] - colsX[1], new StringFormat()),
                    new PrintingTableHeader("Vaskur:", colsX[2], colsX[3] - colsX[2], stringFormat),
                    new PrintingTableHeader("Samtals:", colsX[3], colsX[4] - colsX[3], stringFormat));

                DateTime currDate = DateTime.MinValue;

                long totalPayment = 0;

                OrderPaymentCollection paymentCollection = new OrderPaymentCollection();

                foreach (PayMethod subMethod in MainDatabase.GetDB.PayMethods)
                    paymentCollection.Add(new OrderPayment(0, subMethod.Name, 0));

                foreach (Order order in _orders)
                {
                    foreach (OrderPayment payment in order.Payment)
                    {
                        _paymentSearch = payment.Name;
                        if (paymentCollection.Find(payment.Name) == null)
                        {
                            paymentCollection.Add(new OrderPayment(1, payment.Name, payment.Amount));
                        }
                        else
                        {
                            OrderPayment method = paymentCollection.Find(payment.Name);
                            method.SetId(method.Id + 1);
                            method.Amount += payment.Amount;
                        }
                    }
                }

                foreach (OrderPayment payment in paymentCollection)
                {
                    if (payment.Amount != 0)
                    {
                        if (_orders[0].Date.Year >= 2010 && _orders[0].Date.Year < 2010)
                            this.PrintingTable.Add(new PrintingTableEntry(payment.Name,
                                                payment.Id.ToString(),
                                                string.Format("{0:#,0}", payment.Amount * (1 - (1 / 1.240))),
                                                string.Format("{0:#,0}", payment.Amount)));
                        else if (_orders[_orders.Count - 1].Date.Year >= 2010)
                            this.PrintingTable.Add(new PrintingTableEntry(payment.Name,
                                                payment.Id.ToString(),
                                                string.Format("{0:#,0}", payment.Amount * (1 - (1 / 1.255))),
                                                string.Format("{0:#,0}", payment.Amount)));
						else if (_orders[_orders.Count - 1].Date.Year < 2010)
							this.PrintingTable.Add(new PrintingTableEntry(payment.Name,
												payment.Id.ToString(),
												string.Format("{0:#,0}", payment.Amount * (1 - (1 / 1.245))),
												string.Format("{0:#,0}", payment.Amount)));
						else
							this.PrintingTable.Add(new PrintingTableEntry(payment.Name,
												payment.Id.ToString(),
												"",
												string.Format("{0:#,0}", payment.Amount)));

                        totalPayment += payment.Amount;
                    }
                }

                this.PrintingTable.Add(new PrintingTableEntry("", "", "", ""));
                this.PrintingTable.Add(new PrintingTableEntry("", "", "", string.Format("{0:#,0}", totalPayment)));
            }
        }

        private static string _paymentSearch;
        private static bool FindPaymentMatch(OrderPayment p)
        {
            return (p.Name == _paymentSearch);
        }

        private void PrintHeader()
        {
            PrintHeaderInformation();

            Font fontBold = new Font(_pageFont, FontStyle.Bold);

            _pageGraphics.DrawString("Tímabil", fontBold, _pageBrush, _pageBounds.X, _pageBounds.Y + _pageHeight * 0.1021f);
            _pageGraphics.DrawString(string.Format("\n{0} - {1}", _dateFrom.ToLongDateString(), _dateTo.ToLongDateString()), _pageFont, _pageBrush, _pageBounds.X, _pageBounds.Y + _pageHeight * 0.1021f);
        }
    }
}
