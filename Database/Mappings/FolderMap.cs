using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Database.Entities;

namespace Database.Mappings
{
	public class FolderMap : ClassMap<Database.Entities.Folder>
	{
		public FolderMap()
		{
			Table("folder");
			Id(x => x.Id).Column("id");
			Map(x => x.Name).Column("name");
			Map(x => x.Visible).Column("visible");
		}
	}
}
