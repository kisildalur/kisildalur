using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Database
{
    public class CustomerCollection : ObservableCollection<Customer>
    {
        public CustomerCollection()
            : base()
        {
        }
    }
}
