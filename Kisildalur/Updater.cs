using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

/// <summary>
/// Update class to download and check for updates
/// </summary>
class Updater
{
	/// <summary>
	/// Initialise a new instance of Updater
	/// </summary>
	public Updater()
	{
		_settings = new UpdateSettings();
		_downloaded = false;
	}
	
	/// <summary>
	/// Initialise a new instance of updater with specified parameter
	/// </summary>
	/// <param name="settings"></param>
	public Updater(UpdateSettings settings)
	{
		_settings = settings;
		_downloaded = false;
	}
	
	private UpdateSettings _settings;
	private bool _downloaded;
	
	/// <summary>
	/// Get and modifie settings for updater
	/// </summary>
	public UpdateSettings Settings
	{
		get { return _settings; }
	}
	
	/// <summary>
	/// Check whether a new update is available
	/// </summary>
	/// <returns>Whether it contains a update or not</returns>
	public bool UpdateAvailable()
	{
		WebClient c = new WebClient();
		string url = _settings.Host;
		if (!url.EndsWith("/"))
			url += "/";

		Stream s = c.OpenRead(url + "version.txt");
		StreamReader r = new StreamReader(s);

		string version = r.ReadLine();
		System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\\d+[.]\\d+[.]\\d+[.]\\d+");
		if (regex.IsMatch(version))
		{
			string oldVer = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			if (version != oldVer)
				return true;
			return false;
		}
		throw new FormatException("The version file was invalid.");
	}

	/// <summary>
	/// Download the update file to program folder
	/// </summary>
	public void DownloadUpdate()
	{
		if (File.Exists(Application.StartupPath + "\\" + _settings.File))
			File.Delete(Application.StartupPath + "\\" + _settings.File);
		if (File.Exists(Application.StartupPath + "\\" + _settings.File))
			File.Delete(Application.StartupPath + "\\" + _settings.File);

		WebClient c = new WebClient();
		string url = _settings.Host;
		if (!url.EndsWith("/"))
			url += "/";

		if (_settings.Zipped)
			c.DownloadFile(url + _settings.File, Application.StartupPath + "\\" + _settings.File);
		else
			c.DownloadFile(url + _settings.File, Application.StartupPath + "\\" + _settings.File);

		_downloaded = true;
	}

	/// <summary>
	/// Install the update file
	/// </summary>
	public void InstallUpdate()
	{
		if (!_downloaded)
		{
			throw new FileNotFoundException("No downloaded file was found. Please make sure you downloaded befour installing");
		}

		StreamWriter w = new StreamWriter(Application.StartupPath + "\\install.bat", false);
		w.WriteLine("@ECHO off");
		w.WriteLine("set path=\"C:\\Program Files\\WinRAR\\\";%path%");
		if (_settings.Zipped)
			w.WriteLine("unrar e -o+ " + _settings.File);
		else
			w.WriteLine("copy " + _settings.File + " " + Application.ExecutablePath + " /y");

		w.WriteLine("del " + _settings.File);
		w.WriteLine("START Kisildalur.exe");
		w.Flush();
		w.Close();

		System.Diagnostics.Process.Start(Application.StartupPath + "\\install.bat");
	}
}

/// <summary>
/// Contains settings and other options for Update objects
/// </summary>
class UpdateSettings
{
	/// <summary>
	/// Initialize a new instance of Update Settings
	/// </summary>
	public UpdateSettings()
	{
	}

	/// <summary>
	/// Initialize a new instance of Update Settings woth specified parameters
	/// </summary>
	/// <param name="host">Complete url of location</param>
	/// <param name="file">Name of file</param>
	/// <param name="zipped"></param>
	public UpdateSettings(string host, string file, bool zipped)
	{
		_host = host;
		_file = file;
		_zipped = zipped;
	}

	private string _host;
	private string _file;
	private bool _zipped;

	/// <summary>
	/// Get or set the url which is hosting the file
	/// </summary>
	public string Host
	{
		get { return _host; }
		set { _host = value; }
	}

	/// <summary>
	/// Get or set the name of file
	/// </summary>
	public string File
	{
		get { return _file; }
		set { _file = value; }
	}

	/// <summary>
	/// Specifie whether specified file is zipped
	/// </summary>
	public bool Zipped
	{
		get { return _zipped; }
		set { _zipped = value; }
	}
}
