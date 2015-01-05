using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text;

namespace Administer_Application
{
	[ValueConversion(typeof(int), typeof(Bitmap))]
	public class IntIdToImageThumbPath : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (System.IO.File.Exists(string.Format("{0}\\thumb_images\\{1}_thumb.jpg", System.Windows.Forms.Application.StartupPath, (int)value)))
			{
				string path = string.Format("{0}\\thumb_images\\{1}_thumb.jpg", System.Windows.Forms.Application.StartupPath, (int)value);
				Bitmap b = new Bitmap(path);
				return Imaging.CreateBitmapSourceFromHBitmap(b.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}

	[ValueConversion(typeof(int), typeof(int))]
	public class IntIdToImageExistInt : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (System.IO.File.Exists(string.Format("{0}\\thumb_images\\{1}_small.jpg", System.Windows.Forms.Application.StartupPath, (int)value)))
				return 1;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return 0;
		}
	}

	[ValueConversion(typeof(long), typeof(string))]
	public class PriceToFormattedString : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			long price = (long)value;
			string temp = string.Format("{0:#,0}", price);
			return temp;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return 0;
		}
	}
}
