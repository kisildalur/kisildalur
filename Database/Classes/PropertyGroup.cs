using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database
{
	public class PropertyGroup
	{
		public PropertyGroup()
		{
			_properties = new PropertyCollection();
		}

		public PropertyGroup(int id, string name)
			: this()
		{
			_id = id;
			_name = name;
		}

		private int _id;
		private string _name;
		private PropertyCollection _properties;

		public int Id
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public PropertyCollection Properties
		{
			get { return _properties; }
		}
	}
}
