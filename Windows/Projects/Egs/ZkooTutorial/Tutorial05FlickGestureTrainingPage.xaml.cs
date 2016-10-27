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
    using System.Windows.Threading;

    partial class Tutorial05FlickGestureTrainingPage : Page, IHasTutorialEachPageModelBase
    {
        public TutorialEachPageModelBase ReferenceToTutorialEachPageModelBase { get; set; }

        Tutorial05FlickGestureTrainingPageModel refToViewModel { get; set; }

        public Tutorial05FlickGestureTrainingPage()
        {
            InitializeComponent();
        }

        public void InitializeOnceAtStartup(MainNavigationWindow navigator, Tutorial05FlickGestureTrainingPageModel viewModel)
        {
            Trace.Assert(navigator != null);
            Trace.Assert(viewModel != null);

            this.DataContext = viewModel;
            ReferenceToTutorialEachPageModelBase = viewModel;
            ReplayPracticeNextButtonsUserControl.InitializeOnceAtStartup(viewModel.TutorialAppHeaderMenu);
            TutorialAppHeaderMenuUserControl.InitializeOnceAtStartup(viewModel.TutorialAppHeaderMenu);

            refToViewModel = viewModel;

            InitializeScrollArea();

            this.Loaded += (sender, e) =>
            {
                navigator.SetWindowFullScreen();
                navigator.Title = "ZKOO Tutorial: Flick Gesture Practice";
                viewModel.OnLoaded();
            };
            this.Unloaded += (sender, e) =>
            {
                viewModel.IsCancelling = true;
            };
        }

        bool _IsDragging = false;
        bool IsDragging
        {
            get { return _IsDragging; }
            set
            {
                _IsDragging = value;
                refToViewModel.IsDragging = value;
            }
        }
        double ContentHorizontalOffsetDesired = 0;

        DispatcherTimer DragVelocityEstimationAndFlickScrollingDispatcherTimer = new DispatcherTimer();
        double TimerIntervalTotalSeconds = 1.0 / 60.0;
        Stopwatch DraggingDeltaTimeStopwatch = Stopwatch.StartNew();

        Point DraggingPoint = new Point();
        Point DraggingPointPreviousForScrollToHorizontalOffset = new Point();
        Point DraggingPointPreviousForCursorVelocityEstimation = new Point();
        Vector DraggingCursorVelocity = new Vector();
        double DraggingCursorVelocityEmaAlphaInDeltaTime = 0.2;

        Vector FlickingCursorVelocityOnDragComplete = new Vector();
        Vector FlickingCursorVelocity = new Vector();
        double FlickingCursorVelocityEmaAlphaInDeltaTime;
        void UpdateFlickingCursorVelocityEmaAlphaInDeltaTime()
        {
            // exp(q*1.0)=a  q=log(a)  exp(q*dt)=x  x=exp(log(a)*dt)
            FlickingCursorVelocityEmaAlphaInDeltaTime = Math.Exp(Math.Log(FlickingCursorVelocityEmaAlphaInOneSecond) * TimerIntervalTotalSeconds);
            Debug.WriteLine("FlickingCursorVelocityEmaAlphaInDeltaTime: " + FlickingCursorVelocityEmaAlphaInDeltaTime);
        }
        double _FlickingCursorVelocityEmaAlphaInOneSecond = 0.1;
        double FlickingCursorVelocityEmaAlphaInOneSecond
        {
            get { return _FlickingCursorVelocityEmaAlphaInOneSecond; }
            set { _FlickingCursorVelocityEmaAlphaInOneSecond = value; UpdateFlickingCursorVelocityEmaAlphaInDeltaTime(); }
        }

        void InitializeScrollArea()
        {
            DragVelocityEstimationAndFlickScrollingDispatcherTimer.Interval = TimeSpan.FromSeconds(TimerIntervalTotalSeconds);
            UpdateFlickingCursorVelocityEmaAlphaInDeltaTime();

            ScrollAreaListView.SizeChanged += (sender, e) =>
            {
                ContentHorizontalOffsetDesired = -ScrollAreaListView.ActualWidth / 2;
                ScrollAreaScrollViewer.ScrollToHorizontalOffset(-ContentHorizontalOffsetDesired);
            };

            // TODO: In Windows 7, flick gesture is unavailable in the first execution of this application.
            if (ApplicationCommonSettings.IsToEmulateReportByActualMouseRawInputToDebugViews)
            {
                ScrollAreaListView.MouseLeftButtonDown += (sender, e) => { OnScrollAreaDragStarted(e.GetPosition(ScrollAreaScrollViewer)); };
                ScrollAreaListView.MouseMove += (sender, e) => { OnScrollAreaDragging(e.GetPosition(ScrollAreaScrollViewer)); };
                ScrollAreaListView.MouseLeftButtonUp += (sender, e) => { if (IsDragging) { OnScrollAreaDragEndedFlickStarted(); } };
                ScrollAreaListView.MouseLeave += (sender, e) => { if (IsDragging) { OnScrollAreaDragEndedFlickStarted(); } };
            }
            else
            {
                ScrollAreaListView.TouchDown += (sender, e) => { OnScrollAreaDragStarted(e.GetTouchPoint(ScrollAreaScrollViewer).Position); };
                ScrollAreaListView.TouchMove += (sender, e) => { OnScrollAreaDragging(e.GetTouchPoint(ScrollAreaScrollViewer).Position); };
                ScrollAreaListView.TouchUp += (sender, e) => { if (IsDragging) { OnScrollAreaDragEndedFlickStarted(); } };
                ScrollAreaListView.TouchLeave += (sender, e) => { if (IsDragging) { OnScrollAreaDragEndedFlickStarted(); } };
            }

            if (false)
            {
                if (false)
                {
                    ScrollAreaListView.ManipulationStarted += delegate { Debug.WriteLine("ManipulationStarted"); };
                    ScrollAreaListView.ManipulationDelta += delegate { Debug.WriteLine("ManipulationDelta"); };
                    ScrollAreaListView.ManipulationInertiaStarting += delegate { Debug.WriteLine("ManipulationInertiaStarting"); };
                }
                ScrollAreaListView.ManipulationStarted += (sender, e) => { OnScrollAreaDragStarted(e.ManipulationOrigin); };
                ScrollAreaListView.ManipulationDelta += (sender, e) => { OnScrollAreaDragging(e.ManipulationOrigin); };
                ScrollAreaListView.ManipulationInertiaStarting += (sender, e) => { if (IsDragging) { OnScrollAreaDragEndedFlickStarted(); } };
            }
            DragVelocityEstimationAndFlickScrollingDispatcherTimer.Tick += (sender, e) => { OnScrollAreaDispatcherTimerUpdateScrolling(); };
        }

        void OnScrollAreaDragStarted(Point newDraggingPoint)
        {
            IsDragging = true;
            DraggingDeltaTimeStopwatch.Restart();
            DraggingPoint = newDraggingPoint;
            refToViewModel.DraggingPoint = DraggingPoint;
            refToViewModel.DragStartedPoint = DraggingPoint;
            DraggingPointPreviousForScrollToHorizontalOffset = DraggingPoint;
            DraggingPointPreviousForCursorVelocityEstimation = DraggingPoint;
            DraggingCursorVelocity = new Vector();
            DragVelocityEstimationAndFlickScrollingDispatcherTimer.Start();
        }

        void OnScrollAreaDragging(Point newDraggingPoint)
        {
            if (IsDragging == false) { return; }
            DraggingPoint = newDraggingPoint;
            refToViewModel.DraggingPoint = DraggingPoint;
            var deltaPointForScrollToHorizontalOffset = DraggingPoint - DraggingPointPreviousForScrollToHorizontalOffset;
            ContentHorizontalOffsetDesired += deltaPointForScrollToHorizontalOffset.X;
            ScrollAreaScrollViewer.ScrollToHorizontalOffset(-ContentHorizontalOffsetDesired);
            DraggingPointPreviousForScrollToHorizontalOffset = DraggingPoint;
            UpdateDraggingVelocity();
        }

        void UpdateDraggingVelocity()
        {
            if (DraggingDeltaTimeStopwatch.Elapsed.TotalSeconds < TimerIntervalTotalSeconds) { return; }
            var deltaTime = DraggingDeltaTimeStopwatch.Elapsed.TotalSeconds;
            DraggingDeltaTimeStopwatch.Restart();
            var deltaPointForCursorVelocityEstimation = DraggingPoint - DraggingPointPreviousForCursorVelocityEstimation;
            var newCursorVelocity = deltaPointForCursorVelocityEstimation / deltaTime;
            DraggingCursorVelocity = DraggingCursorVelocityEmaAlphaInDeltaTime * DraggingCursorVelocity + (1.0 - DraggingCursorVelocityEmaAlphaInDeltaTime) * newCursorVelocity;
            DraggingPointPreviousForCursorVelocityEstimation = DraggingPoint;
            if (false)
            {
                Debug.Write("  deltaTime: " + deltaTime);
                Debug.Write("  deltaPointForCursorVelocityEstimation: " + deltaPointForCursorVelocityEstimation);
                Debug.Write("  newCursorVelocity.X: " + newCursorVelocity.X);
                Debug.Write("  DraggingCursorVelocity.X: " + DraggingCursorVelocity.X);
                Debug.WriteLine("");
            }
        }

        void OnScrollAreaDragEndedFlickStarted()
        {
            ContentHorizontalOffsetDesired = -ScrollAreaScrollViewer.ContentHorizontalOffset;
            FlickingCursorVelocityOnDragComplete = DraggingCursorVelocity;
            refToViewModel.FlickingCursorVelocityOnDragCompleteX = FlickingCursorVelocityOnDragComplete.X;

            FlickingCursorVelocity = FlickingCursorVelocityOnDragComplete;
            if (false)
            {
                Debug.Write("  FlickingCursorVelocityOnDragComplete.X: " + FlickingCursorVelocityOnDragComplete.X);
                Debug.WriteLine("");
            }
            IsDragging = false;
        }

        void OnScrollAreaDispatcherTimerUpdateScrolling()
        {
            if (IsDragging)
            {
                UpdateDraggingVelocity();
            }
            else
            {
                FlickingCursorVelocity = FlickingCursorVelocityEmaAlphaInDeltaTime * FlickingCursorVelocity;
                ContentHorizontalOffsetDesired += FlickingCursorVelocity.X * TimerIntervalTotalSeconds;
                ScrollAreaScrollViewer.ScrollToHorizontalOffset(-ContentHorizontalOffsetDesired);
                if (Math.Abs(FlickingCursorVelocity.X) < 1e-5) { DragVelocityEstimationAndFlickScrollingDispatcherTimer.Stop(); }
            }
        }
    }
}
