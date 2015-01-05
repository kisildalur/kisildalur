using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Database;

namespace Kisildalur.Pages
{
	[ValueConversion(typeof(UserPrivileges), typeof(int))]
	public class UserPrivilegesToCombobox : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((UserPrivileges)value)
			{
				case UserPrivileges.Seller:
					return 0;
				default:
					return 1;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((int)value)
			{
				case 0:
					return UserPrivileges.Seller;
				default:
					return UserPrivileges.Admin;
			}
		}
	}

    [ValueConversion(typeof(UserPrivileges), typeof(bool))]
    public class SelectedValueToEnabledUp: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (Main.DB.PayMethods.IndexOf((Database.PayMethod)value) > 0)
                    return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    [ValueConversion(typeof(UserPrivileges), typeof(bool))]
    public class SelectedValueToEnabledDown : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (Main.DB.PayMethods.IndexOf((Database.PayMethod)value) < Main.DB.PayMethods.Count - 1)
                    return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

	[ValueConversion(typeof(double), typeof(double))]
	public class IntWidthToWidthOffset : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				if (value is double)
					return (((double)value) - 210) / 2;
			}
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(double), typeof(double))]
	public class IntMonthToStringName : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				if (value is int)
					return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)value);
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(string), typeof(Visibility))]
	public class TodayToVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				if (value is DateTime)
					if (((DateTime)value).Year == DateTime.Now.Year && ((DateTime)value).Month == DateTime.Now.Month && ((DateTime)value).Day == DateTime.Now.Day)
						return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(double), typeof(double))]
	public class DateTimeToDayCollection : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				if (value is DateTime)
				{
					DateTime today = DateTime.Now;
					DateTime date = (DateTime)value;
					DateTime startMonth = new DateTime(date.Year, date.Month, 1);
					DayCollection days = new DayCollection();

					System.Windows.Media.Color colorBlack = System.Windows.Media.Color.FromRgb(0, 0, 0);
					System.Windows.Media.Color colorWhite = System.Windows.Media.Color.FromRgb(255, 255, 255);


					int daysLastMonth = DateTime.DaysInMonth(date.Year, date.AddMonths(-1).Month);

					for (int i = 0; i < 7; i++)
					{
						if (i == (int)startMonth.DayOfWeek)
							break;
						days.Add(new Day(new DateTime(date.Year, date.AddMonths(-1).Month, daysLastMonth - ((int)startMonth.DayOfWeek - (i + 1))), Visibility.Visible, System.Windows.Media.Color.FromRgb(155, 155, 155), System.Windows.Media.Colors.Transparent));
					}
					int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
					for (int i = 1; i <= daysInMonth; i++)
					{
						if (i == date.Day)
							days.Add(new Day(new DateTime(date.Year, date.Month, i), Visibility.Visible, colorBlack, System.Windows.Media.Color.FromRgb(255, 255, 0)));
						else if (i == today.Day && date.Year == today.Year && date.Month == today.Month)
							days.Add(new Day(new DateTime(date.Year, date.Month, i), Visibility.Visible, colorWhite, System.Windows.Media.Color.FromRgb(0, 0, 255)));
						else
							days.Add(new Day(new DateTime(date.Year, date.Month, i), Visibility.Visible));
					}
					for (int i = 1; days.Count < 42; i++)
					{
						days.Add(new Day(new DateTime(date.Year, date.AddMonths(+1).Month, i), Visibility.Visible, System.Windows.Media.Color.FromRgb(155, 155, 155), System.Windows.Media.Colors.Transparent));
					}
					return days;
				}
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
