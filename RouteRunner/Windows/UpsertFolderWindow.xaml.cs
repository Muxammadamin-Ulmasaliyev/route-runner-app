using RouteRunnerLibrary.Services;
using RouteRunnerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RouteRunnerLibrary.Models;

namespace RouteRunner.Windows;

public partial class UpsertFolderWindow : Window
{
	private readonly FolderService _folderService;

	public UpsertFolderWindow()
	{

		InitializeComponent();
		_folderService = new(new AppDbContext());
	}

	private void folderNameTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			CreateNewFolder();
		}

		e.Handled = true;
	}


	// Add creating subfolders 
	private Folder CreateNewFolder()
	{

		var folderName = folderNameTextBox.Text;

		var createdFolder = _folderService.CreateFolder(new Folder() { Name = folderName });

		return createdFolder;
	}

	private void cancelButton_Click(object sender, RoutedEventArgs e)
	{
		this.Close();
	}
}
