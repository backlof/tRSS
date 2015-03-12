using System;
using System.IO;
using System.Collections.Generic;

using System.Linq;

namespace tRSS.Utilities
{
	/// <summary>
	/// Description of Path.
	/// </summary>
	public static class Paths
	{		
		public static string MakeRelativeDirectory(string path)
		{
			return MakeRelativeDirectory(AppDomain.CurrentDomain.BaseDirectory, path);
		}
		
		public static string MakeRelativeDirectory(string fromPath, string toPath)
		{
			if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
			if (String.IsNullOrEmpty(toPath))   throw new ArgumentNullException("toPath");
			
			Uri fromUri = new Uri(fromPath);
			Uri toUri = new Uri(toPath);
			
			if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.
			if (fromUri.Equals(toUri)) { return "."; }
			
			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			return Uri.UnescapeDataString(relativeUri.ToString()) + Path.DirectorySeparatorChar;;
		}
		
		public static string MakeRelativeFile(string path)
		{
			return MakeRelativeFile(AppDomain.CurrentDomain.BaseDirectory, path);
		}
		
		public static string MakeRelativeFile(string fromPath, string toPath)
		{
			if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
			if (String.IsNullOrEmpty(toPath))   throw new ArgumentNullException("toPath");
			
			Uri fromUri = new Uri(fromPath);
			Uri toUri = new Uri(toPath);
			
			if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.
			if (fromUri.Equals(toUri)) { return Path.DirectorySeparatorChar.ToString(); }
			
			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			return Uri.UnescapeDataString(relativeUri.ToString());
		}
		
		public static string CleanFileName(string fileName)
		{
			var invalidChars = Path.GetInvalidFileNameChars();
			return new string(fileName.Where(x => !invalidChars.Contains(x)).ToArray() );
		}
		
		
	}
}
