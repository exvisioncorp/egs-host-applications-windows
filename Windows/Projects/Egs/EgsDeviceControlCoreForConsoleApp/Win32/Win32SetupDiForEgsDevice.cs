namespace Egs.Win32
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class NativeMethods
    {
        [DllImport("setupapi.dll", SetLastError = true)]
        extern internal static IntPtr SetupDiCreateDeviceInfoList(ref Guid ClassGuid, IntPtr hwndParent);

        /// <summary>
        /// Frees the memory reserved for the DeviceInfoSet returned by SetupDiGetClassDevs.
        /// </summary>
        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool SetupDiEnumDeviceInterfaces(
            IntPtr DeviceInfoSet,
            IntPtr DeviceInfoData,
            ref Guid InterfaceClassGuid,
            int MemberIndex,
            ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        extern internal static IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);

        // for getting detail information
        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool SetupDiGetDeviceInterfaceDetail(
            IntPtr hDevInfo,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
            UInt32 deviceInterfaceDetailDataSize,
            out UInt32 requiredSize,
            IntPtr deviceInfoData
        );

        // for getting buffer size
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool SetupDiGetDeviceInterfaceDetail(
            IntPtr hDevInfo,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            IntPtr deviceInterfaceDetailData,
            UInt32 deviceInterfaceDetailDataSize,
            out UInt32 requiredSize,
            IntPtr deviceInfoData
        );
    }
}

namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;
    using Egs.Win32;

    // NOTE: We referred "USB COMPLETE" by Jan Axelson
    // http://janaxelson.com/usb.htm
    // http://janaxelson.com/hidpage.htm

    internal sealed class Win32SetupDiForEgsDevice
    {
        internal static IList<Guid> TargetHidGuidList;

        static Win32SetupDiForEgsDevice()
        {
            TargetHidGuidList = new List<Guid> { NativeMethods.GUID_DEVINTERFACE_HID, NativeMethods.GUID_DEVINTERFACE_MOUSE };
        }

        internal static readonly ushort VendorId = 0x2BCA;
        //internal static readonly ushort VendorId = 0x040E; // MCCI
        internal static readonly ushort ProductId = 0xA001;

        // mi: multi interface
        internal static readonly string UvcCameraControlInterfaceTag = "mi_00";
        internal static readonly string UvcCameraStreamInterfaceTag = "mi_01";
        internal static readonly string HidTouchScreenInterfaceTag = "mi_02";
        internal static readonly string HidEgsGestureInterfaceTag = "mi_03";

        // col: collection.  USB technical term.  Maybe this collection is not "a collection which contains same kind elements" but "a correction which has various information".
        internal static readonly string HidTouchScreenInterface_VendorSpecificCollectionTag = "col01";
        internal static readonly string HidTouchScreenInterface_MultiTouchCollectionTag = "col02";
        internal static readonly string HidEgsGestureInterface_VendorSpecificCollectionTag = "col01";
        internal static readonly string HidEgsGestureInterface_SingleTouchCollectionTag = "col02";
        internal static readonly string HidEgsGestureInterface_KeyboardCollectionTag = "col03";
        internal static readonly string HidEgsGestureInterface_MouseCollectionTag = "col04";
        internal static readonly string HidEgsGestureInterface_MediaControlCollectionTag = "col05";

        /// <summary>
        /// The list of HidEgsGestureInterface VendorSpecificCollection's DevicePath
        /// </summary>
        internal List<string> HidDevicePathList { get; private set; }

        internal string GetHidDevicePath(int index)
        {
            if (index < 0) { return null; }
            if (index >= HidDevicePathList.Count) { return null; }
            return HidDevicePathList[index];
        }

        internal Win32SetupDiForEgsDevice()
        {
            HidDevicePathList = new List<string>();
        }

        /// <summary>
        /// Use SetupDi API functions to retrieve the device path name of an attached device that belongs to a device interface class.
        /// </summary>
        /// <param name="interfaceClassGuid"> an interface class GUID. </param>
        internal List<string> GetInstalledDeviceDevicePathListByInterfaceClassGuid(Guid interfaceClassGuid)
        {
            uint bufferSize = 0;
            IntPtr detailDataBuffer = IntPtr.Zero;
            IntPtr deviceInfoSet = new System.IntPtr();
            bool lastDevice = false;
            int memberIndex = 0;
            NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
            bool success;
            var newInstalledDeviceDevicePathList = new List<string>();

            try
            {
                // ***
                //  API function

                //  summary 
                //  Retrieves a device information set for a specified group of devices.
                //  SetupDiEnumDeviceInterfaces uses the device information set.

                //  parameters 
                //  Interface class GUID.
                //  Null to retrieve information for all device instances.
                //  Optional handle to a top-level window (unused here).
                //  Flags to limit the returned information to currently present devices 
                //  and devices that expose interfaces in the class specified by the GUID.

                //  Returns
                //  Handle to a device information set for the devices.
                // ***
                deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref interfaceClassGuid, IntPtr.Zero, IntPtr.Zero, NativeMethods.DIGCF_PRESENT | NativeMethods.DIGCF_DEVICEINTERFACE);
                memberIndex = 0;

                // The cbSize element of the deviceInterfaceData structure must be set to
                // the structure's size in bytes. 
                // The size is 28 bytes for 32-bit code and 32 bits for 64-bit code.
                deviceInterfaceData.cbSize = (uint)Marshal.SizeOf(deviceInterfaceData);

                do
                {
                    // Begin with 0 and increment through the device information set until
                    // no more devices are available.

                    // ***
                    //  API function

                    //  summary
                    //  Retrieves a handle to a SP_DEVICE_INTERFACE_DATA structure for a device.
                    //  On return, deviceInterfaceData contains the handle to a
                    //  SP_DEVICE_INTERFACE_DATA structure for a detected device.

                    //  parameters
                    //  DeviceInfoSet returned by SetupDiGetClassDevs.
                    //  Optional SP_DEVINFO_DATA structure that defines a device instance 
                    //  that is a member of a device information set.
                    //  Device interface GUID.
                    //  Index to specify a device in a device information set.
                    //  Pointer to a handle to a SP_DEVICE_INTERFACE_DATA structure for a device.

                    //  Returns
                    //  True on success.
                    // ***
                    success = NativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref interfaceClassGuid, memberIndex, ref deviceInterfaceData);

                    // Find out if a device information set was retrieved.

                    if (!success)
                    {
                        lastDevice = true;
                    }
                    else
                    {
                        // A device is present.

                        // ***
                        //  API function: 

                        //  summary:
                        //  Retrieves an SP_DEVICE_INTERFACE_DETAIL_DATA structure
                        //  containing information about a device.
                        //  To retrieve the information, call this function twice.
                        //  The first time returns the size of the structure.
                        //  The second time returns a pointer to the data.

                        //  parameters
                        //  DeviceInfoSet returned by SetupDiGetClassDevs
                        //  SP_DEVICE_INTERFACE_DATA structure returned by SetupDiEnumDeviceInterfaces
                        //  A returned pointer to an SP_DEVICE_INTERFACE_DETAIL_DATA 
                        //  Structure to receive information about the specified interface.
                        //  The size of the SP_DEVICE_INTERFACE_DETAIL_DATA structure.
                        //  Pointer to a variable that will receive the returned required size of the 
                        //  SP_DEVICE_INTERFACE_DETAIL_DATA structure.
                        //  Returned pointer to an SP_DEVINFO_DATA structure to receive information about the device.

                        //  Returns
                        //  True on success.
                        // ***                     
                        success = NativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, out bufferSize, IntPtr.Zero);

                        // Allocate memory for the SP_DEVICE_INTERFACE_DETAIL_DATA structure using the returned buffer size.
                        detailDataBuffer = Marshal.AllocHGlobal((int)bufferSize);

                        // Store cbSize in the first bytes of the array. The number of bytes varies with 32- and 64-bit systems.
                        Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

                        // Call SetupDiGetDeviceInterfaceDetail again.
                        // This time, pass a pointer to DetailDataBuffer and the returned required buffer size.
                        success = NativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, detailDataBuffer, bufferSize, out bufferSize, IntPtr.Zero);

                        // Skip over cbsize (4 bytes) to get the address of the devicePathList.
                        IntPtr pDevicePathName = new IntPtr(((IntPtr.Size == 4) ? detailDataBuffer.ToInt32() : detailDataBuffer.ToInt64()) + 4);

                        // Get the string containing the devicePathList.
                        string newDevicePath = Marshal.PtrToStringAuto(pDevicePathName);
                        newInstalledDeviceDevicePathList.Add(newDevicePath);

                        // Free the memory allocated previously by AllocHGlobal.
                        if (detailDataBuffer != IntPtr.Zero) { Marshal.FreeHGlobal(detailDataBuffer); }
                    }
                    memberIndex = memberIndex + 1;
                }
                while (!((lastDevice == true)));
                return newInstalledDeviceDevicePathList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero) { NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet); }
            }
        }

        public bool CheckDevicePathContainsVendorIdAndProductId(string checkingDeviceDevicePath, string endPointTag)
        {
            var lowerName = checkingDeviceDevicePath.ToLower(System.Globalization.CultureInfo.InvariantCulture);
            var ret = lowerName.Contains("vid_" + VendorId.ToString("x", System.Globalization.CultureInfo.InvariantCulture));
            ret = ret && lowerName.Contains("pid_" + ProductId.ToString("x", System.Globalization.CultureInfo.InvariantCulture));
            ret = ret && lowerName.Contains(endPointTag);
            return ret;
        }

        internal bool CheckDeviceIsConnected(string checkingDeviceDevicePath)
        {
            //  ***
            //  API function:
            //  CreateFile

            //  Purpose:
            //  Retrieves a handle to a device.

            //  Accepts:
            //  A device path name returned by SetupDiGetDeviceInterfaceDetail
            //  The type of access requested (read/write).
            //  FILE_SHARE attributes to allow other processes to access the device while this handle is open.
            //  A Security structure or IntPtr.Zero. 
            //  A creation disposition value. Use OPEN_EXISTING for devices.
            //  Flags and attributes for files. Not used for devices.
            //  Handle to a template file. Not used.

            //  Returns: a handle without read or write access.
            //  This enables obtaining information about all HIDs, even system
            //  keyboards and mice. 
            //  Separate handles are used for reading and writing.
            //  ***

            // Open the handle without read/write access to enable getting information about any HID, even system keyboards and mice.
            const int isGettingOnlyConnectionState = 0;
            using (var tempHandle = NativeMethods.CreateFile(checkingDeviceDevicePath, isGettingOnlyConnectionState, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, NativeMethods.EFileAttributes.Device, IntPtr.Zero))
            {
                Debug.WriteLine(NativeMethods.GetResultOfApiCall("CreateFile") + Environment.NewLine + "  Returned handle: " + tempHandle.ToString());

                if (tempHandle.IsInvalid) { return false; }

                //  The returned handle is valid, 
                //  so find out if this is the device we're looking for.
                //  Set the Size property of DeviceAttributes to the number of bytes in the structure.
                NativeMethods.HIDD_ATTRIBUTES DeviceAttributes = new NativeMethods.HIDD_ATTRIBUTES();
                DeviceAttributes.Size = (uint)Marshal.SizeOf(DeviceAttributes);
                var hr = NativeMethods.HidD_GetAttributes(tempHandle, ref DeviceAttributes);
                if (hr == false)
                {
                    //  There was a problem in retrieving the information.
                    Debug.WriteLine("Error in filling HIDD_ATTRIBUTES structure.");
                    return false;
                }

#if false
                if (false)
                {
                    Debug.WriteLine("[HIDD_ATTRIBUTES structure filled without error.]");
                    Debug.WriteLine("        Structure size: " + DeviceAttributes.Size);
                    Debug.WriteLine("        Vendor ID: " + Convert.ToString(DeviceAttributes.VendorID, 16));
                    Debug.WriteLine("        Product ID: " + Convert.ToString(DeviceAttributes.ProductID, 16));
                    Debug.WriteLine("        Version Number: " + Convert.ToString(DeviceAttributes.VersionNumber, 16));
                    Debug.WriteLine("");
                }
#endif

                //  Find out if the device matches the one we're looking for.
                if (DeviceAttributes.VendorID == VendorId)
                {
                    return true;
                }
                Debugger.Break();
                return false;
            }
        }

        /// <summary>
        /// DevicePath list of first device which does not have "col" or have "col01", in all connected HID device
        /// </summary>
        internal IList<string> AllConnectedHidDeviceFirstDeviceDevicePathList { get; private set; }
        internal IList<string> AllConnectedCameraDeviceDevicePathList { get; private set; }
        /// <summary>
        /// DevicePath list which is matched to camera, in first device which does not have "col" or have "col01", in all connected HID device
        /// </summary>
        internal IList<string> MatchedConnectedHidDeviceFirstDeviceDevicePathList { get; private set; }
        internal IList<string> MatchedConnectedCameraDeviceDevicePathList { get; private set; }

        internal void Update()
        {
            AllConnectedHidDeviceFirstDeviceDevicePathList = GetInstalledDeviceDevicePathListByInterfaceClassGuid(NativeMethods.GUID_DEVINTERFACE_HID);
            AllConnectedCameraDeviceDevicePathList = GetInstalledDeviceDevicePathListByInterfaceClassGuid(NativeMethods.KSCATEGORY_CAPTURE);

            bool isUsingMultiCollectionHid = true;
            // The value of "col" starts from "01"
            // Our USB descriptor has changed to "all-in-one", so when one device is connected, five "DevicePath"s are enumerated.  This code solve the problem by using only "col01".
            if (isUsingMultiCollectionHid)
            {
                if (ApplicationCommonSettings.IsDebugging)
                {
                    // NOTE(en): DevicePath which values are same except for the value of "col", always contains "col".  If there is not the same DevicePath, the DevicePath does not have "col", maybe.
                    // NOTE(ja): col の値以外が同じDevicePathについては必ずcolが含まれて、同じDevicePathが存在しないものについてはcolも含まれないようである。
                    AllConnectedHidDeviceFirstDeviceDevicePathList = AllConnectedHidDeviceFirstDeviceDevicePathList.OrderBy(e => e).ToList();
                }
                // MUSTDO(en): Serial number should be unique.  But if it is not unique, and the devices with the same serial nubmer are connected, the next code can find only 1 device.  Add test code for the next line.
                // MUSTDO(ja): シリアル番号はユニークであるべきだが、もしユニークでなく、かつシリアル番号が同じデバイスが接続されたときに、1台しか見つからないかもしれない可能性があるので、ちゃんとチェックする。
                AllConnectedHidDeviceFirstDeviceDevicePathList = AllConnectedHidDeviceFirstDeviceDevicePathList
                    .Where(e => (e.Contains("col") == false) || (e.Contains(HidEgsGestureInterface_VendorSpecificCollectionTag)))
                    .ToList();
            }

            MatchedConnectedHidDeviceFirstDeviceDevicePathList = AllConnectedHidDeviceFirstDeviceDevicePathList
                .Where(e => CheckDevicePathContainsVendorIdAndProductId(e, HidEgsGestureInterfaceTag))
                .Where(e => CheckDeviceIsConnected(e))
                .ToList();
            MatchedConnectedCameraDeviceDevicePathList = AllConnectedCameraDeviceDevicePathList
                .Where(e => CheckDevicePathContainsVendorIdAndProductId(e, UvcCameraControlInterfaceTag))
                .ToList();

            if (MatchedConnectedHidDeviceFirstDeviceDevicePathList.Count != MatchedConnectedCameraDeviceDevicePathList.Count)
            {
                // TODO: MUSTDO: Quality Assurance application can stop here.  Check.
                Debugger.Break();
                Debug.WriteLine("SetupDi.MatchedConnectedHidDeviceFirstDeviceDevicePathList.Count != SetupDi.MatchedConnectedCameraDeviceDevicePathList.Count");
            }

#if false
            // TODO: MUSTDO: Should check the relation between multiple camera devices and multiple HID devices
            if (false && MatchedConnectedHidDeviceFirstDeviceDevicePathList.Count >= 2)
            {
                Debugger.Break();
                Debug.WriteLine("Sorry, but you can connect only one Exvision Gesture Camera into a PC currently.");
            }
#endif

            Debug.WriteLine("Called: Win32HidSimpleAccess.UpdateDevicePath()");
            HidDevicePathList = FindHidDevicePathList().Distinct().ToList();
        }

        IList<string> FindHidDevicePathList()
        {
            // TODO: MUSTDO: fix for multiple device connection
            // Use UpdateDevicePathIfDevicePathIsNullOrEmpty() in normal use case.
            // Because if you always use this method in every HID access, CPU usage increase from <1% to 5%-10%.
            // And the UpdateDevicePath() occupies 87.8% of all application CPU times.

            // TODO: MUSTDO: GetInstalledDeviceDevicePathListByInterfaceClassGuid duplicated.  This is not deleted because this code may be faster since it does not get Capability and this was correct.
            // TODO: MUSTDO: Like GetInstalledDeviceDevicePathListByInterfaceClassGuid, it should detect when the app starts and device connection status changes.
            var ret = new List<string>();

            // TODO: MUSTDO: ProductIdは0xA001に統一される。
            foreach (var targetHidGuid in TargetHidGuidList)
            {
                int index = 0;
                // copy
                Guid hidGuid = targetHidGuid;
                NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
                deviceInterfaceData.cbSize = (uint)Marshal.SizeOf(deviceInterfaceData);

                // Enumerate devices.
                var hDevInfo = NativeMethods.SetupDiGetClassDevs(ref hidGuid, IntPtr.Zero, IntPtr.Zero, NativeMethods.DIGCF_DEVICEINTERFACE | NativeMethods.DIGCF_PRESENT);

                while (NativeMethods.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref hidGuid, index, ref deviceInterfaceData))
                {
                    UInt32 size;
                    NativeMethods.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, IntPtr.Zero, 0, out size, IntPtr.Zero);
                    NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = new NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA();
                    deviceInterfaceDetailData.cbSize = (uint)(IntPtr.Size == 8 ? 8 : 5);
                    // Get detail information
                    NativeMethods.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, ref deviceInterfaceDetailData, size, out size, IntPtr.Zero);
                    //Debug.WriteLine(index + " " + deviceInterfaceDetailData.DevicePath + " " + Marshal.GetLastWin32Error());

                    // open a read/write handle to our device using the DevicePath returned
                    SafeFileHandle handle = null;
                    try
                    {
                        handle = NativeMethods.CreateFile(deviceInterfaceDetailData.DevicePath, 0, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, NativeMethods.EFileAttributes.Overlapped, IntPtr.Zero);
                        // create an attributes struct and initialize the size
                        NativeMethods.HIDD_ATTRIBUTES attrib = new NativeMethods.HIDD_ATTRIBUTES();
                        attrib.Size = (uint)Marshal.SizeOf(attrib);
                        // get the attributes of the current device
                        if (NativeMethods.HidD_GetAttributes(handle.DangerousGetHandle(), ref attrib))
                        {
                            //Debug.WriteLine(deviceInterfaceDetailData.DevicePath + " " + attrib.VendorID +" / " +attrib.ProductID);

                            // if the vendor and product IDs match up
                            if (attrib.VendorID != VendorId) { continue; }
                            if (attrib.ProductID != ProductId) { continue; }
                            var lowered = deviceInterfaceDetailData.DevicePath.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                            if (lowered.Contains(HidEgsGestureInterfaceTag) == false) { continue; }
                            if (lowered.Contains(HidEgsGestureInterface_VendorSpecificCollectionTag) == false) { continue; }
                            ret.Add(deviceInterfaceDetailData.DevicePath);
                        }
                    }
                    finally
                    {
                        handle.Close();
                        index++;
                    }
                }
            }
            return ret;
        }
    }
}
