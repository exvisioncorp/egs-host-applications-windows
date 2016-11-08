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
    using System.Diagnostics;

    partial class Tutorial03TapGestureTrainingPage : Page, IHasTutorialEachPageModelBase
    {
        public TutorialEachPageModelBase ReferenceToTutorialEachPageModelBase { get; set; }

        public Tutorial03TapGestureTrainingPage()
        {
            InitializeComponent();
        }

        public void InitializeOnceAtStartup(MainNavigationWindow navigator, Tutorial03TapGestureTrainingPageModel viewModel)
        {
            Trace.Assert(navigator != null);
            Trace.Assert(viewModel != null);


            // ---------- videos and state transition ----------
            PracticeSlideShow01VideoUserControl.SetMediaElementSourceUriByFilePath(System.IO.Path.Combine(viewModel.refToAppModel.CurrentResources.TutorialVideoFilesFolderPath, "tutorial_reference_video_3.mp4"), UriKind.Relative);
            PracticeSlideShow01VideoUserControl.IsVisibleChanged += (sender, e) =>
            {
                if (PracticeSlideShow01VideoUserControl.IsVisible) { PracticeSlideShow01VideoUserControl.Replay(); } else { PracticeSlideShow01VideoUserControl.Pause(); }
            };
            // ---------- videos and state transition ----------


            this.DataContext = viewModel;
            ReferenceToTutorialEachPageModelBase = viewModel;
            ReplayPracticeNextButtonsUserControl.InitializeOnceAtStartup(viewModel.TutorialAppHeaderMenu);
            TutorialAppHeaderMenuUserControl.InitializeOnceAtStartup(viewModel.TutorialAppHeaderMenu);

            LargeCircleAreaRightTop.InitializeOnceAtStartup(viewModel.TutorialLargeCircleAreaButtonRightTop);
            LargeCircleAreaRightBottom.InitializeOnceAtStartup(viewModel.TutorialLargeCircleAreaButtonRightBottom);
            LargeCircleAreaLeftBottom.InitializeOnceAtStartup(viewModel.TutorialLargeCircleAreaButtonLeftBottom);
            LargeCircleAreaLeftTop.InitializeOnceAtStartup(viewModel.TutorialLargeCircleAreaButtonLeftTop);

            this.Loaded += (sender, e) =>
            {
                navigator.SetWindowFullScreen();
                navigator.Title = "ZKOO Tutorial: Tap Gesture Practice";
                viewModel.OnLoaded();
                PracticeSlideShow01VideoUserControl.Replay();
            };
            this.Unloaded += (sender, e) =>
            {
                viewModel.IsCancelling = true;
            };
        }
    }
}
