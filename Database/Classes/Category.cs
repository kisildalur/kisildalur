using System;
using System.Collections.Generic;
using System.Text;


namespace Database
{
	public class Category
	{
		public Category()
		{
			_items = new ItemCollection();
			_propertyGroups = new PropertyGroupCollection();
		}
        public Category(int id, string name, bool visible)
			: this()
        {
            _id = id;
            _name = name;
            _visible = visible;
        }

		private int _id;
		private string _name;
		private ItemCollection _items;
		private PropertyGroupCollection _propertyGroups;
        private bool _deleted = false;
        private bool _visible;
		private int _albumId;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public int ID
		{
			get { return _id; }
			set { _id = value; }
		}

		public ItemCollection Items
		{
			get { return _items; }
			set { _items = value; }
		}

		public PropertyGroupCollection PropertyGroups
		{
			get { return _propertyGroups; }
		}

		public bool Deleted
		{
			get { return _deleted; }
			set { _deleted = value; }
		}

        public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		public int AlbumId
		{
			get { return _albumId; }
			set { _albumId = value; }
		}
	}
}
