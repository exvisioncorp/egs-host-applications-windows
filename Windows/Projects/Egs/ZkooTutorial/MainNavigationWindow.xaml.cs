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
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Windows.Navigation;
    using System.Windows.Threading;

    partial class MainNavigationWindow : NavigationWindow
    {
        public LauncherPage LauncherView { get; private set; }
        public VideoPlayingPage Tutorial00ZkooSetupTutorialVideo { get; private set; }
        public VideoPlayingPage Tutorial01StartGestureTutorialVideo { get; private set; }
        public Tutorial01StartGestureTrainingPage Tutorial01StartGestureTraining { get; private set; }
        public VideoPlayingPage Tutorial02MoveCursorTutorialVideo { get; private set; }
        public Tutorial02MoveCursorTrainingPage Tutorial02MoveCursorTraining { get; private set; }
        public VideoPlayingPage Tutorial03TapGestureTutorialVideo { get; private set; }
        public Tutorial03TapGestureTrainingPage Tutorial03TapGestureTraining { get; private set; }
        public VideoPlayingPage Tutorial04DragGestureTutorialVideo { get; private set; }
        public Tutorial04DragGestureTrainingPage Tutorial04DragGestureTraining { get; private set; }
        public VideoPlayingPage Tutorial05FlickGestureTutorialVideo { get; private set; }
        public Tutorial05FlickGestureTrainingPage Tutorial05FlickGestureTraining { get; private set; }
        public VideoPlayingPage Tutorial06BothHandsGestureTutorialVideo { get; private set; }

        ZkooTutorialModel refToZkooTutorialModel { get; set; }
        DispatcherTimer cursorMonitorTimer { get; set; }

        void DoNothingCommandBindingEventHandler(object sender, ExecutedRoutedEventArgs e) { }

        public MainNavigationWindow()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(NavigationCommands.Refresh, DoNothingCommandBindingEventHandler));
            this.CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack, DoNothingCommandBindingEventHandler));
            this.CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseForward, DoNothingCommandBindingEventHandler));

            // If it sets WindowState to Maximized, the size of the movie can be too big in multiple monitors.
            this.WindowState = WindowState.Normal;
            // If it changes WindowStyle in each page, the layout becomes strange when users change Page.
            this.WindowStyle = WindowStyle.None;

            LauncherView = new LauncherPage();
            Tutorial00ZkooSetupTutorialVideo = new VideoPlayingPage();
            Tutorial00ZkooSetupTutorialVideo.ReplayPracticeNextButtonsUserControl.DialogPracticeButtonUserControl.Visibility = Visibility.Collapsed;
            Tutorial00ZkooSetupTutorialVideo.ReplayPracticeNextButtonsUserControl.DialogNextButtonUserControl.Visibility = Visibility.Visible;

            Tutorial01StartGestureTutorialVideo = new VideoPlayingPage();
            Tutorial01StartGestureTraining = new Tutorial01StartGestureTrainingPage();
            Tutorial02MoveCursorTutorialVideo = new VideoPlayingPage();
            Tutorial02MoveCursorTraining = new Tutorial02MoveCursorTrainingPage();
            Tutorial03TapGestureTutorialVideo = new VideoPlayingPage();
            Tutorial03TapGestureTraining = new Tutorial03TapGestureTrainingPage();
            Tutorial04DragGestureTutorialVideo = new VideoPlayingPage();
            Tutorial04DragGestureTraining = new Tutorial04DragGestureTrainingPage();
            Tutorial05FlickGestureTutorialVideo = new VideoPlayingPage();
            Tutorial05FlickGestureTraining = new Tutorial05FlickGestureTrainingPage();

            Tutorial06BothHandsGestureTutorialVideo = new VideoPlayingPage();
            Tutorial06BothHandsGestureTutorialVideo.ReplayPracticeNextButtonsUserControl.DialogPracticeButtonUserControl.Visibility = Visibility.Collapsed;
            Tutorial06BothHandsGestureTutorialVideo.ReplayPracticeNextButtonsUserControl.DialogNextButtonUserControl.Visibility = Visibility.Collapsed;

            Tutorial00ZkooSetupTutorialVideo.VideoFileNameWithoutDirectory = "tutorial_fullscreen_video_0.mp4";
            Tutorial01StartGestureTutorialVideo.VideoFileNameWithoutDirectory = "tutorial_fullscreen_video_1.mp4";
            Tutorial02MoveCursorTutorialVideo.VideoFileNameWithoutDirectory = "tutorial_fullscreen_video_2.mp4";
            Tutorial03TapGestureTutorialVideo.VideoFileNameWithoutDirectory = "tutorial_fullscreen_video_3.mp4";
            Tutorial04DragGestureTutorialVideo.VideoFileNameWithoutDirectory = "tutorial_fullscreen_video_4.mp4";
            Tutorial05FlickGestureTutorialVideo.VideoFileNameWithoutDirectory = "tutorial_fullscreen_video_5.mp4";
            Tutorial06BothHandsGestureTutorialVideo.VideoFileNameWithoutDirectory = "tutorial_fullscreen_video_6.mp4";

            cursorMonitorTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(100) };
            this.Topmost = !(ApplicationCommonSettings.IsDebugging);
        }

        public void InitializeOnceAtStartup(ZkooTutorialModel zkooTutorialModel)
        {
            Trace.Assert(zkooTutorialModel != null);
            refToZkooTutorialModel = zkooTutorialModel;

            LauncherView.InitializeOnceAtStartup(this, refToZkooTutorialModel);
            Tutorial00ZkooSetupTutorialVideo.InitializeOnceAtStartup(this, refToZkooTutorialModel);
            Tutorial01StartGestureTutorialVideo.InitializeOnceAtStartup(this, refToZkooTutorialModel);
            Tutorial01StartGestureTraining.InitializeOnceAtStartup(this, refToZkooTutorialModel.Tutorial01StartGestureTraining);
            Tutorial02MoveCursorTutorialVideo.InitializeOnceAtStartup(this, refToZkooTutorialModel);
            Tutorial02MoveCursorTraining.InitializeOnceAtStartup(this, refToZkooTutorialModel.Tutorial02MoveCursorTraining);
            Tutorial03TapGestureTutorialVideo.InitializeOnceAtStartup(this, refToZkooTutorialModel);
            Tutorial03TapGestureTraining.InitializeOnceAtStartup(this, refToZkooTutorialModel.Tutorial03TapGestureTraining);
            Tutorial04DragGestureTutorialVideo.InitializeOnceAtStartup(this, refToZkooTutorialModel);
            Tutorial04DragGestureTraining.InitializeOnceAtStartup(this, refToZkooTutorialModel.Tutorial04DragGestureTraining);
            Tutorial05FlickGestureTutorialVideo.InitializeOnceAtStartup(this, refToZkooTutorialModel);
            Tutorial05FlickGestureTraining.InitializeOnceAtStartup(this, refToZkooTutorialModel.Tutorial05FlickGestureTraining);
            Tutorial06BothHandsGestureTutorialVideo.InitializeOnceAtStartup(this, refToZkooTutorialModel);

            UpdatePagesSize();

            Tutorial00ZkooSetupTutorialVideo.Loaded += delegate { OnTutorial00Moved(); };
            Tutorial01StartGestureTutorialVideo.Loaded += delegate { OnTutorial01Moved(); };
            Tutorial01StartGestureTraining.Loaded += delegate { OnTutorial01Moved(); };
            Tutorial02MoveCursorTutorialVideo.Loaded += delegate { OnTutorial02Moved(); };
            Tutorial02MoveCursorTraining.Loaded += delegate { OnTutorial02Moved(); };
            Tutorial03TapGestureTutorialVideo.Loaded += delegate { OnTutorial03Moved(); };
            Tutorial03TapGestureTraining.Loaded += delegate { OnTutorial03Moved(); };
            Tutorial04DragGestureTutorialVideo.Loaded += delegate { OnTutorial04Moved(); };
            Tutorial04DragGestureTraining.Loaded += delegate { OnTutorial04Moved(); };
            Tutorial05FlickGestureTutorialVideo.Loaded += delegate { OnTutorial05Moved(); };
            Tutorial05FlickGestureTraining.Loaded += delegate { OnTutorial05Moved(); };
            Tutorial06BothHandsGestureTutorialVideo.Loaded += delegate { OnTutorial06Moved(); };

            this.KeyUp += (sender, e) =>
            {
                if (true) { if (e.Key == Key.Escape) { ExitTutorial(); } }
            };

            this.Loaded += (sender, e) =>
            {
                refToZkooTutorialModel.EnableUpdatingCameraViewImageButHideWindow();
            };

            cursorMonitorTimer.Tick += (sender, e) =>
            {
                if (refToZkooTutorialModel.RefToHostApp.OnePersonBothHandsViewModel == null) { return; }
                var hand = refToZkooTutorialModel.RefToHostApp.OnePersonBothHandsViewModel.FirstFoundHand;
                if (hand == null || hand.IsTracking == false) { return; }
                NativeMethods.SetCursorPos((int)hand.PositionX, (int)hand.PositionY);
            };
            cursorMonitorTimer.Start();
        }

        void ClearAllTutorialButtonsSelected()
        {
            foreach (var item in refToZkooTutorialModel.TutorialAppHeaderMenu.ImageButtonModelList) { item.IsSelected = false; }
        }

        void OnTutorial00Moved()
        {
            ClearAllTutorialButtonsSelected();
            refToZkooTutorialModel.TutorialAppHeaderMenu.ReplayButtonDestinationPage = Tutorial00ZkooSetupTutorialVideo;
            refToZkooTutorialModel.TutorialAppHeaderMenu.PracticeButtonDestinationPage = Tutorial01StartGestureTutorialVideo;
            refToZkooTutorialModel.TutorialAppHeaderMenu.NextButtonDestinationPage = Tutorial01StartGestureTutorialVideo;
        }
        void OnTutorial01Moved()
        {
            ClearAllTutorialButtonsSelected();
            refToZkooTutorialModel.TutorialAppHeaderMenu.MenuStartButtonModel.IsSelected = true;
            refToZkooTutorialModel.TutorialAppHeaderMenu.ReplayButtonDestinationPage = Tutorial01StartGestureTutorialVideo;
            refToZkooTutorialModel.TutorialAppHeaderMenu.PracticeButtonDestinationPage = Tutorial01StartGestureTraining;
            refToZkooTutorialModel.TutorialAppHeaderMenu.NextButtonDestinationPage = Tutorial02MoveCursorTutorialVideo;
        }
        void OnTutorial02Moved()
        {
            ClearAllTutorialButtonsSelected();
            refToZkooTutorialModel.TutorialAppHeaderMenu.MenuMoveButtonModel.IsSelected = true;
            refToZkooTutorialModel.TutorialAppHeaderMenu.ReplayButtonDestinationPage = Tutorial02MoveCursorTutorialVideo;
            refToZkooTutorialModel.TutorialAppHeaderMenu.PracticeButtonDestinationPage = Tutorial02MoveCursorTraining;
            refToZkooTutorialModel.TutorialAppHeaderMenu.NextButtonDestinationPage = Tutorial03TapGestureTutorialVideo;
        }
        void OnTutorial03Moved()
        {
            ClearAllTutorialButtonsSelected();
            refToZkooTutorialModel.TutorialAppHeaderMenu.MenuTapButtonModel.IsSelected = true;
            refToZkooTutorialModel.TutorialAppHeaderMenu.ReplayButtonDestinationPage = Tutorial03TapGestureTutorialVideo;
            refToZkooTutorialModel.TutorialAppHeaderMenu.PracticeButtonDestinationPage = Tutorial03TapGestureTraining;
            refToZkooTutorialModel.TutorialAppHeaderMenu.NextButtonDestinationPage = Tutorial04DragGestureTutorialVideo;
        }
        void OnTutorial04Moved()
        {
            ClearAllTutorialButtonsSelected();
            refToZkooTutorialModel.TutorialAppHeaderMenu.MenuDragButtonModel.IsSelected = true;
            refToZkooTutorialModel.TutorialAppHeaderMenu.ReplayButtonDestinationPage = Tutorial04DragGestureTutorialVideo;
            refToZkooTutorialModel.TutorialAppHeaderMenu.PracticeButtonDestinationPage = Tutorial04DragGestureTraining;
            refToZkooTutorialModel.TutorialAppHeaderMenu.NextButtonDestinationPage = Tutorial05FlickGestureTutorialVideo;
        }
        void OnTutorial05Moved()
        {
            ClearAllTutorialButtonsSelected();
            refToZkooTutorialModel.TutorialAppHeaderMenu.MenuFlickButtonModel.IsSelected = true;
            refToZkooTutorialModel.TutorialAppHeaderMenu.ReplayButtonDestinationPage = Tutorial05FlickGestureTutorialVideo;
            refToZkooTutorialModel.TutorialAppHeaderMenu.PracticeButtonDestinationPage = Tutorial05FlickGestureTraining;
            refToZkooTutorialModel.TutorialAppHeaderMenu.NextButtonDestinationPage = Tutorial06BothHandsGestureTutorialVideo;
        }
        void OnTutorial06Moved()
        {
            ClearAllTutorialButtonsSelected();
            refToZkooTutorialModel.TutorialAppHeaderMenu.MenuMoreButtonModel.IsSelected = true;
            refToZkooTutorialModel.TutorialAppHeaderMenu.ReplayButtonDestinationPage = Tutorial06BothHandsGestureTutorialVideo;
            refToZkooTutorialModel.TutorialAppHeaderMenu.PracticeButtonDestinationPage = Tutorial06BothHandsGestureTutorialVideo;
            if (ZkooTutorialModel.IsToExitApplicationOrElseNavigateToLauncherWhenTutorialExit)
            {
                refToZkooTutorialModel.TutorialAppHeaderMenu.NextButtonDestinationPage = Tutorial06BothHandsGestureTutorialVideo;
            }
            else
            {
                refToZkooTutorialModel.TutorialAppHeaderMenu.NextButtonDestinationPage = LauncherView;
            }
        }

        internal void OnDisplaySettingsChanged(object sender, EventArgs e)
        {
            UpdatePagesSize();
        }
        internal void UpdatePagesSize()
        {
            // TODO: Should test in changing DPI.  Now it may be OK.
            // MUSTDO: PrimaryScreenWidth/Height is in logical pixel coordinate (in normal settings).
            this.MaxWidth = SystemParameters.PrimaryScreenWidth;
            this.MaxHeight = SystemParameters.PrimaryScreenHeight;
            Tutorial00ZkooSetupTutorialVideo.Width = SystemParameters.PrimaryScreenWidth; Tutorial00ZkooSetupTutorialVideo.Height = SystemParameters.PrimaryScreenHeight;
            Tutorial01StartGestureTutorialVideo.Width = SystemParameters.PrimaryScreenWidth; Tutorial01StartGestureTutorialVideo.Height = SystemParameters.PrimaryScreenHeight;
            Tutorial01StartGestureTraining.Width = SystemParameters.PrimaryScreenWidth; Tutorial01StartGestureTraining.Height = SystemParameters.PrimaryScreenHeight;
            Tutorial02MoveCursorTutorialVideo.Width = SystemParameters.PrimaryScreenWidth; Tutorial02MoveCursorTutorialVideo.Height = SystemParameters.PrimaryScreenHeight;
            Tutorial02MoveCursorTraining.Width = SystemParameters.PrimaryScreenWidth; Tutorial02MoveCursorTraining.Height = SystemParameters.PrimaryScreenHeight;
            Tutorial03TapGestureTutorialVideo.Width = SystemParameters.PrimaryScreenWidth; Tutorial03TapGestureTutorialVideo.Height = SystemParameters.PrimaryScreenHeight;
            Tutorial03TapGestureTraining.Width = SystemParameters.PrimaryScreenWidth; Tutorial03TapGestureTraining.Height = SystemParameters.PrimaryScreenHeight;
            Tutorial04DragGestureTutorialVideo.Width = SystemParameters.PrimaryScreenWidth; Tutorial04DragGestureTutorialVideo.Height = SystemParameters.PrimaryScreenHeight;
            Tutorial04DragGestureTraining.Width = SystemParameters.PrimaryScreenWidth; Tutorial04DragGestureTraining.Height = SystemParameters.PrimaryScreenHeight;
            Tutorial05FlickGestureTutorialVideo.Width = SystemParameters.PrimaryScreenWidth; Tutorial05FlickGestureTutorialVideo.Height = SystemParameters.PrimaryScreenHeight;
            Tutorial05FlickGestureTraining.Width = SystemParameters.PrimaryScreenWidth; Tutorial05FlickGestureTraining.Height = SystemParameters.PrimaryScreenHeight;
            Tutorial06BothHandsGestureTutorialVideo.Width = SystemParameters.PrimaryScreenWidth; Tutorial06BothHandsGestureTutorialVideo.Height = SystemParameters.PrimaryScreenHeight;
        }

        public void SetWindowFullScreen()
        {
            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
        }

        public void SetWindowPositionToCenterOfTheScreen()
        {
            Left = (SystemParameters.PrimaryScreenWidth - this.ActualWidth) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - this.ActualHeight) / 2;
        }

        public void StartTutorial()
        {
            refToZkooTutorialModel.RefToHostApp.SettingsWindow.Visibility = Visibility.Hidden;
            this.Visibility = Visibility.Visible;
            Navigate((Page)Tutorial00ZkooSetupTutorialVideo);
            if (false)
            {
                Navigate((Page)Tutorial01StartGestureTutorialVideo);
                Navigate((Page)Tutorial02MoveCursorTutorialVideo);
                Navigate((Page)Tutorial03TapGestureTutorialVideo);
                Navigate((Page)Tutorial04DragGestureTutorialVideo);
                Navigate((Page)Tutorial05FlickGestureTutorialVideo);
            }
        }

        public void ExitTutorial()
        {
            Navigate(null);
            this.Visibility = Visibility.Collapsed;

            var dialog = new DialogOnTutorialExitingWindow();

            dialog.TutorialExitingWindowCheckBox.IsChecked = refToZkooTutorialModel.RefToHostApp.IsToStartTutorialWhenHostApplicationStart;
            dialog.ShowDialog();
            var hr = dialog.TutorialExitingWindowCheckBox.IsChecked;
            refToZkooTutorialModel.RefToHostApp.IsToStartTutorialWhenHostApplicationStart = hr.HasValue ? hr.Value : true;

            refToZkooTutorialModel.EnableUpdatingCameraViewImageAndShowWindow();
        }
    }
}
