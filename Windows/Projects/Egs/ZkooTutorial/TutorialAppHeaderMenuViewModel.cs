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
    using System.IO;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using Egs;
    using Egs.Views;

    class TutorialAppHeaderMenuViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        public ImageButtonModel MenuStartButtonModel { get; private set; }
        public ImageButtonModel MenuMoveButtonModel { get; private set; }
        public ImageButtonModel MenuTapButtonModel { get; private set; }
        public ImageButtonModel MenuDragButtonModel { get; private set; }
        public ImageButtonModel MenuFlickButtonModel { get; private set; }
        public ImageButtonModel MenuMoreButtonModel { get; private set; }

        public ImageButtonModel MenuReplayButtonModel { get; private set; }
        public ImageButtonModel MenuPracticeButtonModel { get; private set; }
        public ImageButtonModel MenuNextButtonModel { get; private set; }

        public ImageButtonModel MenuExitButtonModel { get; private set; }

        public ImageButtonModel DialogReplayButtonModel { get; private set; }
        public ImageButtonModel DialogPracticeButtonModel { get; private set; }
        public ImageButtonModel DialogNextButtonModel { get; private set; }


        public ZkooTutorialModel refToAppModel { get; private set; }
        public MainNavigationWindow refToNavigator { get; private set; }

        public Page ReplayButtonDestinationPage { get; set; }
        public Page PracticeButtonDestinationPage { get; set; }
        public Page NextButtonDestinationPage { get; set; }

        internal List<ImageButtonModel> ImageButtonModelList = new List<ImageButtonModel>();

        public TutorialAppHeaderMenuViewModel()
        {
            // NOTE: In XAML, it uses not "\n" but "&#10;".
            MenuStartButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_menu_start_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_menu_start_button_selected.png",
                ImageSourceHoveredFileName = "tutorial_common_menu_start_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_menu_start_button_selected.png",
                ImageSourceEnabledFileName = "tutorial_common_menu_start_button_enabled.png",
                ButtonDescriptionText = "Show the Hand Cursor by your Initial Gesture"
            };
            MenuMoveButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_menu_move_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_menu_move_button_selected.png",
                ImageSourceHoveredFileName = "tutorial_common_menu_move_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_menu_move_button_selected.png",
                ImageSourceEnabledFileName = "tutorial_common_menu_move_button_enabled.png",
                ButtonDescriptionText = "Move the Hand Cursor by your hand movement",
            };
            MenuTapButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_menu_tap_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_menu_tap_button_selected.png",
                ImageSourceHoveredFileName = "tutorial_common_menu_tap_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_menu_tap_button_selected.png",
                ImageSourceEnabledFileName = "tutorial_common_menu_tap_button_enabled.png",
                ButtonDescriptionText = "Tap Gesture Practice",
            };
            MenuDragButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_menu_drag_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_menu_drag_button_selected.png",
                ImageSourceHoveredFileName = "tutorial_common_menu_drag_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_menu_drag_button_selected.png",
                ImageSourceEnabledFileName = "tutorial_common_menu_drag_button_enabled.png",
                ButtonDescriptionText = "Drag Gesture Practice",
            };
            MenuFlickButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_menu_flick_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_menu_flick_button_selected.png",
                ImageSourceHoveredFileName = "tutorial_common_menu_flick_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_menu_flick_button_selected.png",
                ImageSourceEnabledFileName = "tutorial_common_menu_flick_button_enabled.png",
                ButtonDescriptionText = "Flick Gesture Practice",
            };
            MenuMoreButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_menu_more_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_menu_more_button_selected.png",
                ImageSourceHoveredFileName = "tutorial_common_menu_more_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_menu_more_button_selected.png",
                ImageSourceEnabledFileName = "tutorial_common_menu_more_button_enabled.png",
                ButtonDescriptionText = "Gestures by Both Hands and the Other Gestures",
            };
            MenuReplayButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_menu_replay_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_menu_replay_button_pressed.png",
                ImageSourceHoveredFileName = "tutorial_common_menu_replay_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_menu_replay_button_pressed.png",
                ImageSourceEnabledFileName = "tutorial_common_menu_replay_button_enabled.png",
                ButtonDescriptionText = "Restart from Video Replay of this Practice",
            };
            MenuPracticeButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_menu_practice_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_menu_practice_button_pressed.png",
                ImageSourceHoveredFileName = "tutorial_common_menu_practice_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_menu_practice_button_pressed.png",
                ImageSourceEnabledFileName = "tutorial_common_menu_practice_button_enabled.png",
                ButtonDescriptionText = "Retry this Practice",
            };
            MenuNextButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_menu_next_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_menu_next_button_pressed.png",
                ImageSourceHoveredFileName = "tutorial_common_menu_next_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_menu_next_button_pressed.png",
                ImageSourceEnabledFileName = "tutorial_common_menu_next_button_enabled.png",
                ButtonDescriptionText = "Move to Next Practice",
            };
            MenuExitButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_menu_exit_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_menu_exit_button_pressed.png",
                ImageSourceHoveredFileName = "tutorial_common_menu_exit_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_menu_exit_button_pressed.png",
                ImageSourceEnabledFileName = "tutorial_common_menu_exit_button_enabled.png",
                ButtonDescriptionText = "Exit the Tutorial",
            };

            DialogReplayButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_dialog_replay_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_dialog_replay_button_pressed.png",
                ImageSourceHoveredFileName = "tutorial_common_dialog_replay_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_dialog_replay_button_pressed.png",
                ImageSourceEnabledFileName = "tutorial_common_dialog_replay_button_enabled.png",
                ButtonDescriptionText = "Restart from Video Replay of this Practice",
            };
            DialogPracticeButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_dialog_practice_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_dialog_practice_button_pressed.png",
                ImageSourceHoveredFileName = "tutorial_common_dialog_practice_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_dialog_practice_button_pressed.png",
                ImageSourceEnabledFileName = "tutorial_common_dialog_practice_button_enabled.png",
                ButtonDescriptionText = "Retry this Practice",
            };
            DialogNextButtonModel = new ImageButtonModel()
            {
                ImageSourceRelativeFolderPath = @"Resources\en\drawable-mdpi\",
                ImageSourceDisabledFileName = "tutorial_common_dialog_next_button_enabled.png",
                ImageSourcePressedFileName = "tutorial_common_dialog_next_button_pressed.png",
                ImageSourceHoveredFileName = "tutorial_common_dialog_next_button_focused.png",
                ImageSourceSelectedFileName = "tutorial_common_dialog_next_button_pressed.png",
                ImageSourceEnabledFileName = "tutorial_common_dialog_next_button_enabled.png",
                ButtonDescriptionText = "Move to Next Practice",
            };

            // TODO: If this is serialized, it is bad to write such code in the constructor.
            ImageButtonModelList.Add(MenuStartButtonModel);
            ImageButtonModelList.Add(MenuMoveButtonModel);
            ImageButtonModelList.Add(MenuTapButtonModel);
            ImageButtonModelList.Add(MenuDragButtonModel);
            ImageButtonModelList.Add(MenuFlickButtonModel);
            ImageButtonModelList.Add(MenuMoreButtonModel);
            ImageButtonModelList.Add(MenuReplayButtonModel);
            ImageButtonModelList.Add(MenuPracticeButtonModel);
            ImageButtonModelList.Add(MenuNextButtonModel);
            ImageButtonModelList.Add(MenuExitButtonModel);

            ImageButtonModelList.Add(DialogReplayButtonModel);
            ImageButtonModelList.Add(DialogPracticeButtonModel);
            ImageButtonModelList.Add(DialogNextButtonModel);
        }

        void Navigate(Page beingNavigatedPage)
        {
            Trace.Assert(beingNavigatedPage != null);

            if (refToNavigator.NavigationService.Content is VideoPlayingPage
                && (refToNavigator.NavigationService.Content == beingNavigatedPage))
            {
                var page = (VideoPlayingPage)refToNavigator.NavigationService.Content;
                // NOTE: No problems, for now.
                refToNavigator.Dispatcher.Invoke(new Action(() =>
                {
                    refToNavigator.NavigationService.Refresh();
                }));
                return;
            }

            if (refToNavigator.NavigationService.Content is IHasTutorialEachPageModelBase)
            {
                var currentPage = refToNavigator.NavigationService.Content as IHasTutorialEachPageModelBase;
                currentPage.ReferenceToTutorialEachPageModelBase.IsCancelling = true;
                currentPage.ReferenceToTutorialEachPageModelBase.CurrentTask.Wait();
                currentPage.ReferenceToTutorialEachPageModelBase.CurrentTask.Dispose();
                if (refToNavigator.NavigationService.Content == beingNavigatedPage)
                {
                    // NOTE: No problems, for now.
                    refToNavigator.Dispatcher.Invoke(new Action(() =>
                    {
                        refToNavigator.NavigationService.Navigate(currentPage);
                        currentPage.ReferenceToTutorialEachPageModelBase.OnLoaded();
                    }));
                    return;
                }
            }

            // NOTE: Before, strange exceptions often occurred.  The reason is the below code.  I leave this code for the reference.
            if (false)
            {
                var nextPage = beingNavigatedPage as IHasTutorialEachPageModelBase;
                if (nextPage != null)
                {
                    nextPage.ReferenceToTutorialEachPageModelBase.OnLoaded();
                }
            }

            refToNavigator.Dispatcher.Invoke(new Action(() =>
            {
                refToNavigator.NavigationService.Navigate(beingNavigatedPage);
            }));
        }

        public void InitializeOnceAtStartup(MainNavigationWindow navigator, ZkooTutorialModel appModel)
        {
            Trace.Assert(navigator != null);
            Trace.Assert(appModel != null);
            refToNavigator = navigator;
            refToAppModel = appModel;

            ReplayButtonDestinationPage = refToNavigator.Tutorial01StartGestureTutorialVideo;
            PracticeButtonDestinationPage = refToNavigator.Tutorial01StartGestureTraining;
            NextButtonDestinationPage = refToNavigator.Tutorial02MoveCursorTutorialVideo;

            MenuStartButtonModel.CommandRaised += (sender, e) => { Navigate(refToNavigator.Tutorial01StartGestureTutorialVideo); };
            MenuMoveButtonModel.CommandRaised += (sender, e) => { Navigate(refToNavigator.Tutorial02MoveCursorTutorialVideo); };
            MenuTapButtonModel.CommandRaised += (sender, e) => { Navigate(refToNavigator.Tutorial03TapGestureTutorialVideo); };
            MenuDragButtonModel.CommandRaised += (sender, e) => { Navigate(refToNavigator.Tutorial04DragGestureTutorialVideo); };
            MenuFlickButtonModel.CommandRaised += (sender, e) => { Navigate(refToNavigator.Tutorial05FlickGestureTutorialVideo); };
            MenuMoreButtonModel.CommandRaised += (sender, e) => { Navigate(refToNavigator.Tutorial06BothHandsGestureTutorialVideo); };
            MenuReplayButtonModel.CommandRaised += delegate { Navigate(ReplayButtonDestinationPage); };
            MenuPracticeButtonModel.CommandRaised += delegate { Navigate(PracticeButtonDestinationPage); };
            MenuNextButtonModel.CommandRaised += delegate { Navigate(NextButtonDestinationPage); };

            MenuExitButtonModel.CommandRaised += delegate
            {
                if (ZkooTutorialModel.IsToExitApplicationOrElseNavigateToLauncherWhenTutorialExit)
                {
                    refToNavigator.ExitTutorial();
                }
                else
                {
                    Navigate(refToNavigator.LauncherView);
                }
            };

            DialogReplayButtonModel.CommandRaised += delegate { Navigate(ReplayButtonDestinationPage); };
            DialogPracticeButtonModel.CommandRaised += delegate { Navigate(PracticeButtonDestinationPage); };
            DialogNextButtonModel.CommandRaised += delegate { Navigate(NextButtonDestinationPage); };
        }
    }
}
