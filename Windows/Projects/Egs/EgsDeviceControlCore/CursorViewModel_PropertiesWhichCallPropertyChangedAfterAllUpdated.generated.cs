namespace Egs
{
    using System;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class CursorViewModel
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsTracking;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool hasToCallPropertyChangedOfIsTracking;
        [DataMember]
        public bool IsTracking
        {
            get { return _IsTracking; }
            internal set { if (value != _IsTracking) { _IsTracking = value; hasToCallPropertyChangedOfIsTracking = true; } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsVisible;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool hasToCallPropertyChangedOfIsVisible;
        [DataMember]
        public bool IsVisible
        {
            get { return _IsVisible; }
            internal set { if (value != _IsVisible) { _IsVisible = value; hasToCallPropertyChangedOfIsVisible = true; } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _PositionX;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool hasToCallPropertyChangedOfPositionX;
        [DataMember]
        public double PositionX
        {
            get { return _PositionX; }
            internal set { if (value != _PositionX) { _PositionX = value; hasToCallPropertyChangedOfPositionX = true; } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _PositionY;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool hasToCallPropertyChangedOfPositionY;
        [DataMember]
        public double PositionY
        {
            get { return _PositionY; }
            internal set { if (value != _PositionY) { _PositionY = value; hasToCallPropertyChangedOfPositionY = true; } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _Rotation;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool hasToCallPropertyChangedOfRotation;
        [DataMember]
        public double Rotation
        {
            get { return _Rotation; }
            internal set { if (value != _Rotation) { _Rotation = value; hasToCallPropertyChangedOfRotation = true; } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _RelativeZ;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool hasToCallPropertyChangedOfRelativeZ;
        [DataMember]
        public double RelativeZ
        {
            get { return _RelativeZ; }
            internal set { if (value != _RelativeZ) { _RelativeZ = value; hasToCallPropertyChangedOfRelativeZ = true; } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _FingerPitch;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool hasToCallPropertyChangedOfFingerPitch;
        [DataMember]
        public double FingerPitch
        {
            get { return _FingerPitch; }
            internal set { if (value != _FingerPitch) { _FingerPitch = value; hasToCallPropertyChangedOfFingerPitch = true; } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsTouching;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool hasToCallPropertyChangedOfIsTouching;
        [DataMember]
        public bool IsTouching
        {
            get { return _IsTouching; }
            internal set { if (value != _IsTouching) { _IsTouching = value; hasToCallPropertyChangedOfIsTouching = true; } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsLongTouching;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool hasToCallPropertyChangedOfIsLongTouching;
        [DataMember]
        public bool IsLongTouching
        {
            get { return _IsLongTouching; }
            internal set { if (value != _IsLongTouching) { _IsLongTouching = value; hasToCallPropertyChangedOfIsLongTouching = true; } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        CursorTapKind _LastTapKind;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool hasToCallPropertyChangedOfLastTapKind;
        [DataMember]
        public CursorTapKind LastTapKind
        {
            get { return _LastTapKind; }
            internal set { if (value != _LastTapKind) { _LastTapKind = value; hasToCallPropertyChangedOfLastTapKind = true; } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _CurrentImageIndex;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool hasToCallPropertyChangedOfCurrentImageIndex;
        [DataMember]
        public int CurrentImageIndex
        {
            get { return _CurrentImageIndex; }
            set { if (value != _CurrentImageIndex) { _CurrentImageIndex = value; hasToCallPropertyChangedOfCurrentImageIndex = true; } }
        }


        internal void CallPropertyChangedOfOnlyUpdatedPropertiesAtOnce()
        {
            if (hasToCallPropertyChangedOfIsTracking) { OnPropertyChanged(nameof(IsTracking)); }
            if (hasToCallPropertyChangedOfIsVisible) { OnPropertyChanged(nameof(IsVisible)); }
            if (hasToCallPropertyChangedOfPositionX) { OnPropertyChanged(nameof(PositionX)); }
            if (hasToCallPropertyChangedOfPositionY) { OnPropertyChanged(nameof(PositionY)); }
            if (hasToCallPropertyChangedOfRotation) { OnPropertyChanged(nameof(Rotation)); }
            if (hasToCallPropertyChangedOfRelativeZ) { OnPropertyChanged(nameof(RelativeZ)); }
            if (hasToCallPropertyChangedOfFingerPitch) { OnPropertyChanged(nameof(FingerPitch)); }
            if (hasToCallPropertyChangedOfIsTouching) { OnPropertyChanged(nameof(IsTouching)); }
            if (hasToCallPropertyChangedOfIsLongTouching) { OnPropertyChanged(nameof(IsLongTouching)); }
            if (hasToCallPropertyChangedOfLastTapKind) { OnPropertyChanged(nameof(LastTapKind)); }
            if (hasToCallPropertyChangedOfCurrentImageIndex) { OnPropertyChanged(nameof(CurrentImageIndex)); }
        }

        internal void CallPropertyChangedOfAllPropertiesAtOnce()
        {
            OnPropertyChanged(nameof(IsTracking));
            OnPropertyChanged(nameof(IsVisible));
            OnPropertyChanged(nameof(PositionX));
            OnPropertyChanged(nameof(PositionY));
            OnPropertyChanged(nameof(Rotation));
            OnPropertyChanged(nameof(RelativeZ));
            OnPropertyChanged(nameof(FingerPitch));
            OnPropertyChanged(nameof(IsTouching));
            OnPropertyChanged(nameof(IsLongTouching));
            OnPropertyChanged(nameof(LastTapKind));
            OnPropertyChanged(nameof(CurrentImageIndex));
            SetFalseToAllHasToCallPeropertyChangedFields();
        }

        internal void SetFalseToAllHasToCallPeropertyChangedFields()
        {
            hasToCallPropertyChangedOfIsTracking = false;
            hasToCallPropertyChangedOfIsVisible = false;
            hasToCallPropertyChangedOfPositionX = false;
            hasToCallPropertyChangedOfPositionY = false;
            hasToCallPropertyChangedOfRotation = false;
            hasToCallPropertyChangedOfRelativeZ = false;
            hasToCallPropertyChangedOfFingerPitch = false;
            hasToCallPropertyChangedOfIsTouching = false;
            hasToCallPropertyChangedOfIsLongTouching = false;
            hasToCallPropertyChangedOfLastTapKind = false;
            hasToCallPropertyChangedOfCurrentImageIndex = false;
        }
    }
}

