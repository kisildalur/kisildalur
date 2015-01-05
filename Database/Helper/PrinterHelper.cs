using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;

namespace Database
{
    public abstract class PrinterHelper
    {
        public PrinterHelper(string header1, string header2)
        {
            _page = 1;
            _printTable = null;
            _printHeaderInformation = true;
            _header1 = header1;
            _header2 = header2;
        }

        protected int _page;
        protected int _userId;
        protected PrintingTable _printTable;
        protected string _header1;
        protected string _header2;
        protected bool _printHeaderInformation;
        protected Rectangle _pageBounds;
        protected int _pageWidth;
        protected int _pageHeight;
        protected Graphics _pageGraphics;
        protected Font _pageFont;
        protected Brush _pageBrush;
        protected StringFormat _pageRightAligned;
        protected float _pageLineHeight;


        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public PrintingTable PrintingTable
        {
            get { return _printTable; }
            set { _printTable = value; }
        }

        /// <summary>
        /// Reset all settings in Printer Helper.
        /// Used to reset settings when PrintPreview was used.
        /// </summary>
        public void ResetHelper()
        {
            _page = 1;
            _printTable = null;
        }

        protected void PrintPage()
        {
            if (_printTable != null)
            {
                if (_printTable.Count != 0)
                {
                    _pageGraphics.FillRectangle(Brushes.LightGray,
                        new Rectangle(_pageBounds.X, Convert.ToInt32(_printTable.HeaderLocationY),
                        _pageBounds.Width, Convert.ToInt32(_pageLineHeight)));

                    foreach (PrintingTableHeader header in _printTable.PrintingHeaders)
                    {
                        _pageGraphics.DrawString(header.Name, _pageFont, _pageBrush, new RectangleF(header.Left, _printTable.HeaderLocationY, header.Width, _pageLineHeight), header.StringFormat);
                    }

                    float differenceInHeight = 0;
                    int rowId = 0;

                    foreach (PrintingTableEntry row in _printTable)
                    {
                        if (row.Font.Size == _pageFont.Size && !row.ContainsImage())
                            differenceInHeight -= 0.3f;
                        float currentRowY = Convert.ToSingle(_printTable.HeaderLocationY + _pageLineHeight * (2 + rowId - differenceInHeight) + (differenceInHeight + rowId) * 3);
                            
                        int rowHeight = row.EntryHeight();
                        float rowDifferenceAdd = 0;
						int colSkip = 0;
                        for (int index = 0; index < _printTable.PrintingHeaders.Length && index < row.Values.Length; index++)
                        {
                            if (row.Values[index].Type == DataType.String)
                            {
                                int newLines = row.Values[index].DataString.Split('\n').Length;
                                _printTable.PrintingHeaders[index].StringFormat.FormatFlags = StringFormatFlags.NoWrap;
                                float width = _printTable.PrintingHeaders[index + colSkip].Width;
								if (row.Values[index].ColumnSpan > 1)
									for (int i = 0; i < row.Values[index].ColumnSpan; i++)
										width += _printTable.PrintingHeaders[index + colSkip + i].Width;
                                _pageGraphics.DrawString(row.Values[index].DataString,
                                    row.Font,
                                    _pageBrush,
                                    new RectangleF(_printTable.PrintingHeaders[index + colSkip].Left,
										currentRowY + Convert.ToInt32((rowHeight - (row.Font.Height * 1) * newLines) / 2),
										width,
										_pageLineHeight + _pageLineHeight * (newLines - 1)),
                                    _printTable.PrintingHeaders[index + colSkip].StringFormat);

                                if (rowDifferenceAdd < Convert.ToSingle(1.0 * (newLines - 1)))
                                    rowDifferenceAdd += Convert.ToSingle(1.0 * (newLines - 1)) - rowDifferenceAdd;
								if (row.Values[index].ColumnSpan > 1)
									colSkip += row.Values[index].ColumnSpan - 1;
                            }
                            else
                            {
                                _pageGraphics.DrawImage(row.Values[index].DataImage,
                                    new Point(Convert.ToInt32(_printTable.PrintingHeaders[index].Left), Convert.ToInt32(currentRowY + (rowHeight - row.Values[index].DataImage.Height) / 2)));

                                if (rowDifferenceAdd < (row.Values[index].DataImage.Height / _pageFont.Height))
                                    rowDifferenceAdd += Convert.ToSingle((row.Values[index].DataImage.Height / _pageFont.Height)) - rowDifferenceAdd;
                            }
                            
                        }
                        differenceInHeight += ((_pageFont.Height - row.Font.Height)) / 10.0f - rowDifferenceAdd;
                        rowId++;
                    }
                }
            }
        }

        protected void PrepareForPrinting(PrintPageEventArgs e)
        {
            _pageBounds = new Rectangle(50, 50, e.PageBounds.Width - 125, e.PageBounds.Height - 100);
            _pageWidth = _pageBounds.Width;
            _pageHeight = _pageBounds.Height;
            _pageGraphics = e.Graphics;
            _pageFont = new Font("Arial", 10);
            _pageBrush = Brushes.Black;
            _pageLineHeight = _pageHeight * 0.016592920353f;
            _pageRightAligned = new StringFormat();
            _pageRightAligned.Alignment = StringAlignment.Far;
        }

        protected bool PrinterStartRangeOk(PrintPageEventArgs e)
        {
            //So that the user can print etc. a single page instead of 3
            if (e.PageSettings.PrinterSettings.PrintRange == PrintRange.SomePages)
            {
                //Encapsulate pages and skip those who should be skipped
                while (!(e.PageSettings.PrinterSettings.FromPage <= _page &&
                        e.PageSettings.PrinterSettings.ToPage >= _page) && _page > 0)
                {
                    //If the first page to be printed is not the first page then skip a page
                    if (e.PageSettings.PrinterSettings.FromPage > _page)
                        _page++;
                    else if (_page >= 4) //If user entered an invalid field etc. From page is 5
                    {
                        //Cancel printing.
                        return false;
                    }
                }
            }
            return true;
        }

        protected void PrintHeaderInformation()
        {
            //Print the image to the top-left corner
            _pageGraphics.DrawImage(Image.FromFile("logo.png"), new Rectangle(_pageBounds.X, _pageBounds.Y, Convert.ToInt32(_pageWidth * 0.47), Convert.ToInt32(_pageHeight * 0.07632)));

            if (_printHeaderInformation)
            {
                //Write information about when this
                //is being printed and who is printing this
				if (_userId != 0)
				{
					_pageGraphics.DrawString("Dagsetning:\nTími:\nStarfsmaður:", _pageFont, _pageBrush, _pageBounds.X + _pageWidth * 0.48f, _pageBounds.Y);
					_pageGraphics.DrawString(string.Format("{0}\n{1}\n{2}",
									 DateTime.Now.ToShortDateString(),
									 DateTime.Now.ToShortTimeString(),
									 MainDatabase.GetDB.Users[_userId, true].Name), _pageFont, _pageBrush,
						_pageBounds.X + _pageWidth * 0.7435135f,
						_pageBounds.Y);
				}
				else
				{
					_pageGraphics.DrawString("Dagsetning:\nTími:", _pageFont, _pageBrush, _pageBounds.X + _pageWidth * 0.48f, _pageBounds.Y);
					_pageGraphics.DrawString(string.Format("{0}\n{1}",
									 DateTime.Now.ToShortDateString(),
									 DateTime.Now.ToShortTimeString()), _pageFont, _pageBrush,
						_pageBounds.X + _pageWidth * 0.7435135f,
						_pageBounds.Y);
				}
            }
            else
            {
                _pageGraphics.DrawString(string.Format("{0}", _header1), _pageFont, _pageBrush, _pageBounds.X + _pageWidth * 0.48f, _pageBounds.Y);
                _pageGraphics.DrawString(string.Format("{0}", _header2), _pageFont, _pageBrush, _pageBounds.X + _pageWidth * 0.7435135f, _pageBounds.Y);
            }
            //Draw a line to seperate the header from the content
            _pageGraphics.DrawLine(Pens.Black, new Point(_pageBounds.X, _pageBounds.Y + Convert.ToInt32(_pageHeight * 0.07632) + 2), new Point(_pageBounds.X + _pageWidth, _pageBounds.Y + Convert.ToInt32(_pageHeight * 0.07632) + 2));
        }
    }
}
