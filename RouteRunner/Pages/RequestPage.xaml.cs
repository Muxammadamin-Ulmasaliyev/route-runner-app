using RouteRunnerLibrary.Models;
using RouteRunnerLibrary;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RouteRunner.Windows;
using RouteRunner.Helpers;
using RouteRunnerLibrary.Services;

namespace RouteRunner.Pages;


public partial class RequestPage : Page
{
	private readonly IApiHelper api = new ApiHelper();

	private SavedRequest _currentRequest;
	private int _currentTabIndex;

	private readonly SavedRequestService _requestService;
	private readonly FolderService _folderService;
	public int GetCurrentRequestId()
	{
		return _currentRequest.Id;
	}
	public string GetCurrentRequestName()
	{
		return _currentRequest.Name;
	}
	public RequestPage(int currentTabIndex, SavedRequest? currentRequest = null)
	{

		_requestService = new(new AppDbContext());
		_folderService = new(new AppDbContext());

		InitializeComponent();
		PopulateHttpVerbsComboBox();

		_currentTabIndex = currentTabIndex;

		_currentRequest = currentRequest;

		if (_currentRequest != null)
		{
			requestNameTextBox.Text = _currentRequest.Name;
			urlTextBox.Text = _currentRequest.Url;
			httpVerbsComboBox.SelectedItem = _currentRequest.HttpVerb;
			saveRequestButton.IsEnabled = _currentRequest.Id == 0;
		}

		PopulateHierarchyOfRequest();

		EventNotificationService.Instance.NewRequestCreatedEvent += NewRequestCreatedEvent;




	}

	private void PopulateHierarchyOfRequest()
	{

		if (_currentRequest.Id != 0)
		{
			var folderId = _currentRequest.FolderId;

			var hierarchy = _folderService.GetHierarchyOfFolder((int)folderId);

			hierarchyTextBlock.Text = string.Join(" / ", hierarchy);
			hierarchyTextBlock.Text += " / ";
		}



	}

	private void NewRequestCreatedEvent(object? sender, (int tabIndex, SavedRequest request) data)
	{
		if (_currentTabIndex == data.tabIndex)
		{

			_currentRequest.Id = data.request.Id;
			_currentRequest.FolderId = data.request.FolderId;
			requestNameTextBox.Text = data.request.Name;

			PopulateHierarchyOfRequest();
		}

	}

	private void PopulateHttpVerbsComboBox()
	{
		httpVerbsComboBox.ItemsSource = new List<HttpVerb>() { HttpVerb.GET, HttpVerb.POST, HttpVerb.PATCH, HttpVerb.PUT, HttpVerb.DELETE, HttpVerb.HEAD };
		httpVerbsComboBox.SelectedIndex = 0;
	}



	// Performing API call
	private async void goButton_Click(object sender, RoutedEventArgs e)
	{
		// Update the UI to show progress bar and status text

		UpdateUIForApiCall();

		if (!api.IsValidUrl(urlTextBox.Text))
		{
			UpdateStatusBarForInvalidUrl();
			return;
		}

		try
		{
			// Perform the API call asynchronously
			HttpResponseInfo responseInfo = await api.CallApiAsync(urlTextBox.Text, (HttpVerb)httpVerbsComboBox.SelectedItem, bodyTextBox.Text);

			if (responseInfo.HttpResponseMessage.IsSuccessStatusCode)
			{
				// Update the UI with results and status
				UpdateUiForSuccess(responseInfo);
			}
			else
			{
				UpdateUiForFailStatusCode(responseInfo);
			}

		}
		catch (Exception ex)
		{
			// Handle exceptions and update UI accordingly
			UpdateUiForError(ex.Message);
		}
	}

	private async Task<string> GetResponseBody(HttpResponseMessage response, bool formatOutput = true)
	{
		string json = await response.Content.ReadAsStringAsync();
		if (formatOutput)
		{
			try
			{
				JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
				json = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
			}
			catch (JsonException ex)
			{
				return json;
			}
		}

		return json;
	}

	private void UpdateUIForApiCall()
	{
		Dispatcher.Invoke(() =>
		{

			responseBodyUpperPanel.Visibility = Visibility.Visible;
			responseTextBlock.Visibility = Visibility.Collapsed;

			responseBodyTextBox.Text = string.Empty;
			responseSizeTextBlock.Text = string.Empty;
			responseStatusCodeTextBlock.Text = string.Empty;
			responseTimeTextBlock.Text = string.Empty;


			statusTextBlock.Foreground = (Brush)statusTextBlock.TryFindResource("SystemFillColorAttentionBrush");

			statusTextBlock.Text = "Calling API . . .";
			progressBar.Visibility = Visibility.Visible;
			progressBar.IsIndeterminate = true;



		});
	}

	private void UpdateStatusBarForInvalidUrl()
	{
		Dispatcher.Invoke(() =>
		{
			statusTextBlock.Foreground = (Brush)statusTextBlock.TryFindResource("SystemFillColorCriticalBrush");
			statusTextBlock.Text = "Invalid URL";
			progressBar.Visibility = Visibility.Collapsed;
			progressBar.IsIndeterminate = false;
		});
	}

	private async void UpdateUiForSuccess(HttpResponseInfo responseInfo)
	{
		var responseBody = await GetResponseBody(responseInfo.HttpResponseMessage);
		var responseStatus = responseInfo.HttpResponseMessage.StatusCode.ToString();
		var responseStatusCode = (int)responseInfo.HttpResponseMessage.StatusCode;
		ResponseSize responseSize = responseInfo.ResponseSize;
		ResponseTime responseTime = responseInfo.ResponseTime;
		Dispatcher.Invoke(() =>
		{
			responseStatusCodeTextBlock.Text = $"Status: {responseStatusCode} {responseStatus}";
			//responseStatusCodeTextBlock.Foreground = Application.Current.Resources["SuccessBrush"] as SolidColorBrush;
			responseStatusCodeTextBlock.Foreground = (Brush)statusTextBlock.TryFindResource("SystemFillColorSuccessBrush");

			responseTimeTextBlock.Text = $"Time: {responseTime.Amount} {responseTime.Metric}";
			//responseTimeTextBlock.Foreground = Application.Current.Resources["SuccessBrush"] as SolidColorBrush;
			responseTimeTextBlock.Foreground = (Brush)statusTextBlock.TryFindResource("SystemFillColorSuccessBrush");

			responseSizeTextBlock.Text = $"Size: {responseSize.Amount} {responseSize.Metric}";
			//responseSizeTextBlock.Foreground = Application.Current.Resources["SuccessBrush"] as SolidColorBrush;
			responseSizeTextBlock.Foreground = (Brush)statusTextBlock.TryFindResource("SystemFillColorSuccessBrush");



			//responseStatusCodeTextBlock.Foreground = Brushes.Green;
			responseBodyTextBox.Text = responseBody;
			statusTextBlock.Foreground = (Brush)statusTextBlock.TryFindResource("SystemFillColorSuccessBrush");
			statusTextBlock.Text = "Ready";
			progressBar.Visibility = Visibility.Collapsed;
			progressBar.IsIndeterminate = false;
		});
	}
	private async void UpdateUiForFailStatusCode(HttpResponseInfo responseInfo)
	{
		var responseBody = await GetResponseBody(responseInfo.HttpResponseMessage);
		var responseStatus = responseInfo.HttpResponseMessage.StatusCode.ToString();
		var responseStatusCode = (int)responseInfo.HttpResponseMessage.StatusCode;
		ResponseSize responseSize = responseInfo.ResponseSize;
		ResponseTime responseTime = responseInfo.ResponseTime;

		Dispatcher.Invoke(() =>
		{
			responseStatusCodeTextBlock.Text = $"Status: {responseStatusCode} {responseStatus}";
			//responseStatusCodeTextBlock.Foreground = Application.Current.Resources["ErrorBrush"] as SolidColorBrush;
			responseStatusCodeTextBlock.Foreground = (Brush)statusTextBlock.TryFindResource("SystemFillColorCriticalBrush");

			responseTimeTextBlock.Text = $"Time: {responseTime.Amount} {responseTime.Metric}";
			//responseTimeTextBlock.Foreground = Application.Current.Resources["ErrorBrush"] as SolidColorBrush;
			responseTimeTextBlock.Foreground = (Brush)statusTextBlock.TryFindResource("SystemFillColorCriticalBrush");

			responseSizeTextBlock.Text = $"Size: {responseSize.Amount} {responseSize.Metric}";
			//responseSizeTextBlock.Foreground = Application.Current.Resources["ErrorBrush"] as SolidColorBrush;
			responseSizeTextBlock.Foreground = (Brush)statusTextBlock.TryFindResource("SystemFillColorCriticalBrush");


			//responseStatusCodeTextBlock.Foreground = Brushes.Green;
			responseBodyTextBox.Text = responseBody;
			statusTextBlock.Text = "Error";
			statusTextBlock.Foreground = (Brush)statusTextBlock.TryFindResource("SystemFillColorCriticalBrush");
			progressBar.Visibility = Visibility.Collapsed;
			progressBar.IsIndeterminate = false;
		});
	}
	private void UpdateUiForError(string errorMessage)
	{
		Dispatcher.Invoke(() =>
		{
			responseBodyTextBox.Text = "Error: " + errorMessage;
			statusTextBlock.Text = "Error";
			statusTextBlock.Foreground = (Brush)statusTextBlock.TryFindResource("SystemFillColorCriticalBrush");

			progressBar.Visibility = Visibility.Collapsed;
			progressBar.IsIndeterminate = false;
		});
	}

	private void urlTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			goButton_Click(sender, e);
			e.Handled = true;
		}
	}



	private void copyResponseBody_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(responseBodyTextBox.Text);
	}

	private void saveRequestButton_Click(object sender, RoutedEventArgs e)
	{

		_currentRequest.Name = requestNameTextBox.Text;
		_currentRequest.HttpVerb = (HttpVerb)httpVerbsComboBox.SelectedItem;
		_currentRequest.Url = urlTextBox.Text;

		if (_currentRequest.Id == 0 || _currentRequest is null)
		{

			var saveRequestWindow = new SaveRequestWindow(_currentRequest, _currentTabIndex);
			saveRequestWindow.Owner = Application.Current.MainWindow;
			Application.Current.MainWindow.Opacity = 0.4;
			saveRequestWindow.ShowDialog();
			Application.Current.MainWindow.Opacity = 1.0;
		}
		else
		{
			// update saved requst in DB

			var updatedRequest = _requestService.UpdateRequest(_currentRequest);

			EventNotificationService.Instance.ExistingRequestSaved(updatedRequest);
			saveRequestButton.IsEnabled = false;
		}


	}

	private void requestNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{

		EventNotificationService.Instance.RequestNameChanged(_currentTabIndex, requestNameTextBox.Text);
		saveRequestButton.IsEnabled = true;

		//	(tabControl.SelectedItem as TabItem).Header = requestNameTextBox.Text;

	}

	private void requestNameTextBox_LostFocus(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(requestNameTextBox.Text))
		{
			requestNameTextBox.Text = "Untitled request";
			return;
		}




	}

	private void httpVerbsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		saveRequestButton.IsEnabled = true;
	}

	private void urlTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		saveRequestButton.IsEnabled = true;

	}


}

