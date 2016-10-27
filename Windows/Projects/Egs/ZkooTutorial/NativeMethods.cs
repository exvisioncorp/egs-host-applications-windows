namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Microsoft.Win32.SafeHandles;

    internal static partial class NativeMethods
    {
        [DllImport("User32.dll")]
        extern internal static bool SetCursorPos(int X, int Y);
    }
}
