using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class Folder
    {
        public Folder() : base() { _flokkar = new CategoryCollection(0); }
        public Folder(int id, string name, bool visible)
        {
            _id = id;
            _name = name;
            _visible = visible;
            _flokkar = new CategoryCollection(_id);
        }

        public void Update()
        {
            _flokkar = new CategoryCollection(_id);
        }

		private int _id;
		private string _name;
		private CategoryCollection _flokkar;
        bool _deleted = false;
        bool _visible;

		public string Name { get { return _name; } set { _name = value; } }
		public int ID { get { return _id; } set { _id = value; } }
		public CategoryCollection Categories { get { return _flokkar; } set { _flokkar = value; } }
        public bool Deleted { get { return _deleted; } set { _deleted = value; } }
        public bool Visible { get { return _visible; } set { _visible = value; } }
    }
}