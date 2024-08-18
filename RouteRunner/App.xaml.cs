using Microsoft.EntityFrameworkCore;
using RouteRunnerLibrary;
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

			AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler;


			using (var context = new AppDbContext())
			{
				context.Database.Migrate();
			}

		}
		private void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs e)
			{
			Exception exception = (Exception)e.ExceptionObject;

			// Show error message in a MessageBox
			MessageBox.Show($"An unhandled exception occurred: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

			// Optionally log the exception or perform other actions

		}
	}

}
