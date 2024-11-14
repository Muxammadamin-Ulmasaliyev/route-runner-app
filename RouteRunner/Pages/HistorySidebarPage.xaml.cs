using RouteRunner.Helpers;
using RouteRunnerLibrary;
using RouteRunnerLibrary.Models;
using RouteRunnerLibrary.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RouteRunner.Pages;


public partial class HistorySidebarPage : Page
{

	private readonly RequestHistoryService _requestHistoryService;
	private ObservableCollection<TreeViewItem> _items;

	public event EventHandler<Request> NewRequestOpened;

	public HistorySidebarPage()
	{

		_requestHistoryService = new(new AppDbContext());
		InitializeComponent();
		PopulateTreeAsync();


	}
	private async void PopulateTreeAsync()
	{
		try
		{
			loadingBar.Visibility = Visibility.Visible;
			await PopulateTree();
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error populating tree: {ex.Message}");
		}
		finally
		{
			loadingBar.Visibility = Visibility.Collapsed;
		}
	}



	private async Task PopulateTree()
	{
		var requestsHistory = await Task.Run(() => _requestHistoryService.GetAvailableDatesFromHistory());
		_items = new ObservableCollection<TreeViewItem>();
		for (int i = 0; i < requestsHistory.Count; i++)
		{
			var groupItem = new TreeViewItem
			{
				Tag = requestsHistory[i],
				Header = StringHelper.DateTimeToString_DD_MonthName_YYYY(requestsHistory[i]),
				IsExpanded = true,
			};
			_items.Add(groupItem);
		}
		historyTree.ItemsSource = _items;
	}

	private void AddRequestToTreeByDate(DateTime date, TreeViewItem groupItem)
	{
		groupItem.Items.Clear();

		var requestContextMenu = GenerateContextMenuForRequest();
		var requests = _requestHistoryService.GetRequestHistoryByDay(date);
		foreach (var request in requests)
		{
			groupItem.Items.Add(ElementDesigner.GenerateCustomRequestInHistoryTreeViewItem(request, requestContextMenu));
		}

		groupItem.IsExpanded = true;
	}


	private Task<List<IGrouping<DateTime, RequestInHistory>>> GetGroupedRequestHistoryAsync()
	{
		return _requestHistoryService.GetGroupedRequestsHistoryAsync();
	}


	private ContextMenu GenerateContextMenuForRequest()
	{
		var contextMenu = new ContextMenu();
		// Open Request MenuItem
		MenuItem openRequestMenuItem = new MenuItem { Header = "Open" };
		openRequestMenuItem.Click += Request_Open_Click;
		contextMenu.Items.Add(openRequestMenuItem);

		// Delete Request MenuItem
		MenuItem deleteRequestMenuItem = new MenuItem { Header = "Delete" };
		deleteRequestMenuItem.Click += Request_Delete_Click;
		contextMenu.Items.Add(deleteRequestMenuItem);
		return contextMenu;
	}



	private void Request_Open_Click(object sender, RoutedEventArgs e)
	{


	}
	private void Request_Delete_Click(object sender, RoutedEventArgs e)
	{

	}
	private void searchBox_TextChanged(Wpf.Ui.Controls.AutoSuggestBox sender, Wpf.Ui.Controls.AutoSuggestBoxTextChangedEventArgs args)
	{

	}

	private void historyTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		var clickedItem = e.OriginalSource as DependencyObject;
		while (clickedItem != null && !(clickedItem is TreeViewItem))
		{
			clickedItem = VisualTreeHelper.GetParent(clickedItem);
		}
		var treeViewItem = clickedItem as TreeViewItem;

		if (treeViewItem == null)
		{
			return;
		}
		if (treeViewItem.Tag is DateTime)
		{
			AddRequestToTreeByDate((DateTime)treeViewItem.Tag, treeViewItem);
			return;
		}
		if (treeViewItem.Tag is RequestInHistory requestInHistory)
		{

			var request = new Request()
			{
				
				Name = requestInHistory.Name,
				Url = requestInHistory.Url,
				HttpVerb = requestInHistory.HttpVerb,
				Body = requestInHistory.Body
			};

			NewRequestOpened?.Invoke(this, request);
			return;
		}
	}

	private void historyTree_MouseWheel(object sender, MouseWheelEventArgs e)
	{
		var scrollViewer = sender as ScrollViewer;
		if (scrollViewer != null)
		{
			if (e.Delta > 0)
			{
				scrollViewer.LineUp();
			}
			else
			{
				scrollViewer.LineDown();
			}
			e.Handled = true;
		}
	}
}
