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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Database;

namespace Kisildalur
{
	/// <summary>
	/// Interaction logic for Vorugeymsla.xaml
	/// </summary>
	public partial class Vorugeymsla : Window
	{
		public Vorugeymsla()
		{
			InitializeComponent();
			this.DataContext = Main.DB;
			this.listItems.Items.CurrentChanged += new EventHandler(listItems_CurrentChanged);
			

			_bindingInfoHeight = BindingOperations.GetBinding(gridProductInfo, Grid.HeightProperty);
			gridProductInfo.Height = 300;

			_bindingProductId = BindingOperations.GetBinding(textboxProductId, TextBox.TextProperty);
			_bindingProductName = BindingOperations.GetBinding(textboxProductName, TextBox.TextProperty);
			_bindingProductSub = BindingOperations.GetBinding(textboxProductSub, TextBox.TextProperty);
			_bindingProductPrice = BindingOperations.GetBinding(textboxProductPrice, TextBox.TextProperty);
			_bindingProductStock = BindingOperations.GetBinding(textboxProductStock, TextBox.TextProperty);
			_bindingProductVisible = BindingOperations.GetBinding(checkboxProductVisible, CheckBox.IsCheckedProperty);
			_bindingIsEnabled = BindingOperations.GetBinding(textboxProductId, TextBox.IsEnabledProperty);
		}

		Binding _bindingInfoHeight;
		Binding _bindingProductId;
		Binding _bindingProductName;
		Binding _bindingProductSub;
		Binding _bindingProductPrice;
		Binding _bindingProductStock;
		Binding _bindingProductVisible;
		Binding _bindingIsEnabled;

		void listItems_CurrentChanged(object sender, EventArgs e)
		{
			ICollectionView dataView = CollectionViewSource.GetDefaultView(this.listItems.ItemsSource);
			// check the dataView isn't null
			if (dataView != null)
			{
				dataView.SortDescriptions.Clear();
				SortDescription sd = new SortDescription("Price", ListSortDirection.Ascending);
				dataView.SortDescriptions.Add(sd);
				dataView.Refresh();
			}
		}

		private void buttonClose_Click(object sender, RoutedEventArgs e)
		{
			if (buttonSaveChanges.IsEnabled)
				buttonSaveChanges_Click(null, null);
			else if (buttonSaveNewProduct.IsEnabled)
				buttonSaveNewProduct_Click(null, null);
			buttonDiscardClose_Click(null, null);
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			Properties.config.Default.Save();
		}

		private void listItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (buttonSaveNewProduct.IsEnabled)
			{
				if (e != null)
				{
					if (e.AddedItems.Count == 0)
						return;
					else if (!string.IsNullOrEmpty(textboxProductId.Text) || !string.IsNullOrEmpty(textboxProductName.Text) || !string.IsNullOrEmpty(textboxProductSub.Text))
					{
						if (MessageBox.Show("Þú varst að vinna að því að búa til vöru. Ef þú vilt hætta við að búa til vöruna og halda áfram því sem þú ætlaðir þér að gera, íttu á 'Já'.", "Vara þegar í vinnslu", MessageBoxButton.YesNo) == MessageBoxResult.No)
						{
							listItems.SelectedIndex = -1;
							return;
						}
					}
				}
				buttonSaveNewProduct.IsEnabled = false;
			}
			
			gridProductInfo.Height = 300;

			if (!buttonCreateNewProduct.IsEnabled)
				buttonCreateNewProduct.IsEnabled = true;

			if (!BindingOperations.IsDataBound(textboxProductId, TextBox.TextProperty))
				BindingOperations.SetBinding(textboxProductId, TextBox.TextProperty, _bindingProductId);
			if (!BindingOperations.IsDataBound(textboxProductId, TextBox.IsEnabledProperty))
				BindingOperations.SetBinding(textboxProductId, TextBox.IsEnabledProperty, _bindingIsEnabled);

			if (!BindingOperations.IsDataBound(textboxProductName, TextBox.TextProperty))
				BindingOperations.SetBinding(textboxProductName, TextBox.TextProperty, _bindingProductName);
			if (!BindingOperations.IsDataBound(textboxProductName, TextBox.IsEnabledProperty))
				BindingOperations.SetBinding(textboxProductName, TextBox.IsEnabledProperty, _bindingIsEnabled);

			if (!BindingOperations.IsDataBound(textboxProductSub, TextBox.TextProperty))
				BindingOperations.SetBinding(textboxProductSub, TextBox.TextProperty, _bindingProductSub);
			if (!BindingOperations.IsDataBound(textboxProductSub, TextBox.IsEnabledProperty))
				BindingOperations.SetBinding(textboxProductSub, TextBox.IsEnabledProperty, _bindingIsEnabled);

			if (!BindingOperations.IsDataBound(textboxProductPrice, TextBox.TextProperty))
				BindingOperations.SetBinding(textboxProductPrice, TextBox.TextProperty, _bindingProductPrice);
			if (!BindingOperations.IsDataBound(textboxProductPrice, TextBox.IsEnabledProperty))
				BindingOperations.SetBinding(textboxProductPrice, TextBox.IsEnabledProperty, _bindingIsEnabled);

			if (!BindingOperations.IsDataBound(textboxProductStock, TextBox.TextProperty))
				BindingOperations.SetBinding(textboxProductStock, TextBox.TextProperty, _bindingProductStock);
			if (!BindingOperations.IsDataBound(textboxProductStock, TextBox.IsEnabledProperty))
				BindingOperations.SetBinding(textboxProductStock, TextBox.IsEnabledProperty, _bindingIsEnabled);

			if (!BindingOperations.IsDataBound(checkboxProductVisible, CheckBox.IsCheckedProperty))
				BindingOperations.SetBinding(checkboxProductVisible, CheckBox.IsCheckedProperty, _bindingProductVisible);
			if (!BindingOperations.IsDataBound(checkboxProductVisible, CheckBox.IsEnabledProperty))
				BindingOperations.SetBinding(checkboxProductVisible, CheckBox.IsEnabledProperty, _bindingIsEnabled);
		}

		private void buttonCreateNewProduct_Click(object sender, RoutedEventArgs e)
		{
			listItems.SelectedIndex = -1;
			buttonCreateNewProduct.IsEnabled = false;
			buttonSaveNewProduct.IsEnabled = true;

			textboxProductId.IsEnabled = true;
			textboxProductName.IsEnabled = true;
			textboxProductSub.IsEnabled = true;
			textboxProductPrice.IsEnabled = true;
			textboxProductStock.IsEnabled = true;
			checkboxProductVisible.IsEnabled = true;

			textboxProductId.Text = "";
			textboxProductName.Text = "";
			textboxProductSub.Text = "";
		}

		private void buttonSaveChanges_Click(object sender, RoutedEventArgs e)
		{
			Item item = listItems.SelectedItem as Item;
			item.ProductID = textboxProductId.Text;
			item.Name = textboxProductName.Text;
			item.Sub = textboxProductSub.Text;
			long price = 0;
			int stock = 0;
			if (long.TryParse(textboxProductPrice.Text, out price))
				item.Price = price;
			if (int.TryParse(textboxProductStock.Text, out stock))
				item.Stock = stock;
			item.Visible = (bool)checkboxProductVisible.IsChecked;
			item.SaveChanges();

			listItems.SelectedIndex = -1;
		}

		private void buttonSaveNewProduct_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(textboxProductId.Text))
			{
				Storyboard gridUnpaidAnimate = (Storyboard)stackpanelProductInfo.FindResource("errorEmptyId");
				gridUnpaidAnimate.Begin(this); 
			}
			if (string.IsNullOrEmpty(textboxProductName.Text))
			{
				Storyboard gridUnpaidAnimate = (Storyboard)stackpanelProductInfo.FindResource("errorEmptyName");
				gridUnpaidAnimate.Begin(this);
			}
			if (string.IsNullOrEmpty(textboxProductPrice.Text))
			{
				Storyboard gridUnpaidAnimate = (Storyboard)stackpanelProductInfo.FindResource("errorEmptyPrice");
				gridUnpaidAnimate.Begin(this);
			}
			if (string.IsNullOrEmpty(textboxProductId.Text) || string.IsNullOrEmpty(textboxProductName.Text) || string.IsNullOrEmpty(textboxProductPrice.Text))
			{
				return;
			}
			Item item = new Item();
			item.ProductID = textboxProductId.Text;
			item.Name = textboxProductName.Text;
			item.Sub = textboxProductSub.Text;
			long price = 0;
			int stock = 0;
			if (long.TryParse(textboxProductPrice.Text, out price))
				item.Price = price;
			if (int.TryParse(textboxProductStock.Text, out stock))
				item.Stock = stock;
			item.Visible = (bool)checkboxProductVisible.IsChecked;
			(treeCategories.SelectedItem as Category).Items.Add(item, true, (treeCategories.SelectedItem as Category).ID);

			listItems.SelectedIndex = -1;
			listItems_SelectionChanged(null, null);
		}

		private void frameRemove_Loaded(object sender, RoutedEventArgs e)
		{
			if (!(sender as Frame).HasContent)
			{
				Pages.PopupConfirmateDelete page = new Kisildalur.Pages.PopupConfirmateDelete();
				page.Tag = (sender as Frame);
				page.RemoveItem += new EventHandler(page_RemoveItem);
				page.DataContext = (sender as Frame).DataContext;

				(sender as Frame).Navigate(page);
			}
		}

		void page_RemoveItem(object sender, EventArgs e)
		{
			listItems.SelectedIndex = -1;
		}

		private void buttonRemoveProduct_Click(object sender, RoutedEventArgs e)
		{
			popup.IsOpen = true;
			(frameRemove.Content as Pages.PopupConfirmateDelete).DataContext = listItems.SelectedItem;
		}

		private void buttonExpandProductInfo_Click(object sender, RoutedEventArgs e)
		{
			Storyboard columnOpen = (Storyboard)gridProductInfo.FindResource("gridOpenAnimation"); // (Storyboard)FindResource("sizePopupClose");
			columnOpen.Completed += new EventHandler(columnOpen_Completed);
			columnOpen.Begin(this);
		}

		void columnOpen_Completed(object sender, EventArgs e)
		{
			if (!BindingOperations.IsDataBound(gridProductInfo, Grid.HeightProperty))
				BindingOperations.SetBinding(gridProductInfo, Grid.HeightProperty, _bindingInfoHeight);
		}

		private void SubItemCount_TextChanged(object sender, TextChangedEventArgs e)
		{
			(((sender as TextBox).Parent as Grid).Children[1] as Button).IsEnabled = true;
		}

		private void buttonSubItemCountSave_Click(object sender, RoutedEventArgs e)
		{
			double count = 1;
			if (double.TryParse((((sender as Button).Parent as Grid).Children[0] as TextBox).Text, out count))
				((sender as Button).DataContext as OrderItem).Count = count;

			(((sender as Button).Parent as Grid).Children[0] as TextBox).Text = count.ToString();
			(sender as Button).IsEnabled = false;
		}

		private void buttonCloseSubItemView_Click(object sender, RoutedEventArgs e)
		{
			Storyboard columnClose = (Storyboard)gridProductInfo.FindResource("gridCloseAnimation"); // (Storyboard)FindResource("sizePopupClose");
			columnClose.Completed += new EventHandler(columnOpen_Completed);
			columnClose.Begin(this);

			columnClose.Completed += new EventHandler(columnClose_Completed);
		}

		void columnClose_Completed(object sender, EventArgs e)
		{
			gridProductInfo.Height = 300;
		}

		private void buttonAddSubItem_Click(object sender, RoutedEventArgs e)
		{
			OrderAddItem addItem = new OrderAddItem();
			addItem.OnItemAdd += new OnItemAdd(addItem_OnItemAdd);
			addItem.ShowDialog();
		}

		void addItem_OnItemAdd(object sender, OrderItem h)
		{
			Item item = listItems.SelectedItem as Item;
			foreach (OrderItem subItem in item.SubProducts)
			{
				if (subItem.ItemId == h.ItemId)
				{
					subItem.Count++;
					return;
				}
			}
			item.SubProducts.Add(h, true);
		}

		private void buttonRemoveItem_RemoveItem(object sender, EventArgs e)
		{
			(listItems.SelectedItem as Item).SubProducts.Remove(sender as OrderItem, true);
		}

		private void buttonMoveProduct_Click(object sender, RoutedEventArgs e)
		{
			FormSelectCategory dialogSelectCategory = new FormSelectCategory("Veldu flokkinn sem þú vilt færa vöruna á");
			dialogSelectCategory.ShowDialog();
			(treeCategories.SelectedItem as Category).Items.Move(listItems.SelectedItem as Item, dialogSelectCategory.SelectedCategory.ID);
			listItems.SelectedIndex = -1;
		}

		private void buttonDiscardClose_Click(object sender, RoutedEventArgs e)
		{
			Properties.config.Default.Save();
			this.Close();
		}
	}
}
