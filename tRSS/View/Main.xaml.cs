﻿using System;
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
		
		void Window_ContentRendered(object sender, EventArgs e)
		{
			VM.Update();
		}
		
		void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			VM.Data.ResetTimer();
		}
		
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			VM.SaveData();
		}
		
		void Generic_Drop(object sender, DragEventArgs e)
		{
			//http://stackoverflow.com/questions/19936149/drag-and-drop-listboxitems-generically?lq=1
			
			Object Target = ((ListBoxItem)(sender)).DataContext;
			Object Dropped = e.Data.GetData(Target.GetType());
			
			ListBox container = Utils.FindAncestor<ListBox>((DependencyObject)sender);

			int RemoveIndex = container.Items.IndexOf(Dropped);
			int TargetIndex = container.Items.IndexOf(Target);
			
			System.Collections.IList list = (System.Collections.IList)container.ItemsSource;
			
			if (RemoveIndex < TargetIndex)
			{
				list.Insert(TargetIndex + 1, Dropped);
				list.RemoveAt(RemoveIndex);
			}
			else if (list.Count > RemoveIndex)
			{
				list.Insert(TargetIndex, Dropped);
				list.RemoveAt(RemoveIndex + 1);
			}
			
			container.SelectedItem = Dropped;
		}
		
		/// <summary>
		/// Helper function that makes sure all Filter.SearchInFeed properties aren't reset to null by the ComboBox when Feeds are changed
		/// </summary>
		void Feed_Drop(object sender, DragEventArgs e)
		{
			Feed Target = ((ListBoxItem)(sender)).DataContext as Feed;
			Feed Dropped = e.Data.GetData(Target.GetType()) as Feed;
			
			ListBox container = Utils.FindAncestor<ListBox>((DependencyObject)sender);

			int RemoveIndex = container.Items.IndexOf(Dropped);
			int TargetIndex = container.Items.IndexOf(Target);
			
			System.Collections.IList list = (System.Collections.IList)container.ItemsSource;
			
			System.Collections.Generic.List<Filter> HasSelectedTarget = new System.Collections.Generic.List<Filter>();
			System.Collections.Generic.List<Filter> HasSelectedDropped = new System.Collections.Generic.List<Filter>();
			
			foreach (Filter f in VM.Data.Filters)
			{
				if (f.SearchInFeed == Target)
				{
					HasSelectedTarget.Add(f);
				}
				if (f.SearchInFeed == Dropped)
				{
					HasSelectedDropped.Add(f);
				}
			}
			
			if (RemoveIndex < TargetIndex)
			{
				list.Insert(TargetIndex + 1, Dropped);
				list.RemoveAt(RemoveIndex);
			}
			else if (list.Count > RemoveIndex)
			{
				list.Insert(TargetIndex, Dropped);
				list.RemoveAt(RemoveIndex + 1);
			}
			
			foreach (Filter f in HasSelectedTarget)
			{
				f.SearchInFeed = Target;
			}
			foreach (Filter f in HasSelectedDropped)
			{
				f.SearchInFeed = Dropped;
			}
			
			container.SelectedItem = Dropped;
		}
		
		void MouseMove_DragDrop(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && sender is ListBoxItem)
			{
				ListBoxItem draggedItem = (ListBoxItem)sender;
				DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
				//draggedItem.IsSelected = true;
			}
		}
	}
}