namespace Egs.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Diagnostics;
    using Egs.DotNetUtility;

    public partial class CameraViewUserControl : UserControl
    {
        CameraViewUserControlModel cameraViewUserControlModel { get; set; }

        public Thickness MarginBetweenUserControlAndContent
        {
            get
            {
                return new Thickness(
                    cameraViewBorder.BorderThickness.Left + cameraViewBorder.Margin.Left,
                    cameraViewBorder.BorderThickness.Top + cameraViewBorder.Margin.Top,
                    cameraViewBorder.BorderThickness.Right + cameraViewBorder.Margin.Right,
                    cameraViewBorder.BorderThickness.Bottom + cameraViewBorder.Margin.Bottom);
            }
        }

        public CameraViewUserControl()
        {
            InitializeComponent();

            if (ApplicationCommonSettings.IsDebugging)
            {
                cameraDeviceIsDisconnectedMessageGrid.IsVisibleChanged += delegate
                {
                    Debug.WriteLine("cameraDeviceIsDisconnectedMessageGrid.IsVisible: " + cameraDeviceIsDisconnectedMessageGrid.IsVisible);
                };
            }
        }

        public void InitializeOnceAtStartup(CameraViewUserControlModel viewModel)
        {
            Trace.Assert(viewModel != null);

            cameraViewUserControlModel = viewModel;
            this.DataContext = viewModel;
        }
    }
}
