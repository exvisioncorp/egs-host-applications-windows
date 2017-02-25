using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following set of attributes. Change these attribute values to modify the information associated with an assembly.
[assembly: AssemblyTitle("EgsDeviceControlCore")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Exvision Corporation")]
[assembly: AssemblyProduct("EGS Device Control Core")]
[assembly: AssemblyCopyright("The MIT License.  Copyright (C) 2015 Exvision Corporation")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible to COM components.  If you need to access a type in this assembly from COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("4ecc2a72-5138-405e-b63e-0c365161547a")]

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

#if DEBUG
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
#else
[assembly: InternalsVisibleTo("EgsHostAppCore,               PublicKey=00240000048000009400000006020000002400005253413100040000010001003fbaf90361cc3521105256fec77b237721692f51604583e0f15e8e91e4d6d2dcb35359c09eaca8576d9741379dc5756db079ea3960c09fb708c51f6a52c1a84fb7352302e8c8d43a68b363bd51bf865dba92c64b06502455248aaafcac4fa31e7dd0dd3c83d25ac77bc21512df6b9cc499bd595db97e58c7d868c62ab52a45c4")]
[assembly: InternalsVisibleTo("EgsHostAppCore.Test,          PublicKey=002400000480000094000000060200000024000052534131000400000100010073c9d91c5b8d6472c6b39ad6ed5e860429e5b5985eeb11eec38b9a0c677b90cd5207d487994e7196b97dd204a85511e866797860b904d977cdf076e65822ed3ca5efa070ac6bf38875dd2b58b86937ef302a73ef707a2170d39646761d256ecc86cd4f16cc6f940f75fa280a699259dd1757505ecdbbc9e11d988b3305913298")]
[assembly: InternalsVisibleTo("ZkooTutorial,                 PublicKey=00240000048000009400000006020000002400005253413100040000010001007bc3abf24182b4678d5f3100b54e7bc5ffbf62a726bc8cbfbeb39edc5990117fd42be13541e4e5141bc4374aa9bbaa75f3e8d2b33410fdbf202038b1a73b9c2197d293e585ed01532537ee7be0cbe7ceca1f7effcb306b05200f9d6da042fc7e4db993257330530f29b1316ac0065567c0d647d019ba94dde6a6a88fa60ac4c0")]
[assembly: InternalsVisibleTo("ZkooHostApp,                  PublicKey=0024000004800000140100000602000000240000525341310008000001000100c95036aaaef33488d831ca13647c63079f2bf2207bb6d0d9f7d826077726ca1005bd6e64f4becf48438adae921ba8c68d70e5108287263e6a456c507b41ba05f0c5e98336d43c6df57c5319e867292d4bd4146922bfb1d4208a8eefdbb7f66005efa5e327b4f1e6a5d83d9eb22b075980c73356b9006518dba2f39d0af2336a330ead210e2f9384e43017a4f3a1bee1a1e29ce75de5c5c34130533ef3b7b4511023c260d5b317d87be397ade8917adac0606f281d0e0488c33ae02a0e6b0774d86a795460dbfe90ec457cfdcadc7c5d9c6ac77df22ef976e08fbff2f4e8b3319be2d5cf9142891e7aa8d49ab823da0e1bf5bac8aa8a1a4f435e9ed7435b4dde8")]
[assembly: InternalsVisibleTo("EgsInternalHostAppExtensions, PublicKey=002400000480000094000000060200000024000052534131000400000100010073c9d91c5b8d6472c6b39ad6ed5e860429e5b5985eeb11eec38b9a0c677b90cd5207d487994e7196b97dd204a85511e866797860b904d977cdf076e65822ed3ca5efa070ac6bf38875dd2b58b86937ef302a73ef707a2170d39646761d256ecc86cd4f16cc6f940f75fa280a699259dd1757505ecdbbc9e11d988b3305913298")]
[assembly: InternalsVisibleTo("ZkooHostAppInternal,          PublicKey=0024000004800000140100000602000000240000525341310008000001000100c95036aaaef33488d831ca13647c63079f2bf2207bb6d0d9f7d826077726ca1005bd6e64f4becf48438adae921ba8c68d70e5108287263e6a456c507b41ba05f0c5e98336d43c6df57c5319e867292d4bd4146922bfb1d4208a8eefdbb7f66005efa5e327b4f1e6a5d83d9eb22b075980c73356b9006518dba2f39d0af2336a330ead210e2f9384e43017a4f3a1bee1a1e29ce75de5c5c34130533ef3b7b4511023c260d5b317d87be397ade8917adac0606f281d0e0488c33ae02a0e6b0774d86a795460dbfe90ec457cfdcadc7c5d9c6ac77df22ef976e08fbff2f4e8b3319be2d5cf9142891e7aa8d49ab823da0e1bf5bac8aa8a1a4f435e9ed7435b4dde8")]
[assembly: InternalsVisibleTo("EgsHost,                      PublicKey=0024000004800000140100000602000000240000525341310008000001000100c95036aaaef33488d831ca13647c63079f2bf2207bb6d0d9f7d826077726ca1005bd6e64f4becf48438adae921ba8c68d70e5108287263e6a456c507b41ba05f0c5e98336d43c6df57c5319e867292d4bd4146922bfb1d4208a8eefdbb7f66005efa5e327b4f1e6a5d83d9eb22b075980c73356b9006518dba2f39d0af2336a330ead210e2f9384e43017a4f3a1bee1a1e29ce75de5c5c34130533ef3b7b4511023c260d5b317d87be397ade8917adac0606f281d0e0488c33ae02a0e6b0774d86a795460dbfe90ec457cfdcadc7c5d9c6ac77df22ef976e08fbff2f4e8b3319be2d5cf9142891e7aa8d49ab823da0e1bf5bac8aa8a1a4f435e9ed7435b4dde8")]
[assembly: InternalsVisibleTo("PowerPointViewer,             PublicKey=0024000004800000140100000602000000240000525341310008000001000100c95036aaaef33488d831ca13647c63079f2bf2207bb6d0d9f7d826077726ca1005bd6e64f4becf48438adae921ba8c68d70e5108287263e6a456c507b41ba05f0c5e98336d43c6df57c5319e867292d4bd4146922bfb1d4208a8eefdbb7f66005efa5e327b4f1e6a5d83d9eb22b075980c73356b9006518dba2f39d0af2336a330ead210e2f9384e43017a4f3a1bee1a1e29ce75de5c5c34130533ef3b7b4511023c260d5b317d87be397ade8917adac0606f281d0e0488c33ae02a0e6b0774d86a795460dbfe90ec457cfdcadc7c5d9c6ac77df22ef976e08fbff2f4e8b3319be2d5cf9142891e7aa8d49ab823da0e1bf5bac8aa8a1a4f435e9ed7435b4dde8")]
[assembly: InternalsVisibleTo("ZkooDeviceFirmwareUpdate,     PublicKey=002400000480000094000000060200000024000052534131000400000100010073c9d91c5b8d6472c6b39ad6ed5e860429e5b5985eeb11eec38b9a0c677b90cd5207d487994e7196b97dd204a85511e866797860b904d977cdf076e65822ed3ca5efa070ac6bf38875dd2b58b86937ef302a73ef707a2170d39646761d256ecc86cd4f16cc6f940f75fa280a699259dd1757505ecdbbc9e11d988b3305913298")]
[assembly: InternalsVisibleTo("ZkooDeviceQualityAssurance,   PublicKey=002400000480000094000000060200000024000052534131000400000100010073c9d91c5b8d6472c6b39ad6ed5e860429e5b5985eeb11eec38b9a0c677b90cd5207d487994e7196b97dd204a85511e866797860b904d977cdf076e65822ed3ca5efa070ac6bf38875dd2b58b86937ef302a73ef707a2170d39646761d256ecc86cd4f16cc6f940f75fa280a699259dd1757505ecdbbc9e11d988b3305913298")]
[assembly: InternalsVisibleTo("ZkooHostServiceApp,           PublicKey=0024000004800000140100000602000000240000525341310008000001000100c95036aaaef33488d831ca13647c63079f2bf2207bb6d0d9f7d826077726ca1005bd6e64f4becf48438adae921ba8c68d70e5108287263e6a456c507b41ba05f0c5e98336d43c6df57c5319e867292d4bd4146922bfb1d4208a8eefdbb7f66005efa5e327b4f1e6a5d83d9eb22b075980c73356b9006518dba2f39d0af2336a330ead210e2f9384e43017a4f3a1bee1a1e29ce75de5c5c34130533ef3b7b4511023c260d5b317d87be397ade8917adac0606f281d0e0488c33ae02a0e6b0774d86a795460dbfe90ec457cfdcadc7c5d9c6ac77df22ef976e08fbff2f4e8b3319be2d5cf9142891e7aa8d49ab823da0e1bf5bac8aa8a1a4f435e9ed7435b4dde8")]
// NOTE: Relative path is available!  But if the (checked out/cloned) path is too deep, too long path can cause some errors!
[assembly: AssemblyKeyFile(@"..\..\..\..\..\..\..\Exvision\CodeSigning\EgsHostAppCoreKeyPairByExvision.snk")]
[assembly: AssemblyDelaySign(false)]
#endif
