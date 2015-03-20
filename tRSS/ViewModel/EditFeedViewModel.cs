
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using tRSS.Model;
using tRSS.Utilities;

namespace tRSS.ViewModel
{
	[Serializable]
	public class EditFeedViewModel : ObjectBase
	{
		public static readonly string FILENAME = "EditFeed";
		
		public EditFeedViewModel(){}
		
		[NonSerialized]
		private Feed _Feed;
		public Feed Feed
		{
			get
			{
				return _Feed;
			}
			set
			{
				_Feed = value;
				onPropertyChanged("Feed");
			}
		}
		
		private WindowModel _Window = new WindowModel(){ Width = 250 };
		public WindowModel Window
		{
			get
			{
				return _Window;
			}
			set
			{
				_Window = value;
				onPropertyChanged("Window");
			}
		}
	}
}
