namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.Views;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    /// <summary>
    /// ViewModel which has information about one person and his two hands
    /// </summary>
    [DataContract]
    public partial class OnePersonBothHandsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _PersonId;
        internal event EventHandler PersonIdChanged;
        protected virtual void OnPersonIdChanged(EventArgs e) { var t = PersonIdChanged; if (t != null) { t(this, e); } }
        [DataMember]
        internal int PersonId
        {
            get { return _PersonId; }
            set
            {
                _PersonId = value;
                OnPersonIdChanged(EventArgs.Empty);
                OnPropertyChanged(nameof(PersonId));
            }
        }

        [DataMember]
        public OptionalValue<CursorImageSetInformation> CursorImageSetInformationOptionalValue { get; internal set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        CursorViewModel _FirstFoundHand;
        public CursorViewModel FirstFoundHand
        {
            get { return _FirstFoundHand; }
            private set { _FirstFoundHand = value; OnPropertyChanged(nameof(FirstFoundHand)); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        CursorViewModel _LeftHand;
        public CursorViewModel LeftHand
        {
            get { return _LeftHand; }
            private set { _LeftHand = value; OnPropertyChanged(nameof(LeftHand)); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        CursorViewModel _RightHand;
        public CursorViewModel RightHand
        {
            get { return _RightHand; }
            private set { _RightHand = value; OnPropertyChanged(nameof(RightHand)); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _LongTapElapsedThresholdInMilliseconds;
        [DataMember]
        public double LongTapElapsedThresholdInMilliseconds
        {
            get { return _LongTapElapsedThresholdInMilliseconds; }
            set
            {
                _LongTapElapsedThresholdInMilliseconds = value;
                _RightHand.LongTapElapsedThresholdInMilliseconds = value;
                _LeftHand.LongTapElapsedThresholdInMilliseconds = value;
            }
        }
        public IList<CursorViewModel> Hands { get; private set; }

        public OnePersonBothHandsViewModel()
        {
            RightHand = new CursorViewModel() { RightOrLeft = RightOrLeftKind.Right };
            LeftHand = new CursorViewModel() { RightOrLeft = RightOrLeftKind.Left };
            Hands = new CursorViewModel[2];
            Hands[(int)RightOrLeftKind.Right] = RightHand;
            Hands[(int)RightOrLeftKind.Left] = LeftHand;
            LongTapElapsedThresholdInMilliseconds = 800.0;

            var defaultCursorImageInformationSetList = ImageInformationSet.CreateDefaultImageInformationSetList(ImageInformationSet.CursorImageSetListFolderRelativePath);
            CursorImageSetInformationOptionalValue = new OptionalValue<CursorImageSetInformation>();
            var defaultOptions = defaultCursorImageInformationSetList.Select(e => new CursorImageSetInformation()
            {
                Index = e.ImageSetIndex,
                Description = e.Description,
                SampleImageSource = DotNetUtility.BitmapImageUtility.LoadBitmapImageFromFile(e.SampleImageFileRelativePath)
            }).ToList();
            CursorImageSetInformationOptionalValue.Options = new List<CursorImageSetInformation>(defaultOptions);
        }

        public void InitializeOnceAtStartup(EgsDevice device)
        {
            RightHand.InitializeOnceAtStartup(device);
            LeftHand.InitializeOnceAtStartup(device);
            CursorImageSetInformationOptionalValue.SelectedItemChanged += (sender, e) =>
            {
                RightHand.CurrentCursorImageSetIndex = CursorImageSetInformationOptionalValue.SelectedItem.Index;
                LeftHand.CurrentCursorImageSetIndex = CursorImageSetInformationOptionalValue.SelectedItem.Index;
            };
            device.EgsGestureHidReport.RecognitionStateChanged += (sender, e) =>
            {
                if (FirstFoundHand == null)
                {
                    if (device.EgsGestureHidReport.Hands[(int)RightOrLeftKind.Right].IsTracking) { FirstFoundHand = _RightHand; }
                    else if (device.EgsGestureHidReport.Hands[(int)RightOrLeftKind.Left].IsTracking) { FirstFoundHand = _LeftHand; }
                }
                else if (FirstFoundHand.IsTracking == false) { FirstFoundHand = null; }
            };
        }
    }
}
