using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Database
{
	public enum UserPrivileges { Seller, Admin }

	public class User : INotifyPropertyChanged
	{
		public User() { _id = -1; }
		public User(int id, string name, string password, UserPrivileges privileges)
		{
			_id = id;
			_name = name;
			_hash = password;
			_privileges = privileges;
		}
        public User(int id, string name, string password, string privileges)
        {
            _id = id;
            _name = name;
            _hash = password;

			switch (privileges)
			{
				case "admin":
					_privileges = UserPrivileges.Admin;
					break;
				default:
					_privileges = UserPrivileges.Seller;
					break;
			}
        }

		private int _id;
		private string _hash;
		private string _name;
		private UserPrivileges _privileges;
		public event PropertyChangedEventHandler PropertyChanged;

		public int ID
		{
			get { return _id; }
			set
			{
				_id = value;
			}
		}
		public string Hash
		{
			get { return _hash; }
			set
			{
				_hash = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Hash"));
			}
		}
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Name"));
			}
		}
		public UserPrivileges Privileges
		{
			get { return _privileges; }
			set
			{
				_privileges = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Privileges"));
			}
		}
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}
}
