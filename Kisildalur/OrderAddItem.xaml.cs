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
using System.Windows.Shapes;
using Database;

namespace Kisildalur
{
	public delegate void OnItemAdd(object sender, OrderItem h);

	/// <summary>
	/// Interaction logic for OrderAddItem.xaml
	/// </summary>
	public partial class OrderAddItem : Window
	{
		public OrderAddItem()
		{
			InitializeComponent();

			this.DataContext = Main.DB;
			this.listItems.Items.CurrentChanged += new EventHandler(Items_CurrentChanged);

            _productIdBinding = BindingOperations.GetBinding(itemProductId, TextBox.TextProperty);
            _productNameBinding = BindingOperations.GetBinding(itemProductName, TextBox.TextProperty);
            _productSubBinding = BindingOperations.GetBinding(itemProductSubTitle, TextBox.TextProperty);
            _productPriceBinding = BindingOperations.GetBinding(itemProductPrice, TextBox.TextProperty);

			textboxBarcode.Focus();
		}

        Binding _productIdBinding;
        Binding _productNameBinding;
        Binding _productSubBinding;
        Binding _productPriceBinding;
        public event OnItemAdd OnItemAdd;

		public string Barcode
		{
			get { return textboxBarcode.Text; }
			set { textboxBarcode.Text = value; }
		}

		void Items_CurrentChanged(object sender, EventArgs e)
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

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Kisildalur.Properties.config.Default.addSplitWidth = (int)((sender as GridSplitter).Parent as Grid).ColumnDefinitions[0].Width.Value;
        }

        private void closeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void addItemToOrder_Click(object sender, RoutedEventArgs e)
        {
            Item item;
            if (listItems.SelectedItem != null)
                item = listItems.SelectedItem as Item;
            else
                item = null;

            int itemId = -1;
            if (item != null)
            {
                if (item.ProductID == itemProductId.Text &&
                    item.Name == itemProductName.Text &&
                    item.Sub == itemProductSubTitle.Text &&
                    item.Price.ToString() == itemProductPrice.Text)
                {
					if (textboxBarcode.Text != item.Barcode)
					{
						item.Barcode = textboxBarcode.Text;
						item.SaveChanges();
						textboxBarcode.Text = "";
					}
                    OnItemAdd_Changed(item);
                }
                else
                    OnItemAdd_Changed(new Item(itemId, textboxBarcode.Text, itemProductId.Text, itemProductName.Text, itemProductSubTitle.Text, "", 0, Convert.ToInt64(itemProductPrice.Text), true, false, 0));
            }
            else if (itemProductPrice.Text != "")
				OnItemAdd_Changed(new Item(itemId, textboxBarcode.Text, itemProductId.Text, itemProductName.Text, itemProductSubTitle.Text, "", 0, Convert.ToInt64(itemProductPrice.Text), true, false, 0));

			textboxBarcode.Text = "";
        }

        private void OnItemAdd_Changed(Item item)
        {
            if (OnItemAdd != null)
                OnItemAdd(this, item.ConvertToOrderItem());
        }

		private void listItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			addItemToOrder_Click(null, null);
		}

        private void listItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!BindingOperations.IsDataBound(itemProductId, TextBox.TextProperty))
                BindingOperations.SetBinding(itemProductId, TextBox.TextProperty, _productIdBinding);
            if (!BindingOperations.IsDataBound(itemProductName, TextBox.TextProperty))
                BindingOperations.SetBinding(itemProductName, TextBox.TextProperty, _productNameBinding);
            if (!BindingOperations.IsDataBound(itemProductSubTitle, TextBox.TextProperty))
                BindingOperations.SetBinding(itemProductSubTitle, TextBox.TextProperty, _productSubBinding);
            if (!BindingOperations.IsDataBound(itemProductPrice, TextBox.TextProperty))
                BindingOperations.SetBinding(itemProductPrice, TextBox.TextProperty, _productPriceBinding);
        }

		private void Bardcode_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && textboxBarcode.Text != "")
			{
				foreach (Folder f in Main.DB.Folders)
				{
					foreach (Category c in f.Categories)
					{
						foreach (Item i in c.Items)
						{
							if (i.Barcode == textboxBarcode.Text)
							{
								DependencyObject dObject = (treeCategories.ItemContainerGenerator.ContainerFromItem(f) as TreeViewItem).ItemContainerGenerator.ContainerFromItem(c);
								System.Reflection.MethodInfo selectMethod = typeof(TreeViewItem).GetMethod("Select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
								selectMethod.Invoke(dObject, new object[] { true });

								listItems.SelectedItem = i;
								addItemToOrder_Click(null, null);
								return;
							}
						}
					}
				}
			}
		}
	}
}
