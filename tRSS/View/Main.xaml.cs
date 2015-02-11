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
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Main : Window
	{
		public MainViewModel VM { get; set; }
		
		public Main()
		{
			InitializeComponent();
			
			VM = new MainViewModel();
			DataContext = VM;
		}
		
		# region Helpers for renaming in ListBox
		
		private TextBox ActiveTextBox;
		private TextBox InFocusTextBox;
		private bool HasFocused = false;
		
		private void EnableEditing()
		{
			HasFocused = true;
			ActiveTextBox = InFocusTextBox;
			ActiveTextBox.Visibility = Visibility.Visible;
		}
		
		private void DisableEditing()
		{
			HasFocused = false;
			ActiveTextBox.Visibility = Visibility.Collapsed;
		}
		
		void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			InFocusTextBox = (TextBox)((Grid)((TextBlock)sender).Parent).Children[1];
			
			if(HasFocused)
			{
				DisableEditing();
			}
			if(e.ClickCount == 2)
			{
				EnableEditing();
			}
		}
		
		void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter || e.Key == Key.Escape)
			{
				DisableEditing();
			}
		}
		
		void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(HasFocused == true)
			{
				DisableEditing();
			}
		}
		
		void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if(!HasFocused && InFocusTextBox != null && e.Key == Key.F2)
			{
				EnableEditing();
			}
		}
		
		# endregion
	}
}