using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Entities
{
	class User
	{
		public User()
		{
		}

		public virtual int Id { get; private set; }
		public virtual string Name { get; set; }
		public virtual string Password { get; set; }
		public virtual UserPrivileges Privileges { get; set; }
	}
}
