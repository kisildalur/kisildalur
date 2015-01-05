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
using Database;

namespace Kisildalur.Controls
{
	/// <summary>
	/// Interaction logic for WebSiteOrder.xaml
	/// </summary>
	public partial class WebSiteOrder : UserControl
	{
		public WebSiteOrder()
		{
			InitializeComponent();
		}

		public event RoutedEventHandler ViewOrderClick;

		private void viewOrder_Click(object sender, RoutedEventArgs e)
		{
			if (ViewOrderClick != null)
				ViewOrderClick(this, e);
		}
	}

	[ValueConversion(typeof(SiteOrder), typeof(Brush))]
	public class WebOrderToBrush : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			
			if (value is SiteOrder)
			{
				SiteOrder order = value as SiteOrder;
				if (DateTime.Now < order.Date || order.Stage == SiteOrderStage.Finished)
					return new SolidColorBrush(Color.FromArgb(255, 207, 207, 207));
				else if (((DateTime.Now - order.Date).Days < 2 && order.Stage == SiteOrderStage.New) ||
						((DateTime.Now - order.Date).Days < 4 && order.Stage == SiteOrderStage.Confirmed))
					return new SolidColorBrush(Color.FromArgb(255, 56, 168, 0));
				else if (((DateTime.Now - order.Date).Days < 4 && order.Stage == SiteOrderStage.New) ||
						((DateTime.Now - order.Date).Days < 6 && order.Stage == SiteOrderStage.Confirmed))
					return new SolidColorBrush(Color.FromArgb(255, 227, 196, 0));
				else
					return new SolidColorBrush(Color.FromArgb(255, 212, 0, 0));
			}
			else
				return new SolidColorBrush(Color.FromArgb(255, 207, 207, 207));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(SiteOrderPaymethod), typeof(string))]
	public class SiteOrderPaymethodToString : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is SiteOrderPaymethod)
			{
				if ((SiteOrderPaymethod)value == SiteOrderPaymethod.PayiedOnTheSpot)
					return "Vörur verða borgaðar í búð";
				if ((SiteOrderPaymethod)value == SiteOrderPaymethod.MoneyTransfer)
					return "Vörur verða greiddar með millifærslu";
				if ((SiteOrderPaymethod)value == SiteOrderPaymethod.Card)
					return "Vörur verða greiddar með símgreiðslu";
				else
					return "Búið að greiða";
			}
			else
				return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(SiteOrderShipping), typeof(string))]
	public class SiteOrderShippingToString : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is SiteOrderShipping)
			{
				if ((SiteOrderShipping)value == SiteOrderShipping.PickedOnTheSpot)
					return "Vörur verða sóttar í búð";
				else
					return "Heimsending";
			}
			else
				return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
