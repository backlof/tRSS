
using System;
using System.Collections.Generic;
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
			VM = new EditFeedViewModel(feed);
			DataContext = VM;
			
			InitializeComponent();
			this.SizeToContent = SizeToContent.Height;
		}
		
		void Save(object sender, RoutedEventArgs e)
		{
			VM.Data.FinalizeEdit();
			VM.SaveData();
			this.Close();
		}
		
		void Cancel(object sender, RoutedEventArgs e)
		{
			VM.SaveData();
			this.Close();
		}
		
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			VM.SaveData();
		}
	}
}