namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using Egs.PropertyTypes;
    using Egs.Win32;

    /// <summary>
    /// HID Report for OS.  Information defined in this class are contained inside EgsDeviceEgsGestureHidReport.
    /// </summary>
    public class EgsDeviceTouchScreenHidReport
    {
        internal EgsDevice Device { get; private set; }
        internal HidReportIds ReportId { get; private set; }
        /// <summary>
        /// In original Win32 implementation, not "contacts" but "contact" is used.
        /// </summary>
        public byte ContactCount { get; internal set; }
        public int ScanTime { get; internal set; }

        public IList<EgsDeviceTouchScreenHidReportContact> Contacts { get; internal set; }

        public event EventHandler ReportUpdated;
        protected virtual void OnReportUpdated(EventArgs e) { var t = ReportUpdated; if (t != null) { t(this, e); } }

        internal EgsDeviceTouchScreenHidReport()
        {
        }

        internal void InitializeOnceAtStartup(EgsDevice device)
        {
            if (device == null)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                throw new ArgumentNullException("device");
            }
            Device = device;
            Contacts = Enumerable.Range(0, Device.TrackableHandsCountMaximum).Select(e => new EgsDeviceTouchScreenHidReportContact()).ToList();
            ResetInternal();
        }

        void ResetInternal()
        {
            foreach (var contact in Contacts) { contact.Reset(); }
        }

        public void Reset()
        {
            ResetInternal();
            OnReportUpdated(EventArgs.Empty);
        }

        internal void UpdateByHidReportAsByteArray(byte[] hidReport)
        {
            // obsolete
            if (false) { UpdateByHidReportAsByteArray_v00(hidReport); }
            UpdateByHidReportAsByteArray_v01(hidReport);
            OnReportUpdated(EventArgs.Empty);
        }

        void UpdateByHidReportAsByteArray_v01(byte[] hidReport)
        {
            Trace.Assert(hidReport[0] == (byte)HidReportIds.TouchScreen);

            ReportId = (HidReportIds)hidReport[0];

            Contacts[0].ContactId = hidReport[1];
            Contacts[0].TipSwitch = ((hidReport[2] & 0x1) != 0x00) ? true : false;
            Contacts[0].InRange = ((hidReport[2] & 0x2) != 0x00) ? true : false;
            Contacts[0].Confidence = ((hidReport[2] & 0x4) != 0x00) ? true : false;
            Contacts[0].X = (int)hidReport[4] * 256 + hidReport[3];
            Contacts[0].Y = (int)hidReport[6] * 256 + hidReport[5];
            // TODO: MUSTDO: what is the correct way?
            Contacts[0].IsTracking = (hidReport[2] != 0);

            Contacts[1].ContactId = hidReport[7];
            Contacts[1].TipSwitch = ((hidReport[8] & 0x1) != 0x00) ? true : false;
            Contacts[1].InRange = ((hidReport[8] & 0x2) != 0x00) ? true : false;
            Contacts[1].Confidence = ((hidReport[8] & 0x4) != 0x00) ? true : false;
            Contacts[1].X = hidReport[10] * 256 + hidReport[9];
            Contacts[1].Y = hidReport[12] * 256 + hidReport[11];
            // TODO: MUSTDO: what is the correct way?
            Contacts[1].IsTracking = (hidReport[8] != 0);

            ContactCount = hidReport[13];
            ScanTime = BitConverter.ToInt32(hidReport, 14);
        }

        void UpdateByHidReportAsByteArray_v00(byte[] hidReport)
        {
            Trace.Assert(hidReport[0] == (byte)HidReportIds.TouchScreen);

            ReportId = (HidReportIds)hidReport[0];
            ContactCount = hidReport[21];
            ScanTime = BitConverter.ToInt32(hidReport, 22);

            if (ContactCount == 0)
            {
                Contacts[0].Reset();
                Contacts[1].Reset();
                return;
            }

            byte handId = hidReport[7];
            Contacts[handId].TipSwitch = ((hidReport[1] & 0x1) != 0x00) ? true : false;
            Contacts[handId].InRange = ((hidReport[1] & 0x2) != 0x00) ? true : false;
            Contacts[handId].Confidence = ((hidReport[1] & 0x4) != 0x00) ? true : false;
            Contacts[handId].ContactId = hidReport[2];
            Contacts[handId].X = hidReport[4] * 256 + hidReport[3];
            Contacts[handId].Y = hidReport[6] * 256 + hidReport[5];
            Contacts[handId].IsTracking = ((hidReport[8] & 0x1) == 0x00);

            if (ContactCount == 0x02)
            {
                handId = hidReport[17];
                Contacts[handId].TipSwitch = ((hidReport[11] & 0x1) != 0x00) ? true : false;
                Contacts[handId].InRange = ((hidReport[11] & 0x2) != 0x00) ? true : false;
                Contacts[handId].Confidence = ((hidReport[11] & 0x4) != 0x00) ? true : false;
                Contacts[handId].ContactId = hidReport[12];
                Contacts[handId].X = hidReport[14] * 256 + hidReport[13];
                Contacts[handId].Y = hidReport[16] * 256 + hidReport[15];
                Contacts[handId].IsTracking = ((hidReport[18] & 0x1) == 0x00);
            }
            else
            {
                Contacts[handId].Reset();
            }
        }

        internal void UpdateByRawMouse(ref NativeMethods.RAWMOUSE mouse)
        {
            // MUSTDO: remember the purpose of this method.
            // NOTE: mouse.lLastX and mouse.lLastY are different between mouse operation and ZKOO HID operation.
            // By input from EgsDevice,    mouse.usFlags == NativeMethods.RawMouseFlags.MOUSE_MOVE_ABSOLUTE. Absolute coordinate.
            // By input from normal mouse, mouse.usFlags == NativeMethods.RawMouseFlags.MOUSE_MOVE_RELATIVE. Relative coordinate, i.e. displacement from previous position.
            if (mouse.usFlags != NativeMethods.RawMouseFlags.MOUSE_MOVE_ABSOLUTE && mouse.usFlags != NativeMethods.RawMouseFlags.MOUSE_MOVE_RELATIVE)
            {
                Contacts[0].Reset();
                Contacts[1].Reset();
                OnReportUpdated(EventArgs.Empty);
                return;
            }

            Contacts[0].X = System.Windows.Forms.Cursor.Position.X;
            Contacts[0].Y = System.Windows.Forms.Cursor.Position.Y;
            var previousIsTouching = Contacts[0].TipSwitch;
            if (previousIsTouching == false && (mouse.buttons.usButtonFlags == NativeMethods.RawMouseButtonFlags.RI_MOUSE_LEFT_BUTTON_DOWN))
            {
                Contacts[0].TipSwitch = true;
            }
            else if (previousIsTouching == true && (mouse.buttons.usButtonFlags == NativeMethods.RawMouseButtonFlags.RI_MOUSE_LEFT_BUTTON_UP))
            {
                Contacts[0].TipSwitch = false;
            }
            // RAWMOUSE does not have information about tracking.  Basically mouse is always active, so it is always "IsTracking == true".
            Contacts[0].IsTracking = true;

            if (false)
            {
                Console.Write("mouse.lLastX={0}  ", mouse.lLastX);
                Console.Write("mouse.lLastY={0}  ", mouse.lLastY);
                Console.Write("Contacts[0].X={0}  ", Contacts[0].X);
                Console.Write("Contacts[0].Y={0}  ", Contacts[0].Y);
                Console.Write("Contacts[0].TipSwitch={0}  ", Contacts[0].TipSwitch);
                Console.WriteLine();
            }
            // 2nd point is not used.
            Contacts[1].Reset();
            OnReportUpdated(EventArgs.Empty);
        }
    }
}
