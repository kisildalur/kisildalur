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
using System.Windows.Shapes;
using Database;

namespace Kisildalur
{
    /// <summary>
    /// Interaction logic for FormSelectCategory.xaml
    /// </summary>
    public partial class FormSelectCategory : Window
    {
        public FormSelectCategory()
        {
            InitializeComponent();

            this.DataContext = Main.DB;
        }

        public FormSelectCategory(string header)
            : this()
        {
            
        }

        public Category SelectedCategory;

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectedCategory = treeCategories.SelectedItem as Category;
            buttonClose_Click(null, null);
        }
    }
}
