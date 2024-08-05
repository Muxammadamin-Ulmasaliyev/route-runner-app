using RouteRunner.Windows;
using RouteRunnerLibrary;
using RouteRunnerLibrary.Models;
using RouteRunnerLibrary.Services;
using System.Windows;
using System.Windows.Controls;

namespace RouteRunner.Pages;

public partial class CollectionsSidebarPage : Page
{
	private readonly FolderService _folderService;



	private List<Folder> _folders;

	public CollectionsSidebarPage()
	{
		InitializeComponent();

		_folderService = new(new AppDbContext());
		PopulateFoldersTree();

	}

	private void PopulateFoldersTree()
	{
		var folders = GetFolders();

		var parentFolders = new List<TreeViewItem>();

		foreach (var folder in folders)
		{
			// this is folder
			if (folder.ParentId is null)
			{
				// I am using Name property for Id

				var newParentFolder = new TreeViewItem() { Header = folder.Name, Name = $"folder{folder.Id}" };
				foreach (var request in folder.SavedRequests)
				{
					// adding requests to folder
					newParentFolder.Items.Add(new TreeViewItem() { Header = request.Name, Name = $"request{request.Id}" });
				}
				parentFolders.Add(newParentFolder);

			}
			//this is subFolder
			else
			{
				var existingParentFolder = parentFolders.FirstOrDefault(pn => pn.Name == $"folder{folder.ParentId}");

				var newSubFolder = new TreeViewItem() { Header = folder.Name, Name = $"folder{folder.Id}" };

				foreach (var request in folder.SavedRequests)
				{
					// adding requests to subfolder
					newSubFolder.Items.Add(new TreeViewItem() { Header = request.Name, Name = $"request{request.Id}" });
				}

				existingParentFolder.Items.Add(newSubFolder);
			}
		}

		foldersTree.ItemsSource = parentFolders;

	}
	private List<Folder> GetFolders()
	{
		return _folderService.GetAllFolders();
	}



	private void addNewFolderButton_Click(object sender, RoutedEventArgs e)
	{
		var upsertFolderWindow = new UpsertFolderWindow();
		this.Opacity = 0.6;
		upsertFolderWindow.ShowDialog();
		this.Opacity = 1.0;
	}



	private void searchBox_Collections_TextChanged(Wpf.Ui.Controls.AutoSuggestBox sender, Wpf.Ui.Controls.AutoSuggestBoxTextChangedEventArgs args)
	{

	}
}
