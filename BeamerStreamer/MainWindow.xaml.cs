using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BeamerStreamer.Classes;
using BeamerStreamer.Controls;
using WPFMediaKit.DirectShow.Controls;
using WPFMediaKit.DirectShow.MediaPlayers;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;


namespace BeamerStreamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        internal static MainWindow Instance { get; private set; }
        public TvRectViewModel TvRectViewModel => TvRectView.DataContext as TvRectViewModel;

        #region Backingfields

        private string message;

        #endregion

        public string InputDeviceName
        {
            get { return Settings.Default.InputDeviceName; }
            set
            {
                Settings.Default.InputDeviceName = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            Message = Properties.Resources.NoSignalDetected;
            Instance = this;
            InitializeComponent();
            Loaded += OnLoaded;
            DataContext = this;
            spectrumAnalyzer.RegisterSoundPlayer(new MicrophoneSpectrum());
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ShowHelpMessage();
        }

        private async Task ShowHelpMessage()
        {
            Message += Environment.NewLine + Properties.Resources.HelpMessage;
            await Task.Delay(4000);
            Message = Message.Replace(Properties.Resources.HelpMessage, "");
            if (string.IsNullOrWhiteSpace(Message))
            {
                Message = null;
            }
        }


        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == ControlKeys.HelpKey)
            {
                ShowHelp();
            }
            if (e.Key == ControlKeys.InputSelectKey)
            {
                SelectVideoInput();
            }
            if (e.Key == ControlKeys.ToggleSoundBarKey)
            {
                ToggleInput(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
            }
            if (e.Key == ControlKeys.ToggleSoundEffectKey)
            {
                ToggleSpectrumEffect();
            }
            if (e.Key == ControlKeys.ExitApplicationKey)
            {
                Close();
                Application.Current.Shutdown();
            }
        }

        private void ShowHelp()
        {
            Task.Run(() => MessageBox.Show(Properties.Resources.SimpleHelp));
        }

        private void SelectVideoInput()
        {
            var dlg = new SelectDeviceDialog() { SelectedDevice = InputDeviceName };
            if (dlg.ShowDialog() ?? false)
            {
                InputDeviceName = dlg.SelectedDevice;
            }
        }

        internal void ToggleSpectrumEffect()
        {
            if (spectrumAnalyzer.Parent == effectGrid)
            {
                effectGrid.Children.Remove(spectrumAnalyzer);
                mainGrid.Children.Add(spectrumAnalyzer);
            }
            else
            {
                mainGrid.Children.Remove(spectrumAnalyzer);
                effectGrid.Children.Add(spectrumAnalyzer);
            }
        }

        private void ToggleInput(bool allowBoth)
        {
            if (!allowBoth)
                videoContainer.Visibility = videoContainer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            spectrumAnalyzer.Visibility = spectrumAnalyzer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void VideoCapElement_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            Message = Message.Replace(Properties.Resources.NoSignalDetected, "");
        }

        private void VideoCapElement_OnMediaFailed(object sender, MediaFailedEventArgs e)
        {
            Message = e.Message;
        }
    }
}
