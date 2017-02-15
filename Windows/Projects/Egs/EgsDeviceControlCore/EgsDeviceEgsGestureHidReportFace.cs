namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using Egs.PropertyTypes;

    public class EgsDeviceEgsGestureHidReportFace
    {
        public bool IsDetected { get; internal set; }
        public bool IsSelected { get; internal set; }
        public System.Drawing.Rectangle Area { get; internal set; }
        public byte Score { get; internal set; }

        internal EgsDeviceEgsGestureHidReportFace()
        {
            Reset();
        }

        internal void Reset()
        {
            IsDetected = false;
            IsSelected = false;
            Area = new System.Drawing.Rectangle();
            Score = 0;
        }
    }
}
