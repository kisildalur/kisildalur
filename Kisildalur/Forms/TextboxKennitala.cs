using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Kisildalur.Forms
{
	public partial class TextboxKennitala : UserControl
	{
		public TextboxKennitala()
		{
			InitializeComponent();
		}

		private void _input_KeyPress(object sender, KeyPressEventArgs e)
		{
			string s = e.KeyChar.ToString();
			switch (s)
			{
				case "0":
					e.Handled = CheckLength();
					break;
				case "1":
					e.Handled = CheckLength();
					break;
				case "2":
					e.Handled = CheckLength();
					break;
				case "3":
					e.Handled = CheckLength();
					break;
				case "4":
					e.Handled = CheckLength();
					break;
				case "5":
					e.Handled = CheckLength();
					break;
				case "6":
					e.Handled = CheckLength();
					break;
				case "7":
					e.Handled = CheckLength();
					break;
				case "8":
					e.Handled = CheckLength();
					break;
				case "9":
					e.Handled = CheckLength();
					break;
				case "-":
					if (_input.Text.Length != 6)
						e.Handled = true;
					break;

				default:
					if (s != "\b" && s != "\t")
						e.Handled = true;
					if (s == "\b" && _input.Text.Length == 8)
					{
						int t = _input.SelectionStart;
						_input.Text = _input.Text.Remove(7);
						_input.SelectionStart = t - 1;
					}																																	
					break;
			}
		}

		private bool CheckLength()
		{
			if (_input.Text.Length == 6)
			{
				_input.Text += "-";
				_input.SelectionStart = _input.Text.Length;
			}
			else if (_input.Text.Length == 11)
				return true;
			return false;
		}

		public override string Text
		{
			get { return _input.Text; }
			set { _input.Text = value; }
		}
	}
}
