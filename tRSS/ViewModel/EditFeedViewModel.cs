
using System;
using System.IO;
using System.Runtime.Serialization;
using tRSS.Model;
using tRSS.Utilities;

namespace tRSS.ViewModel
{
	/// <summary>
	/// Description of FeedEditViewModel.
	/// </summary>
	public class EditFeedViewModel : ObjectBase
	{
		public Feed Data { get; set; }
		public WindowSettings View { get; set; }
		
		private static readonly string FILENAME = "Window-EditFeed";
		
		public EditFeedViewModel(Feed feed)
		{
			Data = feed;
			Data.EditURL = Data.URL;
			Data.EditTitle = Data.Title;
			this.Load();
		}
		
		public void Load()
		{
			string path;
			
			path = ObjectBase.SaveLocation(FILENAME);			
			if(File.Exists(path))
			{
				using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer dcs = new DataContractSerializer(typeof(WindowSettings));
					View = dcs.ReadObject(fs) as WindowSettings;
				}
			}
			else
			{
				View = new WindowSettings();
			}
		}
		
		public void SaveData()
		{
			View.Save(FILENAME);
		}
	}
}
