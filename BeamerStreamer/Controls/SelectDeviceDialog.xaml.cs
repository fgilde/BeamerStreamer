using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WPFMediaKit.DirectShow.Controls;

namespace BeamerStreamer.Controls
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class SelectDeviceDialog
    {
        #region Dependency

        // Using a DependencyProperty as the backing store for SelectedDevice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDeviceProperty =
            DependencyProperty.Register("SelectedDevice", typeof (string), typeof (SelectDeviceDialog),
                new PropertyMetadata(""));

        #endregion
    
        public SelectDeviceDialog()
        {
            DataContext = this;
            AvailableDevices = new ObservableCollection<string>(MultimediaUtil.VideoInputNames);
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                listBoxDevices.Focus();
            };
            KeyDown += OnKeyDown;
        }

        public ObservableCollection<string> AvailableDevices { get; set; }

        public string SelectedDevice
        {
            get { return (string)GetValue(SelectedDeviceProperty); }
            set { SetValue(SelectedDeviceProperty, value); }
        }



        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                OkClick(sender, null);
            if (e.Key == Key.Escape)
                CancelClick(sender, null);
        }

        private void InputDialog_OnMouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                DragMove();            
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
