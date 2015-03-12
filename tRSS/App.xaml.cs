using System;
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
			using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"log.txt"))
			{
				sw.WriteLine(String.Format("[{0}] {1}", DateTime.Now.ToString("g"), e.ToString()));
			}
			e.Handled = true;
		}
	}
}