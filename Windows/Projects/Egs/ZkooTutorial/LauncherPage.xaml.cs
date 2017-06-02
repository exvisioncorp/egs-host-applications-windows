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
    using System.Windows.Media.Animation;
    using System.Diagnostics;

    partial class LauncherPage : Page
    {
        MainNavigationWindow refToNavigator { get; set; }
        ZkooTutorialModel refToAppModel { get; set; }
        LauncherPageModel refToViewModel { get { return refToAppModel.Launcher; } }

        public LauncherPage()
        {
            InitializeComponent();
        }

        internal void InitializeOnceAtStartup(MainNavigationWindow navigator, ZkooTutorialModel appModel)
        {
            Trace.Assert(navigator != null);
            Trace.Assert(appModel != null);
            refToNavigator = navigator;
            refToAppModel = appModel;

            this.DataContext = refToAppModel;

            beginnerModeButton.MouseEnter += delegate
            {
                refToViewModel.CursorOveringLauncherButtonSummary = refToViewModel.BeginnerModeButtonModel;
                refToViewModel.CursorSpeedAndPrecisionModeAndRecommendedApps = refToViewModel.BeginnerModeDetail;
            };
            beginnerModeButton.Click += delegate
            {
                ShrinkAdvancedButtons();
                refToViewModel.IsExtractedAdvancedMode = false;
            };

            standardModeButton.MouseEnter += delegate
            {
                refToViewModel.CursorOveringLauncherButtonSummary = refToViewModel.StandardModeButtonModel;
                refToViewModel.CursorSpeedAndPrecisionModeAndRecommendedApps = refToViewModel.StandardModeDetail;
            };

#if IsToUseAdvancedButtonsInLauncher
            advancedButton.MouseEnter += delegate { refToViewModel.CursorOveringLauncherButtonSummary = refToViewModel.AdvancedButtonModel; };
            advancedButton.Click += delegate { ExtractAdvancedButtons(); };

            highSpeedModeButton.MouseEnter += delegate
            {
                refToViewModel.CursorOveringLauncherButtonSummary = refToViewModel.HighSpeedModeButtonModel;
                refToViewModel.CursorSpeedAndPrecisionModeAndRecommendedApps = refToViewModel.HighSpeedModeDetail;
            };
            highSpeedModeButton.Click += delegate
            {
                ExtractAdvancedButtons();
                refToViewModel.IsExtractedAdvancedMode = true;
            };

            highPrecisionModeButton.MouseEnter += delegate
            {
                refToViewModel.CursorOveringLauncherButtonSummary = refToViewModel.HighPrecisionModeButtonModel;
                refToViewModel.CursorSpeedAndPrecisionModeAndRecommendedApps = refToViewModel.HighPrecisionModeDetail;
            };
            highPrecisionModeButton.Click += delegate
            {
                ExtractAdvancedButtons();
                refToViewModel.IsExtractedAdvancedMode = true;
            };
#endif

            tutorialVideoButton.MouseEnter += delegate { refToViewModel.CursorOveringLauncherButtonSummary = refToViewModel.TutorialVideoButtonModel; };
            tutorialAppButton.MouseEnter += delegate { refToViewModel.CursorOveringLauncherButtonSummary = refToViewModel.TutorialAppButtonModel; };
            tutorialAppButton.Click += delegate { NavigationService.Navigate(refToNavigator.Tutorial01StartGestureTutorialVideo); };
            exitButton.MouseEnter += delegate { refToViewModel.CursorOveringLauncherButtonSummary = refToViewModel.ExitButtonModel; };
            exitButton.Click += delegate { refToNavigator.ExitTutorial(); };



            this.Loaded += delegate
            {
                refToAppModel.EnableUpdatingCameraViewImageAndShowWindow();
                refToNavigator.Title = $"{ApplicationCommonSettings.HostApplicationName} by {ApplicationCommonSettings.SellerShortName}:  Apps Launcher on BlueStacks (beta version)";
                refToNavigator.SetWindowPositionToCenterOfTheScreen();

                if (refToViewModel.IsExtractedAdvancedMode) { ExtractAdvancedButtons(); } else { ShrinkAdvancedButtons(); }
            };

            this.MouseLeftButtonDown += delegate { refToNavigator.DragMove(); };
        }

        void ShrinkAdvancedButtons()
        {
#if IsToUseAdvancedButtonsInLauncher
            advancedButton.Visibility = Visibility.Visible;
            highSpeedModeButton.Visibility = Visibility.Collapsed;
            highPrecisionModeButton.Visibility = Visibility.Collapsed;
#endif
        }

        void ExtractAdvancedButtons()
        {
#if IsToUseAdvancedButtonsInLauncher
            advancedButton.Visibility = Visibility.Collapsed;
            highSpeedModeButton.Visibility = Visibility.Visible;
            highPrecisionModeButton.Visibility = Visibility.Visible;
#endif
        }
    }
}
