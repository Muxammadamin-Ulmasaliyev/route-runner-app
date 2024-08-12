using RouteRunnerLibrary.Services;
using RouteRunnerLibrary;
using System.Windows;
using System.Windows.Input;
using RouteRunnerLibrary.Models;
using System.Data;

namespace RouteRunner.Windows;

public partial class UpsertFolderWindow : Window
{
	private readonly FolderService _folderService;
	public event EventHandler<Folder> NewFolderCreatedEvent;

	private int? parentFolderId = null;

	public UpsertFolderWindow(int? parentFolderId = null)
	{
		this.parentFolderId = parentFolderId;
		InitializeComponent();
		_folderService = new(new AppDbContext());
	}



	private void folderNameTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			saveButton_Click(sender, e);
			e.Handled = true;
		}

	}


	// Add creating subfolders 
	private Folder CreateNewFolder()
	{

		Folder createdFolder;
		var folderName = folderNameTextBox.Text;
		var parentFolderId = this.parentFolderId;

		if (parentFolderId is null)
		{
			createdFolder = _folderService.CreateFolder(new Folder() { Name = folderName });
		}
		else
		{
			createdFolder = _folderService.CreateFolder(new Folder() { Name = folderName, ParentId = parentFolderId });
		}



		return createdFolder;
	}

	private void cancelButton_Click(object sender, RoutedEventArgs e)
	{
		this.Close();
	}

	private void saveButton_Click(object sender, RoutedEventArgs e)
	{
		var createdFolder = CreateNewFolder();
		NewFolderCreatedEvent?.Invoke(this, createdFolder);
		this.Close();
	}

	private void Window_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			saveButton_Click(sender, e);
			e.Handled = true;
		}
		if (e.Key == Key.Escape)
		{
			cancelButton_Click(sender, e);
			e.Handled = true;
		}

	}
}
