
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
		
		public EditFeed(Feed feed)
		{
			
			if (File.Exists(EditFeedViewModel.SaveLocation(EditFeedViewModel.FILENAME)))
			{
				using (Stream stream = File.Open(EditFeedViewModel.SaveLocation(EditFeedViewModel.FILENAME), FileMode.Open))
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
		
		void Save_Button(object sender, RoutedEventArgs e)
		{
			VM.Feed.FinalizeEdit();
			VM.Save(EditFeedViewModel.FILENAME);
			this.Close();
		}
		
		void Cancel_Button(object sender, RoutedEventArgs e)
		{
			VM.Save(EditFeedViewModel.FILENAME);
			this.Close();
		}
		
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			VM.Save(EditFeedViewModel.FILENAME);
		}
		
		#endregion
	}
}