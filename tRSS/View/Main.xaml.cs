using System;
using System.Collections.Generic;
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
using tRSS.ViewModel;

namespace tRSS
{
	public partial class Main : Window
	{
		public MainViewModel VM { get; set; }
		
		public Main()
		{
			VM = new MainViewModel();
			DataContext = VM;
			VM.Data.StartTimer();
			
			InitializeComponent();
		}
		
		void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			VM.Data.ResetTimer();
		}
		
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			VM.SaveData();
		}
	}
}