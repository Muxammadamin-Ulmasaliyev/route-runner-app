using System.Windows;
using System.Windows.Input;
using RouteRunner.Pages;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using RouteRunnerLibrary.Models;
using RouteRunnerLibrary;
using RouteRunner.Helpers;
using RouteRunnerLibrary.Services;
using System.Collections.ObjectModel;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace RouteRunner;

public partial class MainWindow : Window
{

	private readonly FolderService _folderService;
	private bool shiftKeyPressed = false, ctrlKeyPressed = false;
	private ObservableCollection<TabItem> tabItems = new();
	public MainWindow()
	{
		InitializeComponent();
		_folderService = new(new AppDbContext());

		requestsTabControl.ItemsSource = tabItems;
		requestsComboBox.ItemsSource = tabItems;


		EventNotificationService.Instance.RequestNameChangedInTextBoxEvent += HandleRequestNameChangedInTextBoxEvent; ;
		EventNotificationService.Instance.RequestDeletedEvent += HandleRequestDeletedEvent;
		mainFrame.Navigated += (s, e) =>
		{
			if (e.Content is Page page)
			{
				page.SetResourceReference(Page.BackgroundProperty, "DynamicBackgroundBrush");
			}
		};


		mainFrame.Navigate(new BlankRequestPage());



	}


	#region Event Handlers
	private void HandleRequestDeletedEvent(object? sender, Request deletedRequest)
	{
		var tabIndex = IsRequestTabAlreadyOpened(deletedRequest);
		if (tabIndex != -1)
		{
			tabItems.RemoveAt(tabIndex);
		}
	}
	private void CollectionsPage_NewRequestOpened(object? sender, RouteRunnerLibrary.Models.Request request)
	{
		var tabIndex = IsRequestTabAlreadyOpened(request);
		if (tabIndex != -1)
		{
			if (requestsTabControl.SelectedIndex != tabIndex)
			{
				mainFrame.Navigate(requestsTabControl.Items[tabIndex]);
				requestsTabControl.SelectedIndex = tabIndex;
			}
		}
		else
		{
			AddNewRequestToTabView(request);
		}
	}

	private void HistoryPage_NewRequestOpened(object? sender, RouteRunnerLibrary.Models.Request request)
	{
		var tabIndex = IsRequestInHistoryTabAlreadyOpened(request);
		if (tabIndex != -1)
		{
			if (requestsTabControl.SelectedIndex != tabIndex)
			{
				mainFrame.Navigate(requestsTabControl.Items[tabIndex]);
				requestsTabControl.SelectedIndex = tabIndex;
			}
		}
		else
		{
			AddNewRequestToTabView(request);
		}
	}
	#endregion

	private void HandleRequestNameChangedInTextBoxEvent(object? sender, (int tabIndex, string requestName) data)
	{
		if (requestsTabControl.SelectedItem != null)
		{
			(requestsTabControl.Items[data.tabIndex] as TabItem).Header = data.requestName;
		}
	}

	private void requestsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (requestsComboBox.SelectedIndex >= 0)
		{
			mainFrame.Navigate((requestsComboBox.SelectedItem as TabItem).Tag);
			requestsTabControl.SelectedIndex = requestsComboBox.SelectedIndex;
		}
		else
		{
			mainFrame.Navigate(new BlankRequestPage());
		}

	}

	#region Tab Control Events

	private void requestsTabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		if (requestsTabControl.SelectedIndex >= 0)
		{
			mainFrame.Navigate((requestsTabControl.SelectedItem as TabItem).Tag);
			requestsComboBox.SelectedIndex = requestsTabControl.SelectedIndex;
		}
		else
		{
			mainFrame.Navigate(new BlankRequestPage());
		}

	}
	private void closeTabButton_Click(object sender, RoutedEventArgs e)
	{
		if (sender is Wpf.Ui.Controls.Button button && button.Tag is TabItem tabItemToRemove)
		{
			var indexToRemove = requestsTabControl.Items.IndexOf(tabItemToRemove);
			if (indexToRemove > 0)
			{
				requestsTabControl.SelectedIndex = indexToRemove - 1;
				requestsComboBox.SelectedIndex = indexToRemove - 1;
			}
			tabItems.Remove(tabItemToRemove);
		}
	}
	private void addNewRequestButton_Click(object sender, RoutedEventArgs e)
	{
		AddNewRequestToTabView();
	}
	private void AddNewRequestToTabView(Request? request = null)
	{
		TabItem newTabItem;
		if (request == null)
		{
			newTabItem = new TabItem
			{
				Header = "Untitled request",
				Content = null,
			};
		}
		else
		{
			newTabItem = new TabItem
			{
				Header = request.Name,
				Content = null,
			};
		}
		DataTemplate tabHeaderTemplate = (DataTemplate)FindResource("TabHeaderTemplate");
		newTabItem.HeaderTemplate = tabHeaderTemplate;
		tabItems.Add(newTabItem);
		RequestPage requestPage;
		if (request == null)
		{
			requestPage = new RequestPage(requestsTabControl.Items.Count - 1, new Request() { Name = "Untitled request", HttpVerb = HttpVerb.GET, Url = "" });

		}
		else
		{
			requestPage = new RequestPage(requestsTabControl.Items.Count - 1, request);
		}
		tabItems[tabItems.Count - 1].Tag = requestPage;
		requestsTabControl.SelectedIndex = tabItems.Count - 1;
		requestsComboBox.SelectedIndex = tabItems.Count - 1;
		mainFrame.Navigate((requestsTabControl.SelectedItem as TabItem).Tag);
	}

	#endregion








	/// <summary>
	/// Returns the index of the tab if it is already opened, otherwise returns -1
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	private int IsRequestTabAlreadyOpened(Request request)
	{
		for (int i = 0; i < requestsTabControl.Items.Count; i++)
		{
			if (request.Id == ((requestsTabControl.Items[i] as TabItem).Tag as RequestPage).GetCurrentRequestId())
			{
				return i;
			}
		}
		return -1;

	}

	private int IsRequestInHistoryTabAlreadyOpened(Request request)
	{
		for (int i = 0; i < requestsTabControl.Items.Count; i++)
		{
			if (request.Id == ((requestsTabControl.Items[i] as TabItem).Tag as RequestPage).GetCurrentRequestId())
			{
				return i;
			}
		}
		return -1;

	}
	private void sidebarNavigationView_SelectionChanged(NavigationView sender, RoutedEventArgs args)
	{
		var selectedItem = sender.SelectedItem as NavigationViewItem;

		if (selectedItem != null)
		{
			Type selectedPageType = selectedItem.TargetPageType;

			if (selectedPageType == typeof(CollectionsSidebarPage))
			{
				var collectionsPage = new CollectionsSidebarPage();
				collectionsPage.NewRequestOpened += CollectionsPage_NewRequestOpened;
				sender.ReplaceContent(collectionsPage);
			}
			else if (selectedPageType == typeof(EnvironmentsSidebarPage))
			{

			}
			else if (selectedPageType == typeof(HistorySidebarPage))
			{
				var historyPage = new HistorySidebarPage();
				historyPage.NewRequestOpened += HistoryPage_NewRequestOpened;
				sender.ReplaceContent(historyPage);
			}
		}


	}
	private void DynamicScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		if (sender is DynamicScrollViewer scrollViewer)
		{
			double newHorizontalOffset = scrollViewer.HorizontalOffset - e.Delta;

			if (newHorizontalOffset < 0)
			{
				newHorizontalOffset = 0;
			}
			else if (newHorizontalOffset > scrollViewer.ScrollableWidth)
			{
				newHorizontalOffset = scrollViewer.ScrollableWidth;
			}

			scrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);

			e.Handled = true;
		}
	}

	#region Window  Events
	private void Window_StateChangedOrLoaded(object sender, EventArgs e)
	{
		ToggleInfoBarVisibility();
	}
	private void Window_KeyDown(object sender, KeyEventArgs e)
	{

		if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
		{
			shiftKeyPressed = true;
		}
		if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
		{
			ctrlKeyPressed = true;
		}

		if (ctrlKeyPressed && e.Key == Key.N)
		{
			addNewRequestButton_Click(sender, e);
		}

		if (e.Key == Key.F11)
		{

			if (this.WindowState == WindowState.Maximized)
			{
				this.WindowState = WindowState.Normal;
			}
			else
			{
				this.WindowState = WindowState.Maximized;
			}
			e.Handled = true;
		}

	}
	private void Window_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
		{
			shiftKeyPressed = false;
		}
		if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
		{
			ctrlKeyPressed = false;
		}
	}
	private void ToggleInfoBarVisibility()
	{
		if (InternetChecker.IsInternetAvailable())
		{
			infoBar.Visibility = Visibility.Collapsed;
		}
		else
		{
			infoBar.Visibility = Visibility.Visible;
		}
	}
	#endregion




}