namespace Egs.ZkooTutorial
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Threading;
    using System.Windows.Controls;

    partial class VideoPlayingPage : Page
    {
        public string PageTitle { get; set; }
        public string VideoFileNameWithoutDirectory { get; internal set; }
        bool IsMouseHoveredOnTutorialAppHeaderMenuUserControl { get; set; }

        MainNavigationWindow refToNavigator { get; set; }
        ZkooTutorialModel refToAppModel { get; set; }

        int MouseCursorPositionY { get; set; }
        void UpdateAppHeaderMenuVisibility()
        {
            var newBoolValue = IsMouseHoveredOnTutorialAppHeaderMenuUserControl || (videoPlayingUserControl.IsPlaying == false);
            var newOpacity = newBoolValue ? 1.0 : 0.0;
            if (TutorialAppHeaderMenuUserControl.Opacity != newOpacity) { TutorialAppHeaderMenuUserControl.Opacity = newOpacity; }
        }

        public VideoPlayingPage()
        {
            InitializeComponent();
            PageTitle = "";
            VideoFileNameWithoutDirectory = "";
            MouseCursorPositionY = 200;
            TutorialAppHeaderMenuUserControl.Visibility = Visibility.Visible;
            TutorialAppHeaderMenuUserControl.Opacity = 1.0;
            ReplayPracticeNextButtonsUserControl.Visibility = Visibility.Collapsed;

            ReplayPracticeNextButtonsUserControl.DialogNextButtonUserControl.Visibility = Visibility.Collapsed;
        }

        public void InitializeOnceAtStartup(MainNavigationWindow navigator, ZkooTutorialModel appModel)
        {
            Trace.Assert(navigator != null);
            Trace.Assert(appModel != null);
            refToNavigator = navigator;
            refToAppModel = appModel;

            this.DataContext = refToAppModel;

            ReplayPracticeNextButtonsUserControl.InitializeOnceAtStartup(appModel.TutorialAppHeaderMenu);
            TutorialAppHeaderMenuUserControl.InitializeOnceAtStartup(appModel.TutorialAppHeaderMenu);

            this.PreviewMouseMove += (sender, e) =>
            {
                MouseCursorPositionY = (int)e.GetPosition(this).Y;
                UpdateAppHeaderMenuVisibility();
            };

            TutorialAppHeaderMenuUserControl.MouseEnter += (sender, e) =>
            {
                IsMouseHoveredOnTutorialAppHeaderMenuUserControl = true;
                UpdateAppHeaderMenuVisibility();
            };
            TutorialAppHeaderMenuUserControl.MouseLeave += (sender, e) =>
            {
                IsMouseHoveredOnTutorialAppHeaderMenuUserControl = false;
                UpdateAppHeaderMenuVisibility();
            };

            videoPlayingUserControl.IsPlayingChanged += (sender, e) =>
            {
                UpdateAppHeaderMenuVisibility();
                ReplayPracticeNextButtonsUserControl.Visibility = videoPlayingUserControl.IsPlaying ? Visibility.Collapsed : Visibility.Visible;
            };

            this.Loaded += (sender, e) =>
            {
                // NOTE: This does not change device settings and so on.
                // NOTE: Called from asynchronous Task.Run() called from each Page's Loaded event.
                refToNavigator.Title = this.PageTitle;
                refToNavigator.SetWindowFullScreen();
                refToAppModel.EnableUpdatingCameraViewImageButHideWindow();
                videoPlayingUserControl.SetMediaElementSourceUriByFilePath(Path.Combine(refToAppModel.CurrentResources.TutorialVideoFilesFolderPath, VideoFileNameWithoutDirectory), UriKind.Relative);
                videoPlayingUserControl.Replay();
            };
        }
    }
}
