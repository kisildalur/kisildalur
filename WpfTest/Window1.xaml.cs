using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Packaging;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Printing;

namespace WpfTest
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// Create a Print dialog.
			PrintDialog dlg = new PrintDialog();

			// Show the printer dialog.  If the return is "true",
			// the user made a valid selection and clicked "Ok".
			if (dlg.ShowDialog() == true)
			{
				XpsDocumentWriter xpsWriter = PrintQueue.CreateXpsDocumentWriter(dlg.PrintQueue);
				Page1 p = new Page1();
				DocumentPaginator doc = p.GetDocument;
				doc.PageSize = new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight);
				xpsWriter.Write(doc);
			}
		}
	}
}
