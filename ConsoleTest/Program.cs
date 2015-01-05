using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using Database.Core;
using Database.Entities;

namespace ConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
			List<string[]> list = new List<string[]>();
			list.Add(new string[] { "S", "0N1" });
			list.Add(new string[] { "N", "0N" });
			list.Add(new string[] { "N", "0N1" });
			list.Add(new string[] { "N", "" });
			string o = "01";

			int passed = 0;

			StreamReader r = new StreamReader("ingr5.txt");
			if (Recur(0, "S", list, "0"))
				Console.WriteLine("Passed.");
			while (r.Peek() != -1)
			{
				string s = r.ReadLine();
				if (s == "999" || s == "END")
					continue;
				if (!Recur(0, "S", list, s))
					passed++;
			}
			Console.WriteLine("Finished. {0} failed.", passed);
			Console.ReadLine();
		}

		static bool Recur(int step, string s, List<string[]> rules, string outcome)
		{
			if (s == outcome)
			{
				return true;
			}
			else if (s.Length > outcome.Length + 2)
				return false;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == 0 && outcome[i] == 1 || s[i] == 1 && outcome[i] == 0)
					return false;
				if (s[i] == 'S' || s[i] == 'N')
					break;
			}
			for (int i = 0; i < rules.Count; i++)
			{
				int t = s.IndexOf(rules[i][0]);
				if (t != -1)
					if (Recur(step + 1, s.Remove(t, 1).Insert(t, rules[i][1]), rules, outcome))
						return true;
			}
			return false;
		}
	}
}
