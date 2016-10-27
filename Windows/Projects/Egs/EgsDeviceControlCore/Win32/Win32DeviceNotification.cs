namespace Egs.Win32
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class NativeMethods
    {
        // Dbt.h
        internal const int DBT_DEVICEARRIVAL = 0x8000;
        internal const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        // Winuser.h
        internal const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        internal const int DEVICE_NOTIFY_SERVICE_HANDLE = 0x00000001;
        internal const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x0004;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public uint dbcc_size;
            public DeviceType dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string dbcc_name;
        }

        internal enum DeviceType : uint
        {
            DBT_DEVTYP_DEVICEINTERFACE = 5,
            DBT_DEVTYP_DEVNODE = 1,
            DBT_DEVTYP_HANDLE = 6,
            DBT_DEVTYP_NET = 4,
            DBT_DEVTYP_OEM = 0,
            DBT_DEVTYP_PORT = 3,
            DBT_DEVTYP_VOLUME = 2,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DEV_BROADCAST_HDR
        {
            public uint dbch_size;
            public DeviceType dbch_devicetype;
            public uint dbch_reserved;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern internal static IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool UnregisterDeviceNotification(IntPtr Handle);
    }
}

namespace Egs
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Egs.Win32;

    internal sealed class Win32DeviceNotification : IDisposable
    {
        IntPtr deviceNotificationHandle = IntPtr.Zero;
        IntPtr ownerWindowMessageMonitoringWindowHandle;

        internal Win32DeviceNotification(IntPtr windowMessageMonitoringWindowHandle)
        {
            Trace.Assert(windowMessageMonitoringWindowHandle != IntPtr.Zero);
            ownerWindowMessageMonitoringWindowHandle = windowMessageMonitoringWindowHandle;
        }

        internal bool RegisterForDeviceNotificationsByUsbGuid()
        {
            return RegisterForDeviceNotifications(NativeMethods.GUID_DEVINTERFACE_USB_DEVICE);
        }

        internal bool RegisterForDeviceNotifications(Guid classGuid)
        {
            IntPtr broadcastDeviceInterfaceBuffer = IntPtr.Zero;
            try
            {
                // A DEV_BROADCAST_DEVICEINTERFACE header holds information about the request.
                NativeMethods.DEV_BROADCAST_DEVICEINTERFACE broadcastDeviceInterface = new NativeMethods.DEV_BROADCAST_DEVICEINTERFACE();

                // Set the parameters in the DEV_BROADCAST_DEVICEINTERFACE structure.

                broadcastDeviceInterface.dbcc_size = (uint)Marshal.SizeOf(broadcastDeviceInterface);

                // Request to receive notifications about a class of devices.
                broadcastDeviceInterface.dbcc_devicetype = NativeMethods.DeviceType.DBT_DEVTYP_DEVICEINTERFACE;
                broadcastDeviceInterface.dbcc_reserved = 0;

                // Specify the interface class to receive notifications about.
                broadcastDeviceInterface.dbcc_classguid = classGuid;

                // Allocate memory for the buffer that holds the DEV_BROADCAST_DEVICEINTERFACE structure.
                broadcastDeviceInterfaceBuffer = Marshal.AllocHGlobal((int)broadcastDeviceInterface.dbcc_size);

                // Copy the DEV_BROADCAST_DEVICEINTERFACE structure to the buffer.
                // Set fDeleteOld True to prevent memory leaks.
                Marshal.StructureToPtr(broadcastDeviceInterface, broadcastDeviceInterfaceBuffer, true);

                // ***
                //  API function

                //  summary
                //  Request to receive notification messages when a device in an interface class
                //  is attached or removed.

                //  parameters 
                //  Handle to the window that will receive device events.
                //  Pointer to a DEV_BROADCAST_DEVICEINTERFACE to specify the type of 
                //  device to send notifications for.
                //  DEVICE_NOTIFY_WINDOW_HANDLE indicates the handle is a window handle.

                //  Returns
                //  Device notification handle or NULL on failure.
                // ***
                deviceNotificationHandle = NativeMethods.RegisterDeviceNotification(
                    ownerWindowMessageMonitoringWindowHandle, broadcastDeviceInterfaceBuffer, NativeMethods.DEVICE_NOTIFY_WINDOW_HANDLE);

                // TODO: MUSTDO: BUG??
                // Marshal data from the unmanaged block broadcastDeviceInterfaceBuffer to
                // the managed object broadcastDeviceInterface
                // Marshal.PtrToStructure(broadcastDeviceInterfaceBuffer, broadcastDeviceInterface);

                if ((deviceNotificationHandle == IntPtr.Zero))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                if (broadcastDeviceInterfaceBuffer != IntPtr.Zero)
                {
                    // Free the memory allocated previously by AllocHGlobal.
                    Marshal.FreeHGlobal(broadcastDeviceInterfaceBuffer);
                }
            }
        }

        internal string CheckDeviceChangeMessageSenderIsTargetDeviceOrNot(IntPtr LParam)
        {
            try
            {
                NativeMethods.DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new NativeMethods.DEV_BROADCAST_DEVICEINTERFACE();
                NativeMethods.DEV_BROADCAST_HDR devBroadcastHdr = (NativeMethods.DEV_BROADCAST_HDR)Marshal.PtrToStructure(LParam, typeof(NativeMethods.DEV_BROADCAST_HDR));

                if (devBroadcastHdr.dbch_devicetype == NativeMethods.DeviceType.DBT_DEVTYP_DEVICEINTERFACE)
                {
                    // The dbch_devicetype parameter indicates that the event applies to a device interface.
                    // So the structure in LParam is actually a DEV_BROADCAST_INTERFACE structure, 
                    // which begins with a DEV_BROADCAST_HDR.

                    // Obtain the number of characters in dbch_name by subtracting the 32 bytes
                    // in the structure that are not part of dbch_name and dividing by 2 because there are 
                    // 2 bytes per character.
                    int stringSize = System.Convert.ToInt32((devBroadcastHdr.dbch_size - 32) / 2);

                    // The dbcc_name parameter of broadcastDeviceInterface contains the device name. 
                    // Trim dbcc_name to match the size of the string.         
                    devBroadcastDeviceInterface.dbcc_name = new string(new char[stringSize + 1]);

                    // Marshal data from the unmanaged block pointed to by m.LParam 
                    // to the managed object broadcastDeviceInterface.
                    devBroadcastDeviceInterface = (NativeMethods.DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(LParam, typeof(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE));

                    return devBroadcastDeviceInterface.dbcc_name;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            return string.Empty;
        }

        private bool disposed = false;
        void ReleaseDeviceNotificationHandle()
        {
            if (deviceNotificationHandle != IntPtr.Zero) { NativeMethods.UnregisterDeviceNotification(deviceNotificationHandle); deviceNotificationHandle = IntPtr.Zero; }
        }
        public void Dispose()
        {
            if (disposed) { return; }
            ReleaseDeviceNotificationHandle();
            disposed = true;
            GC.SuppressFinalize(this);
        }
        ~Win32DeviceNotification() { Dispose(); }
    }
}
