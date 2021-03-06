﻿namespace Egs
{
    using System;
    using Egs.Win32;

    partial class EgsDevicesWindowMessageReceivingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

                if (deviceNotification != null) { deviceNotification.Dispose(); deviceNotification = null; }
#if USE_OLD_HID
                if (touchRawInputDevices != null) { touchRawInputDevices.Dispose(); touchRawInputDevices = null; }
                if (HidReportsUpdateByWin32RawInput != null) { HidReportsUpdateByWin32RawInput.Dispose(); HidReportsUpdateByWin32RawInput = null; }
#endif

                if (IsToUseActiveWindowHWnd)
                {
                    NativeMethods.SetWindowLong(hMainWindow, -4, oldWndProcPtr);
                    hMainWindow = IntPtr.Zero;
                    oldWndProcPtr = IntPtr.Zero;
                    newWndProcPtr = IntPtr.Zero;
                    newWndProc = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WindowMessageReceivingForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "WindowMessageReceivingForm";
            this.ResumeLayout(false);

        }

        #endregion

    }
}