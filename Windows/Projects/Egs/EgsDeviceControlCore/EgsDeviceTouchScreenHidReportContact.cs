namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// HID Report for Windows OS.  Device Driver of Touch Screen is used.
    /// </summary>
    public class EgsDeviceTouchScreenHidReportContact : IHidReportForCursorViewModel
    {
        // Refer to: Supporting Usages in Touch Digitizer Drivers (Windows 7)
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff553737(v=vs.85).aspx

        /// <summary>
        /// Specifies the identifier of the current contact. An identifier must remain constant while the contact is detected by the device. Each separate concurrent contact must have a unique identifier. Identifiers can be reused if a contact is no longer detected. If the device supports "in-air" packets (the contact is hovering above the surface), the identifier must persist from the time that the contact is detected until the time that it goes out of range. In the report descriptor in the EloMT sample, the comment for this usage is "Temp Identifier."
        /// </summary>
        public byte ContactId { get; internal set; }

        /// <summary>
        /// EgsDevice sends 0 (not touched) for an opened hand, 1 (touched) for a bended hand.  
        /// "Use the tip switch to indicate finger contact and liftoff from the digitizer surface, similar to how a pen reports contact with the digitizer."
        /// </summary>
        internal bool TipSwitch { get; set; }

        /// <summary>
        /// EgsDevice sends 0 (far enough from touch panel) for an opened hand, 1 (touched or close enough) for a bended hand.
        /// "If the device supports z-axis detection, it reports in-range when the transducer is within the region where digitizing is possible. If the device does not support z-axis detection, the driver should set in-range and tip switch when a finger comes in contact with the digitizer."
        /// </summary>
        internal bool InRange { get; set; }

        /// <summary>
        /// EgsDevice always sends 1.
        /// Suggestion from the device about whether the touch contact was an intended or accidental touch. Your device should reject accidental touches as thoroughly as it can and report that information by using the confidence usage. The operating system uses confidence to help improve accidental touch rejection.
        /// </summary>
        internal bool Confidence { get; set; }

        public int X { get; internal set; }
        public int Y { get; internal set; }
        public bool IsTracking { get; internal set; }

        public bool IsTouching { get { return TipSwitch; } }

        internal EgsDeviceTouchScreenHidReportContact()
        {
            Reset();
        }

        internal void Reset()
        {
            ContactId = 0;
            TipSwitch = false;
            InRange = false;
            Confidence = false;
            X = short.MinValue;
            Y = short.MinValue;
            IsTracking = false;
        }
    }
}
