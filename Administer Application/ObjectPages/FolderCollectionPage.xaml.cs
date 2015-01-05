using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Administer_Application.ObjectPages
{
	/// <summary>
	/// Interaction logic for FolderCollection.xaml
	/// </summary>
	public partial class FolderCollectionPage : Page,INotifyPropertyChanged
	{
		public FolderCollectionPage(FolderCollection collection)
		{
			InitializeComponent();
			this.DataContext = collection;
            this.treeCategories.ItemsSource = collection;

			this.treeCategories.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(treeCategories_SelectedItemChanged);
		}

		void treeCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			OnPropertyChanged(new PropertyChangedEventArgs("SelectedItem"));
		}

		public object SelectedItem
		{
			get { return treeCategories.SelectedItem; }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}
}
