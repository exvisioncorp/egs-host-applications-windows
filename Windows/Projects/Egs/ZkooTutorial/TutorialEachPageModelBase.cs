namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO;
    using System.Net;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;
    using System.Windows;
    using Egs;
    using Egs.Views;
    using Egs.DotNetUtility;

    [DataContract]
    abstract class TutorialEachPageModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        public string PageTitle { get; internal set; }
        public string PageDescription { get; internal set; }
        public ZkooTutorialModel refToAppModel { get; protected set; }
        public TutorialAppHeaderMenuViewModel TutorialAppHeaderMenu { get { return refToAppModel.TutorialAppHeaderMenu; } }
        public TutorialUpperSideMessageAreaViewModel TutorialUpperSideMessageArea { get; private set; }
        public EgsHostAppBaseComponents HostApp { get { return refToAppModel.RefToHostApp; } }
        internal void NotifyModelsAgain()
        {
            OnPropertyChanged(nameof(HostApp));
            HostApp.RaiseMultipleObjectsPropertyChanged();
        }

        Visibility _ReplayPracticeNextButtonsUserControlVisibility;
        public Visibility ReplayPracticeNextButtonsUserControlVisibility
        {
            get { return _ReplayPracticeNextButtonsUserControlVisibility; }
            set { _ReplayPracticeNextButtonsUserControlVisibility = value; OnPropertyChanged(nameof(ReplayPracticeNextButtonsUserControlVisibility)); }
        }

        internal Task CurrentTask { get; set; }

        public TutorialLargeCircleAreaButtonModel TutorialLargeCircleAreaButtonRightTop { get { return refToAppModel.TutorialLargeCircleAreaButtonRightTop; } }
        public TutorialLargeCircleAreaButtonModel TutorialLargeCircleAreaButtonRightBottom { get { return refToAppModel.TutorialLargeCircleAreaButtonRightBottom; } }
        public TutorialLargeCircleAreaButtonModel TutorialLargeCircleAreaButtonLeftBottom { get { return refToAppModel.TutorialLargeCircleAreaButtonLeftBottom; } }
        public TutorialLargeCircleAreaButtonModel TutorialLargeCircleAreaButtonLeftTop { get { return refToAppModel.TutorialLargeCircleAreaButtonLeftTop; } }
        public List<TutorialLargeCircleAreaButtonModel> TutorialLargeCircleAreaButtonList { get { return refToAppModel.TutorialLargeCircleAreaButtonList; } }

        public NarrationInformationList Messages { get { return refToAppModel.CurrentResources.Messages; } }

        NarrationInformation _CurrentMessage;
        public NarrationInformation CurrentMessage
        {
            get { return _CurrentMessage; }
            private set { _CurrentMessage = value; OnPropertyChanged(nameof(CurrentMessage)); }
        }

        IAudioPlayer BgmAudio { get; set; }
        IAudioPlayer NarrationAudio { get; set; }
        IAudioPlayer SeAudio { get; set; }

        public TutorialEachPageModelBase()
        {
            DefaultWaitingMillisecondsAfterCompletion = 2000;
            PageTitle = "";
            PageDescription = "";
            _ReplayPracticeNextButtonsUserControlVisibility = Visibility.Collapsed;
            IsCancelling = false;

            TutorialUpperSideMessageArea = new TutorialUpperSideMessageAreaViewModel();
        }

        public abstract void InitializeOnceAtStartup(ZkooTutorialModel appModel);

        public void EnableDetection()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                // NOTE: In some firmware version (>1.1), if face detection is off and hand detection is on, strange borders appears in Camera View.
                refToAppModel.RefToHostApp.Device.Settings.IsToDetectFaces.Value = true;
                refToAppModel.RefToHostApp.Device.Settings.IsToDetectHands.Value = true;
            }));
        }

        public void DisableDetection()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                // NOTE: In some firmware version, the order is important.  If you chagne this order, borders of "Face.Area" does not disappear.
                refToAppModel.RefToHostApp.Device.Settings.IsToDetectHands.Value = false;
                refToAppModel.RefToHostApp.Device.Settings.IsToDetectFaces.Value = false;
            }));
        }

        public bool IsFaceDetected
        {
            get { return refToAppModel.RefToHostApp.Device.EgsGestureHidReport.DetectedFacesCount > 0; }
        }
        public bool IsFaceSelected
        {
            get { return refToAppModel.RefToHostApp.Device.EgsGestureHidReport.SelectedFaceIndex >= 0; }
        }
        public bool IsHandTracking
        {
            get { return refToAppModel.RefToHostApp.Device.EgsGestureHidReport.TrackingHandsCount > 0; }
        }
        public bool IsCancelling { get; internal set; }

        public bool IsStillSpeakingNarration
        {
            get { return NarrationAudio.IsActuallyPlaying; }
        }

        public void SetCurrentMessageWithoutVoiceAfterNarrationStop(NarrationInformation value)
        {
            NarrationAudio.Stop();
            CurrentMessage = value;
        }

        protected void ShowWellDone()
        {
            TutorialUpperSideMessageArea.LeftTextBlockText = Properties.Resources.ZkooTutorialModel_WellDone;
            TutorialUpperSideMessageArea.RightTextBlockText = "";
            TutorialUpperSideMessageArea.Visibility = Visibility.Visible;
            ReplayPracticeNextButtonsUserControlVisibility = Visibility.Visible;
        }

        protected void ShowTryAgain()
        {
            // NOTE: Not used!
            TutorialUpperSideMessageArea.LeftTextBlockText = Properties.Resources.ZkooTutorialModel_TryAgain;
            TutorialUpperSideMessageArea.RightTextBlockText = Properties.Resources.ZkooTutorialModel_TryAgainDetail;
            TutorialUpperSideMessageArea.Visibility = Visibility.Visible;
            ReplayPracticeNextButtonsUserControlVisibility = Visibility.Visible;
        }

        protected void HideUpperSideMessageAreaAndReplayPracticeNextButtons()
        {
            TutorialUpperSideMessageArea.LeftTextBlockText = "";
            TutorialUpperSideMessageArea.RightTextBlockText = "";
            TutorialUpperSideMessageArea.Visibility = Visibility.Collapsed;
            ReplayPracticeNextButtonsUserControlVisibility = Visibility.Collapsed;
        }

        protected void DisableAllCircleAreas()
        {
            foreach (var item in TutorialLargeCircleAreaButtonList) { item.IsEnabled = false; }
        }

        public void SetCurrentMessageWithoutVoice(NarrationInformation value)
        {
            CurrentMessage = value;
        }

        public void SetCurrentMessage(NarrationInformation value)
        {
            CurrentMessage = value;
            NarrationAudio.StartAsync(refToAppModel.CurrentResources.SoundFilesFolderPath + value.OggAudioFileName);
        }

        public int DefaultWaitingMillisecondsAfterCompletion { get; set; }

        public void WaitNarrationComplete()
        {
            WaitNarrationComplete(DefaultWaitingMillisecondsAfterCompletion, () => false);
        }
        public void WaitNarrationComplete(int waitingMillisecondsAfterCompletion)
        {
            WaitNarrationComplete(DefaultWaitingMillisecondsAfterCompletion, () => false);
        }
        public void WaitNarrationComplete(Func<bool> conditionOfStoppingNarrationFunc)
        {
            WaitNarrationComplete(DefaultWaitingMillisecondsAfterCompletion, conditionOfStoppingNarrationFunc);
        }
        public void WaitNarrationComplete(int waitingMillisecondsAfterCompletion, Func<bool> conditionOfStoppingNarrationFunc)
        {
            while (true)
            {
                if (IsStillSpeakingNarration == false) { break; }
                if (conditionOfStoppingNarrationFunc()) { break; }
                if (IsCancelling) { break; }
                Thread.Sleep(50);
            }
            if (IsCancelling) { NarrationAudio.Stop(); return; }
            var stopwatch = Stopwatch.StartNew();
            while (true)
            {
                if (stopwatch.ElapsedMilliseconds > waitingMillisecondsAfterCompletion) { break; }
                if (conditionOfStoppingNarrationFunc()) { break; }
                if (IsCancelling) { break; }
                Thread.Sleep(50);
            }
            NarrationAudio.Stop();
        }

        public void StartEffectSuccess()
        {
            SeAudio.StartAsync(refToAppModel.CurrentResources.SoundFilesFolderPath + "tutorial_effect_success.ogg");
        }
        public void WaitEffectSuccessComplete()
        {
            while (true)
            {
                if (SeAudio.IsActuallyPlaying == false) { break; }
                if (IsCancelling) { break; }
                Thread.Sleep(50);
            }
            if (IsCancelling) { NarrationAudio.Stop(); return; }
        }

        protected bool isToShowReplayPracticeNextButtonsUserControl = false;

        protected void WaitRecognitionOfOneFace()
        {
            Func<bool> isFaceDetectedFunc = () => { return IsFaceDetected; };
            while (IsFaceDetected == false)
            {
                // NOTE: If some face is detected, it says "let the device recognize your face" so that a first-time player can easily understand.
                if (false) { SetCurrentMessage(Messages.View100_Message00100); WaitNarrationComplete(isFaceDetectedFunc); if (IsCancelling || IsFaceDetected) { break; } }
                else { SetCurrentMessage(Messages.View100_Message00100); WaitNarrationComplete(); if (IsCancelling || IsFaceDetected) { break; } }

                SetCurrentMessage(Messages.View100_Message00200); WaitNarrationComplete(isFaceDetectedFunc); if (IsCancelling || IsFaceDetected) { break; }
                SetCurrentMessage(Messages.View100_Message00300); WaitNarrationComplete(isFaceDetectedFunc); if (IsCancelling || IsFaceDetected) { break; }
                if (isToShowReplayPracticeNextButtonsUserControl)
                {
                    ReplayPracticeNextButtonsUserControlVisibility = Visibility.Visible;
                    SetCurrentMessage(Messages.View001_Message00400); WaitNarrationComplete(isFaceDetectedFunc); if (IsCancelling || IsFaceDetected) { break; }
                    SetCurrentMessage(Messages.View001_Message00500); WaitNarrationComplete(isFaceDetectedFunc); if (IsCancelling || IsFaceDetected) { break; }
                    SetCurrentMessage(Messages.View001_Message00600); WaitNarrationComplete(isFaceDetectedFunc); if (IsCancelling || IsFaceDetected) { break; }
                }
                WaitNarrationComplete(10000, isFaceDetectedFunc); if (IsCancelling || IsFaceDetected) { break; }
            }
            if (isToShowReplayPracticeNextButtonsUserControl)
            {
                ReplayPracticeNextButtonsUserControlVisibility = Visibility.Collapsed;
            }
        }

        protected void WaitRecognitionOfOneHand()
        {
            Func<bool> isHandTrackingFunc = () => { return IsHandTracking; };

            while (IsHandTracking == false)
            {
                if (IsFaceDetected == false)
                {
                    WaitRecognitionOfOneFace();
                }
                if (IsCancelling) { return; }

                while (IsHandTracking == false)
                {
                    SetCurrentMessage(Messages.View100_Message00400); WaitNarrationComplete(isHandTrackingFunc); if (IsCancelling || IsHandTracking || (IsFaceDetected == false)) { break; }
                    SetCurrentMessage(Messages.View100_Message00500); WaitNarrationComplete(isHandTrackingFunc); if (IsCancelling || IsHandTracking || (IsFaceDetected == false)) { break; }
                    SetCurrentMessage(Messages.View100_Message00600); WaitNarrationComplete(isHandTrackingFunc); if (IsCancelling || IsHandTracking || (IsFaceDetected == false)) { break; }
                    SetCurrentMessage(Messages.View100_Message00700); WaitNarrationComplete(isHandTrackingFunc); if (IsCancelling || IsHandTracking || (IsFaceDetected == false)) { break; }
                    WaitNarrationComplete(10000, isHandTrackingFunc);
                }
                if (IsCancelling) { return; }
            }
        }

        void CreateAudioPlayerInstances()
        {
            BgmAudio = new NAudioWrapper();
            NarrationAudio = new NAudioWrapper();
            SeAudio = new NAudioWrapper();
            BgmAudio.IsToShowMessageBoxOfExceptions = false;
            NarrationAudio.IsToShowMessageBoxOfExceptions = false;
            SeAudio.IsToShowMessageBoxOfExceptions = false;
        }

        void StopAllSounds()
        {
            BgmAudio.Stop();
            NarrationAudio.Stop();
            SeAudio.Stop();
        }

        internal void OnLoaded()
        {
            CurrentTask = Task.Run(() =>
            {
                NotifyModelsAgain();

                OnLoadedInTaskRun();
                DoFirstStep();
                if (IsCancelling) { OnUnloadedInTaskRun(); return; }
                RepeatPractice();
                OnUnloadedInTaskRun();
            });

        }
        protected virtual void OnLoadedInTaskRun()
        {
            IsCancelling = false;
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();
            refToAppModel.EnableUpdatingCameraViewImageButHideWindow();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                refToAppModel.RefToHostApp.Device.Settings.CursorSpeedAndPrecisionMode.OptionalValue.SelectedIndex = 0;
            }));

            CreateAudioPlayerInstances();
            if (false)
            {
                // TODO: add bgm
                // NOTE: must be called in Task.Run() in OnLoaded
                if (false) BgmAudio.StartAsync(refToAppModel.CurrentResources.SoundFilesFolderPath + @"\bgm.ogg");
            }
        }

        abstract protected void DoFirstStep();

        abstract protected void RepeatPractice();

        protected virtual void OnUnloadedInTaskRun()
        {
            IsCancelling = true;
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();
            StopAllSounds();
        }
    }
}
