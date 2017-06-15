namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Application common settings and constants for attributes defined in AssemblyInfo.cs in each project.
    /// </summary>
    public static class ApplicationCommonSettings
    {
        /// <summary>Seller name.</summary>
        public static string SellerName = "Exvision Corporation";
        /// <summary>Seller short name.</summary>
        public static string SellerShortName = "Exvision";
        /// <summary>Seller support navigate URI.</summary>
        public static string SellerSupportNavigateUriString = @"mailto:support@zkoocamera.com?subject=Gesture Camera Application Error Report&amp;";
        /// <summary>Host application name.</summary>
        public static string HostApplicationName = "Gesture Camera";

        /// <summary>This dll's major version and minor version as string.</summary>
        public const string HostAppCoreDllAssemblyVersionMajorMinorString = "0.9";
        /// <summary>This dll's major version, minor version and build number as string.</summary>
        public const string HostAppCoreDllAssemblyVersionMajorMinorBuildString = "0.9.9015";
        /// <summary>This dll's major version, minor version, build number and revision as string.</summary>
        public const string HostAppCoreDllAssemblyVersionMajorMinorBuildRevisionString = "0.9.9015.0";
        /// <summary>Host application's major version, minor version, build number and revision as string.</summary>
        public const string ZkooHostAppExeAssemblyVersionMajorMinorBuildRevisionString = "0.9.9015.0";
        /// <summary>Default CultureInfo.Name as string.</summary>
        public const string DefaultCultureInfoName = "en";
        /// <summary>Firmware version in an image file of this application.</summary>
        public const string FirmwareVersionInImageFileString = "1.1.8722.0";
        /// <summary>Firmware version of current developing device.</summary>
        internal const string DevelopingFirmwareVersionString = "1.1.9015.0";

        internal const bool IsDeveloperRelease = true;
        internal const bool IsInternalRelease = false;
		internal const bool CanChangeDeviceUsage = true;

        /// <summary>When you set this value to true, HostAppCore.dll shows "Developer" tab on SettingsWindow and output messages for debugging.</summary>
        public static bool IsDebugging { get; set; }
        internal static bool IsDebuggingInternal { get; set; }
        internal static bool IsToEmulateReportByActualMouseRawInputToDebugViews { get; set; }

        static ApplicationCommonSettings()
        {
            // NOTE: IsToEmulateReportByActualMouseRawInputToDebugViews shuold be used only in DEBUG configuration.
            // When I set this to true once, I could not understand the reason of that CameraView and GestureCursor do not appear.
            // Before the app interprets both "emulation by mouse" and "information from ZKOO" into HID reports,
            // but now the app interprets only "emulation by mouse", so it does not draw anything when EGS sends any reports.
#if DEBUG
            IsDebugging = true;
            IsDebuggingInternal = IsInternalRelease;
            IsToEmulateReportByActualMouseRawInputToDebugViews = false;
#else
            IsDebugging = false;
            IsDebuggingInternal = false;
            IsToEmulateReportByActualMouseRawInputToDebugViews = false;
#endif
        }
    }
}
