EGS Host Applications for Windows
==========

## Summary
This repository has the source code and assets of host applications of "ZKOO" device (<http://zkoocamera.com/>).  Users can get official signed binary files as an installer file from our official web site (<http://zkoocamera.com/setup/>).
"EGS" means Exvision Gesture System, and ZKOO was developed as a kind of the EGS devices.  The whole face and hand gesture recognition process is running in the ZKOO device, and the source code is proprietary, so they are not contained in this repository.
ZKOO device sends recognition result to a host device as HID reports, same as a multi-touch touch panel or a mouse.  And it can also send captured images like web cam.  The host application receives the information, and draw "Gesture Cursors" and show "Camera View" image.
In this OSS version, assemblies are not strictly signed, so Gesture Cursors and Camera View cannot appear on Windows Start Screen.  Our official signed version can do that.

## Contents
* EgsSourceCodeGenerator.exe - It generates some source codes and .resx files from some excel sheets in "Documents" folder, based on settings written in Windows/Projects/Egs/DeviceControlCore/VersionAndCultureInfo.t4 (<https://github.com/exvisioncorp/egs-host-applications-windows/blob/master/Windows/Projects/Egs/EgsDeviceControlCore/VersionAndCultureInfo.t4>).
* EgsDeviceControlCore.dll - It monitors device connection, sets settings, gets hand position and opened/bended status, draw "Gesture Cursors" on desktop and captures camera images.  It is built on .NET 3.5.
* EgsHostAppCore.dll - It shows "Settings" window and "Camera View" window, updates device firmware and host application itself.  It is built on .NET 4.5.2.
* ZkooTutorial.dll - A tutorial application of usage of ZKOO device.  It is build on .NET 4.5.2.
* ZkooHostApp.exe - It is the ZKOO host application.  It composites the above dlls, and save and load the whole application settings.  It is build on .NET 4.5.2.
* EGS SDK - SDK of EGS host applications.  Lib folder contains our official signed dll binaries.  Examples folder has some example programs which use the dll files.
* Documents/Test - Before we release our binaries, we do tests based on the Documents/Test/WindowsHostTestScenario_en.xlsx (<https://github.com/exvisioncorp/egs-host-applications-windows/blob/master/Documents/Test/WindowsHostTestScenario_en.xlsx>).

## Requirement
Windows 7 or above.
.NET 3.5 and .NET 4.5.2 are required.  Our official installer (<http://zkoocamera.com/setup/>) helps installation of them.

## Usage
Open Windows/EgsHostAppsOnWindows.sln (<https://github.com/exvisioncorp/egs-host-applications-windows/blob/master/Windows/EgsHostAppsOnWindows.sln>) and build it.

## Author
Exvision Corporation (<http://exvision.co.jp/en>)

## License
The MIT License (<https://github.com/exvisioncorp/egs-host-applications-windows/blob/master/LICENSE.md>)

## Third Party Libraries
* AForge.NET (<http://www.aforgenet.com/framework/license.html>)
 **[NOTE]** Exvision Corporation bought and uses "none LGPL version of the AForge.NET", and the library's licenser allows us to redistribute them as "LGPL version".  The dll files are the same binaries which you can download from NuGet etc.  When you use this EgsSDK, please be aware of LGPL components inside.  You need to get a license of "none LGPL version of the AForge.NET" for development of each none LGPL product which uses EgsSDK.
* JSON.NET (MIT) (<https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md>)
* NAudio, NVorbis, NAudio.Vorbis (Ms-PL) (<https://github.com/naudio/NAudio/blob/master/license.txt>)
* "USB COMPLETE" by Jan Axelson's Lakeview Research (<http://janaxelson.com/hidpage.htm>)
* Some UI code in ZkooTutorial (CPOL) (<http://www.codeproject.com/info/cpol10.aspx>)
