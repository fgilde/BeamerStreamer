using System;
using System.Windows;
using System.Windows.Input;

namespace BeamerStreamer.Controls
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog
    {
        public string Value { get; set; }

        public InputDialog()
        {
            DataContext = this;
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                if (!textBox.IsReadOnly)
                {
                    textBox.SelectAll();
                    textBox.Focus();
                }
            };
            KeyDown += OnKeyDown;
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
            Value = string.Empty;
            DialogResult = false;
        }
    }
}
