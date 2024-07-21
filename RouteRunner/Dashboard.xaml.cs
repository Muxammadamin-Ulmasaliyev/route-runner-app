using RouteRunnerLibrary;
using System.Windows;
using Wpf.Ui.Controls;

namespace RouteRunner;

public partial class MainWindow : Window
{
	private readonly IApiHelper api = new ApiHelper();

	public MainWindow()
	{
		InitializeComponent();

		Loaded += (sender, args) =>
		{
			Wpf.Ui.Appearance.SystemThemeWatcher.Watch(
				this,                                  // Window class
				WindowBackdropType.Mica, // Background type
				true                                   // Whether to change accents automatically
			);
		};
	}

	private async void goButton_Click(object sender, RoutedEventArgs e)
	{
		// Update the UI to show progress bar and status text
		Dispatcher.Invoke(() =>
		{
			statusTextBlock.Text = "Calling api . . .";
			resultsTextBox.Text = string.Empty;
			progressBar.Visibility = Visibility.Visible;
			progressBar.IsIndeterminate = true;
		});

		if (!api.IsValidUrl(urlTextBox.Text))
		{
			Dispatcher.Invoke(() =>
			{
				statusTextBlock.Text = "Invalid URL";
				progressBar.Visibility = Visibility.Collapsed;
				progressBar.IsIndeterminate = false;
			});
			return;
		}
		try
		{
			// Perform the API call asynchronously
			string result = await api.CallApiAsync(urlTextBox.Text);

			// Update the UI with results and status
			Dispatcher.Invoke(() =>
			{
				resultsTextBox.Text = result;
				statusTextBlock.Text = "Ready";
				progressBar.Visibility = Visibility.Collapsed;
				progressBar.IsIndeterminate = false;
			});
		}
		catch (Exception ex)
		{
			// Handle exceptions and update UI accordingly
			Dispatcher.Invoke(() =>
			{
				resultsTextBox.Text = "Error: " + ex.Message;
				statusTextBlock.Text = "Error";
				progressBar.Visibility = Visibility.Collapsed;
				progressBar.IsIndeterminate = false;
			});
		}
	}


}