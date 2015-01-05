using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;

namespace Kisildalur
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main2(string[] args)
        {
			/*if (args.Length > 0)
			{
				if (args[0] == "-version")
				{
					Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version.ToString());
					return;
				}
			}
			Updater update = new Updater(new UpdateSettings("http://www.kisildalur.is", "program.rar", true));
			try
			{
				if (update.UpdateAvailable())
				{
					if (MessageBox.Show("Update is available. Do you want to download it now?", "Update available", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						update.DownloadUpdate();
						update.InstallUpdate();
						return;
					}
				}
			}
			catch (Exception e) { MessageBox.Show("Error:\n\n\t" + e.ToString()); }*/
			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}