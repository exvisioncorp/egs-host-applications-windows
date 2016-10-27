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
    using System.Collections.ObjectModel;

    class Tutorial05FlickGestureTrainingPageModel : TutorialEachPageModelBase
    {
        public ObservableCollection<Tutorial05ListViewItemModel> ListViewItemList { get; set; }
        Visibility _ScrollAreaStackPanelVisibility;
        public Visibility ScrollAreaStackPanelVisibility
        {
            get { return _ScrollAreaStackPanelVisibility; }
            set { _ScrollAreaStackPanelVisibility = value; OnPropertyChanged("ScrollAreaStackPanelVisibility"); }
        }
        public bool IsDragging { get; set; }
        public Point DragStartedPoint { get; set; }
        public Point DraggingPoint { get; set; }

        public double FlickingCursorVelocityOnDragCompleteX { get; set; }

        enum PracticeKind { FirstPractice, MultiplePractices }
        PracticeKind currentPracticeKind { get; set; }
        Stopwatch letUserOnePracticeStopwatch { get; set; }

        public Tutorial05FlickGestureTrainingPageModel()
            : base()
        {
            var brushesProperties = typeof(Brushes).GetProperties();
            var collection = brushesProperties.Select(e =>
            {
                var brush = (SolidColorBrush)e.GetValue(null);
                var color = brush.Color;
                var avg = (color.R + color.G + color.B) / 3;
                var v = avg > 127 ? 0 : 255;
                var ret = new Tutorial05ListViewItemModel();
                ret.BackgroundBrush = brush;
                ret.ForegroundBrush = new SolidColorBrush(Color.FromRgb((byte)v, (byte)v, (byte)v));
                ret.Name = e.Name;
                ret.ValueString = color.ToString();
                return ret;
            });
            var enlargedList = collection.ToList();
            //for (int i = 0; i < 5; i++) { enlargedList.AddRange(collection); }
            ListViewItemList = new ObservableCollection<Tutorial05ListViewItemModel>(enlargedList);

            FlickingCursorVelocityOnDragCompleteX = 0;
            currentPracticeKind = PracticeKind.FirstPractice;
            letUserOnePracticeStopwatch = new Stopwatch();
        }

        public override void InitializeOnceAtStartup(ZkooTutorialModel appModel)
        {
            Trace.Assert(appModel != null);
            refToAppModel = appModel;
        }

        void LetUserOnePractice()
        {
            var baseLengthVector = new Vector(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
            var baseLength = baseLengthVector.Length;

            // If it fails, it loops.
            while (true)
            {
                var stopwatchInWaitingDraggingStart = Stopwatch.StartNew();
                int narrationKind = 0;
                while (IsDragging == false)
                {
                    if (narrationKind == 0 && IsStillSpeakingNarration == false)
                    {
                        stopwatchInWaitingDraggingStart.Restart();
                        narrationKind++;
                    }
                    else if (narrationKind == 1 && stopwatchInWaitingDraggingStart.ElapsedMilliseconds > 2000)
                    {
                        switch (currentPracticeKind)
                        {
                            case PracticeKind.FirstPractice:
                                SetCurrentMessage(Messages.View005_Message00400);
                                break;
                            case PracticeKind.MultiplePractices:
                                SetCurrentMessageWithoutVoice(Messages.View005_Message00400);
                                break;
                            default:
                                Debugger.Break();
                                throw new NotImplementedException();
                        }
                        narrationKind++;
                    }
                    else if (narrationKind == 2 && stopwatchInWaitingDraggingStart.ElapsedMilliseconds > 20000)
                    {
                        SetCurrentMessage(Messages.View005_Message00200);
                        narrationKind = 0;
                    }

                    if (currentPracticeKind == PracticeKind.MultiplePractices
                        && letUserOnePracticeStopwatch.Elapsed.TotalMilliseconds > 60000) { return; }

                    if (IsCancelling) { return; }
                    Thread.Sleep(50);
                }

                switch (currentPracticeKind)
                {
                    case PracticeKind.FirstPractice:
                        SetCurrentMessage(Messages.View005_Message00500);
                        break;
                    case PracticeKind.MultiplePractices:
                        SetCurrentMessageWithoutVoice(Messages.View005_Message00500);
                        break;
                    default:
                        Debugger.Break();
                        throw new NotImplementedException();
                }

                bool isDistanceTooLarge = false;
                while (IsDragging)
                {
                    if (isDistanceTooLarge == false)
                    {
                        var distance = (DraggingPoint - DragStartedPoint).Length;
                        if (false) { Debug.WriteLine("distance: " + distance); }
                        isDistanceTooLarge = distance > 0.5 * baseLength;

                        if (isDistanceTooLarge)
                        {
                            // When users moved their hands too large.
                            switch (currentPracticeKind)
                            {
                                case PracticeKind.FirstPractice:
                                    DisableDetection();
                                    SetCurrentMessage(Messages.View005_Message00600); WaitNarrationComplete(); if (IsCancelling) { return; }
                                    SetCurrentMessage(Messages.View005_Message00900); WaitNarrationComplete(); if (IsCancelling) { return; }
                                    break;
                                case PracticeKind.MultiplePractices:
                                    if (IsStillSpeakingNarration == false && CurrentMessage.Equals(Messages.View005_Message00600) == false)
                                    {
                                        SetCurrentMessage(Messages.View005_Message00600);
                                    }
                                    break;
                                default:
                                    Debugger.Break();
                                    throw new NotImplementedException();
                            }
                            // It restarts from initial detection again.
                            EnableDetection();
                        }
                    }

                    if (currentPracticeKind == PracticeKind.MultiplePractices
                        && letUserOnePracticeStopwatch.Elapsed.TotalMilliseconds > 60000) { return; }

                    if (IsCancelling) { return; }
                    Thread.Sleep(50);
                }
                // When the hand move distance was too large, it does not evaluate the hand speed in stopping drag gesture.
                if (isDistanceTooLarge) { continue; }

                Debug.WriteLine("Math.Abs(FlickingCursorVelocityOnDragCompleteX): " + Math.Abs(FlickingCursorVelocityOnDragCompleteX));
                if (Math.Abs(FlickingCursorVelocityOnDragCompleteX) < 0.2 * baseLength)
                {
                    // If the hand speed in opening the hand was too slow.
                    switch (currentPracticeKind)
                    {
                        case PracticeKind.FirstPractice:
                            DisableDetection();
                            SetCurrentMessage(Messages.View005_Message00800); WaitNarrationComplete(); if (IsCancelling) { return; }
                            SetCurrentMessage(Messages.View005_Message00700); WaitNarrationComplete(); if (IsCancelling) { return; }
                            // It restarts from initial detection again.
                            EnableDetection();
                            break;
                        case PracticeKind.MultiplePractices:
                            SetCurrentMessage(Messages.View005_Message00700);
                            break;
                        default:
                            Debugger.Break();
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    // Succeeded
                    StartEffectSuccess();
                    return;
                }
            }
        }

        protected override void DoFirstStep()
        {
            ScrollAreaStackPanelVisibility = Visibility.Visible;
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();

            DisableDetection();
            SetCurrentMessage(Messages.View005_Message00100); WaitNarrationComplete(); if (IsCancelling) { return; }
            SetCurrentMessage(Messages.View005_Message00200); WaitNarrationComplete(); if (IsCancelling) { return; }
            EnableDetection();
            SetCurrentMessage(Messages.View005_Message00300); if (IsCancelling) { return; }

            currentPracticeKind = PracticeKind.FirstPractice;
            letUserOnePracticeStopwatch.Reset();
            LetUserOnePractice();
            if (IsCancelling) { return; }

            DisableDetection();
            SetCurrentMessage(Messages.View101_Message00100); WaitNarrationComplete(); if (IsCancelling) { return; }
        }

        protected override void RepeatPractice()
        {
            ScrollAreaStackPanelVisibility = Visibility.Visible;
            HideUpperSideMessageAreaAndReplayPracticeNextButtons();

            DisableDetection();
            SetCurrentMessage(Messages.View101_Message00215); WaitNarrationComplete(); if (IsCancelling) { return; }
            EnableDetection();
            SetCurrentMessage(Messages.View101_Message00600); if (IsCancelling) { return; }

            currentPracticeKind = PracticeKind.MultiplePractices;
            // It restarts before 5 times loop.
            letUserOnePracticeStopwatch.Restart();
            int succeededCount = 0;
            while (true)
            {
                LetUserOnePractice();
                if (IsCancelling) { return; }

                if (letUserOnePracticeStopwatch.Elapsed.TotalMilliseconds > 60000)
                {
                    DisableDetection();
                    SetCurrentMessage(Messages.View101_Message00300); WaitNarrationComplete(2000); if (IsCancelling) { return; }
                    SetCurrentMessage(Messages.View101_Message00600); if (IsCancelling) { return; }
                    EnableDetection();
                    succeededCount = 0;
                    letUserOnePracticeStopwatch.Restart();
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

            SetCurrentMessage(Messages.View101_Message00100);
            DisableDetection();
            WaitNarrationComplete();
            if (IsCancelling) { return; }
            Thread.Sleep(2000);

            EnableDetection();
            ScrollAreaStackPanelVisibility = Visibility.Collapsed;
            SetCurrentMessage(Messages.View005_Message02700);
            ShowWellDone();

            while (true)
            {
                if (IsCancelling) { return; }
                Thread.Sleep(100);
            }
        }
    }

    class Tutorial05ListViewItemModel
    {
        public SolidColorBrush BackgroundBrush { get; set; }
        public SolidColorBrush ForegroundBrush { get; set; }
        public string Name { get; set; }
        public string ValueString { get; set; }
    }
}
