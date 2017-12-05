using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following set of attributes. Change these attribute values to modify the information associated with an assembly.
[assembly: AssemblyTitle("EgsDeviceControlCoreForConsoleApps")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Exvision Corporation")]
[assembly: AssemblyProduct("EGS Device Control Core for Console Applications")]
[assembly: AssemblyCopyright("The MIT License.  Copyright (C) 2015 Exvision Corporation")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible to COM components.  If you need to access a type in this assembly from COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("858B2F0D-C257-4805-84EA-4F1C2F612005")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(Egs.ApplicationCommonSettings.HostAppCoreDllAssemblyVersionMajorMinorBuildRevisionString)]
[assembly: AssemblyFileVersion(Egs.ApplicationCommonSettings.HostAppCoreDllAssemblyVersionMajorMinorBuildRevisionString)]

[assembly: InternalsVisibleTo("EgsHostAppCore")]
[assembly: InternalsVisibleTo("EgsHostAppCore.Test")]
[assembly: InternalsVisibleTo("ZkooTutorial")]
[assembly: InternalsVisibleTo("ZkooHostApp")]
[assembly: InternalsVisibleTo("EgsInternalHostAppExtensions")]
[assembly: InternalsVisibleTo("ZkooHostAppInternal")]
[assembly: InternalsVisibleTo("EgsHost")]
[assembly: InternalsVisibleTo("PowerPointViewer")]
[assembly: InternalsVisibleTo("ZkooDeviceFirmwareUpdate")]
[assembly: InternalsVisibleTo("ZkooDeviceQualityAssurance")]
[assembly: InternalsVisibleTo("ZkooHostServiceApp")]
// NOTE: set "uiAccess="false"" in Projects/Egs/ZkooHostApp/Properties/app.manifest
