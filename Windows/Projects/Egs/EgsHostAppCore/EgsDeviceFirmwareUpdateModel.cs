namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System.Windows;
    using System.ComponentModel;
    using System.Globalization;
    using Microsoft.Win32.SafeHandles;
    using Egs.PropertyTypes;
    using Egs.DotNetUtility;
    using Egs.EgsDeviceControlCore.Properties;

    internal class EgsDeviceFirmwareUpdateStateReport
    {
        public string Message { get; set; }
        public string UserNotificationMessage { get; set; }
        string _MessageForDebug;
        public string MessageForDebug
        {
            get { return _MessageForDebug; }
            set
            {
                if (ApplicationCommonSettings.IsDebugging)
                {
                    _MessageForDebug = value;
                }
            }
        }
    }

    internal class EgsDeviceFirmwareUpdateResult
    {
        public bool CanRetryOrElseMustRebootDevice { get; set; }
        public bool IsCanceled { get; set; }
        public bool CompletedOrElseFailed { get; set; }
        public string Message { get; set; }
        public EgsDeviceFirmwareUpdateResult(bool canRetryOrElseMustRebootDevice, bool isCanceled, bool completedOrElseFailed, string message)
        {
            CanRetryOrElseMustRebootDevice = canRetryOrElseMustRebootDevice;
            IsCanceled = isCanceled;
            CompletedOrElseFailed = completedOrElseFailed;
            Message = message;
        }
    }

    public enum FirmwareUpdateProtocolRevisionKinds : int
    {
        Unknown = 0,
        KickStarterFirstRelease = 11448,
        FileUpdater = 0x01000000,
        Latest = 0x02000000
    }

    internal enum EgsDeviceFirmwareUpdateUserActions
    {
        None,
        ConnectDevice,
        DisconnectDevice,
        DoNotDisconnectDevice,
    }

    internal class EgsDeviceFirmwareUpdateModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        public EgsDevice Device { get; private set; }

        IList<string> _FirmwareImageFilePathList = null;
        public IList<string> FirmwareImageFilePathList
        {
            get { return _FirmwareImageFilePathList; }
            set
            {
                _FirmwareImageFilePathList = value;
                OnPropertyChanged("FirmwareImageFilePathList");
                OnPropertyChanged("FirmwareImageFilePathListCount");
            }
        }

        public static FirmwareUpdateProtocolRevisionKinds ConvertFromIntToFirmwareUpdateProtocolRevisionKinds(int value)
        {
            FirmwareUpdateProtocolRevisionKinds ret = FirmwareUpdateProtocolRevisionKinds.Unknown;
            var isFirmwareUpdateProtocolRevisionInImageFileDefined = Enum.IsDefined(typeof(FirmwareUpdateProtocolRevisionKinds), value);
            if (isFirmwareUpdateProtocolRevisionInImageFileDefined) { ret = (FirmwareUpdateProtocolRevisionKinds)value; }
            return ret;
        }
        public EgsDeviceFirmwareUpdateImageFileModel ImageFile { get; private set; }
        public FirmwareUpdateProtocolRevisionKinds FirmwareUpdateProtocolRevisionInDevice { get; private set; }
        public Version FirmwareVersionInDevice { get; private set; }
        public bool IsToShowWarningMessages { get; set; }

        public int RatioOfSendingImageFileInUpdatingByOneFile { get; set; }
        public BackgroundWorker ProgressReport { get; private set; }
        public EgsDeviceFirmwareUpdateStateReport LastStateReport { get; private set; }

        int _CurrentIndexInFirmwareImageFilePathList = 0;
        public int CurrentIndexInFirmwareImageFilePathList
        {
            get { return _CurrentIndexInFirmwareImageFilePathList; }
            private set
            {
                _CurrentIndexInFirmwareImageFilePathList = value;
                OnPropertyChanged("CurrentIndexInFirmwareImageFilePathList");
                OnPropertyChanged("CurrentIndexInFirmwareImageFilePathListForView");
            }
        }
        /// <summary>
        /// CurrentIndexInFirmwareImageFilePathList + 1
        /// </summary>
        public int CurrentIndexInFirmwareImageFilePathListForView
        {
            get { return _CurrentIndexInFirmwareImageFilePathList + 1; }
        }
        public int FirmwareImageFilePathListCount
        {
            get { return FirmwareImageFilePathList.Count; }
        }

        int _PercentProgressInOneFile = 0;
        public int PercentProgressInOneFile
        {
            get { return _PercentProgressInOneFile; }
            private set
            {
                if (_PercentProgressInOneFile != value)
                {
                    _PercentProgressInOneFile = value;
                    OnPropertyChanged("PercentProgressInOneFile");
                    ProgressReport.ReportProgress(PercentProgress);
                }
            }
        }
        public int PercentProgress
        {
            get
            {
                var ret = (CurrentIndexInFirmwareImageFilePathList * 100 + _PercentProgressInOneFile) / FirmwareImageFilePathList.Count;
                ret = Math.Max(0, Math.Min(ret, 100));
                return ret;
            }
        }

        public EgsDeviceFirmwareUpdateResult LastResult { get; private set; }

        public SimpleDelegateCommand FileSelectionCommand { get; private set; }
        public SimpleDelegateCommand StartOrCancelCommand { get; private set; }

        public string StartOrCancelButtonContent
        {
            get { return _IsBusy ? Resources.CommonStrings_Cancel : Resources.CommonStrings_Start; }
        }
        bool _IsBusy = false;
        public bool IsBusy
        {
            get { return _IsBusy; }
            private set
            {
                _IsBusy = value;
                OnPropertyChanged("IsBusy");
                OnPropertyChanged("StartOrCancelButtonContent");
            }
        }
        public bool IsCanceled { get; private set; }

        string _MessageText = "";
        public string MessageText
        {
            get { return _MessageText; }
            set { _MessageText = value; OnPropertyChanged("MessageText"); }
        }

        EgsDeviceFirmwareUpdateUserActions _ExpectedUserAction = EgsDeviceFirmwareUpdateUserActions.None;
        public EgsDeviceFirmwareUpdateUserActions ExpectedUserAction
        {
            get { return _ExpectedUserAction; }
            set
            {
                _ExpectedUserAction = value;
                switch (_ExpectedUserAction)
                {
                    case EgsDeviceFirmwareUpdateUserActions.None:
                        MessageText = "";
                        break;
                    case EgsDeviceFirmwareUpdateUserActions.ConnectDevice:
                        MessageText = Resources.EgsDeviceFirmwareUpdateModel_ZkooNeedsToConnectPowerCable;
                        break;
                    case EgsDeviceFirmwareUpdateUserActions.DisconnectDevice:
                        MessageText = Resources.EgsDeviceFirmwareUpdateModel_ZkooNeedsToDisconnectPowerCable;
                        break;
                    case EgsDeviceFirmwareUpdateUserActions.DoNotDisconnectDevice:
                        MessageText = Resources.EgsDeviceFirmwareUpdateModel_DoNotDisconnectTheDevice;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                OnPropertyChanged("ExpectedUserAction");
            }
        }


        public EgsDeviceFirmwareUpdateModel(EgsDevice device, Version deviceFirmwareVersion)
        {
            Trace.Assert(device != null);
            Trace.Assert(deviceFirmwareVersion != null);
            Device = device;
            FirmwareVersionInDevice = deviceFirmwareVersion;

            RatioOfSendingImageFileInUpdatingByOneFile = 80;

            FileSelectionCommand = new SimpleDelegateCommand();
            FileSelectionCommand.CanPerform = true;
            FileSelectionCommand.PerformEventHandler += delegate { SelectFiles(); };

            StartOrCancelCommand = new SimpleDelegateCommand();
            StartOrCancelCommand.CanPerform = true;
            StartOrCancelCommand.PerformEventHandler += delegate
            {
                if (IsBusy) { CancelAsync(); } else { StartAsync(); }
            };

            ProgressReport = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            ProgressReport.DoWork += delegate
            {
                LastResult = DoWorkInternal();
            };
            ProgressReport.RunWorkerCompleted += delegate
            {
                IsBusy = false;
            };

            IsBusy = false;
            IsCanceled = false;
            LastResult = new EgsDeviceFirmwareUpdateResult(true, false, false, "");
            FirmwareImageFilePathList = new List<string>();
            IsToShowWarningMessages = false;
        }

        public void SelectFiles()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "piemonteApp1.bin";
            ofd.InitialDirectory = @"";  //@"C:\"
            ofd.Filter = "Exvision Firmware Update Binary(*.bin;*.exvbin)|*.bin;*.binexv|All files(*.*)|*.*";
            ofd.FilterIndex = 2;
            // TODO: use Resources
            ofd.Title = "Select file to upload to device";
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = true;
            if (ofd.ShowDialog() != true) { return; }
            Debug.WriteLine(ofd.FileNames);
            FirmwareImageFilePathList = ofd.FileNames.ToList();
        }

        public bool ShowDialogToAskAboutUpdatingDeviceFirmware()
        {
            var msg = Resources.EgsDeviceFirmwareUpdateModel_PleaseUpdateDeviceFirmware + Environment.NewLine;
            var hr = MessageBox.Show(msg, Resources.EgsDeviceFirmwareUpdateModel_DeviceFirmwareUpdate, MessageBoxButton.OKCancel);
            return hr == MessageBoxResult.OK;
        }

        internal void UpdateFirmwareUpdateProtocolRevisionInDevice()
        {
            Debug.WriteLine("SendFirmwareUpdateImage: reading firmware version from device ...");
            byte[] ackmsgbuffer = new byte[64];
            for (int i = 0; i < ackmsgbuffer.Length; i++) { ackmsgbuffer[i] = 0; }
            byte messageId = 0;

            //read feature report
            ackmsgbuffer[0] = (byte)HidReportIds.EgsDeviceFirmwareUpdate;
            try
            {
                // If Sleep(2000), the value of ackmsgbuffer[1] does not become to be expected value.
                Thread.Sleep(3000);
                Device.GetHidFeatureReport(ackmsgbuffer);
                // No relation
                //Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("GetFirmwareUpdateProtocolRevisionInDevice: error getfeature!!!");
                FirmwareUpdateProtocolRevisionInDevice = FirmwareUpdateProtocolRevisionKinds.Unknown;
                return;
            }

            messageId = ackmsgbuffer[1];
            if (messageId != 4)   //messageID=3;
            {
                // MUSTDO: I don't know the specification about this.
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Debug.WriteLine("WaitUpdateFinishResponse: messageID mismatched!!!");
                FirmwareUpdateProtocolRevisionInDevice = FirmwareUpdateProtocolRevisionKinds.Unknown;
                return;
            }
            uint uintValue = (uint)ackmsgbuffer[2] | ((((uint)ackmsgbuffer[3]) << 8) & 0xFF00) | ((((uint)ackmsgbuffer[4]) << 16) & 0xFF0000) | ((((uint)ackmsgbuffer[5]) << 24) & 0xFF000000);
            FirmwareUpdateProtocolRevisionInDevice = ConvertFromIntToFirmwareUpdateProtocolRevisionKinds((int)uintValue);
        }

        internal void UpdateFileListByFirmwareUpdateProtocolRevisionInDevice()
        {
            try
            {
                // send start packet only for get protocol
                var outBuf = new byte[64];
                uint seqNum = 0xFFFF;
                outBuf[0] = (byte)HidReportIds.EgsDeviceFirmwareUpdate;  // report id
                outBuf[2] = (byte)(seqNum & 0xFF); // lo seqnum
                outBuf[3] = (byte)(seqNum >> 8 & 0xFF);  // hi seqnum
                outBuf[4] = 0x00;  // format version=0
                outBuf[5] = 0x00;  // image type= bootimage
                outBuf[6] = 0x01;  // encrypt mode=1, aes
                outBuf[7] = 0x00;  // reserved
                int checksum = 0;
                for (int i = 2; i < 64; i++) { checksum += outBuf[i]; }
                outBuf[1] = (byte)(checksum & 0xFF); // checksum
                Device.SetHidFeatureReport(outBuf);
            }
            catch
            {
                throw;
            }
            UpdateFirmwareUpdateProtocolRevisionInDevice();
            switch (FirmwareUpdateProtocolRevisionInDevice)
            {
                case FirmwareUpdateProtocolRevisionKinds.Unknown:
                case FirmwareUpdateProtocolRevisionKinds.KickStarterFirstRelease:
                    FirmwareImageFilePathList = new List<string>()
                    {
                        @".\Resources\FirmwareImages\fwupdater.bin",
                        @".\Resources\FirmwareImages\piemonteApp1.bin"
                    };
                    break;
                case FirmwareUpdateProtocolRevisionKinds.FileUpdater:
                case FirmwareUpdateProtocolRevisionKinds.Latest:
                    FirmwareImageFilePathList = new List<string>()
                    {
#if DEBUG
                        @".\Resources\FirmwareImages\fwupdater.bin",
#else
                        @".\Resources\FirmwareImages\piemonteApp1.bin"
#endif
                    };
                    break;
                default:
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new InvalidOperationException("");
            }
        }

        public void StartAsync()
        {
            Trace.Assert(ProgressReport.IsBusy == false);
            Device.Settings.IsToMonitorTemperature = false;
            Device.IsUpdatingFirmware = true;
            IsBusy = true;
            IsCanceled = false;
            ProgressReport.RunWorkerAsync();
        }

        public void CancelAsync()
        {
            IsCanceled = true;
            ProgressReport.CancelAsync();
        }

        internal static void DoEvents()
        {
            var frame = new System.Windows.Threading.DispatcherFrame();
            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new System.Windows.Threading.DispatcherOperationCallback(ExitFrames), frame);
            System.Windows.Threading.Dispatcher.PushFrame(frame);
        }

        static object ExitFrames(object f)
        {
            ((System.Windows.Threading.DispatcherFrame)f).Continue = false;
            return null;
        }

        public void LetUserConnectDevice()
        {
            if (Device.IsHidDeviceConnected == false) { ExpectedUserAction = EgsDeviceFirmwareUpdateUserActions.ConnectDevice; }
            while (Device.IsHidDeviceConnected == false)
            {
                DoEvents();
                System.Threading.Thread.Sleep(500);
                if (IsCanceled) { throw new OperationCanceledException(); }
            }
            ExpectedUserAction = EgsDeviceFirmwareUpdateUserActions.DoNotDisconnectDevice;
        }

        public void LetUserRestartDeviceByReconnectingUsbConnector()
        {
            if (Device.IsHidDeviceConnected) { ExpectedUserAction = EgsDeviceFirmwareUpdateUserActions.DisconnectDevice; }
            while (Device.IsHidDeviceConnected)
            {
                DoEvents();
                System.Threading.Thread.Sleep(500);
                if (IsCanceled) { throw new OperationCanceledException(); }
            }
            LetUserConnectDevice();
        }

        EgsDeviceFirmwareUpdateResult DoWorkInternal()
        {
            try
            {
                if (FirmwareImageFilePathList == null) { throw new ArgumentNullException("binaryFilepathList"); }
                if (FirmwareImageFilePathList.Count == 0) { throw new ArgumentException("binaryFilepathList.Count == 0"); }

                CurrentIndexInFirmwareImageFilePathList = 0;
                LastStateReport = new EgsDeviceFirmwareUpdateStateReport() { Message = "File List:" + Environment.NewLine };
                foreach (var firmwareImageFilePath in FirmwareImageFilePathList) { LastStateReport.Message += "  " + firmwareImageFilePath + Environment.NewLine; }
                ProgressReport.ReportProgress(PercentProgress, LastStateReport);

                // NOTE: Rebooting device is unnecessary before staring DFU.  It is OK if the device is connected.
                LetUserConnectDevice();

                _CurrentIndexInFirmwareImageFilePathList = -1;
                foreach (var firmwareImageFilePath in FirmwareImageFilePathList)
                {
                    CurrentIndexInFirmwareImageFilePathList++;
                    // NOTE: If it is not set to 0, ProgressReport become increase unexpectedly.
                    PercentProgressInOneFile = 0;
                    LastStateReport = new EgsDeviceFirmwareUpdateStateReport() { Message = "Sending image file path: " + firmwareImageFilePath + Environment.NewLine };
                    ProgressReport.ReportProgress(PercentProgress, LastStateReport);

                    if (File.Exists(firmwareImageFilePath) == false)
                    {
                        throw new FileNotFoundException(Resources.EgsDeviceFirmwareUpdateModel_DfuImageFileIsInvalid, firmwareImageFilePath);
                    }
                    ImageFile = new EgsDeviceFirmwareUpdateImageFileModel(firmwareImageFilePath);
                    if (ImageFile == null || ImageFile.LoadedImageAsByteArray == null)
                    {
                        throw new FileFormatException(Resources.EgsDeviceFirmwareUpdateModel_DfuImageFileIsInvalid);
                    }

                    bool hasOneFileCompleted = false;
                    while (hasOneFileCompleted == false)
                    {
                        try
                        {
                            PercentProgressInOneFile = 0;
                            var startDateTime = DateTime.Now;
                            LastStateReport = new EgsDeviceFirmwareUpdateStateReport() { Message = "Start Time: " + startDateTime.ToString() + Environment.NewLine };
                            ProgressReport.ReportProgress(PercentProgress, LastStateReport);

                            SendFirmwareUpdateImage(ImageFile.LoadedImageAsByteArray, ImageFile.PayloadOffset, ImageFile.PayloadLength, ImageFile.CrcWord);

                            var endDateTime = DateTime.Now;
                            LastStateReport = new EgsDeviceFirmwareUpdateStateReport() { Message = "End Time: " + DateTime.Now.ToString() + "  Elapsed: " + (endDateTime - startDateTime).ToString() + Environment.NewLine };
                            ProgressReport.ReportProgress(PercentProgress, LastStateReport);

                            hasOneFileCompleted = true;
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            var msg = Resources.EgsDeviceFirmwareUpdateModel_DfuFailed + Environment.NewLine;
                            msg += Resources.EgsDeviceFirmwareUpdateModel_AppWillRetrySendingImageAgain + Environment.NewLine;
                            LastStateReport = new EgsDeviceFirmwareUpdateStateReport() { Message = msg, UserNotificationMessage = msg };
                            LastStateReport.MessageForDebug = ex.Message;
                            ProgressReport.ReportProgress(PercentProgress, LastStateReport);
                        }
                        // NOTE: Whether it success or fails, reboot is necessary.
                        LetUserRestartDeviceByReconnectingUsbConnector();
                    }
                }

                // succeeded
                LastStateReport = new EgsDeviceFirmwareUpdateStateReport();
                LastStateReport.Message = Resources.EgsDeviceFirmwareUpdateModel_DfuCompleted + Environment.NewLine;
                LastStateReport.UserNotificationMessage = Resources.EgsDeviceFirmwareUpdateModel_DfuCompleted + Environment.NewLine;
                LastStateReport.MessageForDebug = Resources.EgsDeviceFirmwareUpdateModel_DfuCompleted;
                ProgressReport.ReportProgress(100, LastStateReport);
                MessageText = LastStateReport.Message;
                return new EgsDeviceFirmwareUpdateResult(false, false, true, Resources.EgsDeviceFirmwareUpdateModel_DfuCompleted);
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine(ex.Message);
                LastStateReport = new EgsDeviceFirmwareUpdateStateReport();
                LastStateReport.Message = Resources.EgsDeviceFirmwareUpdateModel_DfuCanceled + Environment.NewLine + Resources.EgsDeviceFirmwareUpdateModel_RestartDfuFromBeginning + Environment.NewLine;
                LastStateReport.UserNotificationMessage = LastStateReport.Message;
                LastStateReport.MessageForDebug = ex.Message;
                ProgressReport.ReportProgress(PercentProgress, LastStateReport);
                MessageText = LastStateReport.Message;
                return new EgsDeviceFirmwareUpdateResult(false, true, false, Resources.EgsDeviceFirmwareUpdateModel_DfuCanceled);
            }
        }

        void SendFirmwareUpdateImage(byte[] fwUpdateImageFileBuffer, uint payloadOffset, uint payloadLength, uint crcWord)
        {
            int fwUpdateImageFileSize = fwUpdateImageFileBuffer.Length;
            uint packetSize = 64;
            //byte[] fwUpdateImageFileBuffer = new byte[fwUpdateImageFileSize];
            uint bufferIndex;
            uint i;
            int sentNormalPacketCount;//= 5000;
            uint packetSequenceNum;
            byte[] inBuf = new byte[packetSize];
            byte[] outBuf = new byte[packetSize];
            int nakSeqNum;

            //prepare trace to output file
            if (false)
            {
                Trace.Listeners.Add(new TextWriterTraceListener("FirmwareUpdateLog.txt"));
                Trace.AutoFlush = true;
            }

            try
            {
                // send start packet            
                CreateStartPacket(outBuf, payloadLength, crcWord, packetSize);
                Device.SetHidFeatureReport(outBuf);
            }
            catch
            {
                throw;
            }

            Debug.WriteLine("OK! start packet!!!");
            UpdateFirmwareUpdateProtocolRevisionInDevice();

            LastStateReport = new EgsDeviceFirmwareUpdateStateReport();
            LastStateReport.Message = "Start sending boot image to device" + Environment.NewLine;
            LastStateReport.Message += "Current device firmware update protocol revision: " + FirmwareUpdateProtocolRevisionInDevice + Environment.NewLine;
            LastStateReport.Message += "Writing image file firmware update protocol revision: " + ImageFile.ProtocolRevision + Environment.NewLine;
            LastStateReport.Message += "Current device firmware revision: " + FirmwareVersionInDevice + Environment.NewLine;
            LastStateReport.Message += "Writing image file firmware revision: " + ImageFile.FirmwareVersion + Environment.NewLine;
            ProgressReport.ReportProgress(PercentProgress, LastStateReport);

            //-- check firmware version to update--
            if (ImageFile.ProtocolRevision <= FirmwareUpdateProtocolRevisionInDevice)
            {
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Warning!! Image file protocol version= {0} is equal or older than device firmware protocol version= {1}", ImageFile.ProtocolRevision, FirmwareUpdateProtocolRevisionInDevice));
                if (IsToShowWarningMessages)
                {
                    // TODO: use Resources
                    MessageBoxResult mbr = MessageBox.Show("Image file version is equal or older than device firmware version." + Environment.NewLine + "Are you sure to continue?", "[WARNING] Trying to Upload Older Image to Device", MessageBoxButton.YesNo);
                    if (mbr == MessageBoxResult.No)
                    {
                        throw new OperationCanceledException();
                    }
                }
            }

            //-- send normal packet --
            bufferIndex = 0;
            sentNormalPacketCount = 0;
            packetSequenceNum = 1;
            uint loopCounter = 0;
            uint errorCount = 0;
            uint totalByteToSend = (uint)fwUpdateImageFileSize - payloadOffset;

            while ((payloadOffset + bufferIndex) < (fwUpdateImageFileSize - 1))
            {
                // NOTE: This is the long loop!
                if (ProgressReport.CancellationPending) { throw new OperationCanceledException(); }

                //--copy 64-4 bytes--
                for (i = 0; i < (packetSize - 4); i++)
                {
                    uint copyIndex = payloadOffset + bufferIndex + i;
                    if (copyIndex < fwUpdateImageFileSize)
                    {
                        inBuf[i] = fwUpdateImageFileBuffer[copyIndex];
                    }
                    else
                    {
                        //fill the rest with 0
                        inBuf[i] = 0;
                    }
                }

                // Deleted (2016/7/28)
                if (false)
                {
                    // NOTE: Added (2016/04/12)
                    Thread.Sleep(20);
                }

                sentNormalPacketCount = (int)packetSequenceNum;
                CreateFeaturePacket(packetSequenceNum, inBuf, outBuf, packetSize);

                {
                    // If it fails once, it continues to fail after Sleep(100) or waiting with debugger breaking.  Handle is Valid, so DevicePath may be correct.  SetFeature itself returns fail.
                    // Mr.T said, "Before the USB library in the device firmware has a stall problem, so retry was meaningful.  But now the bug is fixed so it may have no relation.".
                    var trialCountMax = 1; // 10;
                    bool hr = false;
                    string errorMessage = "";
                    for (int trialCount = 0; trialCount < trialCountMax; trialCount++)
                    {
                        try
                        {
                            Device.SetHidFeatureReport(outBuf);
                            hr = true;
                        }
                        catch (Exception ex)
                        {
                            if (false) { Debugger.Break(); }
                            errorMessage = ex.Message;
                            Debug.WriteLine(errorMessage);
                            Thread.Sleep(100);
                        }
                        if (hr) { break; }
                    }
                    if (hr == false) { throw new InvalidOperationException(errorMessage); }
                }


                PercentProgressInOneFile = RatioOfSendingImageFileInUpdatingByOneFile * (int)(bufferIndex + 1) / (int)totalByteToSend;
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Sent OK[{4:N2}%]: seqNum= 0x{0:X} bufferIndex= {1} sentPktCnt= 0x{2:X} totalRetransmitCnt={3}", packetSequenceNum, bufferIndex, sentNormalPacketCount, errorCount, PercentProgress));


                //-- get nak seq every 100 packets --
                loopCounter++;
                if ((loopCounter % 100) == 0)
                {
                    nakSeqNum = GetResponseFromDevice();

                    if (nakSeqNum > 0)
                    {
                        // resend NAK packet
                        errorCount++;
                        var report = new EgsDeviceFirmwareUpdateStateReport();
                        report.MessageForDebug = string.Format(CultureInfo.InvariantCulture, "Resend packet with seqNum= 0x{0:X}", nakSeqNum);
                        ProgressReport.ReportProgress(PercentProgress, report);
                        packetSequenceNum = (uint)nakSeqNum;
                    }
                    else
                    {
                        // not nak message, just skip
                        packetSequenceNum++;
                    }
                }
                else
                {
                    packetSequenceNum++;
                }

                // update bufferIndex
                if ((packetSequenceNum - 1) >= 0)
                {
                    bufferIndex = (packetSequenceNum - 1) * (packetSize - 4);
                }
                else
                {
                    bufferIndex = 0;
                }
            }


            // send end packet, commandID= 1(transfer complete)
            CreateEndPacket(outBuf, sentNormalPacketCount, 64, 1);
            try { Device.SetHidFeatureReport(outBuf); }
            catch (Exception ex) { throw new InvalidOperationException("Failed to send end packet.  Error Message: " + ex.Message); }


            // wait device to complete received buffer error checking
            if (WaitUpdateFinishResponse(1, 10000) == false) { throw new InvalidOperationException("No response from device."); }
            Debug.WriteLine("Finish transferring all image data to device successfully.");
            LastStateReport = new EgsDeviceFirmwareUpdateStateReport();


            // TODO: use Resources
            LastStateReport.Message = "Complete transferring boot image to device." + Environment.NewLine + "Now start writing to device flash memory." + Environment.NewLine;
            ProgressReport.ReportProgress(PercentProgress, LastStateReport);
            Debug.WriteLine("TURN OFF device power after click OK prior to flash writing completion, will damage device!");



            Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Sending start write to flash command to device."));
            // send end packet, commandID= 2(start write to flash)
            CreateEndPacket(outBuf, sentNormalPacketCount, 64, 2);


            try { Device.SetHidFeatureReport(outBuf); }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new InvalidOperationException("Sorry, but failed to send end packet.");
            }


            // wait device to complete write to flash
            if (WaitUpdateFinishResponse(2, 60000) == false) { throw new InvalidOperationException("No response from device."); }
        }

        void CreateFeaturePacket(uint seqNum, byte[] inBuf, byte[] outBuf, uint pktLen)
        {
            outBuf[0] = (byte)HidReportIds.EgsDeviceFirmwareUpdate;  // report id
            outBuf[2] = (byte)(seqNum & 0xFF); // lo seqnum
            outBuf[3] = (byte)(seqNum >> 8 & 0xFF);  // hi seqnum

            // copy inBuf to outBuf
            for (int i = 0; i < (pktLen - 4); i++)
            {
                outBuf[4 + i] = (byte)inBuf[i];
            }

            int checksum = 0;
            for (int i = 2; i < pktLen; i++)
            {
                checksum += outBuf[i];
            }
            outBuf[1] = (byte)(checksum & 0xFF); // checksum

            if (false)
            {
                for (int i = 0; i < 256; i++)
                {
                    Debug.WriteLine("outBuf" + i.ToString() + "= " + outBuf[i].ToString());
                }
            }
        }

        void CreateStartPacket(byte[] outBuf, uint payloadLen, uint crcWord, uint oneFeaturePacketLength)
        {
            byte[] inBuf = new byte[64];

            //start ctrl packet
            inBuf[0] = 0x00;  // format version=0
            inBuf[1] = 0x00;  // image type= bootimage
            inBuf[2] = 0x01;  // encrypt mode=1, aes
            inBuf[3] = 0x00;  // reserved
            inBuf[4] = (byte)(payloadLen & 0xFF);          // payloadlen0
            inBuf[5] = (byte)((payloadLen >> 8) & 0xFF);   // payloadlen1
            inBuf[6] = (byte)((payloadLen >> 16) & 0xFF);   // payloadlen2
            inBuf[7] = (byte)((payloadLen >> 24) & 0xFF);   // payloadlen3
            inBuf[8] = (byte)(crcWord & 0xFF);          // crcWord0
            inBuf[9] = (byte)((crcWord >> 8) & 0xFF);   // crcWord1
            inBuf[10] = (byte)((crcWord >> 16) & 0xFF);   // crcWord2
            inBuf[11] = (byte)((crcWord >> 24) & 0xFF);   // crcWord3

            for (uint i = 12; i < (oneFeaturePacketLength - 4); i++)    //-4 for header
            {
                inBuf[i] = 0;
            }

            CreateFeaturePacket(0xFFFF, inBuf, outBuf, 64);

        }

        void CreateEndPacket(byte[] outBuf, int sentNormalPacketCount, int pktLen, byte endControlCommandID)
        {
            byte[] inBuf = new byte[64];

            //start ctrl packet
            inBuf[0] = 0x01;  // format version=1
            inBuf[1] = (byte)(sentNormalPacketCount & 0xFF);  // lo tx pkt cnt 
            inBuf[2] = (byte)(sentNormalPacketCount >> 8 & 0xFF);  // hi tx pkt cnt
            inBuf[3] = endControlCommandID;

            for (int i = 4; i < pktLen - 4; i++)    //-4 for header
            {
                inBuf[i] = 0;
            }

            CreateFeaturePacket(0xFFFE, inBuf, outBuf, 64);
        }

        bool WaitUpdateFinishResponse(int finishStatusId, int timeOutValueInMilliseconds)
        {
            int deviceFinishStatus;
            deviceFinishStatus = 0;
            var stopwatch = Stopwatch.StartNew();
            while (deviceFinishStatus != finishStatusId)
            {
                if (stopwatch.ElapsedMilliseconds > timeOutValueInMilliseconds)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "WaitUpdateFinishResponse: Timeout!!! No response from device after {0}ms", timeOutValueInMilliseconds));
                    return false;
                }
                deviceFinishStatus = GetDeviceFinishStatus();
                var newPercentProgressInOneFile = (int)((100 - RatioOfSendingImageFileInUpdatingByOneFile) * ((double)stopwatch.ElapsedMilliseconds / (double)timeOutValueInMilliseconds) + RatioOfSendingImageFileInUpdatingByOneFile);
                PercentProgressInOneFile = Math.Min(newPercentProgressInOneFile, 100);
                Thread.Sleep(100); //wait 100ms                  
            }
            if (deviceFinishStatus == 2) { PercentProgressInOneFile = 100; }
            return true;
        }

        int GetDeviceFinishStatus()
        {
            byte[] ackmsgbuffer = new byte[64];
            byte messageId = 0;
            byte statusFirstByte = 0;
            int i;

            //clear buffer before get
            for (i = 0; i < 64; i++)
            {
                ackmsgbuffer[i] = 0;
            }

            //read feature report
            ackmsgbuffer[0] = (byte)HidReportIds.EgsDeviceFirmwareUpdate;
            try
            {
                Device.GetHidFeatureReport(ackmsgbuffer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("WaitUpdateFinishResponse: error getfeature!!!");
                return -1;
            }

            messageId = ackmsgbuffer[1];
            statusFirstByte = ackmsgbuffer[2];

            if (messageId != 3)   //messageID=3;
            {
                if (false)
                {
                    Debug.WriteLine("WaitUpdateFinishResponse: messageID mismatched!!!");
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "WaitUpdateFinishResponse: messageID= {2} status= 0x{0:X}{1:X}", ackmsgbuffer[3], ackmsgbuffer[2], ackmsgbuffer[1]));
                }
                return -1;
            }

            return (int)statusFirstByte;
        }

        int GetResponseFromDevice()
        {
            byte[] ackmsgbuffer = new byte[64];
            int nakSequenceNumber;

            uint timeOutCount = 0;
            bool ret;
            uint firmwareVersion;

            ret = false;
            while ((ret == false) && (timeOutCount < 10))
            {
                //retry 10 times
                timeOutCount++;
                ackmsgbuffer[0] = (byte)HidReportIds.EgsDeviceFirmwareUpdate;

                try
                {
                    Device.GetHidFeatureReport(ackmsgbuffer);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "GetHidFeatureReport: seqNum= 0x{0:X}{1:X} ACK/NAK= {2}", ackmsgbuffer[3], ackmsgbuffer[2], ackmsgbuffer[1]));
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "GetNakSequenceNumber: error getfeature!!! timeOutCount= {0}", timeOutCount));
                    Thread.Sleep(100); //wait 100ms    
                    continue;
                }

                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "GetHidFeatureReport: seqNum= 0x{0:X}{1:X} ACK/NAK= {2}", ackmsgbuffer[3], ackmsgbuffer[2], ackmsgbuffer[1]));
                if (false)
                {
                    for (int i = 0; i < 64; i++) { Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}: {1:X}", i, ackmsgbuffer[i])); }
                }

                if (ackmsgbuffer[1] == 2)
                {
                    // NAK
                    nakSequenceNumber = (((int)ackmsgbuffer[2]) & 0xFF) | (((int)ackmsgbuffer[3] << 8) & 0xFF00);
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "GetResponseFromDevice: NAK seqNum= 0x{0:X}", nakSequenceNumber));
                    return nakSequenceNumber;
                }
                else if (ackmsgbuffer[1] == 1)
                {
                    // ACK message 
                    return -1;
                }
                else if (ackmsgbuffer[1] == 3)
                {
                    // status message
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "GetResponseFromDevice: status= {0}", ackmsgbuffer[2]));
                    return -1;
                }
                else if (ackmsgbuffer[1] == 4)
                {
                    // firmware version message
                    firmwareVersion = (uint)ackmsgbuffer[2] | ((((uint)ackmsgbuffer[3]) << 8) & 0xFF00) | ((((uint)ackmsgbuffer[4]) << 16) & 0xFF0000) | ((((uint)ackmsgbuffer[5]) << 24) & 0xFF00);
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "GetResponseFromDevice: firmwareVersion= {0}", firmwareVersion));
                    return -1;

                }

                else
                {
                    //unidentify messageID, set ret to false so we can retry get feature report
                    ret = false;
                    //timeOutCount = 0;
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "GetResponseFromDevice: unidentify messageID!!! timeOutCount= {0}", timeOutCount));
                    Thread.Sleep(100); //wait 100ms       
                }
            }
            // critical error
            return -2;
        }
    }
}
