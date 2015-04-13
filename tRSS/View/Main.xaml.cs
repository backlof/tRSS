using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using tRSS.Model;
using tRSS.ViewModel;
using tRSS.Utilities;
using System.Runtime.Serialization.Formatters.Binary;

namespace tRSS
{
	public partial class Main : Window
	{
		public MainViewModel VM { get; set; }		
		
		public Main()
		{
			if (File.Exists(MainViewModel.SaveLocation(MainViewModel.FILENAME)))
			{
				using (Stream stream = File.Open(MainViewModel.SaveLocation(MainViewModel.FILENAME), FileMode.Open))
				{
					BinaryFormatter bFormatter = new BinaryFormatter();
					VM = bFormatter.Deserialize(stream) as MainViewModel;
				}
			}
			else
			{
				VM = new MainViewModel();
			}
			
			DataContext = VM;
			VM.StartTimer();
			
			InitializeComponent();
		}
		
		#region EVENTS
		
		void Window_ContentRendered(object sender, EventArgs e)
		{
			VM.Update();
		}
		
		void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			VM.ResetTimer();
		}
		
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			VM.Save();
		}
		
		void Window_Activated(object sender, EventArgs e)
		{			
			if (VM.IsNotifying)
			{
				VM.NotifyDeactivate();
			}
			
			MainViewModel.WindowIsActive = true;
		}
		
		void Window_Deactivated(object sender, EventArgs e)
		{
			MainViewModel.WindowIsActive = false;
		}
		
		void GitHub_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://github.com/backlof/tRSS/blob/master/README.md");
		}
		
		void Exit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
		
		void Save_Click(object sender, RoutedEventArgs e)
		{
			VM.Save();
		}
		
		#endregion
	}
}