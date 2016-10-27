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
    class Tutorial03TapGestureTrainingPageModel : TutorialEachPageModelBase
    {
        Visibility _PracticeSlideShow01VideoUserControlVisibility;
        public Visibility PracticeSlideShow01VideoUserControlVisibility
        {
            get { return _PracticeSlideShow01VideoUserControlVisibility; }
            set { _PracticeSlideShow01VideoUserControlVisibility = value; OnPropertyChanged("PracticeSlideShow01VideoUserControlVisibility"); }
        }

        public Tutorial03TapGestureTrainingPageModel()
            : base()
        {
        }

        public override void InitializeOnceAtStartup(ZkooTutorialModel appModel)
        {
            Trace.Assert(appModel != null);
            refToAppModel = appModel;
        }

        int currentTargetCircleAreaIndex { get; set; }

        protected override void DoFirstStep()
        {
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();
            DisableAllCircleAreas();

            PracticeSlideShow01VideoUserControlVisibility = Visibility.Visible;

            DisableDetection();
            if (false)
            {
                currentTargetCircleAreaIndex = 0;
                while (TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].IsHovered == false)
                {
                    // It repeats until users put a gesture cursor on the right-top corner.
                    SetCurrentMessage(Messages.View003_Message00200);
                    Func<bool> isTheButtonIsMouseOver = () => { return TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].IsHovered; };
                    WaitNarrationComplete(isTheButtonIsMouseOver); WaitNarrationComplete(); if (IsCancelling) { return; }
                    WaitNarrationComplete(2000); if (IsCancelling) { return; }
                    Thread.Sleep(100);
                }
                SetCurrentMessage(Messages.View003_Message00200); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View003_Message00300); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View003_Message00400); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View003_Message00500); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View003_Message00700); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View003_Message00800); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View003_Message00900); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View003_Message01000); WaitNarrationComplete(); if (IsCancelling) { return; }
            }
            SetCurrentMessage(Messages.View003_Message00100); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View003_Message01100); WaitNarrationComplete(); if (IsCancelling) { return; }
            EnableDetection();

            for (currentTargetCircleAreaIndex = 0; currentTargetCircleAreaIndex < 4; currentTargetCircleAreaIndex++)
            {
                TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].IsEnabled = true;
                switch (currentTargetCircleAreaIndex)
                {
                    case 0: SetCurrentMessage(Messages.View003_Message01201); WaitNarrationComplete(); break;
                    case 1: SetCurrentMessage(Messages.View003_Message01202); WaitNarrationComplete(); break;
                    case 2: SetCurrentMessage(Messages.View003_Message01203); WaitNarrationComplete(); break;
                    case 3: SetCurrentMessage(Messages.View003_Message01204); WaitNarrationComplete(); break;
                }
                var hasFailedInThisCorner = false;
                Stopwatch stopwatch = new Stopwatch();
                while (true)
                {
                    TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].TapsCount = 0;
                    TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].LongTapsCount = 0;
                    stopwatch = Stopwatch.StartNew();
                    while (TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].IsHovered == false)
                    {
                        if (stopwatch.ElapsedMilliseconds > 60000 && ReplayPracticeNextButtonsUserControlVisibility != Visibility.Visible)
                        {
                            ReplayPracticeNextButtonsUserControlVisibility = Visibility.Visible;
                        }
                        if (ReplayPracticeNextButtonsUserControlVisibility == Visibility.Visible && IsFaceDetected)
                        {
                            ReplayPracticeNextButtonsUserControlVisibility = Visibility.Collapsed;
                            stopwatch.Restart();
                        }
                        if (IsCancelling) { return; }
                        Thread.Sleep(50);
                    }
                    if (TutorialUpperSideMessageArea.Visibility == Visibility.Visible)
                    {
                        HideUpperSideMessageAreaAndReplayPracticeNextButtons();
                    }

                    if (hasFailedInThisCorner == false)
                    {
                        SetCurrentMessage(Messages.View003_Message00700); if (IsCancelling) { return; }
                    }
                    else
                    {
                        SetCurrentMessage(Messages.View003_Message00800); if (IsCancelling) { return; }
                    }

                    TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].Reset();
                    var lastTapsCount = TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].TapsCount;
                    while (IsStillSpeakingNarration)
                    {
                        if (lastTapsCount != TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].TapsCount)
                        {
                            StartEffectSuccess();
                            lastTapsCount = TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].TapsCount;
                        }
                        if (IsCancelling) { return; }
                        Thread.Sleep(50);
                    }

                    // Add waiting time after saying "1, 2, 3".
                    WaitNarrationComplete(1000);

                    if (TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].LongTapsCount > 0)
                    {
                        // Detect long tap.
                        DisableDetection();
                        TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].IsEnabled = false;
                        SetCurrentMessage(Messages.View003_Message00900); WaitNarrationComplete(); if (IsCancelling) { return; }
                        SetCurrentMessage(Messages.View003_Message01000); WaitNarrationComplete(); if (IsCancelling) { return; }
                        EnableDetection();
                        TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].IsEnabled = true;
                    }
                    else if (TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].TapsCount < 3)
                    {
                        // When users fails to tap.
                        DisableDetection();
                        TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].IsEnabled = false;
                        SetCurrentMessage(Messages.View003_Message00300); WaitNarrationComplete(); if (IsCancelling) { return; }
                        SetCurrentMessage(Messages.View003_Message00500); WaitNarrationComplete(); if (IsCancelling) { return; }
                        EnableDetection();
                        TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].IsEnabled = true;
                    }
                    else
                    {
                        // Succeeded
                        SetCurrentMessage(Messages.View101_Message00100); WaitNarrationComplete(); if (IsCancelling) { return; }
                        break;
                    }
                }
                TutorialLargeCircleAreaButtonList[currentTargetCircleAreaIndex].IsEnabled = false;
            }
        }

        protected override void RepeatPractice()
        {
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();
            DisableAllCircleAreas();

            PracticeSlideShow01VideoUserControlVisibility = Visibility.Collapsed;

            EnableDetection();

            var stopwatch = Stopwatch.StartNew();

            SetCurrentMessage(Messages.View101_Message00500);
            ShowWellDone();

            while (true)
            {
                if (IsCancelling) { return; }
                Thread.Sleep(100);
            }
        }
    }
}
