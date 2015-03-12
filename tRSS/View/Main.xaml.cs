using System;
using System.Collections;
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
using tRSS.Utilities;

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
		
		void ListBoxItem_Drop(object sender, DragEventArgs e)
		{
			//http://stackoverflow.com/questions/19936149/drag-and-drop-listboxitems-generically?lq=1
			
			Object Target = ((ListBoxItem)(sender)).DataContext;
			Object Dropped = e.Data.GetData(Target.GetType());
			
			ListBox container = Utils.FindAncestor<ListBox>((DependencyObject)sender);

			int RemoveIndex = container.Items.IndexOf(Dropped);
			int TargetIndex = container.Items.IndexOf(Target);
			
			IList list = (IList)container.ItemsSource;
			
			if (RemoveIndex < TargetIndex)
			{
				list.Insert(TargetIndex + 1, Dropped);
				list.RemoveAt(RemoveIndex);
				container.SelectedIndex = TargetIndex;
			}
			else if (list.Count > RemoveIndex)
			{
				list.Insert(TargetIndex, Dropped);
				list.RemoveAt(RemoveIndex + 1);
				container.SelectedIndex = TargetIndex;
			}

		}
		
		void ListBoxItem_PreviewMouseMoveEvent(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && sender is ListBoxItem)
			{
				ListBoxItem draggedItem = (ListBoxItem)sender;
				DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
				draggedItem.IsSelected = true;
			}
		}
	}
}