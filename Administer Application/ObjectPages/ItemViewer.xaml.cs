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
    /// Interaction logic for ItemViewer.xaml
    /// </summary>
    public partial class ItemViewer : Page
    {
        public ItemViewer()
        {
            InitializeComponent();
			_folderView = new FolderCollectionPage(MainWindow.DB.Folders);
			_itemView = new CategoryPage();
			_folderView.PropertyChanged += new PropertyChangedEventHandler(_folderView_PropertyChanged);
            folderFrame.Navigate(_folderView);
			frameItems.Navigate(_itemView);
        }

		void _folderView_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_folderView.SelectedItem is Category)
			{
				_itemView.Category = (Category)_folderView.SelectedItem;
			}
			else if (_folderView.SelectedItem is Folder)
			{

			}
		}

		ObjectPages.FolderCollectionPage _folderView;
		ObjectPages.CategoryPage _itemView;

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Administer_Application.Properties.config.Default.itemViewerSplit = (int)((sender as GridSplitter).Parent as Grid).ColumnDefinitions[0].Width.Value;
        }


    }
}
