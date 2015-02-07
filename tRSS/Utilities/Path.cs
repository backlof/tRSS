using System;

namespace tRSS.Utilities
{
	/// <summary>
	/// Description of Path.
	/// </summary>
	public static class Path
	{
		public static string MakeRelativePath(string fromPath, string toPath)
		{
			if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
			if (String.IsNullOrEmpty(toPath))   throw new ArgumentNullException("toPath");
			
			Uri fromUri = new Uri(fromPath);
			Uri toUri = new Uri(toPath);
			
			if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.
			
			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			return Uri.UnescapeDataString(relativeUri.ToString());
		}
		
		public static string MakeRelativePath(string path)
		{
			return MakeRelativePath(AppDomain.CurrentDomain.BaseDirectory, path);
		}
	}
}
