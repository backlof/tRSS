
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using tRSS.Model;
using tRSS.ViewModel;

namespace tRSS.View
{
	public partial class EditFeed : Window
	{		
		public EditFeedViewModel VM { get; set; }		
		public static readonly string FILENAME = "EditFeed";
		
		public EditFeed(Feed feed)
		{
			
			if (File.Exists(Utilities.ObjectBase.SaveLocation(FILENAME)))
			{
				using (Stream stream = File.Open(Utilities.ObjectBase.SaveLocation(FILENAME), FileMode.Open))
				{
					BinaryFormatter bFormatter = new BinaryFormatter();
					VM = bFormatter.Deserialize(stream) as EditFeedViewModel;
				}
			}
			else
			{
				VM = new EditFeedViewModel();
			}
			
			VM.Feed = feed;
			VM.Feed.EditTitle = VM.Feed.Title;
			VM.Feed.EditURL = VM.Feed.URL;
			
			DataContext = VM;
			
			InitializeComponent();
			this.SizeToContent = SizeToContent.Height;
		}
		
		#region EVENTS
		
		void Save(object sender, RoutedEventArgs e)
		{
			VM.Feed.FinalizeEdit();
			VM.Save(FILENAME);
			this.Close();
		}
		
		void Cancel(object sender, RoutedEventArgs e)
		{
			VM.Save(FILENAME);
			this.Close();
		}
		
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			VM.Save(FILENAME);
		}
		
		#endregion
	}
}