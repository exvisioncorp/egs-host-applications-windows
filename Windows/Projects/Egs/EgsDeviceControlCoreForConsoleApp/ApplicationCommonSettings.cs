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
        /// <summary>This dll's major version and minor version as string.</summary>
        public const string HostAppCoreDllAssemblyVersionMajorMinorString = "0.9";
        /// <summary>This dll's major version, minor version and build number as string.</summary>
        public const string HostAppCoreDllAssemblyVersionMajorMinorBuildString = "0.9.8330";
        /// <summary>This dll's major version, minor version, build number and revision as string.</summary>
        public const string HostAppCoreDllAssemblyVersionMajorMinorBuildRevisionString = "0.9.8330.0";
        /// <summary>Host application's major version, minor version, build number and revision as string.</summary>
        public const string ZkooHostAppExeAssemblyVersionMajorMinorBuildRevisionString = "0.9.8330.0";
        /// <summary>Default CultureInfo.Name as string.</summary>
        public const string DefaultCultureInfoName = "en";
        /// <summary>Firmware version in an image file of this application.</summary>
        public const string FirmwareVersionInImageFileString = "1.1.8109.0";

        internal const bool IsDeveloperRelease = false;
        internal const bool IsInternalRelease = false;

        /// <summary>When you set this value to true, HostAppCore.dll shows "Developer" tab on SettingsWindow and output messages for debugging.</summary>
        public static bool IsDebugging { get; set; }
        internal static bool IsDebuggingInternal { get; set; }
        internal static bool IsToEmulateReportByActualMouseRawInputToDebugViews { get; set; }

        static ApplicationCommonSettings()
        {
            // NOTE: IsToEmulateReportByActualMouseRawInputToDebugViews shuold be used only in DEBUG configuration.
            // When I set this to true once, I could not understand the reason of that CameraView and GestureCursor do not appear.
            // Before the app interprets both "emulation by mouse" and "information from ZKOO" into HID reports,
            // but now the app interprets only "emulation by mouse", so it does not draw anything when ZKOO sends any reports.
#if DEBUG
            IsDebugging = true;
            IsDebuggingInternal = true;
            IsToEmulateReportByActualMouseRawInputToDebugViews = false;
#else
            IsDebugging = false;
            IsDebuggingInternal = false;
            IsToEmulateReportByActualMouseRawInputToDebugViews = false;
#endif
        }
    }
}
