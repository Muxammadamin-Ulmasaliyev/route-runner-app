﻿using RouteRunnerLibrary.Models;
using RouteRunnerLibrary;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FontAwesome5;
using Wpf.Ui.Appearance;

namespace RouteRunner.Pages;


public partial class RequestPage : Page
{
	private readonly IApiHelper api = new ApiHelper();
	public RequestPage()
	{
		InitializeComponent();
		PopulateHttpVerbsComboBox();

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

			statusTextBlock.Text = "Calling API . . .";
			progressBar.Visibility = Visibility.Visible;
			progressBar.IsIndeterminate = true;



		});
	}

	private void UpdateStatusBarForInvalidUrl()
	{
		Dispatcher.Invoke(() =>
		{
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
			responseStatusCodeTextBlock.Foreground = Application.Current.Resources["SuccessBrush"] as SolidColorBrush;
			responseTimeTextBlock.Text = $"Time: {responseTime.Amount} {responseTime.Metric}";
			responseTimeTextBlock.Foreground = Application.Current.Resources["SuccessBrush"] as SolidColorBrush;
			responseSizeTextBlock.Text = $"Size: {responseSize.Amount} {responseSize.Metric}";
			responseSizeTextBlock.Foreground = Application.Current.Resources["SuccessBrush"] as SolidColorBrush;


			//responseStatusCodeTextBlock.Foreground = Brushes.Green;
			responseBodyTextBox.Text = responseBody;
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
			responseStatusCodeTextBlock.Foreground = Application.Current.Resources["ErrorBrush"] as SolidColorBrush;
			responseTimeTextBlock.Text = $"Time: {responseTime.Amount} {responseTime.Metric}";
			responseTimeTextBlock.Foreground = Application.Current.Resources["ErrorBrush"] as SolidColorBrush;
			responseSizeTextBlock.Text = $"Size: {responseSize.Amount} {responseSize.Metric}";
			responseSizeTextBlock.Foreground = Application.Current.Resources["ErrorBrush"] as SolidColorBrush;

			//responseStatusCodeTextBlock.Foreground = Brushes.Green;
			responseBodyTextBox.Text = responseBody;
			statusTextBlock.Text = "Error";
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

	
}