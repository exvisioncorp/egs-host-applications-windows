namespace Egs
{
    using System;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class CursorViewModel
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        RightOrLeftKind _RightOrLeft;
        public event EventHandler RightOrLeftChanged;
        protected virtual void OnRightOrLeftChanged(EventArgs e)
        {
            var t = RightOrLeftChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("RightOrLeft");
        }
        [DataMember]
        public RightOrLeftKind RightOrLeft
        {
            get { return _RightOrLeft; }
            set
            {
                if (value != _RightOrLeft)
                {
                    _RightOrLeft = value; OnRightOrLeftChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsToShowCursor;
        public event EventHandler IsToShowCursorChanged;
        protected virtual void OnIsToShowCursorChanged(EventArgs e)
        {
            var t = IsToShowCursorChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("IsToShowCursor");
        }
        [DataMember]
        public bool IsToShowCursor
        {
            get { return _IsToShowCursor; }
            set
            {
                if (value != _IsToShowCursor)
                {
                    _IsToShowCursor = value; OnIsToShowCursorChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsToDetectLongTouch;
        public event EventHandler IsToDetectLongTouchChanged;
        protected virtual void OnIsToDetectLongTouchChanged(EventArgs e)
        {
            var t = IsToDetectLongTouchChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("IsToDetectLongTouch");
        }
        [DataMember]
        public bool IsToDetectLongTouch
        {
            get { return _IsToDetectLongTouch; }
            set
            {
                if (value != _IsToDetectLongTouch)
                {
                    _IsToDetectLongTouch = value; OnIsToDetectLongTouchChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsToUpdateVelocities;
        public event EventHandler IsToUpdateVelocitiesChanged;
        protected virtual void OnIsToUpdateVelocitiesChanged(EventArgs e)
        {
            var t = IsToUpdateVelocitiesChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("IsToUpdateVelocities");
        }
        [DataMember]
        public bool IsToUpdateVelocities
        {
            get { return _IsToUpdateVelocities; }
            set
            {
                if (value != _IsToUpdateVelocities)
                {
                    _IsToUpdateVelocities = value; OnIsToUpdateVelocitiesChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _LongTapElapsedThresholdInMilliseconds;
        public event EventHandler LongTapElapsedThresholdInMillisecondsChanged;
        protected virtual void OnLongTapElapsedThresholdInMillisecondsChanged(EventArgs e)
        {
            var t = LongTapElapsedThresholdInMillisecondsChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("LongTapElapsedThresholdInMilliseconds");
        }
        [DataMember]
        public double LongTapElapsedThresholdInMilliseconds
        {
            get { return _LongTapElapsedThresholdInMilliseconds; }
            set
            {
                if (value != _LongTapElapsedThresholdInMilliseconds)
                {
                    _LongTapElapsedThresholdInMilliseconds = value; OnLongTapElapsedThresholdInMillisecondsChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _CurrentCursorImageSetIndex;
        public event EventHandler CurrentCursorImageSetIndexChanged;
        protected virtual void OnCurrentCursorImageSetIndexChanged(EventArgs e)
        {
            var t = CurrentCursorImageSetIndexChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("CurrentCursorImageSetIndex");
        }
        public int CurrentCursorImageSetIndex
        {
            get { return _CurrentCursorImageSetIndex; }
            set
            {
                _CurrentCursorImageSetIndex = value; OnCurrentCursorImageSetIndexChanged(EventArgs.Empty);
            }
        }

    }
}

