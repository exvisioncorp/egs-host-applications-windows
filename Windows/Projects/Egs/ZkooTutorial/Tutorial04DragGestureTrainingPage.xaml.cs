namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Diagnostics;

    partial class Tutorial04DragGestureTrainingPage : Page, IHasTutorialEachPageModelBase
    {
        public TutorialEachPageModelBase ReferenceToTutorialEachPageModelBase { get; set; }

        public Tutorial04DragGestureTrainingPage()
        {
            InitializeComponent();
        }

        public void InitializeOnceAtStartup(MainNavigationWindow navigator, Tutorial04DragGestureTrainingPageModel viewModel)
        {
            Trace.Assert(navigator != null);
            Trace.Assert(viewModel != null);


            // ---------- videos and state transition ----------
            PracticeSlideShow01VideoUserControl.SetMediaElementSourceUriByFilePath(System.IO.Path.Combine(viewModel.refToAppModel.CurrentResources.TutorialVideoFilesFolderPath, "tutorial_reference_video_4.mp4"), UriKind.Relative);
            PracticeSlideShow01VideoUserControl.IsVisibleChanged += (sender, e) =>
            {
                if (PracticeSlideShow01VideoUserControl.IsVisible) { PracticeSlideShow01VideoUserControl.Replay(); } else { PracticeSlideShow01VideoUserControl.Pause(); }
            };
            // ---------- videos and state transition ----------


            this.DataContext = viewModel;
            ReferenceToTutorialEachPageModelBase = viewModel;
            ReplayPracticeNextButtonsUserControl.InitializeOnceAtStartup(viewModel.TutorialAppHeaderMenu);
            TutorialAppHeaderMenuUserControl.InitializeOnceAtStartup(viewModel.TutorialAppHeaderMenu);

            LargeCircleAreaRightTop.InitializeOnceAtStartup(viewModel.TutorialLargeCircleAreaButtonRightTop);
            LargeCircleAreaRightBottom.InitializeOnceAtStartup(viewModel.TutorialLargeCircleAreaButtonRightBottom);
            LargeCircleAreaLeftBottom.InitializeOnceAtStartup(viewModel.TutorialLargeCircleAreaButtonLeftBottom);
            LargeCircleAreaLeftTop.InitializeOnceAtStartup(viewModel.TutorialLargeCircleAreaButtonLeftTop);

            var corners = new FrameworkElement[] { LargeCircleAreaRightTop, LargeCircleAreaRightBottom, LargeCircleAreaLeftBottom, LargeCircleAreaLeftTop };
            var corner_Index_Dict = new Dictionary<object, int>();
            for (int i = 0; i < corners.Length; i++)
            {
                var corner = corners[i];
                corner_Index_Dict[corner] = i;
            }

            this.Loaded += (sender, e) =>
            {
                navigator.SetWindowFullScreen();
                navigator.Title = "ZKOO Tutorial: Drag Gesture Practice";
                viewModel.OnLoaded();
                PracticeSlideShow01VideoUserControl.Replay();
            };
            this.Unloaded += (sender, e) =>
            {
                viewModel.IsCancelling = true;
            };

            DraggingThumb.MouseEnter += (sender, e) => { viewModel.IsDraggingThumbHovered = true; };
            DraggingThumb.MouseLeave += (sender, e) => { viewModel.IsDraggingThumbHovered = false; };
            DraggingThumb.DragStarted += (sender, e) => { viewModel.IsDraggingThumbDragging = true; };
            DraggingThumb.DragCompleted += (sender, e) =>
            {
                var thumbRadius = DraggingThumb.ActualWidth / 2;
                var thumbCenterRelativePoint = new Point(thumbRadius, thumbRadius);
                viewModel.DraggingThumbCenterPoint = DraggingThumb.PointToScreen(thumbCenterRelativePoint);
                foreach (var corner in corners)
                {
                    var index = corner_Index_Dict[corner];
                    var cornerRadius = corner.ActualWidth / 2;
                    var cornerCenterRelativePoint = new Point(cornerRadius, cornerRadius);
                    viewModel.LargeCircleAreaCenterPoint[index] = corner.PointToScreen(cornerCenterRelativePoint);

                    var diffVector = (viewModel.LargeCircleAreaCenterPoint[index] - viewModel.DraggingThumbCenterPoint);
                    var distance = diffVector.Length;
                    viewModel.ThumbToLargeCircleAreaCenterDistanceList[index] = distance;
                    viewModel.TutorialLargeCircleAreaButtonList[index].IsThumbDragged = (distance + thumbRadius < cornerRadius);
                }
                viewModel.IsDraggingThumbDragging = false;
            };
        }
    }
}
