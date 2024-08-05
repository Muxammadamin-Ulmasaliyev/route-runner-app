using System.Configuration;
using System.Data;
using System.Windows;

namespace RouteRunner
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			SQLitePCL.Batteries.Init();

			// Your code here
		}
	}

}
