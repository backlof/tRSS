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
			using(StreamWriter sw = File.AppendText(@"log.txt"))
			{
				sw.WriteLine(String.Format("[{0}] {1}", DateTime.Now.ToString("g"), e.ToString()));
				sw.WriteLine(String.Format("[{0}] {1}", DateTime.Now.ToString("g"), e.Exception.ToString()));
				sw.WriteLine(String.Format("[{0}] {1}", DateTime.Now.ToString("g"), e.Dispatcher.ToString()));
				sw.WriteLine(String.Format("[{0}] {1}", DateTime.Now.ToString("g"), sender.ToString()));
			}
		}
	}
}