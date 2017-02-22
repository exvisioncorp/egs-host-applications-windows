EGS Host Applications for Windows
==========

## Summary
This repository contains the source code and assets of the host application of "ZKOO" device (<http://zkoocamera.com/>).  Users can obtain the official signed binary files as an installer file from our official web site (<http://zkoocamera.com/setup/>).
"EGS" stands for Exvision Gesture System, and ZKOO was developed as a type of EGS device.  The entire face and hand gesture recognition process is run within the ZKOO device, and the source code is proprietary, which means they are not contained in this repository.
The ZKOO device sends recognition results to the host device in the form of HID reports, the same method as a multi-touch touch panel or mouse.  It can also send captured images in the same manner as a webcam.  The host application receives the information to draw "Gesture Cursors" and show the "Camera View" image.
In this OSS version, assemblies are not strictly signed.  Thus, Gesture Cursors and the Camera View cannot appear on the Windows Start Screen.  Only our officially signed version is capable of doing this.

## Contents
* EgsSourceCodeGeneration.exe - It generates source codes and .resx files from excel sheets in "Documents" folder, based on settings in Windows/Projects/Egs/DeviceControlCore/VersionAndCultureInfo.t4 (<https://github.com/exvisioncorp/egs-host-applications-windows/blob/master/Windows/Projects/Egs/EgsDeviceControlCore/VersionAndCultureInfo.t4>).  Built on .NET 3.5.
* EgsDeviceControlCore.dll - It monitors device connection, sets settings, gets hand position and open/bent status, draws "Gesture Cursors" on desktop and captures camera images.  Built on .NET 3.5.
* EgsHostAppCore.dll - It shows "Settings" and "Camera View" windows, updates device firmware and host application.  Built on .NET 4.5.2.
* ZkooTutorial.dll - A tutorial application showing how to use ZKOO device.  Built on .NET 4.5.2.
* ZkooHostApp.exe - This is the ZKOO host application.  It composites the above dlls, saves and loads the entire application settings.  Built on .NET 4.5.2.
* EGS SDK - Lib folder contains our official signed dll binaries.  Examples folder has example programs which use the dll files.
* Documents/Test - Before we release our binaries, we perform tests based on the Documents/Test/WindowsHostTestScenario_en.xlsx (<https://github.com/exvisioncorp/egs-host-applications-windows/blob/master/Documents/Test/WindowsHostTestScenario_en.xlsx>).
* Assets - Files will be copied to folders in the ZkooTutorial project.

## Requirement
Windows 7 or above.
.NET 3.5 and .NET 4.5.2 are required.  Our official installer (<http://zkoocamera.com/setup/>) helps with their installation.
When you use this SDK, please check "prefer 32-bit" in your EXE projects.

## Usage
Open Windows/EgsHostAppsOnWindows.sln (<https://github.com/exvisioncorp/egs-host-applications-windows/blob/master/Windows/EgsHostAppsOnWindows.sln>) and build it.

## Author
Exvision Corporation (<http://exvision.co.jp/en>)

## License
The MIT License (<https://github.com/exvisioncorp/egs-host-applications-windows/blob/master/LICENSE.md>)

## Third Party Libraries
* AForge.NET (<http://www.aforgenet.com/framework/license.html>)
 **[NOTE]** Exvision Corporation bought and uses "none LGPL version of the AForge.NET", and the library's licenser allows us to redistribute them as "LGPL version".  The dll files are the same binaries as those you can download from NuGet etc.  When you use this EgsSDK, please be aware of LGPL components inside.  You need to obtain a license of "none LGPL version of the AForge.NET" for development of each none LGPL product that use EgsSDK.
* JSON.NET (MIT) (<https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md>)
* Dlib <http://dlib.net/> (Boost Software License) (<https://github.com/davisking/dlib/blob/master/dlib/LICENSE.txt>)
* OpenCvSharp (3-clause BSD) (<https://github.com/shimat/opencvsharp/blob/master/LICENSE.txt>)
* NAudio, NVorbis, NAudio.Vorbis (Ms-PL) (<https://github.com/naudio/NAudio/blob/master/license.txt>)
* "USB COMPLETE" by Jan Axelson's Lakeview Research (<http://janaxelson.com/hidpage.htm>)
* Some UI code in ZkooTutorial (CPOL) (<http://www.codeproject.com/info/cpol10.aspx>)
