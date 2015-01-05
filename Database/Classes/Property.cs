using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database
{
	public class Property
	{
		public Property()
		{
		}

		public Property(int id, string name, string description)
		{
			_id = id;
			_name = name;
			_description = description;
		}

		private int _id;
		private string _name;
		private string _description;

		public int Id
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}
	}
}
