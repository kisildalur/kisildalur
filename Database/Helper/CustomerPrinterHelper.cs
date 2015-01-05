using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;


namespace Database
{
	public class CustomerPrinterHelper : PrinterHelper
	{
		/// <summary>
		/// Initialize a new instance of MonthPrinterHelper
		/// </summary>
		/// <param name="order"></param>
		public CustomerPrinterHelper(string header1, string header2)
			: base(header1, header2)
		{
			_page = 0;
			_currentRow = 0;
			_currentItem = 0;
			_total = 0;
			_totalVSK = 0;
			_printingPayment = false;
		}

		double _totalVSK;
		double _total;
		int _currentRow = 0;
		int _currentItem = 0;
		bool _printingPayment = false;
		Customer _customer;
		OrderCollection _orders;
		DateTime _dateFrom;
		DateTime _dateTo;

		public Customer Customer
		{
			get { return _customer; }
			set { _customer = value; }
		}

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

			if (!_printingPayment)
			{
				float[] colsX = new float[] { _pageBounds.X, 
				_pageBounds.X + _pageWidth * 0.1f,
				_pageBounds.X + _pageWidth * 0.2f,
				_pageBounds.X + _pageWidth * 0.65f,
				_pageBounds.X + _pageWidth * 0.67f,
                _pageBounds.X + _pageWidth * 0.78f,
				_pageBounds.X + _pageWidth * 0.89f,
				_pageBounds.Right };

				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Far;
				this.PrintingTable = new PrintingTable(_pageBounds.Y + _pageHeight * 0.016592f * 9 + 4,
					new PrintingTableHeader("Dagur:", colsX[0], colsX[1] - colsX[0], new StringFormat()),
					new PrintingTableHeader("Tími:", colsX[1], colsX[2] - colsX[1], new StringFormat()),
					new PrintingTableHeader("Reikningur:", colsX[2], colsX[3] - colsX[2], new StringFormat()),
					new PrintingTableHeader("", colsX[3], colsX[4] - colsX[3], new StringFormat()),
					new PrintingTableHeader("Vaskur:", colsX[4], colsX[5] - colsX[4], stringFormat),
					new PrintingTableHeader("Án vask:", colsX[5], colsX[6] - colsX[5], stringFormat),
					new PrintingTableHeader("Samtals:", colsX[6], colsX[7] - colsX[6], stringFormat));

				DateTime currDate = DateTime.MinValue;
				_currentRow = 0;
				for (; _currentItem < _orders.Count; _currentItem++)
				{
					int numRows = _orders[_currentItem].Items.Count + _orders[_currentItem].Payment.Count + 1;
					if (_currentRow + numRows > 40)
					{
						e.HasMorePages = true;
						break;
					}
					else
					{
						Order order = _orders[_currentItem];
						_total += order.Total;
						_totalVSK += order.TotalVSK;
						this._printTable.Add(new PrintingTableEntry(
							order.Date.ToShortDateString(),
							order.Date.ToLongTimeString(),
							"Reikningsnúmer: " + order.OrderNumber.ToString(),
							"",
							order.TotalVSK.ToString("#,0"),
							order.TotalWithoutVSK.ToString("#,0"),
							order.Total.ToString("#,0")));
						Font f = new Font(_pageFont.FontFamily, 8, FontStyle.Italic);
						for (int i = 0; i < order.Payment.Count; i++)
						{
							this._printTable.Add(new PrintingTableEntry(f,
								new PrintingTableEntryData(order.Payment[i].Name, 6),
								new PrintingTableEntryData(order.Payment[i].Amount.ToString("#,0") + "   ")));
						}
						for (int orderItem = 0; orderItem < order.Items.Count; orderItem++)
						{
							this._printTable.Add(new PrintingTableEntry(f, new PrintingTableEntryData(order.Items[orderItem].Name, 3), new PrintingTableEntryData(order.Items[orderItem].Count.ToString() + " stk.")));
						}

						_currentRow += numRows;
					}
				}
				if (_currentItem >= _orders.Count)
				{
					this._printTable.Add(new PrintingTableEntry(""));
					this._printTable.Add(new PrintingTableEntry("", "", "", "", "", "", "--------"));
					this._printTable.Add(new PrintingTableEntry(""));
					this._printTable.Add(new PrintingTableEntry("Samtals", "", "", "", _totalVSK.ToString("#,0"), (_total - _totalVSK).ToString("#,0"), _total.ToString("#,0")));
					_printingPayment = true;
					e.HasMorePages = true;
				}
			}
			else
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
				this.PrintingTable.Add(new PrintingTableEntry("Samtals", "", "", string.Format("{0:#,0}", totalPayment)));
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

			_pageGraphics.DrawString("Kennitala: " + _customer.Kennitala, fontBold, _pageBrush, _pageBounds.X, _pageBounds.Y + _pageHeight * 0.1021f);
			_pageGraphics.DrawString(string.Format("\nNafn: {0}", _customer.Name), _pageFont, _pageBrush, _pageBounds.X, _pageBounds.Y + _pageHeight * 0.1021f);

			Font big = new Font(fontBold.FontFamily, 16);
			_pageGraphics.DrawString("Hreyfingalisti", big, _pageBrush, _pageBounds.X + _pageBounds.Width / 2 - 80, _pageBounds.Y + _pageHeight * 0.1021f + 2);
		}
	}
}
