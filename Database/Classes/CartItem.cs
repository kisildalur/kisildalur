using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class CartItem
    {
        public CartItem() { }
        public CartItem(int id, int itemId, string productId, string name, Int64 price, int count)
        {
            _id = id;
            _itemId = itemId;
            _productId = productId;
            _name = name;
            _price = price;
            _count = count;
        }

        int _id;
        int _itemId;
        string _productId;
        string _name;
        Int64 _price;
        int _count;

        public int id { get { return _id; } set { _id = value; } }
        public int ItemId { get { return _itemId; } set { _itemId = value; } }
        public string ProductID { get { return _productId; } set { _productId = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public Int64 Price { get { return _price; } set { _price = value; } }
        public int Count { get { return _count; } set { _count = value; } }
    }
}
