using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data.Odbc;


namespace Database
{
    public class SiteOrderCollection : ObservableCollection<SiteOrder>
    {
        public SiteOrderCollection()
            : base()
        {
        }

        public SiteOrder this[int id, bool searchforid]
        {
            get
            {
                if (searchforid)
                {
                    for (int I = 0; I < base.Count; I++)
                        if (base[I].Id == id)
                            return base[I];
                    return null;
                }
                return base[id];
            }
            set
            {
                if (searchforid)
                {
                    for (int I = 0; I < base.Count; I++)
                        if (base[I].Id == id)
                        {
                            base[I] = value;
                            return;
                        }
                    return;
                }
                base[id] = value;
            }
        }

        public void Update(SiteOrder item)
        {
            try
            {
                MainDatabase.GetDB.Connect();

                int stage = (item.Stage == SiteOrderStage.New ? 1 : (item.Stage == SiteOrderStage.Confirmed ? 2 : 3));

                OdbcCommand command = new OdbcCommand("UPDATE `order` SET stage = " + stage + " WHERE id = " + item.Id, MainDatabase.GetDB.MySQL);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MainDatabase.GetDB.ErrorLog("Error while updating to database", e.Message, e.ToString());
            }
        }
    }
}
