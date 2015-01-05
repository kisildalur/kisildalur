using System;
using System.Windows.Forms;
using System.Collections;

/// <summary>
/// Sorts ListView according to custom columns Comparers.
/// </summary>
public class ListViewSorter : IComparer
{
    /// <summary>
    /// Columns Comparers.
    /// </summary>.
    protected Comparer[] comparers;

    /// <summary>
    /// Current column index.
    /// </summary>
    protected int index;

    /// <summary>
    /// Parent ListView.
    /// </summary>
    protected ListView list;

    /// <summary>
    /// Creates new instance.
    /// </summary>
    public ListViewSorter() : this(null)
    {
    }

    /// <summary>
    /// Creates new instance.
    /// </summary>
    public ListViewSorter(ListView list) : this(list, list != null ? new Comparer[list.Columns.Count] : null)
    {
    }

    /// <summary>
    /// Creates new instance.
    /// </summary>
    public ListViewSorter(ListView list, Comparer[] comparers)
    {
        this.List = list;
       	this.Comparers = comparers;
    }

    /// <summary>
    /// Parent ListView.
    /// </summary>
    public ListView List
    {
        get { return this.list; }
        set
        {
            if (this.list == value) return;

			// unwire old
            if (this.list != null)
            {
                this.list.ColumnClick -= new ColumnClickEventHandler(this.Column_Click);
                this.list.ListViewItemSorter = null;
            }

			// setup data
            this.list = value;
        	this.Comparers = this.list != null ? 
        		new Comparer[this.list.Columns.Count] : 
        		null;

			// wire new
            if (this.list != null)
            {
                this.list.Sorting = SortOrder.None;
                this.list.ColumnClick += new ColumnClickEventHandler(this.Column_Click);
                this.list.ListViewItemSorter = this;
            }
        }
    } // List

    /// <summary>
    /// Column Comparer.
    /// </summary>
    public Comparer this[ColumnHeader column]
    {
    	get {return this[column.Index];}
    	set {this[column.Index] = value;}
    }

    /// <summary>
    /// Column Comparer.
    /// </summary>
    public Comparer this[int index]
    {
    	get {return this.comparers[index];}
    	set {this.comparers[index] = value;}
    }

    /// <summary>
    /// Columns Comparers.
    /// </summary>
    public Comparer[] Comparers
    {
    	get {return this.comparers;}
    	set
        {
        	// verify value
            if (this.list != null)
            {
                if (this.list.Columns.Count != value.Length)
                    throw new Exception("Invalid value.");

                // setup data
                this.comparers = value;
                this.index = -1;
            }
        }
    } // Comparers

    /// <summary>
    /// Sorts list by column.
    /// </summary>
    public void SortBy(ColumnHeader column, bool asc)
    {
    	SortBy(column.Index, asc);
    }
    
    /// <summary>
    /// Sorts list by column.
    /// </summary>
    public void SortBy(int index, bool asc)
    {
        if (this[index] == null) return;

        this.index = index;
        this.list.Sorting = SortOrder.None;
        this.list.Sorting = asc ? 
        	SortOrder.Ascending : 
        	SortOrder.Descending;
    }

    /// <summary>
    /// Compares two items.
    /// </summary>
    
    
    int IComparer.Compare(object x, object y)
    {
    	// verify data
        if (this.list == null || 
            this.list.Sorting == SortOrder.None || 
            this.index < 0) return 0;

		// verify arguments
        ListViewItem ix = x as ListViewItem;
        ListViewItem iy = y as ListViewItem;
        if (ix.ListView != this.list || 
            iy.ListView != this.list)
            throw new Exception("Invalid arguments.");

		// compare
        int ret = 
            (this.list.Sorting==SortOrder.Ascending ? 1 : -1) *
            this[this.index](ix.SubItems[this.index].Text, iy.SubItems[this.index].Text);
        return ret;
    } // Compare()

    // Handles sorting request.
    protected void Column_Click(object sender, ColumnClickEventArgs e)
    {
    	// verify arguments
        ListView list = sender as ListView;
        if (list != this.list)
        	throw new Exception("Invalid arguments.");

        
		// sort
		SortBy(e.Column, this.index != e.Column || this.list.Sorting != SortOrder.Ascending);
        this.list.ListViewItemSorter = this;
    } // Column_Click()

    /// <summary>
    /// Basic comparer.
    /// </summary>
    public delegate int Comparer(string x, string y);

    /// <summary>
    /// Standard strings comparer.
    /// </summary>
    public static int CompareStrings(string x, string y)
    {
        return x.CompareTo(y);
    }

    /// <summary>
    /// Standard numbers comparer.
    /// </summary>
    public static int CompareNumbers(string x, string y)
    {
        double dx = double.Parse(x);
        double dy = double.Parse(y);
    	
        return dx.CompareTo(dy);
    }

    /// <summary>
    /// Standard dates comparer.
    /// </summary>
    public static int CompareDates(string x, string y)
    {
        DateTime dx = DateTime.Parse(x);
        DateTime dy = DateTime.Parse(y);
    	
        return DateTime.Compare(dx, dy);
    }
}//ListViewSorter
