using RouteRunnerLibrary.Models;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace RouteRunner.Helpers;

public static class ElementDesigner
{
	#region Generate Custom TreeViewItems
	public static TreeViewItem GenerateCustomFolderTreeViewItem(Folder folder, ContextMenu folderContextMenu)
	{
		StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal };

		Image icon = new Image
		{
			Source = new BitmapImage(new Uri("pack://application:,,,/Images/folderIcon.png")),
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

		stack.Children.Add(icon);
		stack.Children.Add(text);

		return new TreeViewItem
		{
			Header = stack,
			Name = $"folder{folder.Id}",
			ContextMenu = folderContextMenu,
			Tag = folder
		};
	}

	public static TreeViewItem GenerateCustomRequestTreeViewItem(Request request, ContextMenu requestContextMenu)
	{
		StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal };

		Image icon = new Image
		{
			Source = new BitmapImage(new Uri("pack://application:,,,/Images/requestIcon.png")),
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

		return new TreeViewItem
		{
			Header = stack,
			Name = $"request{request.Id}",
			ContextMenu = requestContextMenu,
			Tag = request
		};
	}

	public static TreeViewItem GenerateCustomRequestInHistoryTreeViewItem(RequestInHistory request, ContextMenu requestContextMenu)
	{
		StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal };

		Image icon = new Image
		{
			Source = new BitmapImage(new Uri("pack://application:,,,/Images/requestIcon.png")),
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

		return new TreeViewItem
		{
			Header = stack,
			Name = $"request{request.Id}",
			ContextMenu = requestContextMenu,
			Tag = request
		};
	}
	#endregion
}
