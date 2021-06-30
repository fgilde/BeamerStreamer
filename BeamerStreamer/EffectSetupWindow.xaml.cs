using System;
using System.Windows;

namespace BeamerStreamer
{
	/// <summary>
	/// Interaction logic for EffectSetupWindow.xaml
	/// </summary>
	public partial class EffectSetupWindow
	{
		public EffectSetupWindow(EffectViewModel effectViewModel)
		{
			InitializeComponent();
			DataContext = effectViewModel;
		}
	}
}
