namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;
    using Egs.Win32;

    public partial class EgsDeviceEgsGestureHidReport
    {

        internal void ResetWhenHostFaceDetectionDidNotDetectAnyFaces()
        {
            MessageId = EgsGestureHidReportMessageIds.DetectingFaces;
            ReportId = HidReportIds.EgsGesture;
            foreach (var face in Faces) { face.Reset(); }
            DetectedFacesCount = 0;
            SelectedFaceIndex = -1;
            foreach (var hand in Hands) { hand.Reset(); }
            TrackingHandsCount = 0;
            OnReportUpdated(EventArgs.Empty);
        }

        internal void UpdateWhenHostFaceDetectionDetectedFaces()
        {
            MessageId = EgsGestureHidReportMessageIds.DetectingFaces;
            ReportId = HidReportIds.EgsGesture;
            for (int i = 0; i < Device.EgsGestureHidReport.Faces.Count; i++)
            {
                if (i < Device.FaceDetectionOnHost.DetectedFaceRectsInCameraViewImage.Count)
                {
                    var item = Device.FaceDetectionOnHost.DetectedFaceRectsInCameraViewImage[i];
                    Faces[i].IsDetected = true;
                    Faces[i].Area = item;
                    Faces[i].IsSelected = (item == Device.FaceDetectionOnHost.SelectedFaceRect);
                    Faces[i].Score = 0;
                }
            }
            int newDetectedFacesCount = Math.Min(Device.EgsGestureHidReport.Faces.Count, Device.FaceDetectionOnHost.DetectedFaceRectsInCameraViewImage.Count);
            DetectedFacesCount = newDetectedFacesCount;

            // NOTE: should be ordered
            SelectedFaceIndex = 0;
            TrackingHandsCount = 0;
            OnReportUpdated(EventArgs.Empty);
        }
    }
}
