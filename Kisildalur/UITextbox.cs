using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


public partial class UITextbox : Form
{
    public UITextbox(string text, string header, string button)
    {
        InitializeComponent();
        this.CenterToScreen();
        this._label.Text = text;
        this.Text = header;
        this._button.Text = button;
        Input = "";
    }

    public string Input;

    private void _button_Click(object sender, EventArgs e)
    {
        if (this._textbox.Text != "")
        {
            Input = _textbox.Text;
            this.Close();
        }
        else
            MessageBox.Show("Please Insert a value");
    }
}