using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kisildalur.Pages
{
	/// <summary>
	/// Interaction logic for Payment.xaml
	/// </summary>
	public partial class Payment : Page
	{
		public Payment()
		{
			InitializeComponent();
			this.DataContextChanged += new DependencyPropertyChangedEventHandler(Payment_DataContextChanged);
		}

		Database.PayMethod _method;
		Popup _popup;

		void Payment_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (this.DataContext != null)
			{
				_method = (Database.PayMethod)this.DataContext;
                if (_method.Id == -1)
                    textboxName.Text = "";
                if (_popup == null)
				    _popup = ((this.Tag as Frame).Parent as Border).Parent as Popup;
			}
		}

		private void buttonRight_Click(object sender, RoutedEventArgs e)
		{
			_method.Name = textboxName.Text;
			if (_method.Id != -1)
				Main.DB.PayMethods.Update(_method);
			else
				Main.DB.PayMethods.Add(_method, true);
			buttonLeft_Click(null, null);
		}

		private void buttonLeft_Click(object sender, RoutedEventArgs e)
		{
			Storyboard sizePopupClose = (Storyboard)_popup.FindResource("sizePopupClose"); // (Storyboard)FindResource("sizePopupClose");
			sizePopupClose.Begin(_popup);
		}
	}
}
