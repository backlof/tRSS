/*
 * Created by SharpDevelop.
 * User: Alexander
 * Date: 07.02.2015
 * Time: 15:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using tRSS.Utilities;

namespace tRSS.Model
{
	/// <summary>
	/// Has lists
	/// Executes business logic so that ViewModels don't have to
	/// Example: Updates
	/// </summary>
	public class Library : INotifyBase
	{
		public Library()
		{
			Test = new Feed("shit");
			onPropertyChanged("Test");
		}
		
		public Feed Test { get; set; }
		public List<Feed> Channels { get; set; }
		public List<Filter> Filters { get; set; }
		
		private int _UpdateInMinutes = 5;	
		public int UpdateInMinutes
		{
			get { return _UpdateInMinutes; }
			set
			{
				if(value < 1)
				{
					UpdateInMinutes = 1;
				}
				else
				{
					_UpdateInMinutes = value;
					onPropertyChanged("UpdateInMinutes");
				}
			}
		}
		
		private string _TorrentDropLocation;		
		public string TorrentDropLocation {
			get { return _TorrentDropLocation; }
			set { _TorrentDropLocation = value; onPropertyChanged("TorrentDropLocation"); }
		}
		
		// ==========================================
		//   MAKE BUSINESS LOGIC FUNCTIONALITY HERE
		// ==========================================
		
		// Update Feeds -> Filter Items
	}
}
