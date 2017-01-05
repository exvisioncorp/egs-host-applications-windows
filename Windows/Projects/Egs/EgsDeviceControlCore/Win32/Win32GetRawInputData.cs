#if USE_OLD_HID
namespace Egs.Win32
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class NativeMethods
    {
        static NativeMethods()
        {
            // This is the only one definition.
        }
    }

    internal static partial class NativeMethods
    {
        internal enum RawInputHeaderType : uint
        {
            Mouse = 0,
            Keyboard = 1,
            Hid = 2,
            RIM_TYPEMOUSE = 0,
            RIM_TYPEKEYBOARD = 1,
            RIM_TYPEHID = 2,
            None = uint.MaxValue,
        }

        internal enum RID : uint
        {
            /// <summary> Get header information. </summary>
            HEADER = 0x10000005,
            /// <summary> Get raw pData. </summary>
            INPUT = 0x10000003,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTHEADER
        {
            [MarshalAs(UnmanagedType.U4)]
            public RawInputHeaderType dwType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwSize;
            public IntPtr hDevice;
            [MarshalAs(UnmanagedType.U4)]
            public int wParam;
        }

        [Flags]
        internal enum RawMouseFlags : ushort
        {
            // https://msdn.microsoft.com/en-us/library/windows/desktop/ms645578(v=vs.85).aspx
            MOUSE_ATTRIBUTES_CHANGED = 0x04,
            MOUSE_MOVE_RELATIVE = 0x0,
            MOUSE_MOVE_ABSOLUTE = 0x1,
            MOUSE_VIRTUAL_DESKTOP = 0x02
        }

        [Flags]
        internal enum RawMouseButtonFlags : ushort
        {
            // https://msdn.microsoft.com/en-us/library/windows/desktop/ms645578(v=vs.85).aspx
            RI_MOUSE_LEFT_BUTTON_DOWN = 0x0001,
            RI_MOUSE_LEFT_BUTTON_UP = 0x0002,
            RI_MOUSE_MIDDLE_BUTTON_DOWN = 0x0010,
            RI_MOUSE_MIDDLE_BUTTON_UP = 0x0020,
            RI_MOUSE_RIGHT_BUTTON_DOWN = 0x0004,
            RI_MOUSE_RIGHT_BUTTON_UP = 0x0008,
            RI_MOUSE_BUTTON_1_DOWN = 0x0001,
            RI_MOUSE_BUTTON_1_UP = 0x0002,
            RI_MOUSE_BUTTON_2_DOWN = 0x0004,
            RI_MOUSE_BUTTON_2_UP = 0x0008,
            RI_MOUSE_BUTTON_3_DOWN = 0x0010,
            RI_MOUSE_BUTTON_3_UP = 0x0020,
            RI_MOUSE_BUTTON_4_DOWN = 0x0040,
            RI_MOUSE_BUTTON_4_UP = 0x0080,
            RI_MOUSE_BUTTON_5_DOWN = 0x100,
            RI_MOUSE_BUTTON_5_UP = 0x0200,
            RI_MOUSE_WHEEL = 0x0400
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RawMouseButtonFlagsAndData
        {
            [MarshalAs(UnmanagedType.U2)]
            public RawMouseButtonFlags usButtonFlags;
            /// <summary>
            /// If usButtonFlags is RI_MOUSE_WHEEL, this member is a signed device that specifies the wheel delta.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonData;
        }

        //typedef struct tagRAWMOUSE {
        //2 00-01    USHORT usFlags;
        //           padding
        //  04-07    union {
        //4 04-07      ULONG  ulButtons;
        //             struct {
        //2 04-05        USHORT usButtonFlags;
        //2 06-07        USHORT usButtonData;
        //             };
        //           };
        //4 08-11    ULONG  ulRawButtons;
        //4 12-15    LONG   lLastX;
        //4 16-19    LONG   lLastY;
        //4 20-23    ULONG  ulExtraInformation;
        //        } RAWMOUSE, *PRAWMOUSE, *LPRAWMOUSE;
        [StructLayout(LayoutKind.Explicit)]
        internal struct RAWMOUSE
        {
            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(0)]
            public RawMouseFlags usFlags;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            public uint ulButtons;
            [FieldOffset(4)]
            public RawMouseButtonFlagsAndData buttons;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(8)]
            public uint ulRawButtons;
            [FieldOffset(12)]
            public int lLastX;
            [FieldOffset(16)]
            public int lLastY;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(20)]
            public uint ulExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWKEYBOARD
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort MakeCode;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Flags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Reserved;
            [MarshalAs(UnmanagedType.U2)]
            public ushort VKey;
            [MarshalAs(UnmanagedType.U4)]
            public uint Message;
            [MarshalAs(UnmanagedType.U4)]
            public uint ExtraInformation;
        }

        //typedef struct tagRAWHID {
        //  DWORD dwSizeHid;
        //  DWORD dwCount;
        //  BYTE  bRawData[1];
        //} RAWHID, *PRAWHID, *LPRAWHID;
        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWHID
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizeHid;
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount;
            //public IntPtr bRawData;
        }

        //typedef struct tagRAWINPUT {
        //    RAWINPUTHEADER header;
        //      union {
        //        RAWMOUSE    mouse;
        //        RAWKEYBOARD keyboard;
        //        RAWHID      hid;
        //      } data;
        //    } RAWINPUT, *PRAWINPUT, *LPRAWINPUT;
        [StructLayout(LayoutKind.Explicit)]
        internal struct RAWINPUT
        {
            [FieldOffset(0)]
            public RAWINPUTHEADER header;
            [FieldOffset(16)]
            public RAWMOUSE mouse;
            [FieldOffset(16)]
            public RAWKEYBOARD keyboard;
            [FieldOffset(16)]
            public RAWHID hid;
        }

        internal enum RIDEV : int
        {
            APPKEYS = 0x00000400,
            CAPTUREMOUSE = 0x00000200,
            DEVNOTIFY = 0x00002000,
            EXCLUDE = 0x00000010,
            EXINPUTSINK = 0x00001000,
            INPUTSINK = 0x00000100,
            NOHOTKEYS = 0x00000200,
            NOLEGACY = 0x00000030,
            PAGEONLY = 0x00000020,
            REMOVE = 0x00000001
        }

        // Do not use "out RAWINPUT pData" instead of 3rd parameter, because that way is incompatible with x64.
        [DllImport("User32.dll")]
        extern internal static uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref int pcbSize, int cbSizeHeader);
    }
}
#endif