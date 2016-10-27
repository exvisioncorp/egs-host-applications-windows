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

    partial class Tutorial04DragGestureTrainingPageModel : TutorialEachPageModelBase
    {
        Visibility _PracticeSlideShow01VideoUserControlVisibility;
        public Visibility PracticeSlideShow01VideoUserControlVisibility
        {
            get { return _PracticeSlideShow01VideoUserControlVisibility; }
            set { _PracticeSlideShow01VideoUserControlVisibility = value; OnPropertyChanged("PracticeSlideShow01VideoUserControlVisibility"); }
        }

        Stopwatch stopwatch { get; set; }
        /// <summary>
        /// NOTE: If the area shows (1), Index is equal to 0.  If the area shows (2), Index is equal to 1.
        /// </summary>
        int destinationAreaIndex { get; set; }
        bool isToSayDetailNarrations { get; set; }

        public Tutorial04DragGestureTrainingPageModel()
            : base()
        {
            _DraggingThumbVisibility = Visibility.Collapsed;
            _DraggingThumbLeft = 0;
            _DraggingThumbTop = 0;
            _DraggingThumbWidth = 150;
            _DraggingThumbHeight = 150;
            _DraggingThumbCenterPoint = new Point();
            _LargeCircleAreaCenterPoint = new Point[4];
            _ThumbToLargeCircleAreaCenterDistanceList = new double[4];
            _IsDraggingThumbDragging = false;
            _IsDraggingThumbHovered = false;
            stopwatch = new Stopwatch();
            destinationAreaIndex = 1;
            isToSayDetailNarrations = false;
        }

        public override void InitializeOnceAtStartup(ZkooTutorialModel appModel)
        {
            Trace.Assert(appModel != null);
            refToAppModel = appModel;

            if (ApplicationCommonSettings.IsDebugging)
            {
                foreach (var item in TutorialLargeCircleAreaButtonList)
                {
                    item.IsThumbDraggedChanged += (sender, e) =>
                    {
                        if (item.IsThumbDragged == false) { return; }
                        var msg = "";
                        msg += "Index: " + item.Index + Environment.NewLine;
                        msg += "Dragged: " + item.IsThumbDragged + Environment.NewLine;
                        msg += "LargeCircleAreaCenterPoint[destinationArea.Index]: " + LargeCircleAreaCenterPoint[item.Index] + Environment.NewLine;
                        msg += "ThumbToLargeCircleAreaCenterDistanceList[destinationArea.Index]: " + ThumbToLargeCircleAreaCenterDistanceList[item.Index] + Environment.NewLine;
                        Debug.WriteLine(msg);
                    };
                }
            }
        }

        void InitializeDraggingThumbPosition()
        {
            // margin = 150,80,150,200, Area Diameter = 280, Thumb Diameter = 150
            double areaDiameter = 300;
            double thumbDiameter = 150;
            Point[] cornerAreaLeftTopPointList = new Point[] { new Point(1524, 161), new Point(1524, 652), new Point(97, 652), new Point(97, 161) };

            if (destinationAreaIndex <= 0) { Debugger.Break(); destinationAreaIndex = 1; }
            if (destinationAreaIndex > 4) { Debugger.Break(); destinationAreaIndex = 4; }
            var startAreaLeftTopPoint = cornerAreaLeftTopPointList[destinationAreaIndex - 1];
            DraggingThumbLeft = startAreaLeftTopPoint.X + areaDiameter / 2 - thumbDiameter / 2;
            DraggingThumbTop = startAreaLeftTopPoint.Y + areaDiameter / 2 - thumbDiameter / 2;
        }

        protected override void DoFirstStep()
        {
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();
            DisableAllCircleAreas();

            PracticeSlideShow01VideoUserControlVisibility = Visibility.Visible;

            destinationAreaIndex = 1;
            InitializeDraggingThumbPosition();
            DraggingThumbOpacity = 0.8;
            DraggingThumbVisibility = Visibility.Visible;

            DisableDetection();
            SetCurrentMessage(Messages.View004_Message00100); WaitNarrationComplete(); if (IsCancelling) { return; }
            if (false)
            {
                SetCurrentMessage(Messages.View004_Message00200); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View004_Message00400); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View004_Message00500); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View004_Message00600); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View004_Message00700); WaitNarrationComplete(); if (IsCancelling) { return; }
                SetCurrentMessage(Messages.View004_Message00800); WaitNarrationComplete(); if (IsCancelling) { return; }
            }
            SetCurrentMessage(Messages.View004_Message00900); WaitNarrationComplete(); if (IsCancelling) { return; }
            EnableDetection();

            isToSayDetailNarrations = true;
            stopwatch = new Stopwatch();
            // NOTE: Users do practices after Stop the stopwatch.
            stopwatch.Stop();
            DoOnePractice();
            if (IsCancelling) { return; }
        }

        bool DoOnePractice()
        {
            if (isToSayDetailNarrations) { SetCurrentMessage(Messages.View004_Message01000); }
            else { SetCurrentMessageWithoutVoice(Messages.View004_Message01000); }
            for (destinationAreaIndex = 1; destinationAreaIndex < 4; destinationAreaIndex++)
            {
                var hasSucceededDrag = false;
                while (hasSucceededDrag == false)
                {
                    TutorialLargeCircleAreaButtonList[destinationAreaIndex - 1].IsEnabled = true;
                    DisableAllCircleAreas();
                    InitializeDraggingThumbPosition();
                    DraggingThumbOpacity = 0.8;

                    // Hover is not completed.  So it notifies that users need to hover the gesture cursor.
                    while (IsDraggingThumbHovered == false)
                    {
                        if (IsStillSpeakingNarration == false && CurrentMessage.Equals(Messages.View004_Message01100) == false)
                        {
                            if (isToSayDetailNarrations) { SetCurrentMessage(Messages.View004_Message01100); }
                            else { SetCurrentMessageWithoutVoice(Messages.View004_Message01100); }
                        }
                        if (IsCancelling) { return false; }
                        if (stopwatch.ElapsedMilliseconds > 120000) { return false; }
                        Thread.Sleep(50);
                    }

                    // Hover is completed, so it notifies that users need to touch to the area.
                    while (IsDraggingThumbDragging == false)
                    {
                        if (IsStillSpeakingNarration == false && CurrentMessage.Equals(Messages.View004_Message01200) == false)
                        {
                            if (isToSayDetailNarrations) { SetCurrentMessage(Messages.View004_Message01200); }
                            else { SetCurrentMessageWithoutVoice(Messages.View004_Message01200); }
                        }
                        if (IsDraggingThumbHovered == false) { break; }
                        if (IsCancelling) { return false; }
                        if (stopwatch.ElapsedMilliseconds > 120000) { return false; }
                        Thread.Sleep(50);
                    }
                    // When the other applications are activated, or users move their hands and so on, the hover is not locked-on, so users need to do this again.
                    if (IsDraggingThumbHovered == false) { continue; }

                    StartEffectSuccess();
                    DisableAllCircleAreas();
                    TutorialLargeCircleAreaButtonList[destinationAreaIndex].IsEnabled = true;
                    DraggingThumbOpacity = 0.5;

                    // Touch is completed, so it notifies that users need to drag.
                    if (isToSayDetailNarrations)
                    {
                        switch (destinationAreaIndex)
                        {
                            case 1: SetCurrentMessage(Messages.View004_Message02102); break;
                            case 2: SetCurrentMessage(Messages.View004_Message02103); break;
                            case 3: SetCurrentMessage(Messages.View004_Message02104); break;
                            default: Debugger.Break(); break;
                        }
                    }
                    else
                    {
                        switch (destinationAreaIndex)
                        {
                            case 1: SetCurrentMessageWithoutVoice(Messages.View004_Message02102); break;
                            case 2: SetCurrentMessageWithoutVoice(Messages.View004_Message02103); break;
                            case 3: SetCurrentMessageWithoutVoice(Messages.View004_Message02104); break;
                            default: Debugger.Break(); break;
                        }
                    }

                    while (IsDraggingThumbDragging)
                    {
                        if (IsDraggingThumbHovered == false) { break; }
                        if (IsCancelling) { return false; }
                        if (stopwatch.ElapsedMilliseconds > 120000) { return false; }
                        Thread.Sleep(50);
                    }
                    // When the other applications are activated, or users move their hands and so on, the hover is not locked-on, so users need to do this again.
                    if (IsDraggingThumbHovered == false) { continue; }

                    // Drag is completed.  It judges it succeeded or not.
                    DraggingThumbOpacity = 0.8;
                    if (TutorialLargeCircleAreaButtonList[destinationAreaIndex].IsThumbDragged)
                    {
                        // Succeeded
                        StartEffectSuccess();
                        if (isToSayDetailNarrations)
                        {
                            SetCurrentMessage(Messages.View101_Message00100);
                            WaitNarrationComplete();
                        }
                        else
                        {
                            SetCurrentMessageWithoutVoiceAfterNarrationStop(Messages.View101_Message00100);
                        }
                        hasSucceededDrag = true;
                    }
                    else
                    {
                        // Failed.  Narration starts.
                        SetCurrentMessage(Messages.View004_Message01600);
                    }
                    if (IsCancelling) { return false; }
                    if (stopwatch.ElapsedMilliseconds > 120000) { return false; }
                    Thread.Sleep(50);
                }
            }
            WaitNarrationComplete(); if (IsCancelling) { return false; }
            return true;
        }

        protected override void RepeatPractice()
        {
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();
            DisableAllCircleAreas();

            PracticeSlideShow01VideoUserControlVisibility = Visibility.Collapsed;

            DisableDetection();
            SetCurrentMessage(Messages.View101_Message00225); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View101_Message00600); if (IsCancelling) { return; }
            EnableDetection();

            stopwatch = Stopwatch.StartNew();
            int succeededCount = 0;
            while (true)
            {
                isToSayDetailNarrations = false;
                var hr = DoOnePractice();
                if (IsCancelling) { return; }

                if (hr == false && stopwatch.ElapsedMilliseconds > 120000)
                {
                    DisableDetection();
                    DisableAllCircleAreas();
                    SetCurrentMessage(Messages.View101_Message00300); WaitNarrationComplete(2000); if (IsCancelling) { return; }
                    SetCurrentMessage(Messages.View101_Message00600); if (IsCancelling) { return; }
                    EnableDetection();
                    succeededCount = 0;
                    stopwatch.Restart();
                    continue;
                }

                succeededCount++;
                if (succeededCount == 5) { break; }

                switch (succeededCount)
                {
                    case 1: SetCurrentMessage(Messages.View101_Message00204); break;
                    case 2: SetCurrentMessage(Messages.View101_Message00203); break;
                    case 3: SetCurrentMessage(Messages.View101_Message00202); break;
                    case 4: SetCurrentMessage(Messages.View101_Message00201); break;
                }
            }

            SetCurrentMessage(Messages.View101_Message00500);
            ShowWellDone();
            DisableAllCircleAreas();

            while (true)
            {
                if (IsCancelling) { return; }
                Thread.Sleep(100);
            }
        }
    }
}
