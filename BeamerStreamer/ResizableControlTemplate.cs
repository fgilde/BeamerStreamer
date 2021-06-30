using System.Windows;
using System.Windows.Controls;

namespace BeamerStreamer
{
	public class ResizableContentControl : ContentControl
	{
		public Visibility ThumbVisibility
		{
			get { return (Visibility)GetValue(ThumbVisibilityProperty); }
			set { SetValue(ThumbVisibilityProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ThumbVisibility.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ThumbVisibilityProperty =
			DependencyProperty.Register("ThumbVisibility", typeof(Visibility), typeof(ResizableContentControl), new PropertyMetadata(Visibility.Visible));

	
	}
}