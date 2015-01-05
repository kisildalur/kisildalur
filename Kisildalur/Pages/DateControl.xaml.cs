using System;
using System.Collections.Generic;
using System.Globalization;
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
	/// Interaction logic for DateControl.xaml
	/// </summary>
	public partial class DateControl : Page
	{
		public DateControl()
		{
			InitializeComponent();
			this.DataContext = DateTime.Now;

			itemsControlDaysNames.ItemsSource = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
		}

		public DateControl(DateTime dateTime)
			: this()
		{
			this.DataContext = dateTime;
		}

		private void buttonChangeDate_Click(object sender, RoutedEventArgs e)
		{
			popupSearch.IsOpen = true;
			Storyboard sizePopupOpen = (Storyboard)popupSearch.FindResource("sizePopupOpen"); // (Storyboard)FindResource("sizePopupClose");
			sizePopupOpen.Begin(this);
		}

		private void buttonPreviousMonth_Click(object sender, RoutedEventArgs e)
		{
			this.DataContext = ((DateTime)this.DataContext).AddMonths(-1);
		}

		private void buttonNextMonth_Click(object sender, RoutedEventArgs e)
		{
			this.DataContext = ((DateTime)this.DataContext).AddMonths(1);
		}

		private void buttonPreviousYear_Click(object sender, RoutedEventArgs e)
		{
			this.DataContext = ((DateTime)this.DataContext).AddYears(-1);
		}

		private void buttonNextYear_Click(object sender, RoutedEventArgs e)
		{
			this.DataContext = ((DateTime)this.DataContext).AddYears(1);
		}

		private void ButtonDay_Click(object sender, RoutedEventArgs e)
		{
			DateTime date = ((sender as Button).DataContext as Day).GetDate;
			this.DataContext = date;

			Storyboard sizePopupClose = (Storyboard)popupSearch.FindResource("sizePopupClose"); // (Storyboard)FindResource("sizePopupClose");
			sizePopupClose.Begin(this);
		}

		private void popupSearch_Closed(object sender, EventArgs e)
		{
			(sender as Popup).Height = 0;
		}

		private void buttonToday_Click(object sender, RoutedEventArgs e)
		{
			this.DataContext = DateTime.Now;
			Storyboard sizePopupClose = (Storyboard)popupSearch.FindResource("sizePopupClose"); // (Storyboard)FindResource("sizePopupClose");
			sizePopupClose.Begin(this);
		}
	}

	class Day
	{
		public Day()
		{
			_background = Brushes.Transparent;
			_foreground = Brushes.Black;
		}

		public Day(DateTime day, Visibility visibility)
			:this()
		{
			_day = day;
			_visibility = visibility;
		}

		public Day(DateTime day, Visibility visibility, Color foreground, Color background)
			: this(day, visibility)
		{
			_foreground = new SolidColorBrush(foreground);
			_background = new SolidColorBrush(background);
		}

		DateTime _day;
		Visibility _visibility;
		SolidColorBrush _foreground;
		SolidColorBrush _background;

		public DateTime GetDate
		{
			get { return _day; }
		}

		public Visibility Visibility
		{
			get { return _visibility; }
		}

		public Brush ColorForeground
		{
			get { return _foreground; }
		}

		public Brush ColorBackground
		{
			get { return _background; }
		}
	}

	class DayCollection : List<Day>
	{
		public DayCollection()
			: base()
		{
		}
	}
}


