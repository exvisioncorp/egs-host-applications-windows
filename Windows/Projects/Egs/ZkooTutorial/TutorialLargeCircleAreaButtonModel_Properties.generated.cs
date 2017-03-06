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

    partial class TutorialLargeCircleAreaButtonModel
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        int _Index;
        public event EventHandler IndexChanged;
        protected virtual void OnIndexChanged(EventArgs e)
        {
            var t = IndexChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(Index));
        }
        public int Index
        {
            get { return _Index; }
            set
            {
                if (value != _Index)
                {
                    _Index = value; OnIndexChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsThumbDragged;
        public event EventHandler IsThumbDraggedChanged;
        protected virtual void OnIsThumbDraggedChanged(EventArgs e)
        {
            var t = IsThumbDraggedChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsThumbDragged));
        }
        public bool IsThumbDragged
        {
            get { return _IsThumbDragged; }
            set
            {
                if (value != _IsThumbDragged)
                {
                    _IsThumbDragged = value; OnIsThumbDraggedChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _TapsCount;
        public event EventHandler TapsCountChanged;
        protected virtual void OnTapsCountChanged(EventArgs e)
        {
            var t = TapsCountChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(TapsCount));
        }
        public int TapsCount
        {
            get { return _TapsCount; }
            set
            {
                if (value != _TapsCount)
                {
                    _TapsCount = value; OnTapsCountChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _LongTapsCount;
        public event EventHandler LongTapsCountChanged;
        protected virtual void OnLongTapsCountChanged(EventArgs e)
        {
            var t = LongTapsCountChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(LongTapsCount));
        }
        public int LongTapsCount
        {
            get { return _LongTapsCount; }
            set
            {
                if (value != _LongTapsCount)
                {
                    _LongTapsCount = value; OnLongTapsCountChanged(EventArgs.Empty);
                }
            }
        }

    }
}

