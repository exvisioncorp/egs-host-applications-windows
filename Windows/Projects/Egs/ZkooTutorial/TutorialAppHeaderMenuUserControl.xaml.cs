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

    partial class TutorialAppHeaderMenuUserControl : UserControl
    {
        public TutorialAppHeaderMenuUserControl()
        {
            InitializeComponent();
        }

        public void InitializeOnceAtStartup(TutorialAppHeaderMenuViewModel model)
        {
            this.DataContext = model;
            MenuStartButtonUserControl.InitializeOnceAtStartup(model.MenuStartButtonModel);
            MenuMoveButtonUserControl.InitializeOnceAtStartup(model.MenuMoveButtonModel);
            MenuTapButtonUserControl.InitializeOnceAtStartup(model.MenuTapButtonModel);
            MenuDragButtonUserControl.InitializeOnceAtStartup(model.MenuDragButtonModel);
            MenuFlickButtonUserControl.InitializeOnceAtStartup(model.MenuFlickButtonModel);
            MenuMoreButtonUserControl.InitializeOnceAtStartup(model.MenuMoreButtonModel);
            MenuReplayButtonUserControl.InitializeOnceAtStartup(model.MenuReplayButtonModel);
            MenuPracticeButtonUserControl.InitializeOnceAtStartup(model.MenuPracticeButtonModel);
            MenuNextButtonUserControl.InitializeOnceAtStartup(model.MenuNextButtonModel);
            MenuExitButtonUserControl.InitializeOnceAtStartup(model.MenuExitButtonModel);
        }
    }
}
