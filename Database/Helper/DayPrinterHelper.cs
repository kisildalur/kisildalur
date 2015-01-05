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
    public class DayPrinterHelper
    {
		/// <summary>
		/// Initialize a new instance of PrinterHelper
		/// </summary>
		/// <param name="order"></param>
        public DayPrinterHelper()
        {
            _paymentCollection = new OrderPaymentCollection();
        }

        int _userId;
        OrderCollection _orders;
        OrderPaymentCollection _paymentCollection;

		/// <summary>
		/// Reset all settings in Printer Helper.
		/// Used to reset settings when PrintPreview was used.
		/// </summary>
		/// <param name="offer">Specifie whether or not this is a special offer (true) or an order (false).</param>
		public void ResetHelper(OrderCollection orders, int userId)
		{
            _orders = orders;
            _userId = userId;
            _paymentCollection.Clear();
		}

		/// <summary>
		/// Print a single page. Run this call every time PrintPage is called in PrintingDocument.
		/// V. 1.0: First version.
		/// </summary>
		/// <param name="e">Print Page events. Needed for printing.</param>
        public void PrintPage(PrintPageEventArgs e)
        {
            //The bounds of the page we can print on
            Rectangle pBounds = new Rectangle(50, 50, e.PageBounds.Width - 125, e.PageBounds.Height - 100);

            //Create some shortcuts to some variables
            //so we dont have to write the full location
            int pWidth = pBounds.Width;
            int pHeight = pBounds.Height;
            Graphics g = e.Graphics;
            Font font = new Font("Arial", 10);
            Brush black = Brushes.Black;


            //The following is an array containing the list
            //of the location on the X-asis for the table
            //so the items align correctly on the X-asis

            //This contains the X-asis for the order table.
            float[] orderColumnX = new float[] { pBounds.X, 
				pBounds.X + pWidth * 0.066783783f,
                pBounds.X + pWidth * 0.137567566f,
                pBounds.X + pWidth * 0.280135132f, 
                pBounds.X + pWidth * 0.343918915f,
				pBounds.X + pWidth * 0.78378378f, 
				pBounds.X + pWidth * 0.89189189f,
				pBounds.Right };

            //This contains the X-asis for the order table.
            float[] paymentColumnX = new float[] { pBounds.X, 
				pBounds.X + pWidth * 0.805108107f, 
				pBounds.X + pWidth * 0.87189189f,
				pBounds.Right };


            //tableHeadY is the variable containing where the
            //the table begins on the X-asis
            float tableHeadY = pBounds.Y + pHeight * 0.016592f * 9 + 4;

            //This is the total height of each line in the table
            float lineHeight = pHeight * 0.016592920353f;

            //Create the format for string that are
            //supposed to align to the right.
            StringFormat rightAligned = new StringFormat();
            rightAligned.Alignment = StringAlignment.Far;

            //Print the header on the page
            PrintHeader(pBounds, g, font, black, pWidth, pHeight, lineHeight, rightAligned);



            //Print the Order table and all it's relating information
            PrintOrderTable(pBounds, g, font, black, orderColumnX, tableHeadY, lineHeight, rightAligned);

            //Create some space between the Order list and Payment list
            tableHeadY = tableHeadY + lineHeight * (8 + _orders.Count);

            //Print the Payment table and all it's relating information
            PrintTablePayment(pBounds, g, font, black, paymentColumnX, tableHeadY, lineHeight, rightAligned);
        }

        /// <summary>
        /// Print a table containing all orders and it's total.
        /// </summary>
        /// <param name="pBounds">The bounds on the page we can print on</param>
        /// <param name="g">The graphics pointer we use to print with</param>
        /// <param name="font">The default font for all text</param>
        /// <param name="black">The color of the text</param>
        /// <param name="orderColumnX">An array containing all the X-asis bounds on the table.</param>
        /// <param name="tableHeadY">The location from where the table should start from the Y-asis</param>
        /// <param name="lineHeight">The height of each line in the table</param>
        /// <param name="rightAligned">String format for text that is supposed to align to the right</param>
        private void PrintOrderTable(Rectangle pBounds, Graphics g, Font font, Brush black, float[] orderColumnX, float tableHeadY, float lineHeight, StringFormat rightAligned)
        {
            //BORDER
            g.FillRectangle(Brushes.LightGray,
                new Rectangle(pBounds.X, Convert.ToInt32(tableHeadY),
                pBounds.Width, Convert.ToInt32(lineHeight)));

            //Print header information for the Order list
            //This will be displayed on top of the grey border
            g.DrawString("Tími", font, black, new RectangleF(orderColumnX[0], tableHeadY, orderColumnX[1] - orderColumnX[0], lineHeight));
            g.DrawString("Númer", font, black, new RectangleF(orderColumnX[1], tableHeadY, orderColumnX[2] - orderColumnX[1], lineHeight));
            g.DrawString("Kennitala", font, black, new RectangleF(orderColumnX[2], tableHeadY, orderColumnX[3] - orderColumnX[2], lineHeight));
            g.DrawString("Fjöldi", font, black, new RectangleF(orderColumnX[3], tableHeadY, orderColumnX[4] - orderColumnX[3], lineHeight));
            g.DrawString("Greiðsluaðferð", font, black, new RectangleF(orderColumnX[4], tableHeadY, orderColumnX[5] - orderColumnX[4], lineHeight));
            g.DrawString("Vaskur", font, black, new RectangleF(orderColumnX[5], tableHeadY, orderColumnX[6] - orderColumnX[5], lineHeight));
            g.DrawString("Samtals", font, black, new RectangleF(orderColumnX[6], tableHeadY, orderColumnX[7] - orderColumnX[6], lineHeight));

            //Initialize variables containing the total of all orders
            long totalVsk = 0;
            long totalPrice = 0;

            //Iterate through every order
            for (int item = 0; item < _orders.Count; item++)
            {
                //Print information
                PrintOrderDetails(g, font, black, rightAligned, orderColumnX, tableHeadY, lineHeight, item);

                //Add to total
                totalVsk += _orders[item].Items.TotalVsk(ItemVsk.items_240) + _orders[item].Items.TotalVsk(ItemVsk.books_7);
                totalPrice += _orders[item].Items.Total();

                //The following code iterates through each payment in
                //current order and add it to our payment collector.
                foreach (OrderPayment payment in _orders[item].Payment)
                {
                    //Save the payment name we are searching for
                    _paymentSearch = payment.Name;

                    //The following determines if the payment is already in
                    //our payment collector and if not, add it
                    if (_paymentCollection.Find(payment.Name) == null)
                    {
                        //Add payment information to our payment collection
                        _paymentCollection.Add(new OrderPayment(1, payment.Name, payment.Amount));
                    }
                    else
                    {
                        //We have found a paymethod containing the same name so
                        //All we have to do is add to the total on our collector
						OrderPayment method = _paymentCollection.Find(payment.Name);

                        //Add one to the number of times this paymethod has been used
                        method.SetId(method.Id + 1);
                        method.Amount += payment.Amount;
                    }
                }
            }

            //Print the total of our Order list to the bottom
            PrintTotalOfOrders(pBounds, g, font, black, orderColumnX, tableHeadY, lineHeight, rightAligned, totalVsk, totalPrice);
        }

        private static string _paymentSearch;
        private static bool FindPaymentMatch(OrderPayment p)
        {
            return (p.Name == _paymentSearch);
        }

        /// <summary>
        /// Print the total of the order table
        /// </summary>
        /// <param name="pBounds">The bounds on the page we can print on</param>
        /// <param name="g">The graphics pointer we use to print with</param>
        /// <param name="font">The default font for all text</param>
        /// <param name="black">The color of the text</param>
        /// <param name="colsX">An array containing all the X-asis bounds on the table.</param>
        /// <param name="tableHeadY">The location from where the table should start from the Y-asis</param>
        /// <param name="lineHeight">The height of each line in the table</param>
        /// <param name="rightAligned">String format for text that is supposed to align to the right</param>
        /// <param name="totalVsk">The total of all the Vsk in the order list</param>
        /// <param name="totalPrice">The total of all the orders in the list (with the VSK)</param>
        private void PrintTotalOfOrders(Rectangle pBounds, Graphics g, Font font, Brush black, float[] colsX, float tableHeadY, float lineHeight, StringFormat rightAligned, long totalVsk, long totalPrice)
        {
            //Calculate the Y on the page using information available to us now
            float currentRowY = tableHeadY + lineHeight * (1 + _orders.Count) + _orders.Count * 3 + 4;

            //Draw the line to seperate the list from the total
            g.DrawLine(Pens.Black, new Point(pBounds.X, Convert.ToInt32(currentRowY)), new Point(pBounds.X + pBounds.Width, Convert.ToInt32(currentRowY)));

            g.DrawString("Samtals",
                font,
                black,
                new RectangleF(colsX[0], currentRowY + 4, colsX[4] - colsX[0], lineHeight));

            g.DrawString(string.Format("{0:#,0}", totalVsk),
                font,
                black,
                new RectangleF(colsX[5], currentRowY + 4, colsX[6] - colsX[5], lineHeight),
                rightAligned);

            g.DrawString(string.Format("{0:#,0}", totalPrice),
                font,
                black,
                new RectangleF(colsX[6], currentRowY + 4, colsX[7] - colsX[6], lineHeight),
                rightAligned);
        }

        /// <summary>
        /// Print the Payment table and all it's relating information
        /// </summary>
        /// <param name="pBounds">The bounds on the page we can print on</param>
        /// <param name="g">The graphics pointer we use to print with</param>
        /// <param name="font">The default font for all text</param>
        /// <param name="black">The color of the text</param>
        /// <param name="paymentColumnX">An array containing all the X-asis bounds on the table.</param>
        /// <param name="tableHeadY">The location from where the table should start from the Y-asis</param>
        /// <param name="lineHeight">The height of each line in the table</param>
        /// <param name="rightAligned">String format for text that is supposed to align to the right</param>
        private void PrintTablePayment(Rectangle pBounds, Graphics g, Font font, Brush black, float[] paymentColumnX, float tableHeadY, float lineHeight, StringFormat rightAligned)
        {
            //Initialize some variables
            long totalPrice = 0;
            int count = 0;

            g.FillRectangle(Brushes.LightGray,
                new Rectangle(pBounds.X, Convert.ToInt32(tableHeadY),
                pBounds.Width, Convert.ToInt32(lineHeight)));

            //Print header information for the Payment list
            //This will be displayed on top of the grey border
            g.DrawString("Greiðsluaðferð", font, black, new RectangleF(paymentColumnX[0], tableHeadY, paymentColumnX[1] - paymentColumnX[0], lineHeight));
            g.DrawString("Fjöldi", font, black, new RectangleF(paymentColumnX[1], tableHeadY, paymentColumnX[2] - paymentColumnX[1], lineHeight));
            g.DrawString("Samtals", font, black, new RectangleF(paymentColumnX[2], tableHeadY, paymentColumnX[3] - paymentColumnX[2], lineHeight));

            //Iterate through our payment collector and print information
            //on each and every payment in our collector
            for (int item = 0; item < _paymentCollection.Count; item++)
            {
                PrintPaymentDetails(g, font, black, rightAligned, paymentColumnX, tableHeadY, lineHeight, item);

                //Add to total
                totalPrice += _paymentCollection[item].Amount;
                count += _paymentCollection[item].Id;
            }
            //Print the total of our Payment list to the bottom
            PrintTotalOfPayment(pBounds, g, font, black, paymentColumnX, tableHeadY, lineHeight, rightAligned, count, totalPrice);
        }

        /// <summary>
        /// Print the total of the payment table.
        /// </summary>
        /// <param name="pBounds">The bounds on the page we can print on</param>
        /// <param name="g">The graphics pointer we use to print with</param>
        /// <param name="font">The default font for all text</param>
        /// <param name="black">The color of the text</param>
        /// <param name="_paymentCollection">The payment collection containing all the payment information</param>
        /// <param name="colsX">An array containing all the X-asis bounds on the table.</param>
        /// <param name="tableHeadY">The location from where the table should start from the Y-asis</param>
        /// <param name="lineHeight">The height of each line in the table</param>
        /// <param name="rightAligned">String format for text that is supposed to align to the right</param>
        /// <param name="count">The total number of all payments</param>
        /// <param name="totalPrice">The total of all payments</param>
        private void PrintTotalOfPayment(Rectangle pBounds, Graphics g, Font font, Brush black, float[] colsX, float tableHeadY, float lineHeight, StringFormat rightAligned, int count, long totalPrice)
        {
            //Calculate the Y on the page using information available to us now
            float currentRowY = tableHeadY + lineHeight * (1 + _paymentCollection.Count) + _paymentCollection.Count * 3 + 4;

            //Draw the line to seperate the list from the total
            g.DrawLine(Pens.Black, new Point(pBounds.X, Convert.ToInt32(currentRowY)), new Point(pBounds.X + pBounds.Width, Convert.ToInt32(currentRowY)));

            g.DrawString("Samtals",
                font,
                black,
                new RectangleF(colsX[0], currentRowY + 4, colsX[1] - colsX[0], lineHeight));

            g.DrawString(count.ToString(),
                font,
                black,
                new RectangleF(colsX[1], currentRowY + 4, colsX[2] - colsX[1], lineHeight),
                rightAligned);

            g.DrawString(string.Format("{0:#,0}", totalPrice),
                font,
                black,
                new RectangleF(colsX[2], currentRowY + 4, colsX[3] - colsX[2], lineHeight),
                rightAligned);
        }

        /// <summary>
        /// Print the header of the page containing information on when this is being printed and by whom.
        /// </summary>
        /// <param name="pBounds">The bounds on the page we can print on</param>
        /// <param name="g">The graphics pointer we use to print with</param>
        /// <param name="font">The default font for all text</param>
        /// <param name="black">The color of the text</param>
        /// <param name="pWidth">The width of the page bounds from which we can print on.</param>
        /// <param name="pHeight">The height of the page bounds from which we can print on.</param>
        /// <param name="lineHeight">The height of each line in the table</param>
        /// <param name="rightAligned">String format for text that is supposed to align to the right</param>
        private void PrintHeader(Rectangle pBounds, Graphics g, Font font, Brush black, int pWidth, int pHeight, float lineHeight, StringFormat rightAligned)
        {
            //Print the image to the top-left corner
            g.DrawImage(Image.FromFile("logo.png"), new Rectangle(pBounds.X, pBounds.Y, Convert.ToInt32(pWidth * 0.47), Convert.ToInt32(pHeight * 0.07632)));

            //Write information about when this
            //is being printed and who is printing this
            g.DrawString("Dagsetning:\nTími:\nStarfsmaður:", font, black, pBounds.X + pWidth * 0.48f, pBounds.Y);
            g.DrawString(string.Format("{0}\n{1}\n{2}",
                             DateTime.Now.ToShortDateString(),
                             DateTime.Now.ToShortTimeString(),
                             MainDatabase.GetDB.Users[_userId, true].Name), font, 
                black, 
                pBounds.X + pWidth * 0.7435135f, 
                pBounds.Y);

            //Draw a line to seperate the header from the content
            g.DrawLine(Pens.Black, new Point(pBounds.X, pBounds.Y + Convert.ToInt32(pHeight * 0.07632) + 2), new Point(pBounds.X + pBounds.Width, pBounds.Y + Convert.ToInt32(pHeight * 0.07632) + 2));

            //Write 
            g.DrawString("Uppgjör", new Font(font, FontStyle.Bold), black, pBounds.X + 30, pBounds.Y + pHeight * 0.1021f);

            //The following code checks to see if the orders do span from
            //one day to another and if so, print the date it spans
            string date = "";
            if (_orders[0].Date.ToShortDateString() != _orders[_orders.Count - 1].Date.ToShortDateString())
                //The date does span over one day, therefore we show what days
                //it spands in the following format etc.: 1. maí 2008 - 2. maí 2008
                date = string.Format("{0} - {1}", _orders[0].Date.ToShortDateString(), _orders[_orders.Count - 1].Date.ToShortDateString());
            else
                //The date does not span over one day
                date = _orders[0].Date.ToShortDateString();

            //Print the date our order collection spans over
            g.DrawString("\nTímabil:", font, black, pBounds.X + 30, pBounds.Y + pHeight * 0.1021f);
            g.DrawString(string.Format("\n{0}", date),
                font, 
                black,
                new RectangleF(pBounds.X + 30, pBounds.Y + pHeight * 0.1021f, (pBounds.X + pWidth * 0.8435135f) - (pBounds.X + pWidth * 0.48f), lineHeight * 3),
                rightAligned);
        }

        /// <summary>
        /// Print a row on the order table with specified index of the order
        /// </summary>
        /// <param name="g">The graphics pointer we use to print with</param>
        /// <param name="font">The default font for all text</param>
        /// <param name="black">The color of the text</param>
        /// <param name="rightAligned">String format for text that is supposed to align to the right</param>
        /// <param name="colsX">An array containing all the X-asis bounds on the table.</param>
        /// <param name="tableHeadY">The location from where the table should start from the Y-asis</param>
        /// <param name="lineHeight">The height of each line in the table</param>
        /// <param name="item">The indes of the order currently being printed.</param>
        private void PrintOrderDetails(Graphics g, Font font, Brush black, StringFormat rightAligned, float[] colsX, float tableHeadY, float lineHeight, int item)
        {
            //Calculate the Y on the page using information available to us now
            float currentRowY = tableHeadY + lineHeight * (1 + item) + item * 3 + 4;

            g.DrawString(_orders[item].Date.ToShortTimeString(),
                font,
                black,
                new RectangleF(colsX[0], currentRowY, colsX[1] - colsX[0], lineHeight));

            g.DrawString(_orders[item].OrderNumber.ToString(),
                font,
                black,
                new RectangleF(colsX[1], currentRowY, colsX[2] - colsX[1], lineHeight));

            g.DrawString(_orders[item].Kennitala,
                font,
                black,
                new RectangleF(colsX[2], currentRowY, colsX[3] - colsX[2], lineHeight));

            g.DrawString(_orders[item].GetNumberOfItems().ToString(),
                font,
                black,
                new RectangleF(colsX[3], currentRowY, colsX[4] - colsX[3], lineHeight));

            g.DrawString(_orders[item].Payment.GetString(),
                font,
                black,
                new RectangleF(colsX[4], currentRowY, colsX[5] - colsX[4], lineHeight));

            g.DrawString(string.Format("{0:#,0}", _orders[item].Items.TotalVsk(ItemVsk.items_240) + _orders[item].Items.TotalVsk(ItemVsk.books_7)),
                font,
                black,
                new RectangleF(colsX[5], currentRowY, colsX[6] - colsX[5], lineHeight),
                rightAligned);

            g.DrawString(string.Format("{0:#,0}", _orders[item].Items.Total()),
                font,
                black,
                new RectangleF(colsX[6], currentRowY, colsX[7] - colsX[6], lineHeight),
                rightAligned);
        }

        /// <summary>
        /// Print a row on the payment table with specified index of the payment
        /// </summary>
        /// <param name="g">The graphics pointer we use to print with</param>
        /// <param name="font">The default font for all text</param>
        /// <param name="black">The color of the text</param>
        /// <param name="rightAligned">String format for text that is supposed to align to the right</param>
        /// <param name="colsX">An array containing all the X-asis bounds on the table.</param>
        /// <param name="tableHeadY">The location from where the table should start from the Y-asis</param>
        /// <param name="lineHeight">The height of each line in the table</param>
        /// <param name="index">The index of the payment currently being printed</param>
        private void PrintPaymentDetails(Graphics g, Font font, Brush black, StringFormat rightAligned, float[] colsX, float tableHeadY, float lineHeight, int index)
        {
            //Calculate the Y on the page using information available to us now
            float currentRowY = tableHeadY + lineHeight * (1 + index) + (index) * 3 + 4;

            g.DrawString(_paymentCollection[index].Name,
                font,
                black,
                new RectangleF(colsX[0], currentRowY, colsX[1] - colsX[0], lineHeight));

            g.DrawString(_paymentCollection[index].Id.ToString(),
                font,
                black,
                new RectangleF(colsX[1], currentRowY, colsX[2] - colsX[1], lineHeight),
                rightAligned);

            g.DrawString(_paymentCollection[index].Amount.ToString(),
                font,
                black,
                new RectangleF(colsX[2], currentRowY, colsX[3] - colsX[2], lineHeight),
                rightAligned);
        }
	}
}
