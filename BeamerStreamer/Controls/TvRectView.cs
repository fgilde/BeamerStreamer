using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Binding = System.Windows.Data.Binding;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace BeamerStreamer.Controls
{

	public class TvRectView : ContentControl
	{

		public TvRectViewModel Model => DataContext as TvRectViewModel;
	    private MediaElement video;
		private FrameworkElement rectTvBackground;
		private FrameworkElement rectTvSimulation;
		private Border mainBorder;
		private ContentPresenter contentPresenter;

		public TvRectView()
		{
			DataContext = new TvRectViewModel();
			Loaded += OnLoaded;
			System.Windows.Application.Current.MainWindow.KeyDown += OnKeyDown;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			video = Template.FindName("video", this) as MediaElement;
			rectTvBackground = Template.FindName("rectTvBackground", this) as FrameworkElement;
			rectTvSimulation = Template.FindName("rectTvSimulation", this) as FrameworkElement;
			mainBorder = Template.FindName("mainBorder", this) as Border;
			contentPresenter = Template.FindName("contentPresenter", this) as ContentPresenter;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
            var parent = mainBorder;//video.Parent as Border;
            foreach (EffectViewItem effectViewItem in Model.EffectViewModel.Effects)
            {
                var b = new Border();

                // b.DataContext = model.Effects[x];
                var binding = new Binding("DisplayedEffect") { Source = effectViewItem };

                b.SetBinding(EffectProperty, binding);
                parent.Child = b;
                parent = b;
            }
            parent.Child = contentPresenter;

            MouseMove += OnMouseMove;
		}

		void OnMouseMove(object sender, MouseEventArgs e)
		{
			Point p = e.GetPosition(video);

			MousePositionAttachedBehavior.SetX(this, p.X);
			MousePositionAttachedBehavior.SetY(this, p.Y);

		}


        private void ChangeTvBackBrush()
        {
            var brush = SelectBrush();
            if (brush != null)
            {
                isTvSimulating = false;
                Model.TvBackground = brush;
            }
        }

        private void ChangeOverlayBrush()
        {
            var brush = SelectBrush();
            if (brush != null)
                Model.OverlayBrush = brush;

        }

        private Brush SelectBrush()
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                isTvSimulating = false;
                return new SolidColorBrush(dlg.Color.ToMediaColor());
            }
            return null;
        }

        private void ToogleSimulateTV()
        {
            isTvSimulating = !isTvSimulating;
            Model.TvBackground = isTvSimulating ? MainWindow.Instance.videoContainer.Background : new SolidColorBrush(Model.SavedSettings.TvBackgroundColor.ToMediaColor());
        }

	    private bool isTvSimulating;

	    private string GetSettingsName()
	    {
	        var dlg = new InputDialog
	        {
	            Width = (System.Windows.Application.Current.MainWindow.ActualWidth - 50),
	            Left = 20,
                textBox =
                {                    
                    Text = $"My Settings {Model.Presets.Count+1}"
                }
            };
	        
	        if (dlg.ShowDialog() ?? false)
	            return dlg.Value;
	        return null;
	    }

        private bool Ask(string message)
        {
            var dlg = new InputDialog
            {
                Width = (System.Windows.Application.Current.MainWindow.ActualWidth - 50),
                Left = 20,
                textBox =
                {
                    IsReadOnly = true,
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Transparent,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    FontSize = 12,
                    Text = message
                }
            };
            return (dlg.ShowDialog() ?? false);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
		{
            if (e.Key == ControlKeys.ResetSettingsKey)
            {
                var deleteAll = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftShift);
                if (Ask(deleteAll ? Properties.Resources.DeleteAllSettings : string.Format(Properties.Resources.DeleteCurrentSettings, Model.CurrentSettingsName)))
                {
                    if (deleteAll)
                        Model.DeleteAllSettings();
                    else
                        Model.DeleteCurrentSettings();
                }
            }

            if (e.Key == ControlKeys.SaveAsPresetKey)
            {
                string name = GetSettingsName();
                if (!string.IsNullOrEmpty(name))
                {
                    var fileName = Path.Combine(Model.SettingsDirectory, $"{name}.{TvRectViewModel.SettingsExtension}");
                    Model.SaveSettingsTo(fileName);
                }
            }

            if (e.Key == ControlKeys.PreviousPresetKey)
            {
                var sets = Model.Presets.ToList();
                if (sets.Count > 1)
                {
                    int idx = sets.IndexOf(Model.CurrentSettingsFile) - 1;
                    if (idx < 0)
                        idx = sets.Count - 1;
                    Model.LoadSettings(sets[idx]);
                }
            }

            if (e.Key == ControlKeys.NextPresetKey)
            {
                var sets = Model.Presets.ToList();
                if (sets.Count > 1)
                {                   
                    int idx = sets.IndexOf(Model.CurrentSettingsFile) + 1;
                    if (idx > sets.Count-1)
                        idx = 0;
                    Model.LoadSettings(sets[idx]);
                }
            }

            if (e.Key == ControlKeys.ChangeRectColorKey)
            {
                ChangeTvBackBrush();
            }
            if (e.Key == ControlKeys.ChangeOverlayColorKey)
            {
                ChangeOverlayBrush();
            }
            if (e.Key == ControlKeys.SetupEffectKey)
            {
                Model.EditEffect();
            }
            if (e.Key == ControlKeys.SimulateTvKey)
            {
                ToogleSimulateTV();
            }

            if (e.Key == ControlKeys.IncrementOverlayOpacityKey && Model.OverlayOpacity < 1)
            {
                Model.OverlayOpacity += 0.1;
                Model.WriteMessage(string.Format(Properties.Resources.OpacityLevel, Model.OverlayOpacity));
            }

            if (e.Key == ControlKeys.DecrementOverlayOpacityKey && Model.OverlayOpacity > 0)
            {
                Model.OverlayOpacity -= 0.1;
                Model.WriteMessage(string.Format(Properties.Resources.OpacityLevel, Model.OverlayOpacity));
            }


            if (System.Windows.Forms.Control.IsKeyLocked(Keys.CapsLock))
            {
                if (e.Key == ControlKeys.RectMoveDownKey)
                {
                    var m = Model.Margin;
                    if(!Keyboard.IsKeyDown(Key.LeftCtrl))
                        m.Top -= 1;
                    if (!Keyboard.IsKeyDown(Key.LeftShift))                        
                        m.Bottom -= 1;
                    Model.Margin = m;
                }
                if (e.Key == ControlKeys.RectMoveUpKey)
                {
                    var m = Model.Margin;
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                        m.Top += 1;
                    if (!Keyboard.IsKeyDown(Key.LeftShift))
                        m.Bottom += 1;
                    Model.Margin = m;
                }

                if (e.Key == ControlKeys.RectMoveRightKey)
                {
                    var m = Model.Margin;
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                        m.Right -= 1;
                    if (!Keyboard.IsKeyDown(Key.LeftShift))
                        m.Left -= 1;
                    Model.Margin = m;
                }
                if (e.Key == ControlKeys.RectMoveLeftKey)
                {
                    var m = Model.Margin;
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                        m.Right += 1;
                    if (!Keyboard.IsKeyDown(Key.LeftShift))
                        m.Left += 1;
                    Model.Margin = m;
                }
            }

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) &&
                !Keyboard.IsKeyDown(Key.RightCtrl) &&
                !Keyboard.IsKeyDown(Key.RightShift) &&
                !Keyboard.IsKeyDown(Key.LeftShift))
            {
                if (e.Key == ControlKeys.RectMoveDownKey)
                    Model.TvTop++;
                if (e.Key == ControlKeys.RectMoveUpKey)
                    Model.TvTop--;
                if (e.Key == ControlKeys.RectMoveLeftKey)
                    Model.TvLeft--;
                if (e.Key == ControlKeys.RectMoveRightKey)
                    Model.TvLeft++;
            }
            else
            {
                if (e.Key == ControlKeys.RectMoveDownKey)
                    Model.TvHeight++;
                if (e.Key == ControlKeys.RectMoveUpKey)
                    Model.TvHeight--;
                if (e.Key == ControlKeys.RectMoveLeftKey)
                    Model.TvWidth--;
                if (e.Key == ControlKeys.RectMoveRightKey)
                    Model.TvWidth++;
            }
        }

		static TvRectView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TvRectView), new FrameworkPropertyMetadata(typeof(TvRectView)));
		}
	}
}
