using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Database;
using Kisildalur.Pages;

namespace Kisildalur
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibililty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visible = (bool)value;
            if (visible)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visible = (Visibility)value;
            if (visible == Visibility.Collapsed)
                return false;
            return true;
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class SelectedItemIsCategory : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Category)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); 
        }
    }

    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToHeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visible = (bool)value;
            if (visible)
                return "Auto";
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string height = (string)value;
            if (height == "Auto")
                return true;
            return false;
        }
    }

	[ValueConversion(typeof(string), typeof(string))]
	public class EmptyStringToHeight : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string text = (string)value;
			if (!string.IsNullOrEmpty(text))
				return "Auto";
			return "0";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return "";
		}
	}
	//EmptyStringToHeight

    [ValueConversion(typeof(int), typeof(string))]
    public class IntIdToStaffName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int id = (int)value;
            return Main.DB.Users[id, true].Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string name = (string)value;
            foreach (Database.User user in Main.DB.Users)
                if (user.Name == name)
                    return user.ID;
            return -1;
        }
    }

    [ValueConversion(typeof(DiscountType), typeof(bool))]
    public class DiscountNoneCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DiscountType type = (DiscountType)value;
            if (type == DiscountType.None)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DiscountType.None;
        }
    }

    [ValueConversion(typeof(DiscountType), typeof(bool))]
    public class DiscountPercentCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DiscountType type = (DiscountType)value;
            if (type == DiscountType.Percent)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DiscountType.Percent;
        }
    }

    [ValueConversion(typeof(DiscountType), typeof(bool))]
    public class DiscountCoinCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DiscountType type = (DiscountType)value;
            if (type == DiscountType.Coin)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DiscountType.Coin;
        }
    }

	[ValueConversion(typeof(DiscountType), typeof(bool))]
	public class DiscountNoneInvertedCheckedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DiscountType type = (DiscountType)value;
			if (type == DiscountType.None)
				return false;
			return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return DiscountType.None;
		}
	}

	[ValueConversion(typeof(int), typeof(int))]
	public class ColumnWidthShortener : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((int)value) - 20;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}

	public class CalculateItemTotal : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
		{
			if (values[0] is long && values[1] is double)
				return ((long)values[0] * (double)values[1]).ToString();
			return "0";
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) 
		{
			throw new NotImplementedException(); 
		}
	}

	[ValueConversion(typeof(DateTime), typeof(string))]
	public class DateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				DateTime dt = (DateTime)value;
				return dt.ToLongDateString();
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(DateTime), typeof(string))]
	public class TimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DateTime dt = (DateTime)value;
			return dt.ToShortTimeString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(Customer), typeof(bool))]
	public class OrderContainsCustomer : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Customer customer = value as Customer;
			if (customer.Kennitala != "" &&
				customer.Name != "")
				return true;
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(int), typeof(Bitmap))]
	public class IntIdToImageThumbPath : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (System.IO.File.Exists(string.Format("{0}\\thumb_images\\{1}_thumb.jpg", System.Windows.Forms.Application.StartupPath, (int)value)))
			{
				try
				{
					return Imaging.CreateBitmapSourceFromHBitmap(new Bitmap(string.Format("{0}\\thumb_images\\{1}_thumb.jpg", System.Windows.Forms.Application.StartupPath, (int)value)).GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				}
				catch (Exception e)
				{
					System.IO.File.Delete(string.Format("{0}\\thumb_images\\{1}_small.jpg", System.Windows.Forms.Application.StartupPath, (int)value));
					return null;
				}
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}

	[ValueConversion(typeof(int), typeof(Bitmap))]
	public class IntIdToImageSmallPath : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (System.IO.File.Exists(string.Format("{0}\\thumb_images\\{1}_small.jpg", System.Windows.Forms.Application.StartupPath, (int)value)))
			{
				try
				{
					return Imaging.CreateBitmapSourceFromHBitmap(new Bitmap(string.Format("{0}\\thumb_images\\{1}_small.jpg", System.Windows.Forms.Application.StartupPath, (int)value)).GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				}
				catch (Exception e)
				{
					System.IO.File.Delete(string.Format("{0}\\thumb_images\\{1}_small.jpg", System.Windows.Forms.Application.StartupPath, (int)value));
					return null;
				}
			}
			return null;
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
    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToBackgroundColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool background = (bool)value;
            if (background)
                return "#FFF888";
            return "#FFFFFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
	[ValueConversion(typeof(bool), typeof(string))]
	public class ItemDescriptionToString : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string output = "";
			if (value != null)
			{
				Item item = value as Item;
				if (!item.Visible)
					output += "Vara er ósýnileg á vefsíðu";
				if (item.SubProducts.Count > 0)
				{
					if (output != "")
						output += "\n";
					output += string.Format("Vara inniheldur {0} vörur sem fylgja með", item.SubProducts.Count);
				}
			}
			return output;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(bool), typeof(string))]
	public class ItemSubItemsToString : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string output = "";
			if (value != null)
			{
				Item item = value as Item;
				if (item.SubProducts.Count > 0)
					output = string.Format("{0} vörur fylgja þessari vöru", item.SubProducts.Count);
			}
			return output;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(bool), typeof(string))]
	public class ItemCalculatePriceToString : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string output = "";
			if (value != null)
			{
				Item item = value as Item;
				if (item.CalculatePrice)
					output = " - Verð reiknast sjálfkrafa";
			}
			return output;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(bool), typeof(string))]
	public class ItemIsVisibleToBool : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				Item item = value as Item;
				return item.Visible;
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(bool), typeof(string))]
	public class ItemSelectedToBool : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				if (value is Folder)
					return false;
				return true;
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(string), typeof(Visibility))]
	public class StringToVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				if (value is string)
					if (value as string != "")
						return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	
}
