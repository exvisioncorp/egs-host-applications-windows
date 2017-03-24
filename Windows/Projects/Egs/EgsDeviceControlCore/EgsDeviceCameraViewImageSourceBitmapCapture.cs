namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Windows.Forms;
    using Egs.PropertyTypes;
    using Egs.Win32;

    /// <summary>
    /// It opens camera device and gets images for WinForms bitmap.  EgsDevice has an object of this class.  Basically this class is managed by EgsDevice.
    /// </summary>
    public partial class EgsDeviceCameraViewImageSourceBitmapCapture : INotifyPropertyChanged
    {
        // TODO: MUSTDO: Fix the specification!  This 
        internal static readonly System.Drawing.Size DefaultCameraViewImageSourceBitmapSize = new System.Drawing.Size(384, 240);
        internal static readonly System.Drawing.Color DefaultCameraViewImageSourceBitmapColor = System.Drawing.Color.Blue;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        internal EgsDevice Device { get; set; }
        internal AForge.Video.DirectShow.VideoCaptureDevice AForgeVideoCaptureInstance { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsCameraDeviceConnected;
        public event EventHandler IsCameraDeviceConnectedChanged;
        protected virtual void OnIsCameraDeviceConnectedChanged(EventArgs e)
        {
            var t = IsCameraDeviceConnectedChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsCameraDeviceConnected));
        }
        public bool IsCameraDeviceConnected
        {
            get { return _IsCameraDeviceConnected; }
            private set
            {
                _IsCameraDeviceConnected = value;
                UpdateIsUpdatingImageSource();
                OnIsCameraDeviceConnectedChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsToUpdateImageSource = true;
        public event EventHandler IsToUpdateImageSourceChanged;
        protected virtual void OnIsToUpdateImageSourceChanged(EventArgs e)
        {
            var t = IsToUpdateImageSourceChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsToUpdateImageSource));
        }
        /// <summary>
        /// When this value is set true and device can work correctly, image source will be updated continuously.
        /// </summary>
        public bool IsToUpdateImageSource
        {
            get { return _IsToUpdateImageSource; }
            set
            {
                _IsToUpdateImageSource = value;
                UpdateIsUpdatingImageSource();
                OnIsToUpdateImageSourceChanged(EventArgs.Empty);
            }
        }
        public event EventHandler IsUpdatingImageSourceChanged;
        protected virtual void OnIsUpdatingImageSourceChanged(EventArgs e)
        {
            var t = IsUpdatingImageSourceChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsUpdatingImageSource));
        }
        public bool IsUpdatingImageSource
        {
            get
            {
                if (AForgeVideoCaptureInstance == null) { return false; }
                return AForgeVideoCaptureInstance.IsRunning;
            }
        }
        internal void UpdateIsUpdatingImageSource()
        {
            bool newValue = IsCameraDeviceConnected && _IsToUpdateImageSource;
            if (AForgeVideoCaptureInstance != null && AForgeVideoCaptureInstance.IsRunning != newValue)
            {
                if (newValue)
                {
                    AForgeVideoCaptureInstance.Start();
                }
                else
                {
                    StopAForgeVideoCaptureInstance();
                }
            }
            OnIsUpdatingImageSourceChanged(EventArgs.Empty);
        }

        // TODO: Check if it must implement IDisposable or not in this case.
        [EditorBrowsable(EditorBrowsableState.Never)]
        System.Drawing.Bitmap _CameraViewImageSourceBitmap;
        [EditorBrowsable(EditorBrowsableState.Never)]
        System.Drawing.Size _CameraViewImageSourceBitmapSize;
        [EditorBrowsable(EditorBrowsableState.Never)]
        System.Drawing.Imaging.PixelFormat _CameraViewImageSourceBitmapPixelFormat;
        public System.Drawing.Size CameraViewImageSourceBitmapSize { get { return _CameraViewImageSourceBitmapSize; } }
        public System.Drawing.Imaging.PixelFormat CameraViewImageSourceBitmapPixelFormat { get { return _CameraViewImageSourceBitmapPixelFormat; } }
        /// <summary>
        /// To draw your "CameraView" by your code, please use this event.
        /// </summary>
        public event EventHandler CameraViewImageSourceBitmapChanged;
        /// <summary>
        /// When the image (captured by Windows) size is changed by some reason, this event will be raised.
        /// </summary>
        public event EventHandler CameraViewImageSourceBitmapSizeOrPixelFormatChanged;
        protected virtual void OnCameraViewImageSourceBitmapChanged(EventArgs e)
        {
            var t = CameraViewImageSourceBitmapChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CameraViewImageSourceBitmap));
        }
        protected virtual void OnCameraViewImageSourceBitmapSizeOrPixelFormatChanged(EventArgs e)
        {
            var t = CameraViewImageSourceBitmapSizeOrPixelFormatChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CameraViewImageSourceBitmap));
            OnPropertyChanged(nameof(CameraViewImageSourceBitmapSize));
            OnPropertyChanged(nameof(CameraViewImageSourceBitmapPixelFormat));
        }

        /// <summary>
        /// When you use this for WinForms PictureBox, use like "PictureBox.Image.Dispose(); PictureBox.Image = (Bitmap)CameraViewImageSourceBitmap.Clone();".
        /// It needs Clone() method, and "Bitmap.Clone()" method is shallow copy and it does not copy image data itself, so it is fast.
        /// This class has an instance of "AForge.Video.DirectShow.VideoCaptureDevice".
        /// And each "VideoCaptureDevice.NewFrame" event sends an instance of "AForge.Video.NewFrameEventArgs" class.
        /// This CameraViewImageSourceBitmap is just a reference to the "NewFrameEventArgs.Frame" property.
        /// That's why Bitmap is IDisposable, but this class does not implement IDIsposable.
        /// So please do not call the Dispose() method by yourself to the return value of this property.
        /// </summary>
        public System.Drawing.Bitmap CameraViewImageSourceBitmap
        {
            get { return _CameraViewImageSourceBitmap; }
            private set
            {
                // NOTE: Even if there is no Compatibility, call an event after updating Bitmap.
                if (value == null)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    _CameraViewImageSourceBitmapSize = new System.Drawing.Size(1, 1);
                    _CameraViewImageSourceBitmapPixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                    OnCameraViewImageSourceBitmapSizeOrPixelFormatChanged(EventArgs.Empty);
                    OnCameraViewImageSourceBitmapChanged(EventArgs.Empty);
                    return;
                }
                _CameraViewImageSourceBitmap = value;
                if (_CameraViewImageSourceBitmapSize != value.Size || _CameraViewImageSourceBitmapPixelFormat != value.PixelFormat)
                {
                    _CameraViewImageSourceBitmapSize = value.Size;
                    _CameraViewImageSourceBitmapPixelFormat = value.PixelFormat;
                    OnCameraViewImageSourceBitmapSizeOrPixelFormatChanged(EventArgs.Empty);
                }
                OnCameraViewImageSourceBitmapChanged(EventArgs.Empty);
            }
        }

        int UvcIsWorkingMonitorIntervalInMilliseconds { get; set; }
        System.Windows.Forms.Timer UvcIsWorkingMonitorTimer { get; set; }
        Stopwatch UvcIsWorkingMonitorStopwatch { get; set; }
        internal bool IsRestartingUvc { get; private set; }
        void StartUvcIsWorkingMonitorTimer()
        {
            UvcIsWorkingMonitorStopwatch.Reset();
            UvcIsWorkingMonitorStopwatch.Start();
            UvcIsWorkingMonitorTimer.Start();
        }
        void StopUvcIsWorkingMonitorTimer()
        {
            UvcIsWorkingMonitorTimer.Stop();
            UvcIsWorkingMonitorStopwatch.Stop();
            UvcIsWorkingMonitorStopwatch.Reset();
        }


        internal EgsDeviceCameraViewImageSourceBitmapCapture()
        {
            AForgeVideoCaptureInstance = null;
            _VideoCaptureDeviceIndex = null;
            _VideoCaptureDeviceName = "";
            _VideoCaptureDeviceDevicePath = "";

            _CameraViewImageSourceBitmap = new System.Drawing.Bitmap(DefaultCameraViewImageSourceBitmapSize.Width, DefaultCameraViewImageSourceBitmapSize.Height);
            _CameraViewImageSourceBitmapSize = _CameraViewImageSourceBitmap.Size;
            _CameraViewImageSourceBitmapPixelFormat = _CameraViewImageSourceBitmap.PixelFormat;

            _IsCameraDeviceConnected = false;

            //OnDeviceDisconnectedDelayTimer.Interval = 2000;
            UvcIsWorkingMonitorIntervalInMilliseconds = EgsDevice.DefaultEgsDevicesManager.DeviceConnectionDelayTimersInterval * 2;
            UvcIsWorkingMonitorTimer = new System.Windows.Forms.Timer() { Interval = UvcIsWorkingMonitorIntervalInMilliseconds };
            UvcIsWorkingMonitorStopwatch = new Stopwatch();
            IsRestartingUvc = false;
        }

        void UvcIsWorkingMonitorTimer_Tick(object sender, EventArgs e)
        {
            if (UvcIsWorkingMonitorStopwatch.ElapsedMilliseconds > UvcIsWorkingMonitorIntervalInMilliseconds)
            {
                StopUvcIsWorkingMonitorTimer();
                if (Device.IsUpdatingFirmware) { return; }
                try
                {
                    IsRestartingUvc = true;
                    Device.ResetDevice();
                    Device.ResetHidReportObjects();
                    Device.StopFaceDetectionAndRestartUvcAndRestartFaceDetection();
                    Device.Settings.IsToDetectFaces.Value = true;
                }
                catch (Exception ex)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new Egs.EgsDeviceOperationException("Could not restart UVC device.", ex);
                }
                finally
                {
                    IsRestartingUvc = false;
                }
            }
        }

        internal void InitializeOnceAtStartup(EgsDevice device)
        {
            Trace.Assert(device != null);
            Device = device;
            DisposeVideoCaptureAndSetIsCameraDeviceConnectedToFalse();
        }

        void StopAForgeVideoCaptureInstance()
        {
            if (AForgeVideoCaptureInstance.IsRunning == false) { return; }
            AForgeVideoCaptureInstance.SignalToStop();
            System.Threading.Thread.Sleep(500);
            // TODO: MUSTDO: It can stop the app unexpectedly.  Should check the reason.
            if (false) { AForgeVideoCaptureInstance.WaitForStop(); }

            var sw = Stopwatch.StartNew();
            bool hasChangedIsRunningToFalse = false;
            while (sw.ElapsedMilliseconds < 1000)
            {
                if (AForgeVideoCaptureInstance.IsRunning == false)
                {
                    hasChangedIsRunningToFalse = true;
                    break;
                }
                Thread.Sleep(100);
            }
            if (false && ApplicationCommonSettings.IsDebugging && hasChangedIsRunningToFalse == false) { Debugger.Break(); }
        }

        void DisposeAForgeVideoCaptureInstance()
        {
            if (AForgeVideoCaptureInstance != null)
            {
                // NOTE: newは1箇所のみで、newした直後にイベントハンドラをアタッチしているので、ここで呼び出してOK。
                StopUvcIsWorkingMonitorTimer();
                UvcIsWorkingMonitorTimer.Tick -= UvcIsWorkingMonitorTimer_Tick;
                AForgeVideoCaptureInstance.NewFrame -= AForgeVideoCaptureInstance_NewFrame;
                StopAForgeVideoCaptureInstance();
                AForgeVideoCaptureInstance = null;
            }
        }

        void DisposeVideoCaptureAndSetIsCameraDeviceConnectedToFalse()
        {
            DisposeAForgeVideoCaptureInstance();
            // NOTE: When device is not connected, camera view color becomes blue.
            if (_CameraViewImageSourceBitmap != null) { _CameraViewImageSourceBitmap.Dispose(); _CameraViewImageSourceBitmap = null; }
            CameraViewImageSourceBitmap = new System.Drawing.Bitmap(DefaultCameraViewImageSourceBitmapSize.Width, DefaultCameraViewImageSourceBitmapSize.Height);
            using (var g = System.Drawing.Graphics.FromImage(CameraViewImageSourceBitmap)) { g.Clear(DefaultCameraViewImageSourceBitmapColor); }
            IsCameraDeviceConnected = false;
        }

        bool TrySetupDeviceOnce()
        {
            try
            {
                DisposeAForgeVideoCaptureInstance();

                AForgeVideoCaptureInstance = new AForge.Video.DirectShow.VideoCaptureDevice(VideoCaptureDeviceDevicePath);
                if (AForgeVideoCaptureInstance == null) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } throw new EgsDeviceOperationException("AForgeVideoCaptureInstance == null."); }
                AForgeVideoCaptureInstance.NewFrame += AForgeVideoCaptureInstance_NewFrame;
                StartUvcIsWorkingMonitorTimer();
                UvcIsWorkingMonitorTimer.Tick += UvcIsWorkingMonitorTimer_Tick;

                // TODO: MUSTDO: AForgeVideoCaptureInstance.VideoResolution = AForgeVideoCaptureInstance.VideoCapabilities[someIndex]; で解像度が変わるが、
                // AForgeVideoCaptureInstance.IsRunning == true のままでは変更できない。
                // そしてSignalToStop()からのWaitForStop()メソッドの実行が完了しないことがある。
                // デバイスがKS版ではなく、後のネゴシエーションの更新版以降では問題ないのか？

                if (AForgeVideoCaptureInstance.VideoCapabilities == null) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } throw new EgsDeviceOperationException("AForgeVideoCaptureInstance.VideoCapabilities == null"); }
                if (VideoCaptureDeviceIndex < 0) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } throw new EgsDeviceOperationException("VideoCaptureDeviceIndex < 0"); }
                var videoCapabilityIndex = Device.Settings.CameraViewImageSourceBitmapSize.OptionalValue.SelectedIndex;
                if (videoCapabilityIndex < 0) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } throw new EgsDeviceOperationException("videoCapabilityIndex < 0"); }
                if (videoCapabilityIndex >= AForgeVideoCaptureInstance.VideoCapabilities.Length) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } throw new EgsDeviceOperationException("videoCapabilityIndex >= AForgeVideoCaptureInstance.VideoCapabilities.Length"); }

                var capability = AForgeVideoCaptureInstance.VideoCapabilities[videoCapabilityIndex];
                if (capability == null) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } throw new EgsDeviceOperationException("capability == null"); }
                int width = capability.FrameSize.Width;
                if (width == 0) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } throw new EgsDeviceOperationException("width == 0"); }
                int height = capability.FrameSize.Height;
                if (height == 0) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } throw new EgsDeviceOperationException("height == 0"); }

                AForgeVideoCaptureInstance.VideoResolution = capability;
                AForgeVideoCaptureInstance.Start();
                IsCameraDeviceConnected = true;

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return false;
            }
        }

        internal bool SetupCameraDevice()
        {
            if (Device.IsUpdatingFirmware) { return false; }
            int trialCountMax = 5;
            try
            {
                for (int trialCount = 0; trialCount < trialCountMax; trialCount++)
                {
                    // TODO: MUSTDO: check
                    if (TrySetupDeviceOnce()) { return true; }
                    // TODO: MUSTDO: check
                    System.Threading.Thread.Sleep(500);
                    string msg = string.Format(CultureInfo.InvariantCulture, "Failed setup device.  deviceId == {0}.  Setup ({1} / {2}).", VideoCaptureDeviceIndex, trialCount + 1, trialCountMax);
                    Debug.WriteLine(msg);
                }
                throw new EgsDeviceOperationException("trialCount == trialCountMax");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (ApplicationCommonSettings.IsDebugging) { MessageBox.Show(ex.Message); }
                DisposeVideoCaptureAndSetIsCameraDeviceConnectedToFalse();
                return false;
            }
        }

        void AForgeVideoCaptureInstance_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            CameraViewImageSourceBitmap = (System.Drawing.Bitmap)eventArgs.Frame;
            UvcIsWorkingMonitorStopwatch.Reset();
            UvcIsWorkingMonitorStopwatch.Start();
        }

        internal void DisposeWithClearingVideoCaptureDeviceInformationOnDeviceDisconnected()
        {
            DisposeVideoCaptureAndSetIsCameraDeviceConnectedToFalse();
            VideoCaptureDeviceIndex = null;
            VideoCaptureDeviceName = "";
            VideoCaptureDeviceDevicePath = "";
        }

        /// <summary>
        /// Return deep-copied bitmap data.
        /// </summary>
        internal System.Drawing.Bitmap GetDeepCopiedBitmap(bool copyPalette = true)
        {
            var ret = new System.Drawing.Bitmap(CameraViewImageSourceBitmap.Width, CameraViewImageSourceBitmap.Height, CameraViewImageSourceBitmap.PixelFormat);
            if (DeepCopyBitmap(CameraViewImageSourceBitmap, ret, copyPalette) == false) { ret = null; }
            return ret;
        }

        /// <summary>
        /// Deep-copy bitmap data.  Bitmaps must have same size and pixel format.
        /// </summary>
        internal static bool DeepCopyBitmap(System.Drawing.Bitmap src, System.Drawing.Bitmap dest, bool isToCopyPallette = false)
        {
            bool copyOk = false;
            copyOk = HasSameSizeAndPixelFormat(src, dest);
            if (copyOk)
            {
                System.Drawing.Imaging.BitmapData bmpDataSrc;
                System.Drawing.Imaging.BitmapData bmpDataDest;
                //Lock Bitmap to get BitmapData
                bmpDataSrc = src.LockBits(new System.Drawing.Rectangle(0, 0, src.Width, src.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, src.PixelFormat);
                bmpDataDest = dest.LockBits(new System.Drawing.Rectangle(0, 0, dest.Width, dest.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, dest.PixelFormat);
                int lenght = bmpDataSrc.Stride * bmpDataSrc.Height;
                NativeMethods.CopyMemory(bmpDataDest.Scan0, bmpDataSrc.Scan0, (uint)lenght);
                src.UnlockBits(bmpDataSrc);
                dest.UnlockBits(bmpDataDest);
                if (isToCopyPallette && src.Palette.Entries.Length > 0) { dest.Palette = src.Palette; }
            }
            return copyOk;
        }

        internal static bool HasSameSizeAndPixelFormat(System.Drawing.Bitmap bmp1, System.Drawing.Bitmap bmp2)
        {
            return ((bmp1.Size == bmp2.Size) && (bmp1.PixelFormat == bmp2.PixelFormat));
        }
    }
}
