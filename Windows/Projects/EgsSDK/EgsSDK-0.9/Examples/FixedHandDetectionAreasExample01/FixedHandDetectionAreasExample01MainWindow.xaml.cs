namespace FixedHandDetectionAreasExample01
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    public partial class FixedHandDetectionAreasExample01MainWindow : Window
    {
        public FixedHandDetectionAreasExample01MainWindow()
        {
            InitializeComponent();

            this.MouseLeftButtonDown += (sender, e) =>
            {
                if (e.ButtonState != MouseButtonState.Pressed) { return; }
                this.DragMove();
            };
        }
    }
}
