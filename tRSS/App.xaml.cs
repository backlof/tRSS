using System;
using System.IO;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;

namespace tRSS
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			using(StreamWriter sw = File.AppendText(@"Error.log"))
			{
				sw.WriteLine(String.Format("[{0}]", DateTime.Now.ToString("g")));
				sw.WriteLine(String.Format("[{EventArgs}] {0}", e.ToString()));
				sw.WriteLine(String.Format("[{Exception}] {0}", e.Exception.ToString()));
				sw.WriteLine(String.Format("[{Dispatcher}] {0}", e.Dispatcher.ToString()));
				sw.WriteLine(String.Format("[{Sender}] {0}", sender.ToString()) + Environment.NewLine);
			}
		}
	}
}