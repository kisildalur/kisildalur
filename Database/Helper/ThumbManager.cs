using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Data.Odbc;

namespace Database
{
	public class ThumbManager
	{
		public ThumbManager(MainDatabase db)
		{
			_db = db;
		}

		private MainDatabase _db;

		public void DownloadThumbs(BackgroundWorker worker, ref List<int> updateList)
		{
            Dictionary<int, Item> albumMap = new Dictionary<int, Item>();
            List<int> albums = new List<int>();
			WebClient c = new WebClient();
			int index = 0, total = _db.GetTotalNumberOfItems();
			string message = "Checking for thumbnails to download";

			RunPrelimenaryChecks();
            
			foreach (Item item in _db.GetItems())
			{
                albumMap.Add(item.Album, item);
                albums.Add(item.Album);
            }

            StringBuilder builder = new StringBuilder();

            if (albumMap.Count == 0)
            {
                TidyupAfterWork(worker);
                return;
            }

            builder.Append(albums[0]);
            for (int i = 0; i < albums.Count; i++)
            {
                builder.Append(",").Append(albums[i]);
            }

            _db.Connect();

            try
            {
                OdbcCommand command = new OdbcCommand(string.Format("select id, fk_album from image where fk_album in ({0})", builder.ToString()), MainDatabase.GetDB.MySQL);
                OdbcDataReader results = command.ExecuteReader();

                while (results.Read())
                {
                    int imageIndex = results.GetInt32(0);
                    int album = results.GetInt32(1);

                    worker.ReportProgress(0, new WorkerReportHandler(message, index, total));

                    try
                    {
                        if ((!File.Exists(string.Format("{0}\\thumb_images\\{1}_thumb.jpg", System.Windows.Forms.Application.StartupPath, albumMap[album].ID)) || updateList.Contains(albumMap[album].ID)))
                        {
                            message = "Downloading images";
                            DownloadThumbImages(c, albumMap[album], imageIndex);
                        }
                    }
                    catch (WebException error)
                    {
                        if (error.Status == WebExceptionStatus.NameResolutionFailure)
                        {
                            TidyupAfterWork(worker);
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        //DB.ErrorLog(string.Format("Error while downloading image 'http://kisildalur.is/web/uploads/images/{0}_thumb.jpg' for {1}", imageIndex, item.Name), error.Message, error.ToString());
                    }

                    index++;
                }
            }
            catch (Exception e)
            {
                _db.ErrorLog("Error while retreaving id for the primary image", e.Message, e.ToString());
            }
			

			TidyupAfterWork(worker);
		}

		private void DownloadThumbImages(WebClient c, Item item, int imageIndex)
		{
			c.DownloadFile(string.Format("http://kisildalur.is/web/uploads/images/{0}_thumb.jpg", imageIndex), string.Format("{0}\\thumb_images\\{1}_thumb.jpg", System.Windows.Forms.Application.StartupPath, item.ID));
			c.DownloadFile(string.Format("http://kisildalur.is/web/uploads/images/{0}_small.jpg", imageIndex), string.Format("{0}\\thumb_images\\{1}_small.jpg", System.Windows.Forms.Application.StartupPath, item.ID));
		}

		private void DeleteThumbImages(Item item)
		{
			File.Delete(string.Format("{0}\\thumb_images\\{1}_thumb.jpg", System.Windows.Forms.Application.StartupPath, item.ID));
			File.Delete(string.Format("{0}\\thumb_images\\{1}_small.jpg", System.Windows.Forms.Application.StartupPath, item.ID));
		}

		private void RunPrelimenaryChecks()
		{
			try
			{
				if (!Directory.Exists(string.Format("{0}\\thumb_images", System.Windows.Forms.Application.StartupPath)))
					Directory.CreateDirectory(string.Format("{0}\\thumb_images", System.Windows.Forms.Application.StartupPath));
			}
			catch (Exception e)
			{
				_db.ErrorLog(string.Format("Error while creating directory: {0}\\thumb_images\n\nPlease create this directory manually or run this program as Administrator.", System.Windows.Forms.Application.StartupPath), e.Message, e.ToString());
			}

			_db.Connect();
		}

		private void TidyupAfterWork(BackgroundWorker worker)
		{
			_db.Disconnect();
			worker.ReportProgress(0, new WorkerReportHandler("Idle", 0, 100));
		}
	}
}
