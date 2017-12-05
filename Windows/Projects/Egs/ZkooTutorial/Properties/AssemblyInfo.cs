using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following set of attributes. Change these attribute values to modify the information associated with an assembly.
[assembly: AssemblyTitle("ZkooTutorial")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Exvision Corporation")]
[assembly: AssemblyProduct("ZKOO Tutorial")]
[assembly: AssemblyCopyright("The MIT License.  Copyright (C) 2015 Exvision Corporation")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible to COM components.  If you need to access a type in this assembly from COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("72D3C57A-195F-4550-ACD0-E422361A14AE")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(Egs.ApplicationCommonSettings.ZkooHostAppExeAssemblyVersionMajorMinorBuildRevisionString)]
[assembly: AssemblyFileVersion(Egs.ApplicationCommonSettings.ZkooHostAppExeAssemblyVersionMajorMinorBuildRevisionString)]

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]
[assembly: System.Windows.ThemeInfo(System.Windows.ResourceDictionaryLocation.None, System.Windows.ResourceDictionaryLocation.SourceAssembly)]

[assembly: InternalsVisibleTo("EgsHostAppCore.Test")]
[assembly: InternalsVisibleTo("ZkooHostApp")]
[assembly: InternalsVisibleTo("ZkooHostAppInternal")]
// NOTE: set "uiAccess="false"" in Projects/Egs/ZkooHostApp/Properties/app.manifest
