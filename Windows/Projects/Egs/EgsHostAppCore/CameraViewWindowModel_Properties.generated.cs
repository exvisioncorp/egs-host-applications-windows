namespace Egs
{
    using System;
    using System.Windows;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class CameraViewWindowModel
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _CanDragMove;
        public event EventHandler CanDragMoveChanged;
        protected virtual void OnCanDragMoveChanged(EventArgs e)
        {
            var t = CanDragMoveChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("CanDragMove");
        }
        [DataMember]
        public bool CanDragMove
        {
            get { return _CanDragMove; }
            set
            {
                _CanDragMove = value; OnCanDragMoveChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _CanResize;
        public event EventHandler CanResizeChanged;
        protected virtual void OnCanResizeChanged(EventArgs e)
        {
            var t = CanResizeChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("CanResize");
        }
        [DataMember]
        public bool CanResize
        {
            get { return _CanResize; }
            set
            {
                _CanResize = value; OnCanResizeChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _Topmost;
        public event EventHandler TopmostChanged;
        protected virtual void OnTopmostChanged(EventArgs e)
        {
            var t = TopmostChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("Topmost");
        }
        [DataMember]
        public bool Topmost
        {
            get { return _Topmost; }
            set
            {
                _Topmost = value; OnTopmostChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _Left;
        public event EventHandler LeftChanged;
        protected virtual void OnLeftChanged(EventArgs e)
        {
            var t = LeftChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("Left");
        }
        [DataMember]
        public int Left
        {
            get { return _Left; }
            set
            {
                _Left = value; OnLeftChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _Top;
        public event EventHandler TopChanged;
        protected virtual void OnTopChanged(EventArgs e)
        {
            var t = TopChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("Top");
        }
        [DataMember]
        public int Top
        {
            get { return _Top; }
            set
            {
                _Top = value; OnTopChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _Width;
        public event EventHandler WidthChanged;
        protected virtual void OnWidthChanged(EventArgs e)
        {
            var t = WidthChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("Width");
        }
        [DataMember]
        public int Width
        {
            get { return _Width; }
            set
            {
                _Width = value; OnWidthChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _Height;
        public event EventHandler HeightChanged;
        protected virtual void OnHeightChanged(EventArgs e)
        {
            var t = HeightChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("Height");
        }
        [DataMember]
        public int Height
        {
            get { return _Height; }
            set
            {
                _Height = value; OnHeightChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _CanShowMenu;
        public event EventHandler CanShowMenuChanged;
        protected virtual void OnCanShowMenuChanged(EventArgs e)
        {
            var t = CanShowMenuChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("CanShowMenu");
        }
        [DataMember]
        public bool CanShowMenu
        {
            get { return _CanShowMenu; }
            set
            {
                _CanShowMenu = value; OnCanShowMenuChanged(EventArgs.Empty);
            }
        }

    }
}

