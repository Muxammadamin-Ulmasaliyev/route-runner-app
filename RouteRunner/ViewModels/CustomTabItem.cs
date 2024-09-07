using System.Windows.Controls;

namespace RouteRunner.ViewModels;

class CustomTabItem : TabItem
{
	public string HeaderAsString
	{
		get
		{
			return this.Header.ToString();
		}
		set
		{
			this.Header = value;
		}
	}
}
