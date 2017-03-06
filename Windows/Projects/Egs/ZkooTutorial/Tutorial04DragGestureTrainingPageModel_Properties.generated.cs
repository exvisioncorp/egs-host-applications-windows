namespace Egs.ZkooTutorial
{
    using System;
    using System.Windows;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    partial class Tutorial04DragGestureTrainingPageModel
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        Visibility _DraggingThumbVisibility;
        public event EventHandler DraggingThumbVisibilityChanged;
        protected virtual void OnDraggingThumbVisibilityChanged(EventArgs e)
        {
            var t = DraggingThumbVisibilityChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(DraggingThumbVisibility));
        }
        public Visibility DraggingThumbVisibility
        {
            get { return _DraggingThumbVisibility; }
            set
            {
                if (value != _DraggingThumbVisibility)
                {
                    _DraggingThumbVisibility = value; OnDraggingThumbVisibilityChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _DraggingThumbLeft;
        public event EventHandler DraggingThumbLeftChanged;
        protected virtual void OnDraggingThumbLeftChanged(EventArgs e)
        {
            var t = DraggingThumbLeftChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(DraggingThumbLeft));
        }
        public double DraggingThumbLeft
        {
            get { return _DraggingThumbLeft; }
            set
            {
                if (value != _DraggingThumbLeft)
                {
                    _DraggingThumbLeft = value; OnDraggingThumbLeftChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _DraggingThumbTop;
        public event EventHandler DraggingThumbTopChanged;
        protected virtual void OnDraggingThumbTopChanged(EventArgs e)
        {
            var t = DraggingThumbTopChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(DraggingThumbTop));
        }
        public double DraggingThumbTop
        {
            get { return _DraggingThumbTop; }
            set
            {
                if (value != _DraggingThumbTop)
                {
                    _DraggingThumbTop = value; OnDraggingThumbTopChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _DraggingThumbWidth;
        public event EventHandler DraggingThumbWidthChanged;
        protected virtual void OnDraggingThumbWidthChanged(EventArgs e)
        {
            var t = DraggingThumbWidthChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(DraggingThumbWidth));
        }
        public double DraggingThumbWidth
        {
            get { return _DraggingThumbWidth; }
            set
            {
                if (value != _DraggingThumbWidth)
                {
                    _DraggingThumbWidth = value; OnDraggingThumbWidthChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _DraggingThumbHeight;
        public event EventHandler DraggingThumbHeightChanged;
        protected virtual void OnDraggingThumbHeightChanged(EventArgs e)
        {
            var t = DraggingThumbHeightChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(DraggingThumbHeight));
        }
        public double DraggingThumbHeight
        {
            get { return _DraggingThumbHeight; }
            set
            {
                if (value != _DraggingThumbHeight)
                {
                    _DraggingThumbHeight = value; OnDraggingThumbHeightChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _DraggingThumbOpacity;
        public event EventHandler DraggingThumbOpacityChanged;
        protected virtual void OnDraggingThumbOpacityChanged(EventArgs e)
        {
            var t = DraggingThumbOpacityChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(DraggingThumbOpacity));
        }
        public double DraggingThumbOpacity
        {
            get { return _DraggingThumbOpacity; }
            set
            {
                if (value != _DraggingThumbOpacity)
                {
                    _DraggingThumbOpacity = value; OnDraggingThumbOpacityChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        Point _DraggingThumbCenterPoint;
        public event EventHandler DraggingThumbCenterPointChanged;
        protected virtual void OnDraggingThumbCenterPointChanged(EventArgs e)
        {
            var t = DraggingThumbCenterPointChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(DraggingThumbCenterPoint));
        }
        public Point DraggingThumbCenterPoint
        {
            get { return _DraggingThumbCenterPoint; }
            set
            {
                if (value != _DraggingThumbCenterPoint)
                {
                    _DraggingThumbCenterPoint = value; OnDraggingThumbCenterPointChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        Point[] _LargeCircleAreaCenterPoint;
        public event EventHandler LargeCircleAreaCenterPointChanged;
        protected virtual void OnLargeCircleAreaCenterPointChanged(EventArgs e)
        {
            var t = LargeCircleAreaCenterPointChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(LargeCircleAreaCenterPoint));
        }
        public Point[] LargeCircleAreaCenterPoint
        {
            get { return _LargeCircleAreaCenterPoint; }
            set
            {
                if (value != _LargeCircleAreaCenterPoint)
                {
                    _LargeCircleAreaCenterPoint = value; OnLargeCircleAreaCenterPointChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double[] _ThumbToLargeCircleAreaCenterDistanceList;
        public event EventHandler ThumbToLargeCircleAreaCenterDistanceListChanged;
        protected virtual void OnThumbToLargeCircleAreaCenterDistanceListChanged(EventArgs e)
        {
            var t = ThumbToLargeCircleAreaCenterDistanceListChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(ThumbToLargeCircleAreaCenterDistanceList));
        }
        public double[] ThumbToLargeCircleAreaCenterDistanceList
        {
            get { return _ThumbToLargeCircleAreaCenterDistanceList; }
            set
            {
                if (value != _ThumbToLargeCircleAreaCenterDistanceList)
                {
                    _ThumbToLargeCircleAreaCenterDistanceList = value; OnThumbToLargeCircleAreaCenterDistanceListChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsDraggingThumbDragging;
        public event EventHandler IsDraggingThumbDraggingChanged;
        protected virtual void OnIsDraggingThumbDraggingChanged(EventArgs e)
        {
            var t = IsDraggingThumbDraggingChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsDraggingThumbDragging));
        }
        public bool IsDraggingThumbDragging
        {
            get { return _IsDraggingThumbDragging; }
            set
            {
                if (value != _IsDraggingThumbDragging)
                {
                    _IsDraggingThumbDragging = value; OnIsDraggingThumbDraggingChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsDraggingThumbHovered;
        public event EventHandler IsDraggingThumbHoveredChanged;
        protected virtual void OnIsDraggingThumbHoveredChanged(EventArgs e)
        {
            var t = IsDraggingThumbHoveredChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsDraggingThumbHovered));
        }
        public bool IsDraggingThumbHovered
        {
            get { return _IsDraggingThumbHovered; }
            set
            {
                if (value != _IsDraggingThumbHovered)
                {
                    _IsDraggingThumbHovered = value; OnIsDraggingThumbHoveredChanged(EventArgs.Empty);
                }
            }
        }

    }
}

