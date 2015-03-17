
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using tRSS.Model;
using tRSS.Utilities;

namespace tRSS.ViewModel
{
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
			if (File.Exists(ObjectBase.SaveLocation(FILENAME)))
			{
				using (Stream stream = File.Open(ObjectBase.SaveLocation(FILENAME), FileMode.Open))
				{
					BinaryFormatter bFormatter = new BinaryFormatter();
					View = bFormatter.Deserialize(stream) as WindowSettings;
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
