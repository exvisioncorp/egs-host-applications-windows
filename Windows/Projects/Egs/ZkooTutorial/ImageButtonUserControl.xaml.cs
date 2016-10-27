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

    partial class ImageButtonUserControl : UserControl
    {
        public ImageButtonUserControl()
            : base()
        {
            InitializeComponent();
        }

        public void InitializeOnceAtStartup(ImageButtonModel model)
        {
            this.DataContext = model;
            thisButton.MouseEnter += (sender, e) => { model.IsHovered = true; };
            thisButton.MouseLeave += (sender, e) =>
            {
                model.IsHovered = false;
                // TODO: Confirm the necessity of the next line.  If it is enabled, the state can change from Pressed to Enabled, soon after users pressed.
                //model.IsPressed = false;
            };
            thisButton.PreviewMouseDown += (sender, e) => { model.IsPressed = true; };
            thisButton.PreviewMouseUp += (sender, e) => { model.IsPressed = false; };
        }
    }
}
