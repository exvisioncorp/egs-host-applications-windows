﻿namespace Egs.ZkooTutorial
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
    using System.Diagnostics;
    using System.IO;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Windows.Threading;
    using Egs;
    using Egs.Views;
    using Egs.PropertyTypes;

    [DataContract]
    public class ZkooTutorialResourcesModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        internal NarrationInformationList Messages { get; private set; }
        internal string ResourcesFolderRelativePath { get { return @"Resources\"; } }
        internal string ImageFilesFolderPath { get { return ResourcesFolderRelativePath + @"drawable-mdpi\"; } }
        internal string SoundFilesFolderPath { get { return ResourcesFolderRelativePath + @"raw\"; } }
        internal string TutorialVideoFilesFolderPath { get { return ResourcesFolderRelativePath + @"raw\"; } }

        public ZkooTutorialResourcesModel()
        {
            Messages = new NarrationInformationList();
        }
    }

    [DataContract]
    class ZkooTutorialModel : INotifyPropertyChanged
    {
        internal const bool IsToExitApplicationOrElseNavigateToLauncherWhenTutorialExit = true;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        // TODO: Communication between applications (processes) should be used.
        internal EgsHostAppBaseComponents RefToHostApp { get; private set; }

        public ZkooTutorialResourcesModel CurrentResources { get; private set; }

        //[DataMember]
        public LauncherPageModel Launcher { get; private set; }
        public TutorialAppHeaderMenuViewModel TutorialAppHeaderMenu { get; private set; }
        public Tutorial01StartGestureTrainingPageModel Tutorial01StartGestureTraining { get; private set; }
        public Tutorial02MoveCursorTrainingPageModel Tutorial02MoveCursorTraining { get; private set; }
        public Tutorial03TapGestureTrainingPageModel Tutorial03TapGestureTraining { get; private set; }
        public Tutorial04DragGestureTrainingPageModel Tutorial04DragGestureTraining { get; private set; }
        public Tutorial05FlickGestureTrainingPageModel Tutorial05FlickGestureTraining { get; private set; }

        public TutorialLargeCircleAreaButtonModel TutorialLargeCircleAreaButtonRightTop { get; private set; }
        public TutorialLargeCircleAreaButtonModel TutorialLargeCircleAreaButtonRightBottom { get; private set; }
        public TutorialLargeCircleAreaButtonModel TutorialLargeCircleAreaButtonLeftBottom { get; private set; }
        public TutorialLargeCircleAreaButtonModel TutorialLargeCircleAreaButtonLeftTop { get; private set; }
        public List<TutorialLargeCircleAreaButtonModel> TutorialLargeCircleAreaButtonList { get; private set; }

        public ZkooTutorialModel(EgsHostAppBaseComponents hostApp)
        {
            Trace.Assert(hostApp != null);

            RefToHostApp = hostApp;
            CurrentResources = new ZkooTutorialResourcesModel();
            Launcher = new LauncherPageModel();
            TutorialAppHeaderMenu = new TutorialAppHeaderMenuViewModel();
            Tutorial01StartGestureTraining = new Tutorial01StartGestureTrainingPageModel();
            Tutorial02MoveCursorTraining = new Tutorial02MoveCursorTrainingPageModel();
            Tutorial03TapGestureTraining = new Tutorial03TapGestureTrainingPageModel();
            Tutorial04DragGestureTraining = new Tutorial04DragGestureTrainingPageModel();
            Tutorial05FlickGestureTraining = new Tutorial05FlickGestureTrainingPageModel();

            TutorialLargeCircleAreaButtonRightTop = new TutorialLargeCircleAreaButtonModel()
            {
                Index = 0,
                ImageSourceDisabledFileName = "tutorial_common_circle1_button_disabled.png",
                ImageSourcePressedFileName = "tutorial_common_circle1_button_pressed.png",
                ImageSourceHoveredFileName = "tutorial_common_circle1_button_hovered.png",
                ImageSourceSelectedFileName = "tutorial_common_circle1_button_pressed.png",
                ImageSourceEnabledFileName = "tutorial_common_circle1_button_enabled.png",
            };
            TutorialLargeCircleAreaButtonRightBottom = new TutorialLargeCircleAreaButtonModel()
            {
                Index = 1,
                ImageSourceDisabledFileName = "tutorial_common_circle2_button_disabled.png",
                ImageSourcePressedFileName = "tutorial_common_circle2_button_pressed.png",
                ImageSourceHoveredFileName = "tutorial_common_circle2_button_hovered.png",
                ImageSourceSelectedFileName = "tutorial_common_circle2_button_pressed.png",
                ImageSourceEnabledFileName = "tutorial_common_circle2_button_enabled.png",
            };
            TutorialLargeCircleAreaButtonLeftBottom = new TutorialLargeCircleAreaButtonModel()
            {
                Index = 2,
                ImageSourceDisabledFileName = "tutorial_common_circle3_button_disabled.png",
                ImageSourcePressedFileName = "tutorial_common_circle3_button_pressed.png",
                ImageSourceHoveredFileName = "tutorial_common_circle3_button_hovered.png",
                ImageSourceSelectedFileName = "tutorial_common_circle3_button_pressed.png",
                ImageSourceEnabledFileName = "tutorial_common_circle3_button_enabled.png",
            };
            TutorialLargeCircleAreaButtonLeftTop = new TutorialLargeCircleAreaButtonModel()
            {
                Index = 3,
                ImageSourceDisabledFileName = "tutorial_common_circle4_button_disabled.png",
                ImageSourcePressedFileName = "tutorial_common_circle4_button_pressed.png",
                ImageSourceHoveredFileName = "tutorial_common_circle4_button_hovered.png",
                ImageSourceSelectedFileName = "tutorial_common_circle4_button_pressed.png",
                ImageSourceEnabledFileName = "tutorial_common_circle4_button_enabled.png",
            };
        }

        void OnCultureInfoChanged()
        {
            foreach (var item in Launcher.LauncherRecommendedAppViewModelList)
            {
                item.AppIconImageSourceRelativeFolderPath = CurrentResources.ImageFilesFolderPath + @"LaunchableApplicationBitmaps\";
            }
            foreach (var item in TutorialAppHeaderMenu.ImageButtonModelList)
            {
                item.ImageSourceRelativeFolderPath = CurrentResources.ImageFilesFolderPath;
            }
            foreach (var item in TutorialLargeCircleAreaButtonList)
            {
                item.ImageSourceRelativeFolderPath = CurrentResources.ImageFilesFolderPath;
            }
        }

        void CheckIfAllSoundFilesExistOrNot()
        {
            if (ApplicationCommonSettings.IsDebugging == false) { return; }

            var hasAllFiles = true;
            try
            {
                var audioPlayer = new NAudioWrapper();
                var messages = new NarrationInformationList();
                var props = messages.GetType().GetProperties();
                foreach (var prop in props)
                {
                    var msg = (NarrationInformation)prop.GetValue(messages);
                    var filePath = CurrentResources.SoundFilesFolderPath + msg.OggAudioFileName;
                    var hr = audioPlayer.StartAsync(filePath);
                    if (hr == false)
                    {
                        hasAllFiles = false;
                        break;
                    }
                }
                audioPlayer.Stop();
            }
            catch (Exception ex)
            {
                hasAllFiles = false;
                MessageBox.Show(ex.Message);
            }
            if (hasAllFiles == false)
            {
                throw new Exception("Application could not find one or more sound files, or application could not play the sound file on the current sound device.");
            }
        }

        public void InitializeOnceAtStartup(EgsHostAppBaseComponents hostApp)
        {
            Trace.Assert(hostApp != null);

            RefToHostApp = hostApp;


            if (ApplicationCommonSettings.IsDebuggingInternal) { CheckIfAllSoundFilesExistOrNot(); }


            RefToHostApp.CameraViewWindowModel.WindowStateHostApplicationsControlMethod.Value = CameraViewWindowStateHostApplicationsControlMethods.KeepMinimized;
            RefToHostApp.CameraViewBordersAndPointersAreDrawnBy.Value = CameraViewBordersAndPointersAreDrawnByKind.HostApplication;

            RefToHostApp.Device.Settings.IsToSendTouchScreenHidReport.Value = true;
            // In Windows 10, users cannot often tap by sending Hovering State
            if (false) { RefToHostApp.Device.Settings.IsToSendHoveringStateOnTouchScreenHidReport.Value = true; }
            RefToHostApp.Device.Settings.IsToSendEgsGestureHidReport.Value = true;
            // Now this settings cannot be set from SettingsWindow, so I comment out the next line.
            RefToHostApp.Device.Settings.IsToDrawBordersOnCameraViewImageByDevice.Value = false;
            Launcher.InitializeOnceAtStartup(this);

            RefToHostApp.OnePersonBothHandsViewModel.RightHand.IsToUpdateVelocities = false;
            RefToHostApp.OnePersonBothHandsViewModel.LeftHand.IsToUpdateVelocities = false;

            TutorialLargeCircleAreaButtonList = new List<TutorialLargeCircleAreaButtonModel>();
            TutorialLargeCircleAreaButtonList.Add(TutorialLargeCircleAreaButtonRightTop);
            TutorialLargeCircleAreaButtonList.Add(TutorialLargeCircleAreaButtonRightBottom);
            TutorialLargeCircleAreaButtonList.Add(TutorialLargeCircleAreaButtonLeftBottom);
            TutorialLargeCircleAreaButtonList.Add(TutorialLargeCircleAreaButtonLeftTop);

            Tutorial01StartGestureTraining.InitializeOnceAtStartup(this);
            Tutorial02MoveCursorTraining.InitializeOnceAtStartup(this);
            Tutorial03TapGestureTraining.InitializeOnceAtStartup(this);
            Tutorial04DragGestureTraining.InitializeOnceAtStartup(this);
            Tutorial05FlickGestureTraining.InitializeOnceAtStartup(this);

            OnCultureInfoChanged();
        }

        public void EnableUpdatingCameraViewImageButHideWindow()
        {
            RefToHostApp.CameraViewWindowModel.WindowStateHostApplicationsControlMethod.Value = CameraViewWindowStateHostApplicationsControlMethods.KeepMinimized;
            RefToHostApp.Device.Settings.IsToDetectFaces.Value = true;
            RefToHostApp.Device.Settings.IsToDetectHands.Value = true;
        }

        public void EnableUpdatingCameraViewImageAndShowWindow()
        {
            RefToHostApp.CameraViewWindowModel.WindowStateHostApplicationsControlMethod.Value = CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods;
            RefToHostApp.CameraViewWindowModel.SetWindowStateToNormal();
            RefToHostApp.Device.Settings.IsToDetectFaces.Value = true;
            RefToHostApp.Device.Settings.IsToDetectHands.Value = true;
        }
    }
}
