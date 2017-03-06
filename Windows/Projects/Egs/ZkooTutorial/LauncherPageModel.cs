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
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using Egs;
    using Egs.Views;

    [DataContract]
    class LauncherPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        ZkooTutorialModel refToAppModel { get; set; }

        internal string PageTitle { get; set; }

        [DataMember]
        public bool IsExtractedAdvancedMode { get; set; }
        [DataMember]
        public bool IsToShowBlueStacksAppIcons { get; set; }
        [DataMember]
        public CursorSpeedAndPrecisionModeButtonModel BeginnerModeButtonModel { get; set; }
        [DataMember]
        public CursorSpeedAndPrecisionModeButtonModel StandardModeButtonModel { get; set; }
        [DataMember]
        public CursorSpeedAndPrecisionModeButtonModel HighSpeedModeButtonModel { get; set; }
        [DataMember]
        public CursorSpeedAndPrecisionModeButtonModel HighPrecisionModeButtonModel { get; set; }
        [DataMember]
        public TextButtonModel AdvancedButtonModel { get; set; }
        [DataMember]
        public LaunchingOtherApplicationButtonModel TutorialVideoButtonModel { get; set; }
        [DataMember]
        public TextButtonModel TutorialAppButtonModel { get; set; }
        [DataMember]
        public TextButtonModel ExitButtonModel { get; set; }


        TextButtonModel _CursorOveringLauncherButtonSummary;
        public TextButtonModel CursorOveringLauncherButtonSummary
        {
            get { return _CursorOveringLauncherButtonSummary; }
            set { _CursorOveringLauncherButtonSummary = value; OnPropertyChanged(nameof(CursorOveringLauncherButtonSummary)); }
        }


        [DataMember]
        public LauncherCursorSpeedAndPrecisionModeAndRecommendedAppsViewModel BeginnerModeDetail { get; set; }
        [DataMember]
        public LauncherCursorSpeedAndPrecisionModeAndRecommendedAppsViewModel StandardModeDetail { get; set; }
        [DataMember]
        public LauncherCursorSpeedAndPrecisionModeAndRecommendedAppsViewModel HighSpeedModeDetail { get; set; }
        [DataMember]
        public LauncherCursorSpeedAndPrecisionModeAndRecommendedAppsViewModel HighPrecisionModeDetail { get; set; }


        LauncherCursorSpeedAndPrecisionModeAndRecommendedAppsViewModel _CursorSpeedAndPrecisionModeAndRecommendedApps;
        public LauncherCursorSpeedAndPrecisionModeAndRecommendedAppsViewModel CursorSpeedAndPrecisionModeAndRecommendedApps
        {
            get { return _CursorSpeedAndPrecisionModeAndRecommendedApps; }
            set { _CursorSpeedAndPrecisionModeAndRecommendedApps = value; OnPropertyChanged(nameof(CursorSpeedAndPrecisionModeAndRecommendedApps)); }
        }

        internal List<LauncherRecommendedAppViewModel> LauncherRecommendedAppViewModelList = new List<LauncherRecommendedAppViewModel>();

        public LauncherPageModel()
        {
            PageTitle = "ZKOO by Exvision:  Apps Launcher on BlueStacks (beta version)";

            IsExtractedAdvancedMode = false;
            IsToShowBlueStacksAppIcons = true;

            BeginnerModeButtonModel = new CursorSpeedAndPrecisionModeButtonModel()
            {
                ButtonContentText = "Beginner",
                ButtonDescriptionText = "First try this mode to get accustomed to the Exvision Gesture System.",
                ModeDescription = "Beginner",
            };
            StandardModeButtonModel = new CursorSpeedAndPrecisionModeButtonModel()
            {
                ButtonContentText = "Standard",
                ButtonDescriptionText = "Use this mode once you get accustomed to the Exvision Gesture System.",
                ModeDescription = "Standard",
            };
            HighSpeedModeButtonModel = new CursorSpeedAndPrecisionModeButtonModel()
            {
                ButtonContentText = "High\r\nSpeed",
                ButtonDescriptionText = "World's fastest feature mode.",
                ModeDescription = "High Speed",
            };
            HighPrecisionModeButtonModel = new CursorSpeedAndPrecisionModeButtonModel()
            {
                ButtonContentText = "High\r\nPrecision",
                ButtonDescriptionText = "Smooth and accurate gesture mode.",
                ModeDescription = "High Precision",
            };

            AdvancedButtonModel = new TextButtonModel()
            {
                ButtonContentText = "Advanced",
                ButtonDescriptionText = "Click to show advanced gesture modes.",
            };
            TutorialAppButtonModel = new TextButtonModel()
            {
                ButtonContentText = "Tutorial\r\nApp",
                ButtonDescriptionText = "Tutorial application for the Exvision Gesture System.",
            };
            TutorialVideoButtonModel = new LaunchingOtherApplicationButtonModel()
            {
                ButtonContentText = "Tutorial\r\nVideo",
                ButtonDescriptionText = "View the tutorial video.",
                ProcessStartInfoFileName = @"https://ksr-video.imgix.net/assets/005/058/642/1cf5269fd1908abc74ff3b7a83e5fe27_h264_high.mp4",
                ProcessStartInfoWorkingDirectory = "",
                ProcessStartInfoArguments = "",
            };
            ExitButtonModel = new TextButtonModel()
            {
                ButtonContentText = "Exit",
                ButtonDescriptionText = "Exit this application.",
            };

            BeginnerModeDetail = new LauncherCursorSpeedAndPrecisionModeAndRecommendedAppsViewModel()
            {
                ModeButtonModel = BeginnerModeButtonModel,
                ModeDescriptionText = "Beginner",
                RecommendedAppsToPlayText = "Recommended apps to play in Beginner mode",
                RecommendedAppLeft = new LauncherRecommendedAppViewModel()
                {
                    AppTitle = "Candy Crush",
                    LaunchingOtherApplicationButtonModel = new LaunchingOtherApplicationButtonModel()
                    {
                        ButtonContentText = "",
                        ButtonDescriptionText = "",
                        ProcessStartInfoFileName = "C:\\Program Files (x86)\\BlueStacks\\HD-RunApp.exe",
                        ProcessStartInfoWorkingDirectory = "",
                        ProcessStartInfoArguments = "Android com.king.candycrushsaga com.king.candycrushsaga.CandyCrushSagaActivity"
                    },
                    AppIconImageSourceFileName = "app_candycrush.png"
                },
                RecommendedAppRight = new LauncherRecommendedAppViewModel()
                {
                    AppTitle = "Cut The Rope",
                    LaunchingOtherApplicationButtonModel = new LaunchingOtherApplicationButtonModel()
                    {
                        ButtonContentText = "",
                        ButtonDescriptionText = "",
                        ProcessStartInfoFileName = "C:\\Program Files (x86)\\BlueStacks\\HD-RunApp.exe",
                        ProcessStartInfoWorkingDirectory = "",
                        ProcessStartInfoArguments = "Android com.zeptolab.ctr.paid com.zeptolab.ctr.CtrApp"
                    },
                    AppIconImageSourceFileName = "app_cuttherope.png"
                }
            };
            StandardModeDetail = new LauncherCursorSpeedAndPrecisionModeAndRecommendedAppsViewModel()
            {
                ModeButtonModel = StandardModeButtonModel,
                ModeDescriptionText = "Standard",
                RecommendedAppsToPlayText = "Recommended apps to play in Standard mode",
                RecommendedAppLeft = new LauncherRecommendedAppViewModel()
                {
                    AppTitle = "Fruit Ninja",
                    LaunchingOtherApplicationButtonModel = new LaunchingOtherApplicationButtonModel()
                    {
                        ButtonContentText = "",
                        ButtonDescriptionText = "",
                        ProcessStartInfoFileName = "C:\\Program Files (x86)\\BlueStacks\\HD-RunApp.exe",
                        ProcessStartInfoWorkingDirectory = "",
                        ProcessStartInfoArguments = "Android com.halfbrick.fruitninja com.halfbrick.mortar.MortarGameActivity"
                    },
                    AppIconImageSourceFileName = "app_fruitninja.png"
                },
                RecommendedAppRight = new LauncherRecommendedAppViewModel()
                {
                    AppTitle = "Smash Hit",
                    LaunchingOtherApplicationButtonModel = new LaunchingOtherApplicationButtonModel()
                    {
                        ButtonContentText = "",
                        ButtonDescriptionText = "",
                        ProcessStartInfoFileName = "C:\\Program Files (x86)\\BlueStacks\\HD-RunApp.exe",
                        ProcessStartInfoWorkingDirectory = "",
                        ProcessStartInfoArguments = "Android com.mediocre.smashhit com.mediocre.smashhit.Main"
                    },
                    AppIconImageSourceFileName = "app_smashhit.png"
                }
            };
            HighSpeedModeDetail = new LauncherCursorSpeedAndPrecisionModeAndRecommendedAppsViewModel()
            {
                ModeButtonModel = HighSpeedModeButtonModel,
                ModeDescriptionText = "High Speed",
                RecommendedAppsToPlayText = "Recommended apps to play in High Speed mode",
                RecommendedAppLeft = new LauncherRecommendedAppViewModel()
                {
                    AppTitle = "Fruit Ninja",
                    LaunchingOtherApplicationButtonModel = new LaunchingOtherApplicationButtonModel()
                    {
                        ButtonContentText = "",
                        ButtonDescriptionText = "",
                        ProcessStartInfoFileName = "C:\\Program Files (x86)\\BlueStacks\\HD-RunApp.exe",
                        ProcessStartInfoWorkingDirectory = "",
                        ProcessStartInfoArguments = "Android com.halfbrick.fruitninja com.halfbrick.mortar.MortarGameActivity"
                    },
                    AppIconImageSourceFileName = "app_fruitninja.png"
                },
                RecommendedAppRight = new LauncherRecommendedAppViewModel()
                {
                    AppTitle = "Smash Hit",
                    LaunchingOtherApplicationButtonModel = new LaunchingOtherApplicationButtonModel()
                    {
                        ButtonContentText = "",
                        ButtonDescriptionText = "",
                        ProcessStartInfoFileName = "C:\\Program Files (x86)\\BlueStacks\\HD-RunApp.exe",
                        ProcessStartInfoWorkingDirectory = "",
                        ProcessStartInfoArguments = "Android com.mediocre.smashhit com.mediocre.smashhit.Main"
                    },
                    AppIconImageSourceFileName = "pp_smashhit.png"
                }
            };
            HighPrecisionModeDetail = new LauncherCursorSpeedAndPrecisionModeAndRecommendedAppsViewModel()
            {
                ModeButtonModel = HighPrecisionModeButtonModel,
                ModeDescriptionText = "High Precision",
                RecommendedAppsToPlayText = "Recommended apps to play in High Precision mode",
                RecommendedAppLeft = new LauncherRecommendedAppViewModel()
                {
                    AppTitle = "Paperless",
                    LaunchingOtherApplicationButtonModel = new LaunchingOtherApplicationButtonModel()
                    {
                        ButtonContentText = "",
                        ButtonDescriptionText = "",
                        ProcessStartInfoFileName = "C:\\Program Files (x86)\\BlueStacks\\HD-RunApp.exe",
                        ProcessStartInfoWorkingDirectory = "",
                        ProcessStartInfoArguments = "Android air.com.jeremieklemke.drawing air.com.jeremieklemke.drawing.AppEntry"
                    },
                    AppIconImageSourceFileName = "app_paperless.png"
                },
                RecommendedAppRight = new LauncherRecommendedAppViewModel()
                {
                    AppTitle = "Flow Free",
                    LaunchingOtherApplicationButtonModel = new LaunchingOtherApplicationButtonModel()
                    {
                        ButtonContentText = "",
                        ButtonDescriptionText = "",
                        ProcessStartInfoFileName = "C:\\Program Files (x86)\\BlueStacks\\HD-RunApp.exe",
                        ProcessStartInfoWorkingDirectory = "",
                        ProcessStartInfoArguments = "Android com.bigduckgames.flow com.bigduckgames.flow.flow"
                    },
                    AppIconImageSourceFileName = "app_flowfree.png"
                }
            };

            _CursorOveringLauncherButtonSummary = StandardModeButtonModel;
            _CursorSpeedAndPrecisionModeAndRecommendedApps = StandardModeDetail;


            if (IsToShowBlueStacksAppIcons == false)
            {
                BeginnerModeDetail.RecommendedAppLeft.AppIconImageSourceFileName = "";
                BeginnerModeDetail.RecommendedAppRight.AppIconImageSourceFileName = "";
                StandardModeDetail.RecommendedAppLeft.AppIconImageSourceFileName = "";
                StandardModeDetail.RecommendedAppRight.AppIconImageSourceFileName = "";
                HighSpeedModeDetail.RecommendedAppLeft.AppIconImageSourceFileName = "";
                HighSpeedModeDetail.RecommendedAppRight.AppIconImageSourceFileName = "";
                HighPrecisionModeDetail.RecommendedAppLeft.AppIconImageSourceFileName = "";
                HighPrecisionModeDetail.RecommendedAppRight.AppIconImageSourceFileName = "";
            }
        }

        public void InitializeOnceAtStartup(ZkooTutorialModel appModel)
        {
            Trace.Assert(appModel != null);
            refToAppModel = appModel;

            LauncherRecommendedAppViewModelList.Add(BeginnerModeDetail.RecommendedAppLeft);
            LauncherRecommendedAppViewModelList.Add(BeginnerModeDetail.RecommendedAppRight);
            LauncherRecommendedAppViewModelList.Add(StandardModeDetail.RecommendedAppLeft);
            LauncherRecommendedAppViewModelList.Add(StandardModeDetail.RecommendedAppRight);
            LauncherRecommendedAppViewModelList.Add(HighSpeedModeDetail.RecommendedAppLeft);
            LauncherRecommendedAppViewModelList.Add(HighSpeedModeDetail.RecommendedAppRight);
            LauncherRecommendedAppViewModelList.Add(HighPrecisionModeDetail.RecommendedAppLeft);
            LauncherRecommendedAppViewModelList.Add(HighPrecisionModeDetail.RecommendedAppRight);

            BeginnerModeButtonModel.CommandRaised += (sender, e) => ChangeEgsDeviceCursorSpeedAndPrecisionMode(BeginnerModeButtonModel);
            StandardModeButtonModel.CommandRaised += (sender, e) => ChangeEgsDeviceCursorSpeedAndPrecisionMode(StandardModeButtonModel);
            HighSpeedModeButtonModel.CommandRaised += (sender, e) => ChangeEgsDeviceCursorSpeedAndPrecisionMode(HighSpeedModeButtonModel);
            HighPrecisionModeButtonModel.CommandRaised += (sender, e) => ChangeEgsDeviceCursorSpeedAndPrecisionMode(HighPrecisionModeButtonModel);


            TutorialVideoButtonModel.CommandRaised += (sender, e) =>
            {
                ((LaunchingOtherApplicationButtonModel)sender).StartProcess();
            };
            BeginnerModeDetail.RecommendedAppLeft.LaunchingOtherApplicationButtonModel.CommandRaised += (sender, e) =>
            {
                ((LaunchingOtherApplicationButtonModel)sender).StartProcess();
                var keyDescription = BeginnerModeDetail.ModeButtonModel.ModeDescription;
                refToAppModel.RefToHostApp.Device.Settings.CursorSpeedAndPrecisionMode.OptionalValue.SelectSingleItemByPredicate(item => item.Description == keyDescription);
            };
            BeginnerModeDetail.RecommendedAppRight.LaunchingOtherApplicationButtonModel.CommandRaised += (sender, e) =>
            {
                ((LaunchingOtherApplicationButtonModel)sender).StartProcess();
                var keyDescription = BeginnerModeDetail.ModeButtonModel.ModeDescription;
                refToAppModel.RefToHostApp.Device.Settings.CursorSpeedAndPrecisionMode.OptionalValue.SelectSingleItemByPredicate(item => item.Description == keyDescription);
            };
            StandardModeDetail.RecommendedAppLeft.LaunchingOtherApplicationButtonModel.CommandRaised += (sender, e) =>
            {
                ((LaunchingOtherApplicationButtonModel)sender).StartProcess();
                var keyDescription = StandardModeDetail.ModeButtonModel.ModeDescription;
                refToAppModel.RefToHostApp.Device.Settings.CursorSpeedAndPrecisionMode.OptionalValue.SelectSingleItemByPredicate(item => item.Description == keyDescription);
            };
            StandardModeDetail.RecommendedAppRight.LaunchingOtherApplicationButtonModel.CommandRaised += (sender, e) =>
            {
                ((LaunchingOtherApplicationButtonModel)sender).StartProcess();
                var keyDescription = StandardModeDetail.ModeButtonModel.ModeDescription;
                refToAppModel.RefToHostApp.Device.Settings.CursorSpeedAndPrecisionMode.OptionalValue.SelectSingleItemByPredicate(item => item.Description == keyDescription);
            };
            HighSpeedModeDetail.RecommendedAppLeft.LaunchingOtherApplicationButtonModel.CommandRaised += (sender, e) =>
            {
                ((LaunchingOtherApplicationButtonModel)sender).StartProcess();
                var keyDescription = HighSpeedModeDetail.ModeButtonModel.ModeDescription;
                refToAppModel.RefToHostApp.Device.Settings.CursorSpeedAndPrecisionMode.OptionalValue.SelectSingleItemByPredicate(item => item.Description == keyDescription);
            };
            HighSpeedModeDetail.RecommendedAppRight.LaunchingOtherApplicationButtonModel.CommandRaised += (sender, e) =>
            {
                ((LaunchingOtherApplicationButtonModel)sender).StartProcess();
                var keyDescription = HighSpeedModeDetail.ModeButtonModel.ModeDescription;
                refToAppModel.RefToHostApp.Device.Settings.CursorSpeedAndPrecisionMode.OptionalValue.SelectSingleItemByPredicate(item => item.Description == keyDescription);
            };
            HighPrecisionModeDetail.RecommendedAppLeft.LaunchingOtherApplicationButtonModel.CommandRaised += (sender, e) =>
            {
                ((LaunchingOtherApplicationButtonModel)sender).StartProcess();
                var keyDescription = HighPrecisionModeDetail.ModeButtonModel.ModeDescription;
                refToAppModel.RefToHostApp.Device.Settings.CursorSpeedAndPrecisionMode.OptionalValue.SelectSingleItemByPredicate(item => item.Description == keyDescription);
            };
            HighPrecisionModeDetail.RecommendedAppRight.LaunchingOtherApplicationButtonModel.CommandRaised += (sender, e) =>
            {
                ((LaunchingOtherApplicationButtonModel)sender).StartProcess();
                var keyDescription = HighPrecisionModeDetail.ModeButtonModel.ModeDescription;
                refToAppModel.RefToHostApp.Device.Settings.CursorSpeedAndPrecisionMode.OptionalValue.SelectSingleItemByPredicate(item => item.Description == keyDescription);
            };
        }

        void ChangeEgsDeviceCursorSpeedAndPrecisionMode(CursorSpeedAndPrecisionModeButtonModel launchingElement)
        {
            var hr = refToAppModel.RefToHostApp.Device.Settings.CursorSpeedAndPrecisionMode.OptionalValue.SelectSingleItemByPredicate(e => e.Description == launchingElement.ModeDescription);
            if (hr == false)
            {
                Debugger.Break();
                hr = refToAppModel.RefToHostApp.Device.Settings.CursorSpeedAndPrecisionMode.OptionalValue.SelectSingleItemByPredicate(e => e.Description == "Standard");
                Debug.Assert(hr);
            }
        }
    }
}
