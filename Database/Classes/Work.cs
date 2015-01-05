using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
	public enum Status { New, Working, Finished };

	class Work
	{
		public Work()
		{
		}

		private string _kennitala;
		private string _name;
		private string _telephone;
		private string _address;

		private Status _status;

		private string _desc;
		//private Items _items;

		public string Kennitala
		{
			get { return _kennitala; }
			set { _kennitala = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Telephone
		{
			get { return _telephone; }
			set { _telephone = value; }
		}


		public Status Status
		{
			get { return _status; }
			set { _status = value; }
		}


		public string Address
		{
			get { return _address; }
			set { _address = value; }
		}

		public string Description
		{
			get { return _desc; }
			set { _desc = value; }
		}
	}
}
