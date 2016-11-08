namespace Egs.DotNetUtility.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Diagnostics;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SliderOnDragCompleted : Slider
    {
#if false
        protected static void ValueOnThumbDragCompletedChangedForFramework(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = (SliderOnDragCompleted)sender;
            obj.OnValueChanged(obj.Value, obj.ValueOnThumbDragCompleted);
        }
        public static readonly DependencyProperty ValueOnThumbDragCompletedProperty = DependencyProperty.Register("ValueOnThumbDragCompleted", typeof(double), typeof(SliderOnDragCompleted), new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(ValueOnThumbDragCompletedChangedForFramework)));
        public double ValueOnThumbDragCompleted
        {
            get { return (double)GetValue(ValueOnThumbDragCompletedProperty); }
            set { SetValue(ValueOnThumbDragCompletedProperty, value); }
        }
#endif

        bool isToShowInfo = false;
        bool isThumbDragging = false;

        public SliderOnDragCompleted()
            : base()
        {
        }

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            if (isToShowInfo) { Debug.WriteLine("[Start] base.OnThumbDragStarted(e)"); }
            isThumbDragging = true;
            base.OnThumbDragStarted(e);
            if (isToShowInfo) { Debug.WriteLine("[  End] base.OnThumbDragStarted(e);"); }
        }

        //protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        //{
        //    if (isToShowInfo) { Debug.WriteLine("[Start] base.OnThumbDragDelta(e  Value:" + Value); }
        //    // Thumbが動かない。 if (isThumbDragging) { return; }
        //    // Thumbが動かない。 if (isThumbDragging) { e.Handled = true; return; }
        //    // 毎回Valueが更新されてしまう。 if (isThumbDragging) { e.Handled = true; }
        //    base.OnThumbDragDelta(e);
        //    if (isToShowInfo) { Debug.WriteLine("[  End] base.OnThumbDragDelta(e);"); }
        //}

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (isToShowInfo) { Debug.WriteLine("[Start] base.OnValueChanged(oldValue, newValue);  Value:" + Value + "  oldValue:" + oldValue + "  newValue:" + newValue); }
            //if (isThumbDragging == false) { ValueOnThumbDragCompleted = Value; }
            if (isThumbDragging) { return; }
            base.OnValueChanged(oldValue, newValue);
            if (isToShowInfo) { Debug.WriteLine("[  End] base.OnValueChanged(oldValue, newValue);"); }
        }

        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            if (isToShowInfo) { Debug.WriteLine("[Start] base.OnThumbDragCompleted(e);  Value:" + Value); }
            isThumbDragging = false;
            //ValueOnThumbDragCompleted = Value;
            base.OnThumbDragCompleted(e);
            if (isToShowInfo) { Debug.WriteLine("[  End] base.OnThumbDragCompleted(e);"); }
        }
    }
}
