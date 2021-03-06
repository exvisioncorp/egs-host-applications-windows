﻿namespace Egs
{
    using System;
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
            OnPropertyChanged(nameof(RightOrLeft));
        }
        [DataMember]
        public RightOrLeftKind RightOrLeft
        {
            get { return _RightOrLeft; }
            set
            {
                if (_RightOrLeft != value)
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
            OnPropertyChanged(nameof(IsToShowCursor));
        }
        [DataMember]
        public bool IsToShowCursor
        {
            get { return _IsToShowCursor; }
            set
            {
                if (_IsToShowCursor != value)
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
            OnPropertyChanged(nameof(IsToDetectLongTouch));
        }
        [DataMember]
        public bool IsToDetectLongTouch
        {
            get { return _IsToDetectLongTouch; }
            set
            {
                if (_IsToDetectLongTouch != value)
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
            OnPropertyChanged(nameof(IsToUpdateVelocities));
        }
        [DataMember]
        public bool IsToUpdateVelocities
        {
            get { return _IsToUpdateVelocities; }
            set
            {
                if (_IsToUpdateVelocities != value)
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
            OnPropertyChanged(nameof(LongTapElapsedThresholdInMilliseconds));
        }
        [DataMember]
        public double LongTapElapsedThresholdInMilliseconds
        {
            get { return _LongTapElapsedThresholdInMilliseconds; }
            set
            {
                if (_LongTapElapsedThresholdInMilliseconds != value)
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
            OnPropertyChanged(nameof(CurrentCursorImageSetIndex));
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

