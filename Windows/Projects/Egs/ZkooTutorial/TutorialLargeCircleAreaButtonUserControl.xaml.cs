namespace Egs.ZkooTutorial
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

    partial class TutorialLargeCircleAreaButtonUserControl : UserControl
    {
        public TutorialLargeCircleAreaButtonUserControl()
            : base()
        {
            InitializeComponent();
        }

        public void InitializeOnceAtStartup(TutorialLargeCircleAreaButtonModel model)
        {
            this.DataContext = model;
            thisButton.MouseEnter += (sender, e) => { model.IsHovered = true; };
            thisButton.MouseLeave += (sender, e) =>
            {
                model.IsHovered = false;
                // TODO: Check that it should be commented out or not.  Maybe no problems.
                //model.IsPressed = false;
            };
            thisButton.PreviewMouseDown += (sender, e) => { model.IsPressed = true; };
            thisButton.PreviewMouseUp += (sender, e) => { model.IsPressed = false; };
            thisButton.Click += (sender, e) => { model.TapsCount++; };
            thisButton.PreviewMouseRightButtonDown += (sender, e) => { model.LongTapsCount++; };
        }
    }
}
