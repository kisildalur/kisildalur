using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Database;
using AODL;
using AODL.Document;
using AODL.Document.SpreadsheetDocuments;
using AODL.Document.Content;
using AODL.Document.Styles;
using AODL.Document.Content.Tables;
using AODL.Document.Content.Text;

namespace Kisildalur
{
	class DocumentReport
	{
		Customer _customer;
		SpreadsheetDocument _document;

		public DocumentReport(Customer customer)
		{
			_customer = customer;
		}

		public void Generate(string filename)
		{
			_document = new SpreadsheetDocument();
			_document.New();

			CellStyle s = new CellStyle(_document, "header");
			s.CellProperties.BackgroundColor = "#DDDDDD";
			s.CellProperties.Border = "0.002cm solid #000000";
			_document.Styles.Add(s);
			s = new CellStyle(_document, "border_bottom");
			s.CellProperties.BorderBottom = "0.002cm solid #000000";
			_document.Styles.Add(s);
			s = new CellStyle(_document, "border_top");
			s.CellProperties.BorderTop = "0.002cm solid #000000";
			_document.Styles.Add(s);

			ColumnStyle c = new ColumnStyle(_document, "width1");
			c.ColumnProperties.Width = "2.267cm";
			_document.Styles.Add(c);
			c = new ColumnStyle(_document, "width2");
			c.ColumnProperties.Width = "3.967cm";
			_document.Styles.Add(c);
			c = new ColumnStyle(_document, "width3");
			c.ColumnProperties.Width = "3.302cm";
			_document.Styles.Add(c);
			c = new ColumnStyle(_document, "width4");
			c.ColumnProperties.Width = "6.234cm";
			_document.Styles.Add(c);
			c = new ColumnStyle(_document, "width5");
			c.ColumnProperties.Width = "1.0cm";
			_document.Styles.Add(c);

			TextStyle te = new TextStyle(_document, "large");
			te.TextProperties.FontSize = "10.5pt";
			te.TextProperties.Bold = "bold";
			_document.Styles.Add(te);

			te = new TextStyle(_document, "small");
			te.TextProperties.FontSize = "8";
			te.TextProperties.Italic = "italic";
			_document.Styles.Add(te);

			GenerateMonthlyReport();
			_document.SaveTo(filename);
		}

		private void GenerateMonthlyReport()
		{
			Table t = new Table(_document, "Mánaðarskýrslur", "all");
			
			t.ColumnCollection.Add(new Column(t, "width3"));
			t.ColumnCollection.Add(new Column(t, "width2"));
			t.ColumnCollection.Add(new Column(t, "width1"));
			t.ColumnCollection.Add(new Column(t, "width3"));
			t.ColumnCollection.Add(new Column(t, "width1"));

			_document.TableCollection.Add(t);
			
			string header = string.Format("Kennitala:\nNafn:{0}{1}",
				!string.IsNullOrEmpty(_customer.Address1) ? "\nHeimilisfang:" : "",
				!string.IsNullOrEmpty(_customer.City) ? "\nPóstfang:" : "");
			InsertTextAt(t, 0, 1, header);
			header = string.Format("{0}\n{1}{2}{3}", _customer.Kennitala, _customer.Name,
				!string.IsNullOrEmpty(_customer.Address1) ? "\n" + _customer.Address1 + " " + _customer.Address2 : "",
				!string.IsNullOrEmpty(_customer.City) ? "\n" + _customer.Zip + " " + _customer.City : "");
			InsertTextAt(t, 1, 1, header);

			InsertTextAt(t, 0, 7, "Dagsetning", "header");
			InsertTextAt(t, 1, 7, "Pöntunarnúmer", "header");
			InsertTextAt(t, 2, 7, "Fjöldi", "header");
			InsertTextAt(t, 3, 7, "Samtals án VSK", "header");
			InsertTextAt(t, 4, 7, "VSK", "header");
			InsertTextAt(t, 5, 7, "Samtals", "header");

			DateTime lastDate = DateTime.MinValue;
			int skip = -2;
			OrderCollection ordersTemp = new OrderCollection();
			double total = 0;
			double totalvsk = 0;
			for (int i = 0; i < _customer.Orders.Count; i++)
			{
				Order o = _customer.Orders[i];
				
				if (o.Date.Month != lastDate.Month || o.Date.Year != lastDate.Year)
				{
					if (i > 0)
					{
						PrintTotal(t, skip, total, totalvsk, i);
						total = totalvsk = 0;
					}
					lastDate = o.Date;
					InsertTextAt(t, 0, 8 + i + skip + 3, string.Format("{1} {0:MMMM}", lastDate, lastDate.Year), "border_bottom", "large");
					InsertTextAt(t, 1, 8 + i + skip + 3, "", "border_bottom");
					InsertTextAt(t, 2, 8 + i + skip + 3, "", "border_bottom");
					InsertTextAt(t, 3, 8 + i + skip + 3, "", "border_bottom");
					InsertTextAt(t, 4, 8 + i + skip + 3, "", "border_bottom");
					InsertTextAt(t, 5, 8 + i + skip + 3, "", "border_bottom");
					skip += 4;

					if (ordersTemp.Count > 0)
						PrintDetailMonthReport(ordersTemp);
					ordersTemp.Clear();
				}
				InsertTextAt(t, 0, 8 + i + skip, o.Date.ToShortDateString() + " " + o.Date.ToShortTimeString());
				InsertTextAt(t, 1, 8 + i + skip, o.OrderNumber.ToString());
				InsertTextAt(t, 2, 8 + i + skip, o.Items.Count.ToString());
				InsertTextAt(t, 3, 8 + i + skip, o.Total.ToString("#,0"));
				InsertTextAt(t, 4, 8 + i + skip, o.TotalWithoutVSK.ToString("#,0"));
				InsertTextAt(t, 5, 8 + i + skip, o.Total.ToString("#,0"));
				ordersTemp.Add(o);

				total += o.Total;
				totalvsk += o.TotalVSK;
			}

			if (_customer.Orders.Count > 0)
			{
				PrintTotal(t, skip, total, totalvsk, _customer.Orders.Count);
				PrintDetailMonthReport(ordersTemp);

				skip += 4;

				InsertTextAt(t, 0, 8 + _customer.Orders.Count + skip, "Alls", "border_bottom", "large");
				InsertTextAt(t, 1, 8 + _customer.Orders.Count + skip, "", "border_bottom");
				InsertTextAt(t, 2, 8 + _customer.Orders.Count + skip, "", "border_bottom");
				InsertTextAt(t, 3, 8 + _customer.Orders.Count + skip, "", "border_bottom");
				InsertTextAt(t, 4, 8 + _customer.Orders.Count + skip, "", "border_bottom");
				InsertTextAt(t, 5, 8 + _customer.Orders.Count + skip, "", "border_bottom");
				skip++;
				total = totalvsk = 0;
				foreach (Order o in _customer.Orders)
				{
					total += o.Total;
					totalvsk += o.TotalVSK;
				}
				PrintTotal(t, skip, total, totalvsk, _customer.Orders.Count);
				skip += 3;
				PrintPayment(t, skip, _customer.Orders, _customer.Orders.Count, false);
			}
		}

		private void PrintDetailMonthReport(OrderCollection orders)
		{
			Table t = new Table(_document, string.Format("{0:00}/{1}", orders[0].Date.Month, orders[0].Date.Year), orders[0].Date.Year + "_" + orders[0].Date.Month);

			t.ColumnCollection.Add(new Column(t, "width3"));
			t.ColumnCollection.Add(new Column(t, "width4"));
			t.ColumnCollection.Add(new Column(t, "width5"));
			t.ColumnCollection.Add(new Column(t, "width3"));
			t.ColumnCollection.Add(new Column(t, "width1"));

			string header = string.Format("Kennitala:\nNafn:{0}{1}",
				!string.IsNullOrEmpty(_customer.Address1) ? "\nHeimilisfang:" : "",
				!string.IsNullOrEmpty(_customer.City) ? "\nPóstfang:" : "");
			InsertTextAt(t, 0, 1, header);
			header = string.Format("{0}\n{1}{2}{3}", _customer.Kennitala, _customer.Name,
				!string.IsNullOrEmpty(_customer.Address1) ? "\n" + _customer.Address1 + " " + _customer.Address2 : "",
				!string.IsNullOrEmpty(_customer.City) ? "\n" + _customer.Zip + " " + _customer.City : "");
			InsertTextAt(t, 1, 1, header);

			InsertTextAt(t, 0, 7, string.Format("Pantanir yfir {0:MMMM} mánuðinn árið {1}", orders[0].Date, orders[0].Date.Year), "default", "large");

			InsertTextAt(t, 0, 9, "Dagsetning", "header");
			InsertTextAt(t, 1, 9, "Pöntunarnúmer", "header");
			InsertTextAt(t, 2, 9, "", "header");
			InsertTextAt(t, 3, 9, "Samtals án VSK", "header");
			InsertTextAt(t, 4, 9, "VSK", "header");
			InsertTextAt(t, 5, 9, "Samtals", "header");

			int skip = 0;
			double total = 0;
			double totalvsk = 0;
			for (int i = 0; i < orders.Count; i++)
			{
				Order o = orders[i];
				
				InsertTextAt(t, 0, 10 + i + skip, o.Date.ToShortDateString() + " " + o.Date.ToShortTimeString());
				InsertTextAt(t, 1, 10 + i + skip, o.OrderNumber.ToString());
				InsertTextAt(t, 2, 10 + i + skip, "");
				InsertTextAt(t, 3, 10 + i + skip, o.Total.ToString("#,0"));
				InsertTextAt(t, 4, 10 + i + skip, o.TotalWithoutVSK.ToString("#,0"));
				InsertTextAt(t, 5, 10 + i + skip, o.Total.ToString("#,0"));

				for (int p = 0; p < o.Payment.Count; p++)
				{
					skip++;
					InsertTextAt(t, 0, 10 + i + skip, o.Payment[p].Name, "default", "small");
					InsertTextAt(t, 5, 10 + i + skip, o.Payment[p].Amount.ToString("#,0"), "default", "small");
				}
				for (int item = 0; item < o.Items.Count; item++)
				{
					skip++;
					InsertTextAt(t, 0, 10 + i + skip, string.Format("{0}{1}", o.Items[item].Name, !string.IsNullOrEmpty(o.Items[item].SubName) ? " - " + o.Items[item].SubName : ""), "default", "small");
					InsertTextAt(t, 2, 10 + i + skip, o.Items[item].Count.ToString(), "default", "small");
				}

				total += o.Total;
				totalvsk += o.TotalVSK;
				skip++;
			}
			PrintTotal(t, skip, total, totalvsk, orders.Count + 1);
			skip += 3;
			PrintPayment(t, skip, orders, orders.Count, true);

			_document.TableCollection.Add(t);
		}


		private void PrintTotal(Table t, int skip, double total, double totalvsk, int i)
		{
			InsertTextAt(t, 0, 8 + i + skip, "Samtals", "border_top");
			InsertTextAt(t, 1, 8 + i + skip, "", "border_top");
			InsertTextAt(t, 2, 8 + i + skip, "", "border_top");
			InsertTextAt(t, 3, 8 + i + skip, (total - totalvsk).ToString("#,0"), "border_top");
			InsertTextAt(t, 4, 8 + i + skip, totalvsk.ToString("#,0"), "border_top");
			InsertTextAt(t, 5, 8 + i + skip, total.ToString("#,0"), "border_top");
		}

		private void PrintPayment(Table t, int skip, OrderCollection orders, int i, bool skipCheck)
		{
			bool doCheck = false, runAgain = false;
			if (orders[0].Date.Year < 2015 && !skipCheck)
			{
				InsertTextAt(t, 0, 8 + i + skip, "Greiðslumáti (fyrir 2015)", "border_bottom", "large");
				doCheck = true;
				if (orders[orders.Count - 1].Date.Year >= 2010)
					runAgain = true;
			}
			else
				InsertTextAt(t, 0, 8 + i + skip, "Greiðslumáti", "border_bottom", "large");

			InsertTextAt(t, 1, 8 + i + skip, "", "border_bottom");
			InsertTextAt(t, 2, 8 + i + skip, "", "border_bottom");
			InsertTextAt(t, 3, 8 + i + skip, "", "border_bottom");
			InsertTextAt(t, 4, 8 + i + skip, "", "border_bottom");
			InsertTextAt(t, 5, 8 + i + skip, "", "border_bottom");
			skip++;
			int oi = 0;

			do
			{
				double total = 0, totalvsk = 0;
				OrderPaymentCollection paymentCollection = new OrderPaymentCollection();
				for (; oi < orders.Count; oi++)
				{
					if (doCheck)
					{
						if (orders[oi].Date.Year >= 2015)
						{
							doCheck = false;
							break;
						}
					}
					else if (runAgain)
					{
						runAgain = false;
						if (oi == orders.Count)
							return;
						skip += 2;
						InsertTextAt(t, 0, 8 + i + skip, "Greiðslumáti (eftir 2010)", "border_bottom", "large");
						InsertTextAt(t, 1, 8 + i + skip, "", "border_bottom");
						InsertTextAt(t, 2, 8 + i + skip, "", "border_bottom");
						InsertTextAt(t, 3, 8 + i + skip, "", "border_bottom");
						InsertTextAt(t, 4, 8 + i + skip, "", "border_bottom");
						InsertTextAt(t, 5, 8 + i + skip, "", "border_bottom");
						skip++;
					}

					foreach (OrderPayment payment in orders[oi].Payment)
					{
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

				for (int p = 0; p < paymentCollection.Count; p++)
				{
					double curtotalvsk = 0;
					if (doCheck)
						curtotalvsk = paymentCollection[p].Amount * (1 - (1 / 1.255));
					else
						curtotalvsk = paymentCollection[p].Amount * (1 - (1 / 1.240));
					totalvsk += curtotalvsk;
					total += paymentCollection[p].Amount;
					InsertTextAt(t, 0, 8 + i + skip, paymentCollection[p].Name);
					InsertTextAt(t, 2, 8 + i + skip, paymentCollection[p].Id.ToString());
					InsertTextAt(t, 3, 8 + i + skip, (paymentCollection[p].Amount - totalvsk).ToString("#,0"));
					InsertTextAt(t, 4, 8 + i + skip, totalvsk.ToString("#,0"));
					InsertTextAt(t, 5, 8 + i + skip, paymentCollection[p].Amount.ToString("#,0"));
					skip++;
				}
				InsertTextAt(t, 0, 8 + i + skip, "Samtals", "border_top");
				InsertTextAt(t, 1, 8 + i + skip, "", "border_top");
				InsertTextAt(t, 2, 8 + i + skip, "", "border_top");
				
				InsertTextAt(t, 3, 8 + i + skip, (total - totalvsk).ToString("#,0"), "border_top");
				InsertTextAt(t, 4, 8 + i + skip, totalvsk.ToString("#,0"), "border_top");
				InsertTextAt(t, 5, 8 + i + skip, total.ToString("#,0"), "border_top");
			} while (runAgain);
		}

		private void InsertTextAt(Table t, int x, int y, string text)
		{
			InsertTextAt(t, x, y, text, "default");
		}

		private void InsertTextAt(Table t, int x, int y, string text, string cellStyle)
		{
			InsertTextAt(t, x, y, text, cellStyle, "default");
		}

		private void InsertTextAt(Table t, int x, int y, string text, string cellStyle, string textStyle)
		{
			string[] split = text.Split('\n');
			while (t.Rows.Count < y + 1)
			{
				Row r = new Row(t);
				t.Rows.Add(r);
			}
			for (int i = 0; i < split.Length; i++)
			{
				Cell c = t.CreateCell();
				c.StyleName = cellStyle;
				Paragraph p = new Paragraph(_document, textStyle);

				if (textStyle == "large")
				{
					p.ParagraphStyle = new ParagraphStyle(_document, "large");
					p.ParagraphStyle.TextProperties.FontSize = "10.5pt";
					p.ParagraphStyle.TextProperties.Bold = "bold";
					_document.Styles.Add(p.ParagraphStyle);
				}
				FormatedText te = new FormatedText(_document, "t1", split[i]);
				te.TextStyle = new TextStyle(_document, textStyle);

				p.TextContent.Add(te);
				c.Content.Add(p);
				t.InsertCellAt(y + i, x, c);
			}
			
		}
	}
}
