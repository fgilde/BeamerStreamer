using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BeamerStreamer
{
    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += MoveThumbDragDelta;
        }

        private void MoveThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as Control;

            if (designerItem != null)
            {
                double left = Canvas.GetLeft(designerItem);
                double top = Canvas.GetTop(designerItem);

                Canvas.SetLeft(designerItem, left + e.HorizontalChange);
                Canvas.SetTop(designerItem, top + e.VerticalChange);
            }
        }
    }
}
