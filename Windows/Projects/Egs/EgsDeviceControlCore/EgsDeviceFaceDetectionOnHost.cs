namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using DotNetUtility;

    [DataContract]
    public partial class EgsDeviceFaceDetectionOnHost : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        // NOTE: CalibratedFocalLength is the focal length near its optical axis [mm].

        // NOTE: EGS devices return various information on the coordinate of their capture image.
        //       Camera view image size is different from capture image (in device) size.

        public void SetCameraViewImageScale_DividedBy_CaptureImageScale_ToCameraViewImageHeight_DividedBy_CaptureImageheight()
        {
            Trace.Assert(CaptureImageHeight > 0);
            CameraViewImageScale_DividedBy_CaptureImageScale = CameraViewImageHeight / CaptureImageHeight;
        }

        public void Update_DetectorImageScale_DividedBy_CameraViewImageScale()
        {
            // (CaptureImageBinnedPixelSize * CaptureImageFaceWidth) : CalibratedFocalLength == RealFaceBreadth : RealDetectableFaceZMaximum
            // CaptureImageFaceWidth = (CalibratedFocalLength * RealFaceBreadth) / (CaptureImageBinnedPixelSize * RealDetectableFaceZMaximum);

            // (DetectorImageBinnedPixelSize * DetectorImageDetectableFaceWidthMinimum) == (CaptureImageBinnedPixelSize * CaptureImageFaceWidth)
            // DetectorImageBinnedPixelSize == (CaptureImageBinnedPixelSize / CameraViewImageScale_DividedBy_CaptureImageScale) / DetectorImageScale_DividedBy_CameraViewImageScale

            // ((CaptureImageBinnedPixelSize / CameraViewImageScale_DividedBy_CaptureImageScale) / DetectorImageScale_DividedBy_CameraViewImageScale)
            //                                        * DetectorImageDetectableFaceWidthMinimum  == (CaptureImageBinnedPixelSize * ((CalibratedFocalLength * RealFaceBreadth) / (CaptureImageBinnedPixelSize * RealDetectableFaceZMaximum)))
            // ((CaptureImageBinnedPixelSize / CameraViewImageScale_DividedBy_CaptureImageScale) / DetectorImageScale_DividedBy_CameraViewImageScale) * DetectorImageDetectableFaceWidthMinimum
            //                                                                                   == CalibratedFocalLength * RealFaceBreadth / RealDetectableFaceZMaximum
            // ((CaptureImageBinnedPixelSize / CameraViewImageScale_DividedBy_CaptureImageScale) * RealDetectableFaceZMaximum                                 ) * DetectorImageDetectableFaceWidthMinimum
            //                                                                                   == CalibratedFocalLength * RealFaceBreadth * DetectorImageScale_DividedBy_CameraViewImageScale

            // DetectorImageScale_DividedBy_CameraViewImageScale = ((CaptureImageBinnedPixelSize / CameraViewImageScale_DividedBy_CaptureImageScale) * RealDetectableFaceZMaximum) * DetectorImageDetectableFaceWidthMinimum / (CalibratedFocalLength * RealFaceBreadth);
            DetectorImageScale_DividedBy_CameraViewImageScale = (CaptureImageBinnedPixelSize * RealDetectableFaceZMaximum * DetectorImageDetectableFaceWidthMinimum) / (CameraViewImageScale_DividedBy_CaptureImageScale * CalibratedFocalLength * RealFaceBreadth);
        }

        [DataMember]
        public RangedDouble MaxDetectableDistanceInMeter { get; private set; }
        public double RealDetectableFaceZMaximum { get { return MaxDetectableDistanceInMeter * 1000.0; } }

        [DataMember]
        public RangedDouble RealFaceBreadth { get; private set; }
        [DataMember]
        public RangedDouble RealPalmBreadth { get; private set; }

        [DataMember]
        public RangedDouble RealShoulderBreadth { get; private set; }
        [DataMember]
        public RangedDouble RealHandDetectionAreaCenterXOffset_DividedBy_RealShoulderBreadth { get; private set; }
        [DataMember]
        public RangedDouble RealHandDetectionAreaCenterYOffset_DividedBy_RealFaceBreadth { get; private set; }
        [DataMember]
        public RangedDouble RealHandDetectionAreaCenterZOffset_DividedBy_RealShoulderBreadth { get; private set; }
        [DataMember]
        public RangedDouble RealHandDetectionAreaWidth_DividedBy_RealPalmBreadth { get; private set; }
        [DataMember]
        public RangedDouble RealHandDetectionAreaHeight_DividedBy_RealPalmBreadth { get; private set; }

        [DataMember]
        public RangedDouble RealHandDetectionAreaCenterXOffset { get; private set; }
        [DataMember]
        public RangedDouble RealHandDetectionAreaCenterYOffset { get; private set; }
        [DataMember]
        public RangedDouble RealHandDetectionAreaCenterZOffset { get; private set; }
        [DataMember]
        public RangedDouble RealHandDetectionAreaWidth { get; private set; }
        [DataMember]
        public RangedDouble RealHandDetectionAreaHeight { get; private set; }

        /// <summary>
        /// -3 => High Sensitivity.  +3 => High Specificity
        /// </summary>
        [DataMember]
        public RangedInt SensitivityAndSpecificity { get; private set; }
        public double DlibHogSvmThreshold { get { return (SensitivityAndSpecificity - 4.0) / 10.0; } }

        /// <summary>
        /// Decided by detector's specification.  In some case, dectector can find smaller faces by enlarging input image.
        /// </summary>
        [DataMember]
        public int DetectorImageDetectableFaceWidthMinimum { get; set; }

        [DataMember]
        public RangedInt DetectFaceIntervalMillisecondsMinimum { get; private set; }
        Stopwatch DetectFaceIntervalMinimumStopwatch { get; set; }
        System.ComponentModel.BackgroundWorker Worker { get; set; }

        Stopwatch DetectionElapsedStopwatch { get; set; }
        DlibSharp.Array2dUchar DlibArray2dUcharImage { get; set; }
        DlibSharp.FrontalFaceDetector DlibHogSvm { get; set; }
        public IList<System.Drawing.Rectangle> DetectedFaceRectsInCameraViewImage { get; private set; }
        public bool IsFaceDetected
        {
            get { return (DetectedFaceRectsInCameraViewImage != null) && (DetectedFaceRectsInCameraViewImage.Count > 0); }
        }
        public Nullable<System.Drawing.Rectangle> SelectedFaceRect { get; private set; }

        /// <summary>
        /// Decided by device capability.
        /// </summary>
        public int HandDetectionScaleForEgsDeviceMaximum { get; internal set; }
        public double HandDetectionScaleForEgsDevice_DividedBy_CaptureImagePalmImageWidth { get; private set; }
        public double CaptureImagePalmImageWidthMaximum { get { return HandDetectionScaleForEgsDeviceMaximum / HandDetectionScaleForEgsDevice_DividedBy_CaptureImagePalmImageWidth; } }
        public double RealDetectablePalmZMaximum
        {
            get
            {
                // (CaptureImagePalmImageWidthMaximum * CaptureImageBinnedPixelSize) : CalibratedFocalLength == RealPalmBreadth : RealDetectablePalmZMaximum
                var ret = (CalibratedFocalLength * RealPalmBreadth) / (CaptureImagePalmImageWidthMaximum * CaptureImageBinnedPixelSize);
                return ret;
            }
        }

        internal EgsDevice Device { get; private set; }

        public event EventHandler FaceDetectionCompleted;
        protected virtual void OnFaceDetectionCompleted(EventArgs e)
        {
            var t = FaceDetectionCompleted; if (t != null) { t(this, e); }
        }



        public EgsDeviceFaceDetectionOnHost()
        {
            // +X:Right  +Y:Bottom  +Z:Back (camera to user)

            // Parameters input by user
            // 3[m].  10 feet UI
            MaxDetectableDistanceInMeter = new RangedDouble(3.0, 1.0, 5.0, 0.1, 0.5, 0.1);
            // (140,200) Avg: M:162 F:156
            RealFaceBreadth = new RangedDouble(159, 140, 200, 1, 5, 1);
            // ( 65, 95) Avg: M: 82 F: 74
            RealPalmBreadth = new RangedDouble(78, 65, 95, 1, 3, 1);

            IsToUpdateRealHandDetectionAreaFromBodyParameters = true;
            // (310,440) Avg: M:397 F:361
            RealShoulderBreadth = new RangedDouble(379, 310, 440, 1, 10, 1);
            RealHandDetectionAreaCenterXOffset_DividedBy_RealShoulderBreadth = new RangedDouble(0.68, 0, 1, 0.01, 0.1, 0.01);
            RealHandDetectionAreaCenterYOffset_DividedBy_RealFaceBreadth = new RangedDouble(0.63, 0, 2, 0.01, 0.1, 0.01);
            RealHandDetectionAreaCenterZOffset_DividedBy_RealShoulderBreadth = new RangedDouble(-0.26, -1, 0, 0.01, 0.1, 0.01);
            RealHandDetectionAreaWidth_DividedBy_RealPalmBreadth = new RangedDouble(4.5, 3, 6, 0.1, 1.0, 0.1);
            RealHandDetectionAreaHeight_DividedBy_RealPalmBreadth = new RangedDouble(5.1, 3, 6, 0.1, 1.0, 0.1);

            RealHandDetectionAreaCenterXOffset = new RangedDouble(RealHandDetectionAreaCenterXOffset_DividedBy_RealShoulderBreadth * RealShoulderBreadth, 0, 500, 10, 50, 10);
            RealHandDetectionAreaCenterYOffset = new RangedDouble(RealHandDetectionAreaCenterYOffset_DividedBy_RealFaceBreadth * RealFaceBreadth, -100, 200, 10, 50, 10);
            RealHandDetectionAreaCenterZOffset = new RangedDouble(RealHandDetectionAreaCenterZOffset_DividedBy_RealShoulderBreadth * RealShoulderBreadth, -300, 0, 10, 50, 10);
            RealHandDetectionAreaWidth = new RangedDouble(RealHandDetectionAreaWidth_DividedBy_RealPalmBreadth * RealPalmBreadth, 100, 500, 10, 50, 10);
            RealHandDetectionAreaHeight = new RangedDouble(RealHandDetectionAreaHeight_DividedBy_RealPalmBreadth * RealPalmBreadth, 100, 500, 10, 50, 10);

            SensitivityAndSpecificity = new RangedInt(0, -3, 3, 1, 1, 1);

            DetectFaceIntervalMillisecondsMinimum = new RangedInt(200, 0, 2000, 10, 100, 10);
            DetectFaceIntervalMinimumStopwatch = Stopwatch.StartNew();
            Worker = new System.ComponentModel.BackgroundWorker();
            Worker.DoWork += delegate
            {
                DetectFaces();
            };
            Worker.RunWorkerCompleted += delegate
            {
                DetectFaces_RunWorkerCompleted();
            };
            IsDetecting = false;

            DlibArray2dUcharImage = new DlibSharp.Array2dUchar();
            DlibHogSvm = new DlibSharp.FrontalFaceDetector();
            DetectionElapsedStopwatch = new Stopwatch();

            IsToUpdateRealHandDetectionAreaFromBodyParametersChanged += delegate { UpdateRealHandDetectionAreaParametersFromRealBodyParameters(); };
            RealFaceBreadth.ValueChanged += delegate { UpdateRealHandDetectionAreaParametersFromRealBodyParameters(); };
            RealPalmBreadth.ValueChanged += delegate { UpdateRealHandDetectionAreaParametersFromRealBodyParameters(); };
            RealShoulderBreadth.ValueChanged += delegate { UpdateRealHandDetectionAreaParametersFromRealBodyParameters(); };
            RealHandDetectionAreaCenterXOffset_DividedBy_RealShoulderBreadth.ValueChanged += delegate { UpdateRealHandDetectionAreaParametersFromRealBodyParameters(); };
            RealHandDetectionAreaCenterYOffset_DividedBy_RealFaceBreadth.ValueChanged += delegate { UpdateRealHandDetectionAreaParametersFromRealBodyParameters(); };
            RealHandDetectionAreaCenterZOffset_DividedBy_RealShoulderBreadth.ValueChanged += delegate { UpdateRealHandDetectionAreaParametersFromRealBodyParameters(); };
            RealHandDetectionAreaWidth_DividedBy_RealPalmBreadth.ValueChanged += delegate { UpdateRealHandDetectionAreaParametersFromRealBodyParameters(); };
            RealHandDetectionAreaHeight_DividedBy_RealPalmBreadth.ValueChanged += delegate { UpdateRealHandDetectionAreaParametersFromRealBodyParameters(); };

            Reset();
        }

        public void UpdateRealHandDetectionAreaParametersFromRealBodyParameters()
        {
            if (IsToUpdateRealHandDetectionAreaFromBodyParameters)
            {
                RealHandDetectionAreaCenterXOffset.Value = RealHandDetectionAreaCenterXOffset_DividedBy_RealShoulderBreadth * RealShoulderBreadth;
                RealHandDetectionAreaCenterYOffset.Value = RealHandDetectionAreaCenterYOffset_DividedBy_RealFaceBreadth * RealFaceBreadth;
                RealHandDetectionAreaCenterZOffset.Value = RealHandDetectionAreaCenterZOffset_DividedBy_RealShoulderBreadth * RealShoulderBreadth;
                RealHandDetectionAreaWidth.Value = RealHandDetectionAreaWidth_DividedBy_RealPalmBreadth * RealPalmBreadth;
                RealHandDetectionAreaHeight.Value = RealHandDetectionAreaHeight_DividedBy_RealPalmBreadth * RealPalmBreadth;
            }
        }

        internal void InitializeOnceAtStartup(EgsDevice device)
        {
            if (device == null)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                throw new ArgumentNullException("device");
            }
            Device = device;
            Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged += Device_CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged;
        }

        public void Reset()
        {
            // (Kickstarter Version)
            CalibratedFocalLength = 2.92;

            if (ApplicationCommonSettings.IsDebuggingInternal)
            {
                // (Wider lens for development in Exvision)
                CalibratedFocalLength = 2.39;
            }

            // Pixel Size (One Side Length): 0.0028[mm] (2x2 binning).
            CaptureImageBinnedPixelSize = 0.0028;

            CaptureImageWidth = 768;
            CaptureImageHeight = 480;
            CameraViewImageWidth = 384;
            CameraViewImageHeight = 240;

            MaxDetectableDistanceInMeter.Value = 3.0;
            RealFaceBreadth.Value = 159;
            RealPalmBreadth.Value = 78;

            IsToUpdateRealHandDetectionAreaFromBodyParameters = true;
            RealShoulderBreadth.Value = 379;
            RealHandDetectionAreaCenterXOffset_DividedBy_RealShoulderBreadth.Value = 0.68;
            RealHandDetectionAreaCenterYOffset_DividedBy_RealFaceBreadth.Value = 0.63;
            RealHandDetectionAreaCenterZOffset_DividedBy_RealShoulderBreadth.Value = -0.26;
            RealHandDetectionAreaWidth_DividedBy_RealPalmBreadth.Value = 4.5;
            RealHandDetectionAreaHeight_DividedBy_RealPalmBreadth.Value = 5.1;

            UpdateRealHandDetectionAreaParametersFromRealBodyParameters();

            SensitivityAndSpecificity.Value = 0;

            DetectorImageDetectableFaceWidthMinimum = 73;
            SetCameraViewImageScale_DividedBy_CaptureImageScale_ToCameraViewImageHeight_DividedBy_CaptureImageheight();
            Update_DetectorImageScale_DividedBy_CameraViewImageScale();
            Debug.WriteLine("DetectorImageScale_DividedBy_CameraViewImageScale: " + DetectorImageScale_DividedBy_CameraViewImageScale);

            DetectFaceIntervalMillisecondsMinimum.Value = 200;

            //IsDetecting = false;

            DetectedFaceRectsInCameraViewImage = new List<System.Drawing.Rectangle>();


            HandDetectionScaleForEgsDeviceMaximum = 25;
            // When CaptureImagePalmBreadth is about 30, HandDetectionScaleForEgsDevice is 8.
            HandDetectionScaleForEgsDevice_DividedBy_CaptureImagePalmImageWidth = 8.0 / 30.0;

            ResetDeviceSettingsRelatedProperties();
        }

        public void ResetDeviceSettingsRelatedProperties()
        {
            CameraViewImageRightHandDetectionArea = new System.Drawing.Rectangle();
            CameraViewImageLeftHandDetectionArea = new System.Drawing.Rectangle();
            CaptureImageRightHandDetectionArea = new System.Drawing.Rectangle();
            CaptureImageLeftHandDetectionArea = new System.Drawing.Rectangle();
            CaptureImagePalmImageWidth = 0;
        }

        void Device_CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged(object sender, EventArgs e)
        {
            if (Device.Settings.FaceDetectionMethod.Value != PropertyTypes.FaceDetectionMethods.DefaultProcessOnEgsHostApplication)
            {
                return;
            }
            if (Device.IsDetectingFaces == false)
            {
                return;
            }

            if (false && ApplicationCommonSettings.IsDebugging)
            {
                // Draw the latest result before return;
                if (SelectedFaceRect.HasValue)
                {
                    using (var g = System.Drawing.Graphics.FromImage(Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmap))
                    {
                        var pen = new System.Drawing.Pen(System.Drawing.Brushes.Green, 5);
                        g.DrawRectangle(pen, SelectedFaceRect.Value);
                    }
                }
            }

            if (DetectFaceIntervalMinimumStopwatch.ElapsedMilliseconds < DetectFaceIntervalMillisecondsMinimum) { return; }
            DetectFaceIntervalMinimumStopwatch.Reset();
            DetectFaceIntervalMinimumStopwatch.Start();
            DetectFaceRunWorkerAsync(Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmap);
        }

        public void DetectFaceRunWorkerAsync(System.Drawing.Bitmap cameraViewImageBitmap)
        {
            if (IsDetecting) { return; }

            IsDetecting = true;

            // Access to Bitmap must be in the same thread.
            Debug.Assert(cameraViewImageBitmap != null);
            Debug.Assert(cameraViewImageBitmap.Size.IsEmpty == false);
            CameraViewImageWidth = cameraViewImageBitmap.Width;
            CameraViewImageHeight = cameraViewImageBitmap.Height;
            using (var clonedBmp = (System.Drawing.Bitmap)cameraViewImageBitmap.Clone())
            {
                DlibArray2dUcharImage.SetBitmap(clonedBmp);
            }
            Worker.RunWorkerAsync();
        }

        public void DetectFaces()
        {
            try
            {
                Update_DetectorImageScale_DividedBy_CameraViewImageScale();
                var scale = DetectorImageScale_DividedBy_CameraViewImageScale;
                var detectorImageWidth = (int)(CameraViewImageWidth * scale);
                var detectorImageHeight = (int)(CameraViewImageHeight * scale);
                if (false)
                {
                    Debug.WriteLine("DetectorImageWidth: " + detectorImageWidth);
                    Debug.WriteLine("DetectorImageHeight: " + detectorImageHeight);
                }
                if (ApplicationCommonSettings.IsDeveloperRelease) { DetectionElapsedStopwatch.Reset(); DetectionElapsedStopwatch.Start(); }
                DlibArray2dUcharImage.ResizeImage(detectorImageWidth, detectorImageHeight);
                DetectedFaceRectsInCameraViewImage = DlibHogSvm.DetectFaces(DlibArray2dUcharImage, DlibHogSvmThreshold)
                    .Select(e => new System.Drawing.Rectangle((int)(e.X / scale), (int)(e.Y / scale), (int)(e.Width / scale), (int)(e.Height / scale)))
                    .ToList();
                if (ApplicationCommonSettings.IsDeveloperRelease) { DetectionElapsedMilliseconds = (int)DetectionElapsedStopwatch.ElapsedMilliseconds; }
            }
            catch (Exception ex)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Console.WriteLine(ex.Message);
            }
        }

        void DetectFaces_RunWorkerCompleted()
        {
            if (IsFaceDetected == false && Device != null)
            {
                SelectedFaceRect = null;
                Device.EgsGestureHidReport.ResetWhenHostFaceDetectionDidNotDetectAnyFaces();
            }
            else
            {
                DetectedFaceRectsInCameraViewImage = DetectedFaceRectsInCameraViewImage.OrderBy(e => DistanceFromCameraViewImageCenter(e)).ToList();
                SelectedFaceRect = DetectedFaceRectsInCameraViewImage.First();
                Device.EgsGestureHidReport.UpdateWhenHostFaceDetectionDetectedFaces();
            }

            UpdateEgsDeviceSettingsHandDetectionAreas(SelectedFaceRect);
            UpdateDeviceSettings(Device.Settings);

            // TODO: MUSTDO: Check the order of event raising and IsDetecting = false.
            OnPropertyChanged(nameof(IsFaceDetected));
            OnPropertyChanged(nameof(SelectedFaceRect));
            OnFaceDetectionCompleted(EventArgs.Empty);

            IsDetecting = false;
        }

        /// <summary>
        /// You can set "Hand Detection Areas" by setting "cameraViewImageFaceRect" detected by your method.
        /// </summary>
        public void UpdateEgsDeviceSettingsHandDetectionAreas(Nullable<System.Drawing.Rectangle> cameraViewImageFaceRectNullable)
        {
            if (cameraViewImageFaceRectNullable.HasValue == false)
            {
                ResetDeviceSettingsRelatedProperties();
                return;
            }
            var cameraViewImageFaceRect = cameraViewImageFaceRectNullable.Value;
            if (cameraViewImageFaceRect.Width <= 0)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                ResetDeviceSettingsRelatedProperties();
                return;
            }

            // TODO: MUSTDO: SDK users can set SetCameraViewImageScale_DividedBy_CaptureImageScale by their method.
            SetCameraViewImageScale_DividedBy_CaptureImageScale_ToCameraViewImageHeight_DividedBy_CaptureImageheight();

            double CameraViewImagePixelSizeOnSensor = CaptureImageBinnedPixelSize / CameraViewImageScale_DividedBy_CaptureImageScale;

            // (CameraViewImagePixelSizeOnSensor * cameraViewImageFaceRect.Width) : CalibratedFocalLength = RealFaceBreadth : RealFaceCenterZ
            double RealFaceBreadthOnSensor = CameraViewImagePixelSizeOnSensor * cameraViewImageFaceRect.Width;
            double RealFaceCenterZ = CalibratedFocalLength * (RealFaceBreadth / RealFaceBreadthOnSensor);

            // You can define different Z values to right and left.  (for example 2 players play)
            double RealDetectionAreaCenterZ = RealFaceCenterZ + RealHandDetectionAreaCenterZOffset;
            if (RealDetectionAreaCenterZ < 10)
            {
                // < 1[cm]
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                RealDetectionAreaCenterZ = 10;
                RealFaceCenterZ = RealDetectionAreaCenterZ - RealHandDetectionAreaCenterZOffset;
            }

            // Positive X => Right
            // Positive Y => Bottom
            double NormalizedCameraViewImageFaceCenterX = cameraViewImageFaceRect.X + cameraViewImageFaceRect.Width / 2.0 - CameraViewImageWidth / 2.0;
            double RealFaceCenterX = (CameraViewImagePixelSizeOnSensor * NormalizedCameraViewImageFaceCenterX) * (RealFaceCenterZ / CalibratedFocalLength);
            double NormalizedCameraViewImageFaceCenterY = cameraViewImageFaceRect.Y + cameraViewImageFaceRect.Height / 2.0 - CameraViewImageHeight / 2.0;
            double RealFaceCenterY = (CameraViewImagePixelSizeOnSensor * NormalizedCameraViewImageFaceCenterY) * (RealFaceCenterZ / CalibratedFocalLength);

            double RealRightDetectionAreaCenterX = RealFaceCenterX + RealHandDetectionAreaCenterXOffset;
            double RealRightDetectionAreaCenterY = RealFaceCenterY + RealHandDetectionAreaCenterYOffset;
            double RealRightDetectionAreaLeft = RealRightDetectionAreaCenterX - RealHandDetectionAreaWidth / 2.0;
            double RealRightDetectionAreaTop = RealRightDetectionAreaCenterY - RealHandDetectionAreaHeight / 2.0;
            double RealLeftDetectionAreaCenterX = RealFaceCenterX - RealHandDetectionAreaCenterXOffset;
            double RealLeftDetectionAreaCenterY = RealFaceCenterY + RealHandDetectionAreaCenterYOffset;
            double RealLeftDetectionAreaLeft = RealLeftDetectionAreaCenterX - RealHandDetectionAreaWidth / 2.0;
            double RealLeftDetectionAreaTop = RealLeftDetectionAreaCenterY - RealHandDetectionAreaHeight / 2.0;


            // (CaptureImageBinnedPixelSize * CaptureImageXY) : CalibratedFocalLength = RealXY : RealZ
            // CaptureImageXY = (CalibratedFocalLength / (CaptureImageBinnedPixelSize * RealZ)) * RealXY
            double CameraViewImagePixelsCount_DividedBy_Real = CalibratedFocalLength / (CameraViewImagePixelSizeOnSensor * RealDetectionAreaCenterZ);
            double CameraViewImageRightDetectionAreaX = CameraViewImagePixelsCount_DividedBy_Real * RealRightDetectionAreaLeft + CameraViewImageWidth / 2.0;
            double CameraViewImageRightDetectionAreaY = CameraViewImagePixelsCount_DividedBy_Real * RealRightDetectionAreaTop + CameraViewImageHeight / 2.0;
            double CameraViewImageRightDetectionAreaWidth = CameraViewImagePixelsCount_DividedBy_Real * RealHandDetectionAreaWidth;
            double CameraViewImageRightDetectionAreaHeight = CameraViewImagePixelsCount_DividedBy_Real * RealHandDetectionAreaHeight;
            double CameraViewImageLeftDetectionAreaX = CameraViewImagePixelsCount_DividedBy_Real * RealLeftDetectionAreaLeft + CameraViewImageWidth / 2.0;
            double CameraViewImageLeftDetectionAreaY = CameraViewImagePixelsCount_DividedBy_Real * RealLeftDetectionAreaTop + CameraViewImageHeight / 2.0;
            double CameraViewImageLeftDetectionAreaWidth = CameraViewImagePixelsCount_DividedBy_Real * RealHandDetectionAreaWidth;
            double CameraViewImageLeftDetectionAreaHeight = CameraViewImagePixelsCount_DividedBy_Real * RealHandDetectionAreaHeight;
            CameraViewImageRightHandDetectionArea = new System.Drawing.Rectangle(
                (int)CameraViewImageRightDetectionAreaX,
                (int)CameraViewImageRightDetectionAreaY,
                (int)CameraViewImageRightDetectionAreaWidth,
                (int)CameraViewImageRightDetectionAreaHeight);
            CameraViewImageLeftHandDetectionArea = new System.Drawing.Rectangle(
                (int)CameraViewImageLeftDetectionAreaX,
                (int)CameraViewImageLeftDetectionAreaY,
                (int)CameraViewImageLeftDetectionAreaWidth,
                (int)CameraViewImageLeftDetectionAreaHeight);

            double CaptureImagePalmImageWidth_DividedBy_RealPalmBreadth = CalibratedFocalLength / (CaptureImageBinnedPixelSize * RealDetectionAreaCenterZ);
            double CaptureImageRightDetectionAreaX = CaptureImagePalmImageWidth_DividedBy_RealPalmBreadth * RealRightDetectionAreaLeft + CaptureImageWidth / 2.0;
            double CaptureImageRightDetectionAreaY = CaptureImagePalmImageWidth_DividedBy_RealPalmBreadth * RealRightDetectionAreaTop + CaptureImageHeight / 2.0;
            double CaptureImageRightDetectionAreaWidth = CaptureImagePalmImageWidth_DividedBy_RealPalmBreadth * RealHandDetectionAreaWidth;
            double CaptureImageRightDetectionAreaHeight = CaptureImagePalmImageWidth_DividedBy_RealPalmBreadth * RealHandDetectionAreaHeight;
            double CaptureImageLeftDetectionAreaX = CaptureImagePalmImageWidth_DividedBy_RealPalmBreadth * RealLeftDetectionAreaLeft + CaptureImageWidth / 2.0;
            double CaptureImageLeftDetectionAreaY = CaptureImagePalmImageWidth_DividedBy_RealPalmBreadth * RealLeftDetectionAreaTop + CaptureImageHeight / 2.0;
            double CaptureImageLeftDetectionAreaWidth = CaptureImagePalmImageWidth_DividedBy_RealPalmBreadth * RealHandDetectionAreaWidth;
            double CaptureImageLeftDetectionAreaHeight = CaptureImagePalmImageWidth_DividedBy_RealPalmBreadth * RealHandDetectionAreaHeight;
            CaptureImageRightHandDetectionArea = new System.Drawing.Rectangle(
                (int)CaptureImageRightDetectionAreaX,
                (int)CaptureImageRightDetectionAreaY,
                (int)CaptureImageRightDetectionAreaWidth,
                (int)CaptureImageRightDetectionAreaHeight);
            CaptureImageLeftHandDetectionArea = new System.Drawing.Rectangle(
                (int)CaptureImageLeftDetectionAreaX,
                (int)CaptureImageLeftDetectionAreaY,
                (int)CaptureImageLeftDetectionAreaWidth,
                (int)CaptureImageLeftDetectionAreaHeight);

            CaptureImagePalmImageWidth = CaptureImagePalmImageWidth_DividedBy_RealPalmBreadth * RealPalmBreadth;
        }

        public void UpdateDeviceSettings(EgsDeviceSettings deviceSettings)
        {
            RatioRect RightHandDetectionAreaRatioRect = new RatioRect();
            RatioRect LeftHandDetectionAreaRatioRect = new RatioRect();
            RightHandDetectionAreaRatioRect.XRange.From = (float)(CaptureImageRightHandDetectionArea.X / CaptureImageWidth);
            RightHandDetectionAreaRatioRect.XRange.To = (float)((CaptureImageRightHandDetectionArea.X + CaptureImageRightHandDetectionArea.Width) / CaptureImageWidth);
            RightHandDetectionAreaRatioRect.YRange.From = (float)(CaptureImageRightHandDetectionArea.Y / CaptureImageHeight);
            RightHandDetectionAreaRatioRect.YRange.To = (float)((CaptureImageRightHandDetectionArea.Y + CaptureImageRightHandDetectionArea.Height) / CaptureImageHeight);
            LeftHandDetectionAreaRatioRect.XRange.From = (float)(CaptureImageLeftHandDetectionArea.X / CaptureImageWidth);
            LeftHandDetectionAreaRatioRect.XRange.To = (float)((CaptureImageLeftHandDetectionArea.X + CaptureImageLeftHandDetectionArea.Width) / CaptureImageWidth);
            LeftHandDetectionAreaRatioRect.YRange.From = (float)(CaptureImageLeftHandDetectionArea.Y / CaptureImageHeight);
            LeftHandDetectionAreaRatioRect.YRange.To = (float)((CaptureImageLeftHandDetectionArea.Y + CaptureImageLeftHandDetectionArea.Height) / CaptureImageHeight);

            var newHandDetectionScaleForEgsDevice = HandDetectionScaleForEgsDevice_DividedBy_CaptureImagePalmImageWidth * CaptureImagePalmImageWidth;
            if (newHandDetectionScaleForEgsDevice > HandDetectionScaleForEgsDeviceMaximum)
            {
                if (false) { Debug.WriteLine("newHandDetectionScaleForEgsDevice: " + newHandDetectionScaleForEgsDevice); }
            }
            int HandDetectionScaleForEgsDevice = (int)Math.Min(newHandDetectionScaleForEgsDevice, short.MaxValue);

            if (HandDetectionScaleForEgsDevice == 0)
            {
                // TODO: MUSTDO: Test another way.
                if (deviceSettings.IsToDetectHandsOnDevice.Value != false) { deviceSettings.IsToDetectHandsOnDevice.Value = false; }
            }
            else
            {
                // It has to set Value property atomically.
                deviceSettings.RightHandDetectionAreaOnFixed.Value = RightHandDetectionAreaRatioRect;
                deviceSettings.RightHandDetectionScaleOnFixed.RangedValue.Value = HandDetectionScaleForEgsDevice;
                deviceSettings.LeftHandDetectionAreaOnFixed.Value = LeftHandDetectionAreaRatioRect;
                deviceSettings.LeftHandDetectionScaleOnFixed.RangedValue.Value = HandDetectionScaleForEgsDevice;

                var newIsToDetectHandsOnDevice = deviceSettings.IsToDetectHands.Value;
                if (deviceSettings.IsToDetectHandsOnDevice.Value != newIsToDetectHandsOnDevice) { deviceSettings.IsToDetectHandsOnDevice.Value = newIsToDetectHandsOnDevice; }
            }
        }

        #region IDisposable
        private bool disposed = false;
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) { return; }
            if (disposing)
            {
                // dispose managed objects, and dispose objects that implement IDisposable
                if (Device != null)
                {
                    // When InitializeOnceAtStartup is not called.
                    Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged -= Device_CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged;
                }
                DetectFaceIntervalMinimumStopwatch.Reset();
                DetectFaceIntervalMinimumStopwatch.Start();
                while (IsDetecting)
                {
                    System.Threading.Thread.Sleep(50);
                    if (DetectFaceIntervalMinimumStopwatch.ElapsedMilliseconds > 2000)
                    {
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        Console.WriteLine("FaceDetection BackgroundWorkder did not completed.");
                        break;
                    }
                }
                if (DlibArray2dUcharImage != null) { DlibArray2dUcharImage.Dispose(); DlibArray2dUcharImage = null; }
                if (DlibHogSvm != null) { DlibHogSvm.Dispose(); DlibHogSvm = null; }
            }
            // release any unmanaged objects and set the object references to null
            disposed = true;
        }
        ~EgsDeviceFaceDetectionOnHost() { Dispose(false); }
        #endregion
    }
}
