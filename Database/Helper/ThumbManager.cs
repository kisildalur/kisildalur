using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.ComponentModel;

namespace Database
{
	public class ThumbManager
	{
		public ThumbManager(MainDatabase db)
		{
			_db = db;
		}

		private MainDatabase _db;
        private Dictionary<int, Item> _albumMap;

		public void DownloadThumbs(BackgroundWorker worker, ref List<int> updateList)
		{
			WebClient c = new WebClient();
			int index = 0, total = _db.GetTotalNumberOfItems();
			string message = "Checking for thumbnails to download";

			RunPrelimenaryChecks();
            
			foreach (Item item in _db.GetItems())
			{
				int imageIndex = item.GetPrimaryImageIndex();

				worker.ReportProgress(0, new WorkerReportHandler(message, index, total));

				try
				{
					if ((!File.Exists(string.Format("{0}\\thumb_images\\{1}_thumb.jpg", System.Windows.Forms.Application.StartupPath, item.ID)) || updateList.Contains(item.ID)) && imageIndex != -1)
					{
						message = "Downloading images";
						DownloadThumbImages(c, item, imageIndex);
					}
					else if (updateList.Contains(item.ID) && imageIndex == -1)
						DeleteThumbImages(item);
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
