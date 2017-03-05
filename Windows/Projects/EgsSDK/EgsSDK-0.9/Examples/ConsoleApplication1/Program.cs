namespace ConsoleApplication1
{
    using System;
    using System.Collections.Generic;
    using Egs;

    class Program
    {
        static void Main(string[] args)
        {
            EgsDevice Device = EgsDevice.GetDefaultEgsDevice();
            Device.Settings.IsToDetectFaces.Value = true;
            Device.Settings.IsToDetectHands.Value = true;
            Device.Settings.IsToDrawBordersOnCameraViewImageByDevice.Value = true;
            Device.Settings.CameraViewImageSourceBitmapSize.OptionalValue.SelectedIndex = 1;

            Device.EgsGestureHidReport.ReportUpdated += (sender, e) =>
            {
                if (Device.EgsGestureHidReport.IsStandingBy) { return; }

                if (Device.EgsGestureHidReport.IsFaceDetecting)
                {
                    if (Device.EgsGestureHidReport.SelectedFaceIndex >= 0)
                    {
                        int i = Device.EgsGestureHidReport.SelectedFaceIndex;
                        // Area is System.Drawing.Rectangle, so this project needs a reference to System.Drawing.dll.
                        Console.WriteLine("SelectedFaceIndex: " + i + "  Faces[" + i + "].Area: " + Device.EgsGestureHidReport.Faces[i].Area);
                    }
                }
                else
                {
                    var hand0 = Device.EgsGestureHidReport.Hands[0];
                    var hand1 = Device.EgsGestureHidReport.Hands[1];
                    if ((hand0.IsTracking || hand1.IsTracking) == false) { return; }
                    if (hand0.IsTracking) { Console.Write("Right: " + hand0.X + ", " + hand0.Y + ", " + (hand0.IsTouching ? "Touch" : "Hover") + "  "); }
                    if (hand1.IsTracking) { Console.Write("Left: " + hand1.X + ", " + hand1.Y + ", " + (hand1.IsTouching ? "Touch" : "Hover")); }
                    Console.WriteLine("");
                }
            };

            Console.ReadKey();

            Device.Settings.IsToDetectFaces.Value = false;
            Device.Settings.IsToDetectHands.Value = false;
            Device.Settings.IsToDrawBordersOnCameraViewImageByDevice.Value = false;
            EgsDevice.CloseDefaultEgsDevice();
        }
    }
}
