using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RouteRunner.CustomControls;

public class NavButton : ListBoxItem
{
	static NavButton()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(NavButton), new FrameworkPropertyMetadata(typeof(NavButton)));
	}

	public Uri NavLink
	{
		get { return (Uri)GetValue(NavLinkProperty); }
		set { SetValue(NavLinkProperty, value); }
	}

	// Using a DependencyProperty as the backing store for NavLink.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty NavLinkProperty =
		DependencyProperty.Register("NavLink", typeof(Uri), typeof(NavButton), new PropertyMetadata(null));




	public Geometry Icon
	{
		get { return (Geometry)GetValue(IconProperty); }
		set { SetValue(IconProperty, value); }
	}

	// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty IconProperty =
		DependencyProperty.Register("Icon", typeof(Geometry), typeof(NavButton), new PropertyMetadata(null));


	public double ScaleFactor
	{
		get { return (double)GetValue(ScaleFactorProperty); }
		set { SetValue(ScaleFactorProperty, value); }
	}

	public static readonly DependencyProperty ScaleFactorProperty =
		DependencyProperty.Register("ScaleFactor", typeof(double), typeof(NavButton), new PropertyMetadata(1.0));

	protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
	{
		base.OnRenderSizeChanged(sizeInfo);

		// Apply scaling to the LayoutTransform
		if (sizeInfo.HeightChanged || sizeInfo.WidthChanged)
		{
			var scaleTransform = new ScaleTransform(ScaleFactor, ScaleFactor);
			LayoutTransform = scaleTransform;
		}
	}

}
