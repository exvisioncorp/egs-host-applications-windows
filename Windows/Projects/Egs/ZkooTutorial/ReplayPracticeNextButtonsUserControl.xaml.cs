namespace Egs.ZkooTutorial
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

    partial class ReplayPracticeNextButtonsUserControl : UserControl
    {
        public ReplayPracticeNextButtonsUserControl()
        {
            InitializeComponent();
        }

        public void InitializeOnceAtStartup(TutorialAppHeaderMenuViewModel model)
        {
            this.DataContext = model;
            DialogReplayButtonUserControl.InitializeOnceAtStartup(model.DialogReplayButtonModel);
            DialogPracticeButtonUserControl.InitializeOnceAtStartup(model.DialogPracticeButtonModel);
            DialogNextButtonUserControl.InitializeOnceAtStartup(model.DialogNextButtonModel);
        }
    }
}
