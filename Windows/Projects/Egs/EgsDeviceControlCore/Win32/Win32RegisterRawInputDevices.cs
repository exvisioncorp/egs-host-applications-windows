namespace Egs.Win32
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTDEVICE
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsagePage;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsage;
            [MarshalAs(UnmanagedType.U4)]
            public int dwFlags;
            public IntPtr hwndTarget;
        }

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);
    }
}

namespace Egs
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Egs.Win32;

    internal sealed class Win32RegisterRawInputDevices : IDisposable
    {
        IntPtr ownerWmInputMessageReceivingWindowHandle;

        internal Win32RegisterRawInputDevices(IntPtr windowMessageMonitoringWindowHandle)
        {
            Trace.Assert(windowMessageMonitoringWindowHandle != IntPtr.Zero);
            ownerWmInputMessageReceivingWindowHandle = windowMessageMonitoringWindowHandle;
        }

        internal void RegisterRawInputDevices()
        {
            NativeMethods.RAWINPUTDEVICE[] devices = new NativeMethods.RAWINPUTDEVICE[3];

            // for TouchScreen
            devices[0].usUsagePage = 0x0D; //Digitizer 
            devices[0].usUsage = 0x04;//Touch Screen
            devices[0].hwndTarget = ownerWmInputMessageReceivingWindowHandle;
            devices[0].dwFlags = (int)NativeMethods.RIDEV.INPUTSINK;

            // for Mouse
            devices[1].usUsagePage = 0x01; //Generic Desktop
            devices[1].usUsage = 0x02;//Mouse
            devices[1].hwndTarget = ownerWmInputMessageReceivingWindowHandle;
            devices[1].dwFlags = (int)NativeMethods.RIDEV.INPUTSINK;

            // for VSC TouchScreen (VSC=Vendor SpeCific)
            devices[2].usUsagePage = 0xFF00; //Vendor
            devices[2].usUsage = 0x01;//Vendor (Touch Screen)
            devices[2].hwndTarget = ownerWmInputMessageReceivingWindowHandle;
            devices[2].dwFlags = (int)NativeMethods.RIDEV.INPUTSINK;

            // devices.Length: The number of "WM_INPUT enabled" devices
            int size = Marshal.SizeOf(typeof(NativeMethods.RAWINPUTDEVICE));
            NativeMethods.RegisterRawInputDevices(devices, (uint)devices.Length, (uint)size);
        }

        internal void UnregisterRawInputDevices()
        {
            NativeMethods.RAWINPUTDEVICE[] devices = new NativeMethods.RAWINPUTDEVICE[3];

            // for TouchScreen
            devices[0].usUsagePage = 0x0D; //Digitizer 
            devices[0].usUsage = 0x04;//Touch Screen
            devices[0].dwFlags = (int)NativeMethods.RIDEV.REMOVE;

            // for Mouse
            devices[1].usUsagePage = 0x01; //Generic Desktop
            devices[1].usUsage = 0x02;//Mouse
            devices[1].dwFlags = (int)NativeMethods.RIDEV.REMOVE;

            // for VSC TouchScreen (VSC=Vendor SpeCific)
            devices[2].usUsagePage = 0xFF00; //Vendor
            devices[2].usUsage = 0x01;//Vendor (Touch Screen)
            devices[2].dwFlags = (int)NativeMethods.RIDEV.REMOVE;

            // devices.Length: The number of "WM_INPUT enabled" devices
            int size = Marshal.SizeOf(typeof(NativeMethods.RAWINPUTDEVICE));
            NativeMethods.RegisterRawInputDevices(devices, (uint)devices.Length, (uint)size);
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (disposed) { return; }
            // MUSTDO: test
            UnregisterRawInputDevices();
            disposed = true;
            GC.SuppressFinalize(this);
        }
        ~Win32RegisterRawInputDevices() { Dispose(); }
    }
}
