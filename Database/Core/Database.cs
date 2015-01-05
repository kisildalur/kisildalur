using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Database.Entities;
using Database.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Database.Core
{
	public class DatabaseFactory
	{
		ISessionFactory _factory;

		public DatabaseFactory()
		{
		}

		public void Load()
		{
			_factory = Fluently.Configure()
				.ExposeConfiguration(cfg => cfg.Properties.Add("use_proxy_validator", "false"))
				.Database(MySQLConfiguration.Standard
				.ConnectionString(x => x.Server("localhost").Username("kisill_2").Database("kisill_2").Password("kisill_2")))
				.Mappings(
					x => x.FluentMappings
						.AddFromAssemblyOf<Database.Mappings.FolderMap>()
				).ExposeConfiguration(BuildSchema)
				.BuildSessionFactory();
		}

		protected void BuildSchema(Configuration config)
		{
			//new SchemaExport(config).Create(false, true);
		}

		public ISession OpenSession()
		{
			return _factory.OpenSession();
		}
	}
}
