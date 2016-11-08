namespace Egs.Win32
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;

    internal static partial class NativeMethods
    {
        internal const uint FORMAT_MESSAGE_FROM_SYSTEM = 0X1000;
        [DllImport("kernel32.dll", EntryPoint = "FormatMessageW", CharSet = CharSet.Unicode, SetLastError = true)]
        extern internal static int FormatMessage(uint dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageZId, string lpBuffer, int nSize, IntPtr Arguments);

        /// <summary>
        /// Get text that describes the result of an API call
        /// </summary>
        /// <param name="functionName"> the name of the API function. </param>
        internal static string GetResultOfApiCall(string functionName)
        {
            // Returns the result code for the last API call.
            int resultCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
            string result = new string(Convert.ToChar(0), 129);

            // Get the result message that corresponds to the code.
            IntPtr temp = IntPtr.Zero;
            int byteLength = NativeMethods.FormatMessage(NativeMethods.FORMAT_MESSAGE_FROM_SYSTEM, temp, resultCode, 0, result, 128, IntPtr.Zero);

            // Subtract two characters from the message to strip the CR and LF.
            if (byteLength > 2) { result = result.Remove(byteLength - 2, 2); }

            // Create the string to return.
            result = Environment.NewLine + functionName + Environment.NewLine + "Result = " + result + Environment.NewLine;

            return result;
        }

        [Flags]
        internal enum EFileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }

        //[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        extern internal static SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] EFileAttributes flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool CloseHandle(IntPtr hObject);
    }
}
