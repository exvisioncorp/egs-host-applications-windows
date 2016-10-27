namespace Egs.Win32
{
    using System;

    internal static partial class NativeMethods
    {
        // used for RegisterForDeviceNotificationsByUsbGuid
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff545972(v=vs.85).aspx
        internal static readonly Guid GUID_DEVINTERFACE_USB_DEVICE = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff545982(v=vs.85).aspx
        internal static readonly Guid GUID_DEVINTERFACE_USB_HUB = new Guid("F18A0E88-C30C-11D0-8815-00A0C906BED8");
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff548325(v=vs.85).aspx
        internal static readonly Guid KSCATEGORY_CAPTURE = new Guid("65e8773d-8f56-11d0-a3b9-00a0c9223196");
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff548536(v=vs.85).aspx
        internal static readonly Guid KSCATEGORY_VIDEO = new Guid("6994AD05-93EF-11D0-A3CC-00A0C9223196");
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff545860(v=vs.85).aspx
        internal static readonly Guid GUID_DEVINTERFACE_HID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff545881(v=vs.85).aspx
        internal static readonly Guid GUID_DEVINTERFACE_KEYBOARD = new Guid("884b96c3-56ef-11d1-bc8c-00a0c91405dd");
        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff545912(v=vs.85).aspx
        internal static readonly Guid GUID_DEVINTERFACE_MOUSE = new Guid("378DE44C-56EF-11D1-BC8C-00A0C91405DD");

        // Detection order is CAPTURE -> VIDEO.
        internal static readonly string DEVICE_NAME_OF_GUID_DEVINTERFACE_USB_DEVICE = @"\\?\USB#VID_2BCA&PID_A001#EXVA00100000#{a5dcbf10-6530-11d2-901f-00c04fb951ed}";
    }
}
