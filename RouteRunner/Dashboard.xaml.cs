using System.Windows;
using Wpf.Ui.Appearance;
using System.Windows.Input;
using RouteRunner.Pages;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;
using RouteRunner.CustomControls;
using System.Windows.Shapes;

namespace RouteRunner;

public partial class MainWindow : Window
{

	private List<RequestPage> requestsList = new() { new RequestPage() };

	private bool shiftKeyPressed = false, ctrlKeyPressed = false;
	public MainWindow()
	{
		InitializeComponent();




		/*Loaded += (sender, args) =>
		{
			Wpf.Ui.Appearance.SystemThemeWatcher.Watch(
				this,                                    // Window class
				Wpf.Ui.Controls.WindowBackdropType.Mica, // Background type
				true                                     // Whether to change accents automatically
			);
		};*/
		//SystemThemeWatcher.Watch(this as System.Windows.Window);

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

	private void ApplyExtendedTabItemStyle(TabItem tabItem)
	{
		// Retrieve the existing style (if any)
		Style baseStyle = (Style)FindResource(typeof(TabItem));

		// Clone the base style to preserve existing setters and triggers
		Style extendedStyle = new Style(typeof(TabItem), baseStyle);

		// Add additional setters and triggers
		extendedStyle.Setters.Add(new Setter(TabItem.BorderBrushProperty, Brushes.Gray));
		extendedStyle.Setters.Add(new Setter(TabItem.BackgroundProperty, Brushes.LightGray));
		extendedStyle.Setters.Add(new Setter(TabItem.CursorProperty, Cursors.Hand));

		// Trigger for when the mouse is over the tab item
		Trigger isMouseOverTrigger = new Trigger
		{
			Property = TabItem.IsMouseOverProperty,
			Value = true
		};

		isMouseOverTrigger.Setters.Add(new Setter(TabItem.BackgroundProperty, Brushes.LightGray));
		extendedStyle.Triggers.Add(isMouseOverTrigger);

		// Trigger for when the tab item is selected
		Trigger isSelectedTrigger = new Trigger
		{
			Property = TabItem.IsSelectedProperty,
			Value = true
		};
		isSelectedTrigger.Setters.Add(new Setter(TabItem.BackgroundProperty, Brushes.Red));
		extendedStyle.Triggers.Add(isSelectedTrigger);

		// Apply the style to the TabItem
		tabItem.Style = extendedStyle;
	}


	private void requestsTabControl_Loaded(object sender, RoutedEventArgs e)
	{
		TabItem newTabItem = new TabItem
		{
			Header = "Untitled request", // Text or content of the TabItem header
			Content = null
		};
		DataTemplate tabHeaderTemplate = (DataTemplate)FindResource("TabHeaderTemplate");


		// Apply the HeaderTemplate to the TabItem
		newTabItem.HeaderTemplate = tabHeaderTemplate;

		ApplyExtendedTabItemStyle(newTabItem);
		// Add the TabItem to the TabControl
		requestsTabControl.Items.Add(newTabItem);

		mainFrame.Navigate(requestsList[0]);

		requestsTabControl.SelectedIndex = 0;
		requestsTabControl.SelectedItem = requestsTabControl.Items[0];


	}

	private void requestsTabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		if (requestsTabControl.SelectedIndex >= 0)
		{
			mainFrame.Navigate(requestsList[requestsTabControl.SelectedIndex]);

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
				requestsTabControl.SelectedItem = requestsTabControl.Items[indexToRemove - 1];
				requestsTabControl.SelectedIndex = indexToRemove - 1;
			}

			requestsTabControl.Items.Remove(tabItemToRemove);
			requestsList.RemoveAt(indexToRemove);


		}
	}

	private void addNewRequestButton_Click(object sender, RoutedEventArgs e)
	{
		TabItem newTabItem = new TabItem
		{
			Header = "Untitled request", // Text or content of the TabItem header
			Content = null
		};


		DataTemplate tabHeaderTemplate = (DataTemplate)FindResource("TabHeaderTemplate");

		ApplyExtendedTabItemStyle(newTabItem);
		newTabItem.HeaderTemplate = tabHeaderTemplate;

		requestsTabControl.Items.Add(newTabItem);

		requestsList.Add(new RequestPage());

		requestsTabControl.SelectedItem = requestsTabControl.Items[requestsTabControl.Items.Count - 1];
		requestsTabControl.SelectedIndex = requestsTabControl.Items.Count - 1;

		mainFrame.Navigate(requestsList[requestsList.Count - 1]);



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

	

	

	private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var selected = sidebar.SelectedItem as NavButton;
		sidebarFrame.Navigate(selected.NavLink);

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
}