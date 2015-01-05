using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public abstract class ClassBase
    {
        public ClassBase()
        {
        }

        public ClassBase(MainDatabase database)
        {
            _database = database;
        }

        protected MainDatabase _database;

        public virtual void SetDatabase(MainDatabase database)
        {
            _database = database;
        }
    }
}
