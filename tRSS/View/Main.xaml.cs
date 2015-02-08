﻿using System;
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
	}
}