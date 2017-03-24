namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Threading;
    using System.Diagnostics;
    using System.IO;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;

    [DataContract]
    class Tutorial01StartGestureTrainingPageModel : TutorialEachPageModelBase
    {
        Visibility _PracticeSlideShow01VideoUserControlVisibility;
        public Visibility PracticeSlideShow01VideoUserControlVisibility
        {
            get { return _PracticeSlideShow01VideoUserControlVisibility; }
            set { _PracticeSlideShow01VideoUserControlVisibility = value; OnPropertyChanged(nameof(PracticeSlideShow01VideoUserControlVisibility)); }
        }
        Visibility _PracticeSlideShow02VideoUserControlVisibility;
        public Visibility PracticeSlideShow02VideoUserControlVisibility
        {
            get { return _PracticeSlideShow02VideoUserControlVisibility; }
            set { _PracticeSlideShow02VideoUserControlVisibility = value; OnPropertyChanged(nameof(PracticeSlideShow02VideoUserControlVisibility)); }
        }

        public Tutorial01StartGestureTrainingPageModel()
            : base()
        {
        }

        public override void InitializeOnceAtStartup(ZkooTutorialModel appModel)
        {
            Trace.Assert(appModel != null);
            refToAppModel = appModel;
        }

        protected override void DoFirstStep()
        {
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();

            PracticeSlideShow01VideoUserControlVisibility = Visibility.Visible;
            PracticeSlideShow02VideoUserControlVisibility = Visibility.Collapsed;

            Func<bool> isFaceDetectedFunc = () => { return IsFaceDetected; };
            Func<bool> isFaceSelectedFunc = () => { return IsFaceSelected; };
            Func<bool> isHandTrackingFunc = () => { return IsHandTracking; };
            Func<bool> isNotHandTrackingFunc = () => { return IsHandTracking == false; };

            DisableDetection();
            SetCurrentMessage(Messages.View001_Message00100); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View001_Message00200); WaitNarrationComplete(); if (IsCancelling) { return; }
            // Start only face detection
            Application.Current.Dispatcher.Invoke(new Action(() => { refToAppModel.RefToHostApp.Device.Settings.IsToDetectFaces.Value = true; }));

            // Face detection
            WaitRecognitionOfOneFace();
            if (IsCancelling) { return; }

            // Succeeded to detect faces
            StartEffectSuccess();
            WaitNarrationComplete(1000);
            SetCurrentMessage(Messages.View001_Message00300); WaitNarrationComplete(); if (IsCancelling) { return; }

            PracticeSlideShow01VideoUserControlVisibility = Visibility.Collapsed;
            PracticeSlideShow02VideoUserControlVisibility = Visibility.Visible;

            // Explanation of the way of hand detection
            // Start both of face detection and hand detection
            EnableDetection();
            SetCurrentMessage(Messages.View100_Message00500); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View100_Message00600); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View100_Message00700); WaitNarrationComplete(); if (IsCancelling) { return; }

            // Hand detection
            WaitRecognitionOfOneHand();
            if (IsCancelling) { return; }

            // Succeeded to detect hands
            StartEffectSuccess();
            WaitNarrationComplete(1000);
            SetCurrentMessage(Messages.View001_Message00900); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View001_Message01000); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View001_Message01100); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View001_Message01200); WaitNarrationComplete(); if (IsCancelling) { return; }

            // Check again that hand is detected, before checking that users stop their hand gesture
            WaitRecognitionOfOneHand();
            if (IsCancelling) { return; }

            // Check that users stopped their hand gesture
            WaitNarrationComplete(1000);
            while (IsHandTracking)
            {
                SetCurrentMessage(Messages.View001_Message01300); WaitNarrationComplete(5000, isNotHandTrackingFunc); if (IsCancelling || IsHandTracking == false) { break; }
            }
            if (IsCancelling) { return; }

            // Hand gesture is completed
            StartEffectSuccess();
            DisableDetection();
            PracticeSlideShow01VideoUserControlVisibility = Visibility.Collapsed;
            PracticeSlideShow02VideoUserControlVisibility = Visibility.Collapsed;
            SetCurrentMessage(Messages.View001_Message01400); WaitNarrationComplete(); if (IsCancelling) { return; }
        }

        protected override void RepeatPractice()
        {
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();

            DisableDetection();
            SetCurrentMessage(Messages.View001_Message01500); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View001_Message01600); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View101_Message00600); if (IsCancelling) { return; }
            EnableDetection();
            PracticeSlideShow01VideoUserControlVisibility = Visibility.Collapsed;
            PracticeSlideShow02VideoUserControlVisibility = Visibility.Collapsed;

            var stopwatch = Stopwatch.StartNew();
            int succeededCount = 0;
            bool isHandTrackingPrevious = false;
            while (true)
            {
                if (IsStillSpeakingNarration == false && IsHandTracking == false && CurrentMessage.Equals(Messages.View100_Message00400) == false)
                {
                    // It notifies that users need to let the device recognize their hands again, because hand recognition is completed
                    SetCurrentMessageWithoutVoice(Messages.View100_Message00400); if (IsCancelling) { return; }
                }
                if (IsStillSpeakingNarration == false && IsHandTracking == true && CurrentMessage.Equals(Messages.View001_Message01300) == false)
                {
                    // Hand is already recognized, so it notifies that users need to put their hands down.
                    SetCurrentMessageWithoutVoice(Messages.View001_Message01300); if (IsCancelling) { return; }
                }
                if (isHandTrackingPrevious == false && IsHandTracking == true)
                {
                    isHandTrackingPrevious = true;
                }
                if (isHandTrackingPrevious == true && IsHandTracking == false)
                {
                    StartEffectSuccess();
                    succeededCount++;
                    if (succeededCount == 5) { break; }
                    switch (succeededCount)
                    {
                        case 1: SetCurrentMessage(Messages.View101_Message00204); if (IsCancelling) { return; } break;
                        case 2: SetCurrentMessage(Messages.View101_Message00203); if (IsCancelling) { return; } break;
                        case 3: SetCurrentMessage(Messages.View101_Message00202); if (IsCancelling) { return; } break;
                        case 4: SetCurrentMessage(Messages.View101_Message00201); if (IsCancelling) { return; } break;
                    }
                    isHandTrackingPrevious = false;
                }
                if (stopwatch.ElapsedMilliseconds > 60000)
                {
                    DisableDetection();
                    SetCurrentMessage(Messages.View101_Message00300); WaitNarrationComplete(); if (IsCancelling) { return; }
                    SetCurrentMessage(Messages.View001_Message01600); WaitNarrationComplete(); if (IsCancelling) { return; }
                    SetCurrentMessage(Messages.View101_Message00600); if (IsCancelling) { return; }
                    EnableDetection();
                    succeededCount = 0;
                    stopwatch.Restart();
                }
                if (IsCancelling) { return; }
                Thread.Sleep(50);
            }

            WaitEffectSuccessComplete();
            SetCurrentMessage(Messages.View101_Message00500);
            EnableDetection();
            ShowWellDone();

            // It waits until some button is pushed.
            while (true)
            {
                if (IsCancelling) { return; }
                Thread.Sleep(100);
            }
        }
    }
}
