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

    partial class Tutorial01StartGestureTrainingPage : Page, IHasTutorialEachPageModelBase
    {
        public TutorialEachPageModelBase ReferenceToTutorialEachPageModelBase { get; set; }

        public Tutorial01StartGestureTrainingPage()
        {
            InitializeComponent();
        }

        public void InitializeOnceAtStartup(MainNavigationWindow navigator, Tutorial01StartGestureTrainingPageModel viewModel)
        {
            Trace.Assert(navigator != null);
            Trace.Assert(viewModel != null);


            // ---------- videos and state transition ----------
            PracticeSlideShow01VideoUserControl.SetMediaElementSourceUriByFilePath(System.IO.Path.Combine(viewModel.refToAppModel.CurrentResources.TutorialVideoFilesFolderPath, "tutorial_reference_video_1_1.mp4"), UriKind.Relative);
            PracticeSlideShow01VideoUserControl.IsVisibleChanged += (sender, e) =>
            {
                if (PracticeSlideShow01VideoUserControl.IsVisible) { PracticeSlideShow01VideoUserControl.Replay(); } else { PracticeSlideShow01VideoUserControl.Pause(); }
            };

            PracticeSlideShow02VideoUserControl.SetMediaElementSourceUriByFilePath(System.IO.Path.Combine(viewModel.refToAppModel.CurrentResources.TutorialVideoFilesFolderPath, "tutorial_reference_video_1_2.mp4"), UriKind.Relative);
            PracticeSlideShow02VideoUserControl.IsVisibleChanged += (sender, e) =>
            {
                if (PracticeSlideShow02VideoUserControl.IsVisible) { PracticeSlideShow02VideoUserControl.Replay(); } else { PracticeSlideShow02VideoUserControl.Pause(); }
            };
            // ---------- videos and state transition ----------


            this.DataContext = viewModel;
            ReferenceToTutorialEachPageModelBase = viewModel;
            ReplayPracticeNextButtonsUserControl.InitializeOnceAtStartup(viewModel.TutorialAppHeaderMenu);
            TutorialAppHeaderMenuUserControl.InitializeOnceAtStartup(viewModel.TutorialAppHeaderMenu);

            this.Loaded += (sender, e) =>
            {
                navigator.SetWindowFullScreen();
                navigator.Title = "ZKOO Tutorial: Start Gesture Practice";
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
