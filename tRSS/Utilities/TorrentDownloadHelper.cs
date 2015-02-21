
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace tRSS.Utilities
{
	/// <summary>
	/// Description of TorrentDownloadHelper.
	/// </summary>
	public static class TorrentDownloadHelper
	{
		/*
		public static Task<WebResponse> GetResponseAsync(this WebRequest request)
		{
			return TaskFactory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);
		}*/
		
		
		public static async Task<bool> GetTorrent(string url, string location, string title)
		{
			byte[] result;
			byte[] buffer = new byte[4096];

			WebRequest wr = WebRequest.Create(url);
			wr.ContentType = "application/x-bittorrent";
			
			using (WebResponse response = await wr.GetResponseAsync())
			{

				try
				{
					bool gzip = response.Headers["Content-Encoding"] == "gzip";
					
					var responseStream = gzip ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress) : response.GetResponseStream();

					using (MemoryStream memoryStream = new MemoryStream())
					{
						int count = 0;
						do
						{
							count = responseStream.Read(buffer, 0, buffer.Length);
							memoryStream.Write(buffer, 0, count);
						}
						while (count != 0);

						result = memoryStream.ToArray();
						
						// Can't find original filename
						using (BinaryWriter writer = new BinaryWriter(new FileStream(location + Paths.CleanFileName(title) + ".torrent", FileMode.Create)))
						{
							writer.Write(result);
						}
					}
					
					return true;
				}
				catch (WebException we)
				{
					System.Diagnostics.Debug.WriteLine(we.ToString());
					return false;
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine(location);
					System.Diagnostics.Debug.WriteLine(title);
					System.Diagnostics.Debug.WriteLine(e.ToString());
					return false;
				}
			}
		}
	}
}
