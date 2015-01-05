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

namespace WpfTest
{
	/// <summary>
	/// Interaction logic for Page1.xaml
	/// </summary>
	public partial class Page1 : Page
	{
		public Page1()
		{
			InitializeComponent();

			List<string> list = new List<string>();
			for (int i = 0; i < 9; i++)
			{
				list.Add("Nulla facilisi. Quisque sed dolor dolor, sed porttitor arcu. Sed sodales velit at est aliquam aliquam. Nullam sodales quam sed dui rutrum accumsan. Suspendisse dapibus, enim eu condimentum scelerisque, ante justo posuere lorem, eget sollicitudin urna lorem in justo. Sed pellentesque quam id ante ornare euismod. Vestibulum quis dolor quam. Nunc gravida lectus vel lectus consequat at condimentum sem lacinia. In aliquet, tortor ut vulputate ultricies, urna dolor vehicula metus, vel cursus neque nisi sit amet diam. Proin a erat et arcu pretium sodales. Duis varius pharetra elit, sit amet interdum dolor malesuada quis. Nam facilisis, elit in venenatis accumsan, ante lectus imperdiet enim, nec consequat velit enim non diam. Vestibulum scelerisque varius libero consequat fringilla. In congue fermentum tellus, et posuere quam condimentum ut. Mauris dictum sodales venenatis. Nullam a leo mi, at pulvinar ipsum. Phasellus iaculis massa ut lectus aliquam egestas.");
			}
			TableRowGroup g = table.RowGroups[0];
			for (int i = 0; i < list.Count; i++)
			{
				g.DataContext = list[i];

			}
			this.DataContext = list;
		}

		public DocumentPaginator GetDocument
		{
			get { return viewer.Document.DocumentPaginator; }
		}
	}
}
