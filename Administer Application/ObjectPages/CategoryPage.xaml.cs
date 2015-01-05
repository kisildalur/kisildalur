using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
	/// Interaction logic for Folder.xaml
	/// </summary>
	public partial class CategoryPage : Page,INotifyPropertyChanged
	{
		public CategoryPage()
		{
			InitializeComponent();

			_editCategoryBinding = BindingOperations.GetBinding(gridEditCategory, Grid.HeightProperty);
			gridEditCategory.Height = 0;

			_items = null;
			this.DataContext = this;
		}

		Category _items;
		Binding _editCategoryBinding;
		public event PropertyChangedEventHandler PropertyChanged;

		public Category Category
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Category"));
				gridEditCategory.Height = 0;
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		private void editCategory_Click(object sender, RoutedEventArgs e)
		{
			Storyboard columnOpen = (Storyboard)gridEditCategory.FindResource("gridOpenAnimation"); // (Storyboard)FindResource("sizePopupClose");
			columnOpen.Completed += new EventHandler(columnOpen_Completed);
			columnOpen.Begin(this);
		}

		void columnOpen_Completed(object sender, EventArgs e)
		{
			if (!BindingOperations.IsDataBound(gridEditCategory, Grid.HeightProperty))
				BindingOperations.SetBinding(gridEditCategory, Grid.HeightProperty, _editCategoryBinding);
		}

		private void buttonCancelEditing_Click(object sender, RoutedEventArgs e)
		{
			Storyboard columnClose = (Storyboard)gridEditCategory.FindResource("gridCloseAnimation"); // (Storyboard)FindResource("sizePopupClose");
			columnClose.Completed += new EventHandler(columnClose_Completed);
			columnClose.Begin(this);
		}

		void columnClose_Completed(object sender, EventArgs e)
		{
			gridEditCategory.Height = 0;
		}

		private void buttonSaveEditing_Click(object sender, RoutedEventArgs e)
		{
			buttonCancelEditing_Click(null, null);
		}

		private void frameAddPropertyGroup_Loaded(object sender, RoutedEventArgs e)
		{
			if (!(sender as Frame).HasContent)
			{
				ObjectPages.PropertyGroup pagePropertyGroup = new PropertyGroup();
				pagePropertyGroup.Tag = (sender as Frame);
				pagePropertyGroup.DataContext = (sender as Frame).DataContext;

				(sender as Frame).Navigate(pagePropertyGroup);
			}
		}

		private void buttonAddPropertyGroup_Click(object sender, RoutedEventArgs e)
		{
			popupAddPropertyGroup.IsOpen = true;
			Storyboard popupOpen = (Storyboard)this.popupAddPropertyGroup.FindResource("sizePopupOpen"); // (Storyboard)FindResource("sizePopupClose");
			popupOpen.Begin(this);
		}

		private void buttonEditPropertyGroup_Click(object sender, RoutedEventArgs e)
		{
			Popup popup = (((sender as Button).Parent as StackPanel).Children[2] as Popup);
			popup.IsOpen = true;
			Storyboard popupOpen = (Storyboard)popup.FindResource("sizePopupOpen"); // (Storyboard)FindResource("sizePopupClose");
			popupOpen.Begin(popup);
		}
	}
}
