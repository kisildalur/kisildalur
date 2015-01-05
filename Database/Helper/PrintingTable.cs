using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;

namespace Database
{
    public class PrintingTable : List<PrintingTableEntry>
    {
        public PrintingTable(float headerStart, params PrintingTableHeader[] columnNames)
            : base()
        {
            _headerStart = headerStart;
            _columnNames = columnNames;
        }

        float _headerStart;
        PrintingTableHeader[] _columnNames;

        public float HeaderLocationY
        {
            get { return _headerStart; }
        }

        public PrintingTableHeader[] PrintingHeaders
        {
            get { return _columnNames; }
        }
    }

    public class PrintingTableHeader
    {
        public PrintingTableHeader(string name, float left, float width, StringFormat stringFormat)
        {
            _name = name;
            _left = left;
            _width = width;
            _stringFormat = stringFormat;
        }

        string _name;
        float _left;
        float _width;
        StringFormat _stringFormat;

        public string Name
        {
            get { return _name; }
        }

        public float Left
        {
            get { return _left; }
        }

        public float Width
        {
            get { return _width; }
        }

        public StringFormat StringFormat
        {
            get { return _stringFormat; }
        }
    }

    public class PrintingTableEntry
    {
        public PrintingTableEntry(params string[] values)
            : this(new Font("Arial", 10), new StringFormat(), values)
        {
        }

        public PrintingTableEntry(Font font, params string[] values)
            : this(font, new StringFormat(), values)
        {
        }

        public PrintingTableEntry(StringFormat alignment, params string[] values)
            : this(new Font("Arial", 10), alignment, values)
        {
        }

        public PrintingTableEntry(Font font, StringFormat alignment, params string[] values)
        {
            _font = font;
            _alignment = alignment;
            _values = new PrintingTableEntryData[values.Length];
            for (int index = 0; index < values.Length; index++)
                _values[index] = new PrintingTableEntryData(values[index]);
        }

        public PrintingTableEntry(params PrintingTableEntryData[] values)
            : this(new Font("Arial", 10), new StringFormat(), values)
        {
        }

        public PrintingTableEntry(Font font, params PrintingTableEntryData[] values)
            : this(font, new StringFormat(), values)
        {
        }

        public PrintingTableEntry(StringFormat alignment, params PrintingTableEntryData[] values)
            : this(new Font("Arial", 10), alignment, values)
        {
        }

        public PrintingTableEntry(Font font, StringFormat alignment, params PrintingTableEntryData[] values)
        {
            _font = font;
            _alignment = alignment;
            _values = values;
        }



        Font _font;
        StringFormat _alignment;
        PrintingTableEntryData[] _values;

        public bool ContainsImage()
        {
            foreach (PrintingTableEntryData data in _values)
                if (data.Type == DataType.Image)
                    return true;
            return false;
        }

        public int EntryHeight()
        {
            int countNewLines = 0;
            foreach (PrintingTableEntryData data in _values)
                if (data.Type == DataType.String)
                    if (countNewLines < data.DataString.Split('\n').Length)
                        countNewLines = data.DataString.Split('\n').Length;
            int height = Convert.ToInt32(_font.Height * (countNewLines));
            foreach (PrintingTableEntryData data in _values)
                if (data.Type == DataType.Image)
                    if (height < data.DataImage.Height)
                        height = data.DataImage.Height;

            return height;
        }

        public Font Font
        {
            get { return _font; }
        }

        public StringFormat StringFormat
        {
            get { return _alignment; }
        }

        public PrintingTableEntryData[] Values
        {
            get { return _values; }
        }
    }

    public class PrintingTableEntryData
    {
        public PrintingTableEntryData(string value)
			: this(value, 1)
        {
        }

		public PrintingTableEntryData(string value, int columnSpan)
		{
			_dataString = value;
			_type = DataType.String;
			_columnSpan = columnSpan;
		}

        public PrintingTableEntryData(Image value)
        {
            _dataImage = new Bitmap(value);
            _type = DataType.Image;
        }

        string _dataString;
        Bitmap _dataImage;
        DataType _type;
		int _columnSpan;

        public string DataString
        {
            get { return _dataString; }
        }

        public Bitmap DataImage
        {
            get { return _dataImage; }
        }

        public DataType Type
        {
            get { return _type; }
        }

		public int ColumnSpan
		{
			get { return _columnSpan; }
		}
    }

    public enum DataType { String, Image}
}
