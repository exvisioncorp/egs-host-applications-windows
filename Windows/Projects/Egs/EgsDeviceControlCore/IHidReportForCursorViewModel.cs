namespace Egs
{
    using System;

    /// <summary>
    /// Implemented in EgsDeviceTouchScreenHidReport and EgsDeviceEgsGestureHidReport
    /// </summary>
    public interface IHidReportForCursorViewModel
    {
        /// <summary>
        /// The device is tracking the hand or not
        /// </summary>
        bool IsTracking { get; }
        /// <summary>
        /// Recognized hand's X position
        /// </summary>
        int X { get; }
        /// <summary>
        /// Recognized hand's Y position
        /// </summary>
        int Y { get; }
        /// <summary>
        /// Recognized hand is bended or not
        /// </summary>
        bool IsTouching { get; }
    }
}
