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

    class Tutorial02MoveCursorTrainingPageModel : TutorialEachPageModelBase
    {
        Visibility _PracticeSlideShow01VideoUserControlVisibility;
        public Visibility PracticeSlideShow01VideoUserControlVisibility
        {
            get { return _PracticeSlideShow01VideoUserControlVisibility; }
            set { _PracticeSlideShow01VideoUserControlVisibility = value; OnPropertyChanged("PracticeSlideShow01VideoUserControlVisibility"); }
        }

        NarrationInformation[] cornerAreaNarrations { get; set; }

        public Tutorial02MoveCursorTrainingPageModel()
            : base()
        {
        }

        public override void InitializeOnceAtStartup(ZkooTutorialModel appModel)
        {
            Trace.Assert(appModel != null);
            refToAppModel = appModel;
            cornerAreaNarrations = new NarrationInformation[]
            { 
                Messages.View002_Message00501,
                Messages.View002_Message00502,
                Messages.View002_Message00503,
                Messages.View002_Message00504
            };
        }

        protected override void DoFirstStep()
        {
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();
            DisableAllCircleAreas();

            PracticeSlideShow01VideoUserControlVisibility = Visibility.Visible;

            DisableDetection();
            SetCurrentMessage(Messages.View002_Message00100); WaitNarrationComplete(); if (IsCancelling) { return; }
            //SetCurrentMessage(Messages.View002_Message00200); WaitNarrationComplete(); if (IsCancelling) { return; }
            //SetCurrentMessage(Messages.View002_Message00300); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View002_Message00400); WaitNarrationComplete(); if (IsCancelling) { return; }
            EnableDetection();

            for (int itemIndex = 0; itemIndex < 4; itemIndex++)
            {
                TutorialLargeCircleAreaButtonList[itemIndex].IsEnabled = true;
                SetCurrentMessage(cornerAreaNarrations[itemIndex]); if (IsCancelling) { return; }
                while (TutorialLargeCircleAreaButtonList[itemIndex].IsHovered == false)
                {
                    if (IsCancelling) { return; }
                    Thread.Sleep(50);
                }
                StartEffectSuccess();
                TutorialLargeCircleAreaButtonList[itemIndex].IsEnabled = false;
            }

            DisableDetection();
            SetCurrentMessage(Messages.View101_Message00100); WaitNarrationComplete(); if (IsCancelling) { return; }
            WaitNarrationComplete(1000); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View002_Message00700); WaitNarrationComplete(); if (IsCancelling) { return; }
        }

        protected override void RepeatPractice()
        {
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();
            DisableAllCircleAreas();

            PracticeSlideShow01VideoUserControlVisibility = Visibility.Collapsed;

            DisableDetection();
            SetCurrentMessage(Messages.View101_Message00215); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View101_Message00600); if (IsCancelling) { return; }
            EnableDetection();

            var stopwatch = Stopwatch.StartNew();
            var isTimeOver = false;
            var succeededCount = 0;
            var hasCompletedTheRestCount = true;
            var hasSaidTheStatus = false;

            while (true)
            {
                DisableAllCircleAreas();
                for (int itemIndex = 0; itemIndex < 4; itemIndex++)
                {
                    TutorialLargeCircleAreaButtonList[itemIndex].IsEnabled = true;
                    hasSaidTheStatus = false;
                    while (TutorialLargeCircleAreaButtonList[itemIndex].IsHovered == false)
                    {
                        if (hasCompletedTheRestCount == false && IsStillSpeakingNarration == false)
                        {
                            hasCompletedTheRestCount = true;
                        }
                        if (hasCompletedTheRestCount && hasSaidTheStatus == false)
                        {
                            SetCurrentMessageWithoutVoice(cornerAreaNarrations[itemIndex]);
                            hasSaidTheStatus = true;
                        }
                        if (stopwatch.ElapsedMilliseconds > 60000)
                        {
                            isTimeOver = true;
                            break;
                        }
                        if (IsCancelling) { return; }
                        Thread.Sleep(50);
                    }
                    if (isTimeOver) { break; }
                    StartEffectSuccess();
                    TutorialLargeCircleAreaButtonList[itemIndex].IsEnabled = false;
                }
                if (isTimeOver)
                {
                    DisableAllCircleAreas();
                    DisableDetection();
                    SetCurrentMessage(Messages.View101_Message00300); WaitNarrationComplete(); if (IsCancelling) { return; }
                    SetCurrentMessage(Messages.View101_Message00600); if (IsCancelling) { return; }
                    EnableDetection();
                    succeededCount = 0;
                    stopwatch.Restart();
                    isTimeOver = false;
                }
                else
                {
                    succeededCount++;
                    if (succeededCount == 5)
                    {
                        break;
                    }
                    switch (succeededCount)
                    {
                        case 1: SetCurrentMessage(Messages.View101_Message00204); break;
                        case 2: SetCurrentMessage(Messages.View101_Message00203); break;
                        case 3: SetCurrentMessage(Messages.View101_Message00202); break;
                        case 4: SetCurrentMessage(Messages.View101_Message00201); break;
                    }
                    hasCompletedTheRestCount = false;
                }
            }

            WaitEffectSuccessComplete();
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
