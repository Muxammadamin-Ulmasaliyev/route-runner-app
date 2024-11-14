using RouteRunner.Helpers;
using RouteRunner.Windows;
using RouteRunnerLibrary;
using RouteRunnerLibrary.Models;
using RouteRunnerLibrary.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using TreeViewItem = System.Windows.Controls.TreeViewItem;

namespace RouteRunner.Pages;

public partial class CollectionsSidebarPage : Page
{
	private readonly FolderService _folderService;
	private readonly SavedRequestService _requestService;

	public event EventHandler<Request> NewRequestOpened;

	public CollectionsSidebarPage()
	{
		InitializeComponent();
		_folderService = new(new AppDbContext());
		_requestService = new(new AppDbContext());


		PopulateFoldersTreeAsync();

		EventNotificationService.Instance.NewRequestCreatedEvent += NewRequestCreatedEventHandler;
		EventNotificationService.Instance.ExistingRequestSavedEvent += ExistingRequestSavedEventHandler; ;
	}

	private void ExistingRequestSavedEventHandler(object? sender, Request updatedRequest)
	{
		UpdateRequestInTreeView(updatedRequest);
	}

	private void UpdateRequestInTreeView(Request updatedRequest)
	{
		var existingRequestTreeViewItem = FindTreeViewItemByName(foldersTree.Items, $"request{updatedRequest.Id}");

		if (existingRequestTreeViewItem != null)
		{
			var existingParentFolder = FindTreeViewItemByName(foldersTree.Items, $"folder{updatedRequest.FolderId}");

			if (existingParentFolder != null)
			{
				var index = existingParentFolder.Items.IndexOf(existingRequestTreeViewItem);

				existingParentFolder.Items[index] = ElementDesigner.GenerateCustomRequestTreeViewItem(updatedRequest, GenerateContextMenuForRequest());
			}
			else
			{
				MessageBox.Show("Parent folder not found.");
			}

		}
		else
		{
			MessageBox.Show("request  not found.");
		}
	}

	private void NewRequestCreatedEventHandler(object? sender, (int tabIndex, Request newRequest) data)
	{
		AddNewRequestToTree(data.newRequest);
	}

	private async void PopulateFoldersTreeAsync()
	{
		try
		{
			// Show loading indicator
			loadingBar.Visibility = Visibility.Visible;

			// Populate TreeView with data from the backend
			await PopulateFoldersTree();

		}
		catch (Exception ex)
		{
			// Handle exceptions (e.g., logging)
			MessageBox.Show($"Error populating folders: {ex.Message}");
		}
		finally
		{
			// Hide loading indicator
			loadingBar.Visibility = Visibility.Collapsed;
		}
	}

	private async Task PopulateFoldersTree()
	{
		var folderContextMenu = GenerateContextMenuForFolder();
		var requestContextMenu = GenerateContextMenuForRequest();

		var folders = await Task.Run(() => GetFolders());
		//var folders = GetFolders();

		var folderItems = new Dictionary<int, TreeViewItem>();
		var rootItems = new List<TreeViewItem>();

		foreach (var folder in folders)
		{
			// Create TreeViewItem for each folder
			var newFolder = ElementDesigner.GenerateCustomFolderTreeViewItem(folder, folderContextMenu);


			// Add requests to the folder
			foreach (var request in folder.SavedRequests)
			{

				var requestItem = ElementDesigner.GenerateCustomRequestTreeViewItem(request, requestContextMenu);

				// Create TreeViewItem for each request


				newFolder.Items.Add(requestItem);
			}

			// Store the item in the dictionary
			folderItems[folder.Id] = newFolder;

			// If the folder has no parent, it'` a root folder
			if (folder.ParentId == null)
			{
				rootItems.Add(newFolder);
			}
		}

		// Set parent-child relationships
		foreach (var folder in folders)
		{
			if (folder.ParentId != null)
			{
				if (folderItems.TryGetValue(folder.ParentId.Value, out var parentItem))
				{
					parentItem.Items.Add(folderItems[folder.Id]);
				}
			}
		}

		// Clear previous items and add the new root items
		foldersTree.Items.Clear();
		foreach (var rootItem in rootItems)
		{
			foldersTree.Items.Add(rootItem);
		}

	}




	#region ContextMenu_Operations
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
	private ContextMenu GenerateContextMenuForFolder()
	{
		var contextMenu = new ContextMenu();
		MenuItem newFolderMenuItem = new MenuItem { Header = "New Folder" };
		newFolderMenuItem.Click += Folder_NewFolder_Click;
		contextMenu.Items.Add(newFolderMenuItem);

		// New Request MenuItem
		MenuItem newRequestMenuItem = new MenuItem { Header = "New Request" };
		newRequestMenuItem.Click += Folder_NewRequest_Click;
		contextMenu.Items.Add(newRequestMenuItem);

		// Delete Folder MenuItem
		MenuItem deleteFolderMenuItem = new MenuItem { Header = "Delete Folder" };
		deleteFolderMenuItem.Click += Folder_Delete_Click;
		contextMenu.Items.Add(deleteFolderMenuItem);

		return contextMenu;
	}


	private void Request_Delete_Click(object sender, RoutedEventArgs e)
	{
		var contextMenuItem = sender as MenuItem;
		var contextMenu = contextMenuItem.Parent as ContextMenu;

		if (contextMenu != null)
		{
			var requestTreeViewItemToRemove = contextMenu.PlacementTarget as TreeViewItem;

			var existingParentFolder = requestTreeViewItemToRemove.Parent as TreeViewItem;

			var requestToDelete = requestTreeViewItemToRemove.Tag as Request;

			var result = MessageBox.Show("Are you sure to delete request : " + requestToDelete.Name + " ?", "Warning"
				, MessageBoxButton.YesNo
				, MessageBoxImage.Warning
				, System.Windows.MessageBoxResult.No);


			if (result == MessageBoxResult.Yes)
			{

				if (existingParentFolder != null)
				{
					var deleteResult = _requestService.DeleteRequest(requestToDelete.Id);
					if (deleteResult is false)
					{
						MessageBox.Show("Error  in  deletion.");
						return;
					}
					existingParentFolder.Items.Remove(requestTreeViewItemToRemove);
					EventNotificationService.Instance.RequestDeleted_CollectionsTreeView(requestToDelete);
				}
				else
				{
					MessageBox.Show("Parent folder not found.");
				}

			}

		}
	}


	private void Request_Open_Click(object sender, RoutedEventArgs e)
	{
		var menuItem = sender as MenuItem;

		var contextMenu = menuItem.Parent as ContextMenu;

		var treeViewItem = contextMenu.PlacementTarget as TreeViewItem;

		if (treeViewItem.Tag is Request request)
		{
			//var request = _requelstService.GetRequestById(int.Parse(treeViewItem.Name.Remove(0, 7)));
			NewRequestOpened?.Invoke(this, request);
		}

	}

	private void Folder_Delete_Click(object sender, RoutedEventArgs e)
	{

		var contextMenuItem = sender as MenuItem;
		var contextMenu = contextMenuItem.Parent as ContextMenu;

		if (contextMenu is not null)
		{
			// Get the TreeViewItem from the ContextMenu

			var treeViewItem = contextMenu.PlacementTarget as TreeViewItem;

			// "Are you sure to delete folder : " + ((treeViewItem.Header as StackPanel).Children[1] as TextBlock).Text + " ?", "Warning"
			var result = MessageBox.Show("Are you sure to delete folder : " + (treeViewItem.Tag as Folder).Name + " ?", "Warning"
				, MessageBoxButton.YesNo
				, MessageBoxImage.Warning
				, MessageBoxResult.No);


			if (result == MessageBoxResult.Yes)
			{
				// Get the folderId from the TreeViewItem

				var folderToDelete = FindTreeViewItemByName(foldersTree.Items, treeViewItem.Name);

				var parentFolder = folderToDelete.Parent as TreeViewItem;

				if (parentFolder is null)
				{
					foldersTree.Items.Remove(folderToDelete);
				}
				else
				{
					parentFolder.Items.Remove(folderToDelete);
				}

				//var folderIdToDelete = int.Parse(treeViewItem.Name.Remove(0, 6));
				var folderIdToDelete = (treeViewItem.Tag as Folder).Id;

				DeleteFolder(folderIdToDelete);
			}


		}
	}

	private void DeleteFolder(int folderIdToDelete)
	{
		_folderService.DeleteFolder(folderIdToDelete);
	}

	private void Folder_NewRequest_Click(object sender, RoutedEventArgs e)
	{
		var contextMenuItem = sender as MenuItem;

		var contextMenu = contextMenuItem.Parent as ContextMenu;

		if (contextMenu != null)
		{
			// Get the TreeViewItem from the ContextMenu

			var treeViewItem = contextMenu.PlacementTarget as TreeViewItem;
			// Get the folderId from the TreeViewItem
			//var parentFolderId = int.Parse(treeViewItem.Name.Remove(0, 6));
			var parentFolderId = (treeViewItem.Tag as Folder).Id;

			var createdNewRequest = CreateNewRequest(parentFolderId);

			AddNewRequestToTree(createdNewRequest);


		}
	}

	private Request CreateNewRequest(int parentFolderId)
	{
		var newRequest = _requestService.CreateRequest(new Request() { Name = $"New Request {DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss")}", Url = "", HttpVerb = HttpVerb.GET, FolderId = parentFolderId });
		return newRequest;
	}


	private void AddNewRequestToTree(Request request)
	{
		// Create a new TreeViewItem for the request

		var newRequestTreeViewItem = ElementDesigner.GenerateCustomRequestTreeViewItem(request, GenerateContextMenuForRequest());

		// Find the existing parent folder in the TreeView
		var existingParentFolder = FindTreeViewItemByName(foldersTree.Items, $"folder{request.FolderId}");

		if (existingParentFolder != null)
		{
			// Add the new folder as a sub-folder to the parent
			existingParentFolder.Items.Add(newRequestTreeViewItem);
		}
		else
		{
			MessageBox.Show("Parent folder not found.");
		}
	}

	private void Folder_NewFolder_Click(object sender, RoutedEventArgs e)
	{
		var contextMenuItem = sender as MenuItem;
		var contextMenu = contextMenuItem.Parent as ContextMenu;

		if (contextMenu != null)
		{

			var treeViewItem = contextMenu.PlacementTarget as TreeViewItem;

			// Get the folderId from the TreeViewItem
			//var parentFolderId = int.Parse(treeViewItem.Name.Remove(0, 6));
			var parentFolderId = (treeViewItem.Tag as Folder).Id;

			var upsertFolderWindow = new UpsertFolderWindow(parentFolderId);


			upsertFolderWindow.NewFolderCreatedEvent += NewFolder_Created;


			this.Opacity = 0.6;
			upsertFolderWindow.ShowDialog();
			this.Opacity = 1.0;
		}
	}

	private List<Folder> GetFolders()
	{
		return _folderService.GetAllFolders();
	}


	#endregion



	private void addNewFolderButton_Click(object sender, RoutedEventArgs e)
	{
		var upsertFolderWindow = new UpsertFolderWindow();
		upsertFolderWindow.NewFolderCreatedEvent += NewFolder_Created;
		this.Opacity = 0.6;
		upsertFolderWindow.ShowDialog();
		this.Opacity = 1.0;
	}


	private void AddFolderToTree(Folder folder)
	{
		var newFolderTreeViewItem = ElementDesigner.GenerateCustomFolderTreeViewItem(folder, GenerateContextMenuForFolder());

		// Check if the folder has a parent
		if (folder.ParentId is null)
		{
			// If no parent, add it directly to the root level of the TreeView
			foldersTree.Items.Add(newFolderTreeViewItem);
		}
		else
		{
			// Find the existing parent folder in the TreeView
			var existingParentFolder = FindTreeViewItemByName(foldersTree.Items, $"folder{folder.ParentId}");

			if (existingParentFolder != null)
			{
				// Add the new folder as a sub-folder to the parent
				existingParentFolder.Items.Add(newFolderTreeViewItem);
			}
			else
			{
				MessageBox.Show("Parent folder not found.");
			}
		}
	}

	// Recursive method to find a TreeViewItem by its Name property
	private TreeViewItem FindTreeViewItemByName(ItemCollection items, string name)
	{
		foreach (TreeViewItem item in items)
		{
			if (item.Name == name)
			{
				return item;
			}

			// Recursively search in child items
			var foundItem = FindTreeViewItemByName(item.Items, name);

			if (foundItem != null)
			{
				return foundItem;
			}
		}

		return null;
	}



	private void searchBox_Collections_TextChanged(Wpf.Ui.Controls.AutoSuggestBox sender, Wpf.Ui.Controls.AutoSuggestBoxTextChangedEventArgs args)
	{

	}


	private void NewFolder_Created(object sender, Folder newFolder)
	{
		AddFolderToTree(newFolder);
	}







	private void foldersTree_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		var clickedItem = e.OriginalSource as DependencyObject;

		// Traverse up the visual tree to find the TreeViewItem

		while (clickedItem != null && !(clickedItem is TreeViewItem))
		{
			clickedItem = VisualTreeHelper.GetParent(clickedItem);
		}

		var treeViewItem = clickedItem as TreeViewItem;
		//var treeViewItem = sender as TreeViewItem;

		if (treeViewItem.Tag is Request request)
		{
			//var request = _requestService.GetRequestById(int.Parse(treeViewItem.Name.Remove(0, 7)));
			NewRequestOpened?.Invoke(this, request);
		}



	}
}
