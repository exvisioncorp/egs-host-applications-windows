namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    class DragAndDropBehaviour
    {
        // Written by Zoetrope
        // http://zoetrope.hatenablog.jp/entry/20090619/1245420512
        internal static string GetReferenceUrl { get { return @"http://zoetrope.hatenablog.jp/entry/20090619/1245420512"; } }

        internal static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DragAndDropBehaviour), new UIPropertyMetadata(false, OnIsEnabledChanged));
        internal static bool GetIsEnabled(Thumb thumb) { return (bool)thumb.GetValue(IsEnabledProperty); }
        internal static void SetIsEnabled(Thumb thumb, bool value) { thumb.SetValue(IsEnabledProperty, value); }

        static void OnIsEnabledChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Thumb thumb = depObj as Thumb;
            if (thumb == null) { return; }
            if (e.NewValue is bool == false) { return; }

            bool isEnabled = (bool)e.NewValue;
            if (isEnabled)
            {
                thumb.DragStarted += ThumbDragStarted;
                thumb.DragDelta += ThumbDragDelta;
                thumb.DragCompleted += ThumbDragCompleted;
            }
            else
            {
                thumb.DragStarted -= ThumbDragStarted;
                thumb.DragDelta -= ThumbDragDelta;
                thumb.DragCompleted -= ThumbDragCompleted;
            }
        }

        static void ThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            if (thumb == null)
            {
                return;
            }
            thumb.Cursor = Cursors.Hand;
        }

        static void ThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            if (thumb == null)
            {
                return;
            }
            thumb.Cursor = Cursors.Arrow;
        }

        static void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            if (thumb == null)
            {
                return;
            }

            double left = (double)thumb.GetValue(Canvas.LeftProperty);
            double top = (double)thumb.GetValue(Canvas.TopProperty);

            thumb.SetValue(Canvas.LeftProperty, left + e.HorizontalChange);
            thumb.SetValue(Canvas.TopProperty, top + e.VerticalChange);
        }
    }
}
