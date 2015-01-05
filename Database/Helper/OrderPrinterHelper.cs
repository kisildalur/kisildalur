using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;


namespace Database
{
	/// <summary>
	/// PrinterHelper is used to print an order.
	/// </summary>
    public class OrderPrinterHelper : PrinterHelper
    {
		/// <summary>
		/// Initialize a new instance of PrinterHelper
		/// </summary>
		/// <param name="order"></param>
        public OrderPrinterHelper(string header1, string header2)
            : base(header1, header2)
        {
            _printHeaderInformation = false;
        }

        
        Order _order;

		/// <summary>
		/// Print a single page. Run this call every time PrintPage is called in PrintingDocument.
		/// V. 1.1: Added support for printing range.
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

            float[] colsX = new float[] { _pageBounds.X, 
				_pageBounds.X + _pageWidth * 0.123783783f, 
				_pageBounds.X + _pageWidth * 0.733513513f, 
				_pageBounds.X + _pageWidth * 0.81513513f, 
				_pageBounds.X + _pageWidth * 0.89189189f, 
				_pageBounds.Right };
            float tableHeadY = _pageBounds.Y + _pageHeight * 0.016592f * 11 + 4;

            PrintHeader();

            base.PrintPage();

			_pageGraphics.DrawString(_order.Comment, _pageFont, _pageBrush, _pageBounds.X, _pageBounds.Y + _pageLineHeight * 50 + 4);
            _pageGraphics.DrawString("Samtals:", _pageFont, _pageBrush, colsX[2] - 60, _pageBounds.Y + _pageLineHeight * 54 + 4);


            // Búa til streng yfir öll vsk sem eru möguleg, ef vsk af einhverri tegund er 0, þá birtir hún það ekki
            StringBuilder build = new StringBuilder("Án vsk:\n");
			StringBuilder build2 = new StringBuilder("");
            long total = _order.Items.Total();
            long vsk_245 = _order.Items.TotalVsk(ItemVsk.items_240);
            long vsk_7 = _order.Items.TotalVsk(ItemVsk.books_7);
            long vsk_0 = _order.Items.TotalVsk(ItemVsk.other_0);
            long totalVsk = vsk_245 + vsk_7 + vsk_0;
			build2.AppendFormat("{0:#,0}\n", total - totalVsk);
			if (vsk_245 != 0)
			{
				if (_order.Date.Year < 2010)
					build.Append("Vsk 24,5%:\n");
                else if (_order.Date.Year < 2015)
					build.Append("Vsk 25,5%:\n");
                else
                    build.Append("Vsk 24,0%:\n");
				build2.AppendFormat("{0:#,0}\n", vsk_245);
			}
			if (vsk_7 != 0)
			{
				build.Append("Vsk 7%:\n");
				build2.AppendFormat("{0:#,0}\n", vsk_7);
			}
			if (vsk_245 == 0 && vsk_7 == 0)
			{
				build.Append("Vsk:\n");
				build2.Append("0\n");
			}
            build.Append("Alls:\n\n");
			build2.AppendFormat("{0:#,0}\n", total);
			build2.Append("======");
			_pageGraphics.DrawString(build.ToString(), _pageFont, _pageBrush, colsX[2], _pageBounds.Y + _pageLineHeight * 54 + 4);
            _pageGraphics.DrawString(build2.ToString(), _pageFont, _pageBrush, new RectangleF(_pageBounds.X + _pageWidth * 0.89189189f,
                                                                                   _pageBounds.Y + _pageHeight * 0.016592f * 54 + 4,
                                                                                   _pageWidth * 0.10918918f,
                                                                                   _pageHeight), _pageRightAligned);

            int addExtra = 0;
            if (_order.Payment.Count > 2)
                addExtra = _order.Payment.Count - 2;
            long totalPayment = 0;
            build.Remove(0, build.Length);
			build2.Remove(0, build2.Length);
            build.Append("Greiðslumáti:\n");
			build2.Append("\n");
            foreach (OrderPayment payment in _order.Payment)
            {
				if (payment.Name != "")
				{
					build.AppendFormat("{0}:\n", payment.Name);
					build2.AppendFormat("{0:#,0}\n", payment.Amount);
					totalPayment += payment.Amount;
				}
            }
            if (totalPayment > total)
            {
                build.Append("Til baka:");
				build2.AppendFormat("-{0:#,0}\n", totalPayment - total);
            }
            _pageGraphics.DrawString(build.ToString(),
                            _pageFont,
                            _pageBrush,
                            _pageBounds.X,
                            _pageBounds.Y + _pageHeight * 0.016592f * (54 - addExtra) + 4);
            _pageGraphics.DrawString(build2.ToString(),
                            _pageFont,
                            _pageBrush,
                            new RectangleF(_pageBounds.X,
                                           _pageBounds.Y + _pageHeight * 0.016592f * (54 - addExtra) + 4,
                                           _pageWidth * 0.30918918f,
                                           _pageHeight), _pageRightAligned);


            PrintFooter();


			//This part specifies whether a new page should be printed.
			//It encapsulates the page attribute and then manipulates the HasMorePage variable

			//Check if we have printed all pages
            if (_page >= 2 || (_page == 0 && !_order.PrintTwoCopies))
                e.HasMorePages = false;
			//Check if a new page should be printed if the order is just a normal order
            else
            {
				//Verifie whether or not a new page should be printed if a specific range has been specified
				if (e.PageSettings.PrinterSettings.PrintRange == PrintRange.SomePages)
				{
					//If the last page is not this one, then we can continue :)
					if (e.PageSettings.PrinterSettings.ToPage != _page)
						e.HasMorePages = true;
				}
				else
					e.HasMorePages = true;
            }
            _page++;
        }

		public void PrintHeaderOnly(PrintPageEventArgs e)
		{
			base.PrepareForPrinting(e);
			this.PrintHeader();
		}

        private new void PrepareForPrinting(PrintPageEventArgs e)
        {
            base.PrepareForPrinting(e);

            if (this.PrintingTable == null)
            {
                float[] colsX = new float[] { _pageBounds.X, 
				_pageBounds.X + _pageWidth * 0.123783783f, 
				_pageBounds.X + _pageWidth * 0.733513513f, 
				_pageBounds.X + _pageWidth * 0.81513513f, 
				_pageBounds.X + _pageWidth * 0.89189189f, 
				_pageBounds.Right };
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Far;
                this.PrintingTable = new PrintingTable(_pageBounds.Y + _pageHeight * 0.016592f * 11 + 4,
                    new PrintingTableHeader("Vörunúmer", colsX[0], colsX[1] - colsX[0], new StringFormat()),
                    new PrintingTableHeader("Lýsing:", colsX[1], colsX[2] - colsX[1], new StringFormat()),
                    new PrintingTableHeader("Verð:", colsX[2], colsX[3] - colsX[2], stringFormat),
                    new PrintingTableHeader("Magn:", colsX[3], colsX[4] - colsX[3], stringFormat),
                    new PrintingTableHeader("Upphæð", colsX[4], colsX[5] - colsX[4], stringFormat));

                int item = 0;

                foreach (OrderItem orderItem in _order.Items)
                {
                    Font font = new Font(_pageFont, FontStyle.Regular);
                    if (_order.Items[item].Price == 0)
                        font = new Font(_pageFont, FontStyle.Italic);

                    string price = "", total = "";
                    if (!_order.HidePrice && _order.Items[item].Price != 0)
                    {
                        price = string.Format("{0:#,0}", orderItem.Price);
                        total = string.Format("{0:#,0}", orderItem.Count * orderItem.Price);
                    }

                    this.PrintingTable.Add(new PrintingTableEntry(font, orderItem.Vorunr, orderItem.Name, price, orderItem.Count.ToString(), total));

                    if (!string.IsNullOrEmpty(orderItem.SubName))
                        this.PrintingTable.Add(new PrintingTableEntry(new Font(_pageFont.FontFamily, 8.5f), "", string.Format("     {0}", orderItem.SubName)));

                    
                    foreach (OrderItem subItem in orderItem.SubItems)
                        this.PrintingTable.Add(new PrintingTableEntry(new Font(_pageFont.FontFamily, 8.5f, FontStyle.Italic), subItem.Vorunr, string.Format("     {0}", subItem.Name), "", subItem.Count.ToString()));


                    if (orderItem.Discount.Type != DiscountType.None)
                    {
                        
                        if (orderItem.Discount.Type == DiscountType.Coin)
                            this.PrintingTable.Add(new PrintingTableEntry(new Font(_pageFont, FontStyle.Italic), "    Afsláttur", "", "", "", string.Format("-{0:#,0}", orderItem.Discount.CoinDiscount)));
                        else
                            this.PrintingTable.Add(new PrintingTableEntry(new Font(_pageFont, FontStyle.Italic), "    Afsláttur", "", "", "", string.Format("-{0:#,0}", orderItem.Discount.CoinDiscount)));
                    }
                }

                if (_order.GlobalDiscount.Type != DiscountType.None)
                {
                    string countValue = "", totalValue = "";
                    if (_order.GlobalDiscount.Type == DiscountType.Percent)
                    {
                        countValue = string.Format("{0}%", _order.GlobalDiscount.PercentDiscount);
                        totalValue = string.Format("{0:#,0}", _order.Items.Total() - _order.Items.Total(false));
                    }
                    else
                        totalValue = string.Format("{0:#,0}", -_order.GlobalDiscount.CoinDiscount);
                    this.PrintingTable.Add(new PrintingTableEntry(""));
                    this.PrintingTable.Add(new PrintingTableEntry(_order.GlobalDiscount.Text, "", "", countValue, totalValue));
                }

            }
        }

        private void PrintFooter()
        {
            Font fontBold = new Font(_pageFont, FontStyle.Bold);

            if (_page != 0 && !_order.PrintTwoCopies)
            {
                if (_order.Abyrgd == -1)
                {
                    _pageGraphics.DrawString("Lífstíðarábyrgð",
                        _pageFont,
                        _pageBrush,
                        _pageBounds.X,
                        _pageBounds.Y + _pageLineHeight * 58 + 4);
                }
                else
                {
                    _pageGraphics.DrawString(string.Format("{0} ára ábyrgð", _order.Abyrgd),
                        _pageFont,
                        _pageBrush,
                        _pageBounds.X,
                        _pageBounds.Y + _pageLineHeight * 58 + 4);
                }
            }

            switch (_page)
            {
                case 1:
                    _pageGraphics.DrawString("Frumrit, gildir einnig sem ábyrgðarskírteini", fontBold, _pageBrush, _pageBounds.X, _pageBounds.Y + _pageLineHeight * 59 + 4);
                    break;
                case 2:
                    _pageGraphics.DrawString("Samrit fyrir bókhald", fontBold, _pageBrush, _pageBounds.X, _pageBounds.Y + _pageLineHeight * 59 + 4);
                    break;
                case 3:
                    _pageGraphics.DrawString("Eintak Söluaðila", fontBold, _pageBrush, _pageBounds.X, _pageBounds.Y + _pageLineHeight * 59 + 4);
                    break;
            }
            //_pageGraphics.DrawString("Tilboð, gildir í 7 daga", fontBold, _pageBrush, _pageBounds.X, _pageBounds.Y + _pageLineHeight * 55 + 4);
        }

        private void PrintHeader()
        {
            PrintHeaderInformation();

            _pageGraphics.DrawString("Dagsetning:\nTími:\nStarfsmaður:", _pageFont, _pageBrush, _pageBounds.X + _pageWidth * 0.48f, _pageBounds.Y + _pageLineHeight * 6);
            _pageGraphics.DrawString(string.Format("{0}\n{1}\n{2}",
                             _order.Date.ToShortDateString(),
							 _order.Date.ToShortTimeString(),
                             MainDatabase.GetDB.Users[_userId, true].Name), _pageFont, _pageBrush,
                _pageBounds.X + _pageWidth * 0.7435135f,
                _pageBounds.Y + _pageLineHeight * 6);

            if (_page != 0 && !_order.PrintTwoCopies)
            {
				if (!string.IsNullOrEmpty(_order.Customer.Kennitala))
				{
					if (_order.Customer.Kennitala.Length == 10)
						_order.Customer.Kennitala = _order.Customer.Kennitala.Insert(6, "-");
                    _pageGraphics.DrawString(string.Format("Kt. {0}", _order.Customer.Kennitala),
							_pageFont,
							_pageBrush,
							_pageBounds.X + 30,
							_pageBounds.Y + _pageLineHeight * 6);
				}

                _pageGraphics.DrawString(string.Format("{0}", _order.Customer.Name),
                        _pageFont,
                        _pageBrush,
                        _pageBounds.X + 30,
                        _pageBounds.Y + _pageLineHeight * 7);

                _pageGraphics.DrawString(string.Format("{0} {1}", _order.Customer.Address1, _order.Customer.Address2),
                        _pageFont,
                        _pageBrush,
                        _pageBounds.X + 30,
                        _pageBounds.Y + _pageLineHeight * 8);

                _pageGraphics.DrawString(string.Format("{0} {1}", _order.Customer.Zip, _order.Customer.City),
                        _pageFont,
                        _pageBrush,
                        _pageBounds.X + 30,
                        _pageBounds.Y + _pageLineHeight * 9);
            }
        }

		public Order Order
		{
			get { return _order; }
			set
            {
                _order = value;
                _userId = _order.UserID;
            }
		}
	}
}
