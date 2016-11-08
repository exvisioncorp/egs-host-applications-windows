namespace Egs.PropertyTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;
    using System.Windows;
    using Egs.DotNetUtility;

    internal class HostToDeviceCommandFeatureReport
    {
        internal const int ByteArrayDataLength = 64;
        internal byte[] ByteArrayData { get; set; }

        internal HidReportIds ReportIdAsHidReportKind
        {
            get { return (HidReportIds)ByteArrayData[0]; }
            set { ByteArrayData[0] = (byte)value; }
        }
        internal byte ReportId
        {
            get { return ByteArrayData[0]; }
            set { ByteArrayData[0] = value; }
        }
        internal byte MessageId
        {
            get { return ByteArrayData[1]; }
            set { ByteArrayData[1] = value; }
        }
        internal byte CategoryId
        {
            get { return ByteArrayData[2]; }
            set { ByteArrayData[2] = value; }
        }
        internal byte PropertyId
        {
            get { return ByteArrayData[3]; }
            set { ByteArrayData[3] = value; }
        }

        internal HostToDeviceCommandFeatureReport()
        {
            ByteArrayData = new byte[ByteArrayDataLength];
            ReportIdAsHidReportKind = HidReportIds.EgsDeviceSettings;
        }

        internal static HostToDeviceCommandFeatureReport SaveSettingsToFlashCommandFeatureReport
        {
            get { return new HostToDeviceCommandFeatureReport() { ReportId = 0x0B, MessageId = 0x20, CategoryId = 0x00, PropertyId = 0x00 }; }
        }

        internal static HostToDeviceCommandFeatureReport ResetDeviceCommandFeatureReport
        {
            get { return new HostToDeviceCommandFeatureReport() { ReportId = 0x0B, MessageId = 0x20, CategoryId = 0x00, PropertyId = 0x10 }; }
        }
    }
}
