using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Kisildalur
{
    public partial class Main_Login : Form
    {
        public Main_Login()
        {
            InitializeComponent();
            this.CenterToScreen();
			this.Size = Properties.config.Default.lSize;

			this.Size = Properties.config.Default.lSize;
        }

        private void _enter_Click(object sender, EventArgs e)
        {
            this.Close();
        }

		private void Login_SizeChanged(object sender, EventArgs e)
		{
			Properties.config.Default.lSize = this.Size;
		}
    }
}