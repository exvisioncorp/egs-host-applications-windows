namespace Egs
{
    using System;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class EgsDeviceFaceDetectionOnHost
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        double _CalibratedFocalLength;
        public event EventHandler CalibratedFocalLengthChanged;
        protected virtual void OnCalibratedFocalLengthChanged(EventArgs e)
        {
            var t = CalibratedFocalLengthChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CalibratedFocalLength));
        }
        public double CalibratedFocalLength
        {
            get { return _CalibratedFocalLength; }
            set
            {
                if (_CalibratedFocalLength != value)
                {
                    _CalibratedFocalLength = value; OnCalibratedFocalLengthChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _CaptureImageBinnedPixelOneSideLength;
        public event EventHandler CaptureImageBinnedPixelOneSideLengthChanged;
        protected virtual void OnCaptureImageBinnedPixelOneSideLengthChanged(EventArgs e)
        {
            var t = CaptureImageBinnedPixelOneSideLengthChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CaptureImageBinnedPixelOneSideLength));
        }
        public double CaptureImageBinnedPixelOneSideLength
        {
            get { return _CaptureImageBinnedPixelOneSideLength; }
            set
            {
                if (_CaptureImageBinnedPixelOneSideLength != value)
                {
                    _CaptureImageBinnedPixelOneSideLength = value; OnCaptureImageBinnedPixelOneSideLengthChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _CaptureImageWidth;
        public event EventHandler CaptureImageWidthChanged;
        protected virtual void OnCaptureImageWidthChanged(EventArgs e)
        {
            var t = CaptureImageWidthChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CaptureImageWidth));
        }
        public double CaptureImageWidth
        {
            get { return _CaptureImageWidth; }
            set
            {
                if (_CaptureImageWidth != value)
                {
                    _CaptureImageWidth = value; OnCaptureImageWidthChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _CaptureImageHeight;
        public event EventHandler CaptureImageHeightChanged;
        protected virtual void OnCaptureImageHeightChanged(EventArgs e)
        {
            var t = CaptureImageHeightChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CaptureImageHeight));
        }
        public double CaptureImageHeight
        {
            get { return _CaptureImageHeight; }
            set
            {
                if (_CaptureImageHeight != value)
                {
                    _CaptureImageHeight = value; OnCaptureImageHeightChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _CameraViewImageWidth;
        public event EventHandler CameraViewImageWidthChanged;
        protected virtual void OnCameraViewImageWidthChanged(EventArgs e)
        {
            var t = CameraViewImageWidthChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CameraViewImageWidth));
        }
        public double CameraViewImageWidth
        {
            get { return _CameraViewImageWidth; }
            set
            {
                if (_CameraViewImageWidth != value)
                {
                    _CameraViewImageWidth = value; OnCameraViewImageWidthChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _CameraViewImageHeight;
        public event EventHandler CameraViewImageHeightChanged;
        protected virtual void OnCameraViewImageHeightChanged(EventArgs e)
        {
            var t = CameraViewImageHeightChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CameraViewImageHeight));
        }
        public double CameraViewImageHeight
        {
            get { return _CameraViewImageHeight; }
            set
            {
                if (_CameraViewImageHeight != value)
                {
                    _CameraViewImageHeight = value; OnCameraViewImageHeightChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _CameraViewImageScale_DividedBy_CaptureImageScale;
        public event EventHandler CameraViewImageScale_DividedBy_CaptureImageScaleChanged;
        protected virtual void OnCameraViewImageScale_DividedBy_CaptureImageScaleChanged(EventArgs e)
        {
            var t = CameraViewImageScale_DividedBy_CaptureImageScaleChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CameraViewImageScale_DividedBy_CaptureImageScale));
        }
        public double CameraViewImageScale_DividedBy_CaptureImageScale
        {
            get { return _CameraViewImageScale_DividedBy_CaptureImageScale; }
            set
            {
                if (_CameraViewImageScale_DividedBy_CaptureImageScale != value)
                {
                    _CameraViewImageScale_DividedBy_CaptureImageScale = value; OnCameraViewImageScale_DividedBy_CaptureImageScaleChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _DetectorImageScale_DividedBy_CameraViewImageScale;
        public event EventHandler DetectorImageScale_DividedBy_CameraViewImageScaleChanged;
        protected virtual void OnDetectorImageScale_DividedBy_CameraViewImageScaleChanged(EventArgs e)
        {
            var t = DetectorImageScale_DividedBy_CameraViewImageScaleChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(DetectorImageScale_DividedBy_CameraViewImageScale));
        }
        public double DetectorImageScale_DividedBy_CameraViewImageScale
        {
            get { return _DetectorImageScale_DividedBy_CameraViewImageScale; }
            private set
            {
                if (_DetectorImageScale_DividedBy_CameraViewImageScale != value)
                {
                    _DetectorImageScale_DividedBy_CameraViewImageScale = value; OnDetectorImageScale_DividedBy_CameraViewImageScaleChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsDetecting;
        public event EventHandler IsDetectingChanged;
        protected virtual void OnIsDetectingChanged(EventArgs e)
        {
            var t = IsDetectingChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsDetecting));
        }
        public bool IsDetecting
        {
            get { return _IsDetecting; }
            private set
            {
                if (_IsDetecting != value)
                {
                    _IsDetecting = value; OnIsDetectingChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsToUpdateRealHandDetectionAreaFromBodyParameters;
        public event EventHandler IsToUpdateRealHandDetectionAreaFromBodyParametersChanged;
        protected virtual void OnIsToUpdateRealHandDetectionAreaFromBodyParametersChanged(EventArgs e)
        {
            var t = IsToUpdateRealHandDetectionAreaFromBodyParametersChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsToUpdateRealHandDetectionAreaFromBodyParameters));
        }
        [DataMember]
        public bool IsToUpdateRealHandDetectionAreaFromBodyParameters
        {
            get { return _IsToUpdateRealHandDetectionAreaFromBodyParameters; }
            set
            {
                if (_IsToUpdateRealHandDetectionAreaFromBodyParameters != value)
                {
                    _IsToUpdateRealHandDetectionAreaFromBodyParameters = value; OnIsToUpdateRealHandDetectionAreaFromBodyParametersChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        System.Drawing.Rectangle _CameraViewImageRightHandDetectionArea;
        public event EventHandler CameraViewImageRightHandDetectionAreaChanged;
        protected virtual void OnCameraViewImageRightHandDetectionAreaChanged(EventArgs e)
        {
            var t = CameraViewImageRightHandDetectionAreaChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CameraViewImageRightHandDetectionArea));
        }
        public System.Drawing.Rectangle CameraViewImageRightHandDetectionArea
        {
            get { return _CameraViewImageRightHandDetectionArea; }
            private set
            {
                if (_CameraViewImageRightHandDetectionArea != value)
                {
                    _CameraViewImageRightHandDetectionArea = value; OnCameraViewImageRightHandDetectionAreaChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        System.Drawing.Rectangle _CameraViewImageLeftHandDetectionArea;
        public event EventHandler CameraViewImageLeftHandDetectionAreaChanged;
        protected virtual void OnCameraViewImageLeftHandDetectionAreaChanged(EventArgs e)
        {
            var t = CameraViewImageLeftHandDetectionAreaChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CameraViewImageLeftHandDetectionArea));
        }
        public System.Drawing.Rectangle CameraViewImageLeftHandDetectionArea
        {
            get { return _CameraViewImageLeftHandDetectionArea; }
            private set
            {
                if (_CameraViewImageLeftHandDetectionArea != value)
                {
                    _CameraViewImageLeftHandDetectionArea = value; OnCameraViewImageLeftHandDetectionAreaChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        System.Drawing.Rectangle _CaptureImageRightHandDetectionArea;
        public event EventHandler CaptureImageRightHandDetectionAreaChanged;
        protected virtual void OnCaptureImageRightHandDetectionAreaChanged(EventArgs e)
        {
            var t = CaptureImageRightHandDetectionAreaChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CaptureImageRightHandDetectionArea));
        }
        public System.Drawing.Rectangle CaptureImageRightHandDetectionArea
        {
            get { return _CaptureImageRightHandDetectionArea; }
            private set
            {
                if (_CaptureImageRightHandDetectionArea != value)
                {
                    _CaptureImageRightHandDetectionArea = value; OnCaptureImageRightHandDetectionAreaChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        System.Drawing.Rectangle _CaptureImageLeftHandDetectionArea;
        public event EventHandler CaptureImageLeftHandDetectionAreaChanged;
        protected virtual void OnCaptureImageLeftHandDetectionAreaChanged(EventArgs e)
        {
            var t = CaptureImageLeftHandDetectionAreaChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CaptureImageLeftHandDetectionArea));
        }
        public System.Drawing.Rectangle CaptureImageLeftHandDetectionArea
        {
            get { return _CaptureImageLeftHandDetectionArea; }
            private set
            {
                if (_CaptureImageLeftHandDetectionArea != value)
                {
                    _CaptureImageLeftHandDetectionArea = value; OnCaptureImageLeftHandDetectionAreaChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        double _CaptureImagePalmImageWidth;
        public event EventHandler CaptureImagePalmImageWidthChanged;
        protected virtual void OnCaptureImagePalmImageWidthChanged(EventArgs e)
        {
            var t = CaptureImagePalmImageWidthChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CaptureImagePalmImageWidth));
        }
        public double CaptureImagePalmImageWidth
        {
            get { return _CaptureImagePalmImageWidth; }
            private set
            {
                if (_CaptureImagePalmImageWidth != value)
                {
                    _CaptureImagePalmImageWidth = value; OnCaptureImagePalmImageWidthChanged(EventArgs.Empty);
                }
            }
        }

    }
}

