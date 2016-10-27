namespace Egs.Win32
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.InteropServices;

    internal static partial class NativeMethods
    {
        public const int DIGCF_DEFAULT = 0x1;
        public const int DIGCF_PRESENT = 0x2;
        public const int DIGCF_ALLCLASSES = 0x4;
        public const int DIGCF_PROFILE = 0x8;
        public const int DIGCF_DEVICEINTERFACE = 0x10;
        public static IntPtr InvalidHandleValue = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_DEVICE_INTERFACE_DATA
        {
            public uint cbSize;
            public Guid InterfaceClassGuid;
            public uint Flags;
            public IntPtr RESERVED;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public uint cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HIDD_ATTRIBUTES
        {
            public uint Size;
            public ushort VendorID;
            public ushort ProductID;
            public ushort VersionNumber;
        }

        [DllImport("hid.dll")]
        [return: MarshalAs(UnmanagedType.U1)]
        extern internal static bool HidD_GetAttributes(IntPtr HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

        [DllImport("hid.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U1)]
        extern internal static bool HidD_SetFeature(SafeFileHandle HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength);

        //  from hidpi.h
        //  Typedef enum defines a set of integer constants for HidP_Report_Type
        internal const short HidP_Input = 0;
        internal const short HidP_Output = 1;
        internal const short HidP_Feature = 2;

        internal struct HIDP_CAPS
        {
            internal short Usage;
            internal short UsagePage;
            internal short InputReportByteLength;
            internal short OutputReportByteLength;
            internal short FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            internal short[] Reserved;
            internal short NumberLinkCollectionNodes;
            internal short NumberInputButtonCaps;
            internal short NumberInputValueCaps;
            internal short NumberInputDataIndices;
            internal short NumberOutputButtonCaps;
            internal short NumberOutputValueCaps;
            internal short NumberOutputDataIndices;
            internal short NumberFeatureButtonCaps;
            internal short NumberFeatureValueCaps;
            internal short NumberFeatureDataIndices;

            internal int UsageDescriptionNumber { get { return UsagePage * 256 + Usage; } }
            internal string HidUsageDescription
            {
                get
                {
                    int usage = UsageDescriptionNumber;
                    if (usage == Convert.ToInt32(0X102)) { return "mouse"; }
                    if (usage == Convert.ToInt32(0X106)) { return "keyboard"; }
                    return "";
                }
            }
        }

        //  If IsRange is false, UsageMin is the Usage and UsageMax is unused.
        //  If IsStringRange is false, StringMin is the string index and StringMax is unused.
        //  If IsDesignatorRange is false, DesignatorMin is the designator index and DesignatorMax is unused.

        internal struct HidP_Value_Caps
        {
            internal short UsagePage;
            internal byte ReportID;
            internal int IsAlias;
            internal short BitField;
            internal short LinkCollection;
            internal short LinkUsage;
            internal short LinkUsagePage;
            internal int IsRange;
            internal int IsStringRange;
            internal int IsDesignatorRange;
            internal int IsAbsolute;
            internal int HasNull;
            internal byte Reserved;
            internal short BitSize;
            internal short ReportCount;
            internal short Reserved2;
            internal short Reserved3;
            internal short Reserved4;
            internal short Reserved5;
            internal short Reserved6;
            internal int LogicalMin;
            internal int LogicalMax;
            internal int PhysicalMin;
            internal int PhysicalMax;
            internal short UsageMin;
            internal short UsageMax;
            internal short StringMin;
            internal short StringMax;
            internal short DesignatorMin;
            internal short DesignatorMax;
            internal short DataIndexMin;
            internal short DataIndexMax;
        }

        /// <summary>
        /// Remove any Input reports waiting in the buffer.
        /// </summary>
        /// <param name="HidDeviceObject"> a handle to a device. </param>
        /// <returns>
        /// True on success, False on failure.
        /// </returns>
        [DllImport("hid.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        extern internal static bool HidD_FlushQueue(SafeFileHandle HidDeviceObject);

        [DllImport("hid.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        extern internal static bool HidD_FreePreparsedData(IntPtr PreparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        extern internal static bool HidD_GetAttributes(SafeFileHandle HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

        /// <summary>
        /// reads a Feature report from the device.
        /// </summary>
        /// <param name="HidDeviceObject"> the handle for learning about the device and exchanging Feature reports. </param>
        /// <param name="ReportBuffer"> contains the requested report.</param>
        /// <param name="ReportBufferLength"></param>
        [DllImport("hid.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        extern internal static bool HidD_GetFeature(SafeFileHandle HidDeviceObject, byte[] ReportBuffer, int ReportBufferLength);

        /// <summary>
        /// reads an Input report from the device using a control transfer.
        /// </summary>
        /// <param name="HidDeviceObject"> the handle for learning about the device and exchanging Feature reports. </param>
        /// <param name="ReportBuffer"> contains the requested report. </param>
        /// <param name="ReportBufferLength"></param>
        [DllImport("hid.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        extern internal static bool HidD_GetInputReport(SafeFileHandle HidDeviceObject, byte[] ReportBuffer, int ReportBufferLength);

        [DllImport("hid.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        extern internal static bool HidD_GetNumInputBuffers(SafeFileHandle HidDeviceObject, ref int NumberBuffers);

        [DllImport("hid.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        extern internal static bool HidD_GetPreparsedData(SafeFileHandle HidDeviceObject, ref IntPtr PreparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        extern internal static bool HidD_SetNumInputBuffers(SafeFileHandle HidDeviceObject, int NumberBuffers);

        /// <summary>
        /// Writes an Output report to the device using a control transfer.
        /// </summary>
        /// <param name="HidDeviceObject"> handle to the device.  </param>
        /// <param name="ReportBuffer"> contains the report ID and report data. </param>
        /// <param name="ReportBufferLength"></param>
        [DllImport("hid.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        extern internal static bool HidD_SetOutputReport(SafeFileHandle HidDeviceObject, byte[] ReportBuffer, int ReportBufferLength);

        [DllImport("hid.dll", SetLastError = true)]
        extern internal static int HidP_GetCaps(IntPtr PreparsedData, ref HIDP_CAPS Capabilities);

        [DllImport("hid.dll", SetLastError = true)]
        extern internal static int HidP_GetValueCaps(int ReportType, byte[] ValueCaps, ref int ValueCapsLength, IntPtr PreparsedData);
    }
}
