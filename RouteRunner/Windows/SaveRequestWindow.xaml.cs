using RouteRunner.Helpers;
using RouteRunnerLibrary;
using RouteRunnerLibrary.Models;
using RouteRunnerLibrary.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents.Serialization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = Wpf.Ui.Controls.Image;
using ListView = System.Windows.Controls.ListView;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace RouteRunner.Windows;

public partial class SaveRequestWindow : Window
{
	private SavedRequest _currentRequest;
	private readonly FolderService _folderService;
	private readonly SavedRequestService _requestService;

	public SaveRequestWindow(SavedRequest request)
	{

		_folderService = new FolderService(new AppDbContext());
		_requestService = new SavedRequestService(new AppDbContext());
		_currentRequest = request;

		InitializeComponent();

		requestNameTextBox.Text = request.Name;

		PopulateFoldersListView();

		breadCrumbBar.Items.Add(new Folder() { Id = -1, Name = "Home" });

	}

	private void PopulateFoldersListView()
	{
		// Fetch folders once and store them
		var folders = GetFolders();
		foldersListView.Items.Clear();

		// Reuse the BitmapImage instance for the icon
		var folderIcon = new BitmapImage(new Uri("pack://application:,,,/Images/folderIcon.png"));

		foreach (var folder in folders)
		{
			var listViewItem = CreateCustomStyledFolderListViewItem(folder, folderIcon);
			foldersListView.Items.Add(listViewItem);
		}
	}

	private ListViewItem CreateCustomStyledFolderListViewItem(Folder folder, BitmapImage folderIcon)
	{
		// Reuse BitmapImage instance for each item
		Image icon = new Image
		{
			Source = folderIcon,
			Width = 18,
			Height = 18,
			Margin = new Thickness(0, 0, 5, 0)
		};

		TextBlock text = new TextBlock
		{
			FontSize = 16,
			Text = folder.Name,
			VerticalAlignment = VerticalAlignment.Center
		};

		// Use a horizontal StackPanel for layout
		StackPanel stack = new StackPanel
		{
			Orientation = Orientation.Horizontal
		};
		stack.Children.Add(icon);
		stack.Children.Add(text);

		// Return a new ListViewItem
		return new ListViewItem
		{
			Content = stack,
			Tag = folder
		};
	}

	private ListViewItem CreateCustomStyledRequestListViewItem(SavedRequest request, BitmapImage requestIcon)
	{
		StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal };
		Image icon = new Image
		{
			Source = requestIcon,
			Width = 18,
			Height = 18,
			Margin = new Thickness(0, 0, 5, 0)
		};

		TextBlock text = new TextBlock
		{
			FontSize = 16,
			Text = request.Name,
			VerticalAlignment = VerticalAlignment.Center
		};

		stack.Children.Add(icon);
		stack.Children.Add(text);


		return new ListViewItem()
		{
			Content = stack,
			Tag = request
		};
	}

	#region DB operations
	private List<Folder> GetFolders()
	{
		return _folderService.GetRootParentFolders();
	}

	private List<Folder> GetSubFolders(int parentFolderId)
	{
		return _folderService.GetSubFolders(parentFolderId);

	}

	private Folder GetFolderById(int id)
	{
		return _folderService.GetFolderById(id);
	}

	private List<SavedRequest> GetRequests(int folderId)
	{

		return _requestService.GetRequestsByFolderId(folderId);
	}
	#endregion

	private void foldersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var selectedFolder = (e.AddedItems[0] as ListViewItem).Tag as Folder;

		PopulateFoldersListView(selectedFolder.Id);
	}

	private void AddPathToBreadCrumbBar(Folder folder)
	{
		breadCrumbBar.Items.Add(folder);
	}



	private void PopulateFoldersListView(int parentFolderId)
	{
		var folders = GetSubFolders(parentFolderId);

		var folderIcon = new BitmapImage(new Uri("pack://application:,,,/Images/folderIcon.png"));
		var requestIcon = new BitmapImage(new Uri("pack://application:,,,/Images/requestIcon.png"));

		foldersListView.Items.Clear();

		foreach (var folder in folders)
		{
			var listViewItem = CreateCustomStyledFolderListViewItem(folder, folderIcon);
			foldersListView.Items.Add(listViewItem);
		}

		var requests = GetRequests(parentFolderId);
		foreach (var request in requests)
		{
			var listViewItem = CreateCustomStyledRequestListViewItem(request, requestIcon);


			foldersListView.Items.Add(listViewItem);
		}

	}

	private void foldersListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{

		if (sender is ListView listView)
		{
			var point = e.GetPosition(listView);

			var clickedItem = VisualTreeHelper.HitTest(listView, point)?.VisualHit;

			while (clickedItem != null && !(clickedItem is ListViewItem))
			{
				clickedItem = VisualTreeHelper.GetParent(clickedItem);
			}

			if (clickedItem is ListViewItem listViewItem)
			{
				if (listViewItem.Tag is SavedRequest)
				{
					return;
				}

				var clickedFolder = (clickedItem as ListViewItem).Tag as Folder;

				PopulateFoldersListView(clickedFolder.Id);

				AddPathToBreadCrumbBar(clickedFolder);
			}
		}
	}


	private void breadCrumbBar_ItemClicked(Wpf.Ui.Controls.BreadcrumbBar sender, Wpf.Ui.Controls.BreadcrumbBarItemClickedEventArgs args)
	{
		var clickedItem = args.Item;

		if (clickedItem is Folder clickedFolder)
		{
			if (clickedFolder.Id == -1)
			{
				PopulateFoldersListView();
			}
			else
			{
				PopulateFoldersListView(clickedFolder.Id);

			}


			var selectedIndex = args.Index;

			var length = breadCrumbBar.Items.Count;
			for (int i = length - 1; i > selectedIndex; i--)
			{
				breadCrumbBar.Items.RemoveAt(i);
			}

		}


	}

	private void DynamicScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		var scrollViewer = sender as ScrollViewer;

		if (scrollViewer != null)
		{
			// Check if the user is scrolling up or down
			if (e.Delta > 0)
			{
				// Scroll up
				scrollViewer.LineUp();
			}
			else
			{
				// Scroll down
				scrollViewer.LineDown();
			}

			// Mark the event as handled so that the default scrolling doesn't occur
			e.Handled = true;
		}
	}

	private void cancelButton_Click(object sender, RoutedEventArgs e)
	{
		this.Close();
	}

	private void saveRequestButton_Click(object sender, RoutedEventArgs e)
	{

		if (breadCrumbBar.Items.Count == 1)
		{
			System.Windows.MessageBox.Show("Please select a folder to save the request to.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
			return;
		}


		var parentFolder = breadCrumbBar.Items[breadCrumbBar.Items.Count - 1] as Folder;

		var createdRequest = _requestService.CreateRequest(new SavedRequest()
		{
			Name = requestNameTextBox.Text,
			HttpVerb = _currentRequest.HttpVerb,
			Url = _currentRequest.Url,
			FolderId = parentFolder.Id
		});


		EventNotificationService.Instance.NewRequestCreated(createdRequest);


		this.Close();



	}
}

