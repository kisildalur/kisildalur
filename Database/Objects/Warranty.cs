using System;
using System.Collections.Generic;
using System.Text;


namespace Database
{
	public enum WarrantyType { LifeTime, SpecificYears, Default}

	public class Warranty
	{
		public Warranty()
		{
			_type = WarrantyType.Default;
		}

        public Warranty(string type, int year)
        {
            _year = year;
            switch (type)
            {
                case "Litetime":
                    _type = WarrantyType.LifeTime;
                    break;

                case "SpecificYears":
                    _type = WarrantyType.SpecificYears;
                    break;

                default:
                    _type = WarrantyType.Default;
                    break;
            }
        }

		private WarrantyType _type;
		private int _year;

		public WarrantyType Type
		{
			get { return _type; }
			set { _type = value; }
		}
		public int Years
		{
			get { return _year; }
			set { _year = value; }
		}
	}
}
