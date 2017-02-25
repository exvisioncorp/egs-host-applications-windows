namespace Egs.PropertyTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;
    using Egs.EgsDeviceControlCore.Properties;
    using Egs.DotNetUtility;

    public class EgsHostOptionalPropertyDetailBase
    {
        // NOTE: Do not implement IPropertyChanged here.  Use only for Optional<T>.  All strings are written in Resources.  Before I used "[DataMember]", but it is obsoleted.
        public string DescriptionKey { get; internal set; }
        public string Description { get { return string.IsNullOrEmpty(DescriptionKey) ? "" : Resources.ResourceManager.GetString(DescriptionKey, Resources.Culture); } }
        public override string ToString() { return Description; }
    }


    public enum FaceDetectionIsProcessedByKind
    {
        Stopped,
        HostApplication,
        Device,
    }

    public class FaceDetectionIsProcessedByDetail : EgsHostOptionalPropertyDetailBase
    {
        public FaceDetectionIsProcessedByKind EnumValue { get; internal set; }

        public static List<FaceDetectionIsProcessedByDetail> GetDefaultList()
        {
            var ret = new List<FaceDetectionIsProcessedByDetail>();
            ret.Add(new FaceDetectionIsProcessedByDetail()
            {
                EnumValue = FaceDetectionIsProcessedByKind.Stopped,
                DescriptionKey = Name.Of(() => Resources.FaceDetectionIsProcessedByDetail_ByHostApplication_Description)
            });
            ret.Add(new FaceDetectionIsProcessedByDetail()
            {
                EnumValue = FaceDetectionIsProcessedByKind.HostApplication,
                DescriptionKey = Name.Of(() => Resources.FaceDetectionIsProcessedByDetail_ByHostApplication_Description)
            });
            ret.Add(new FaceDetectionIsProcessedByDetail()
            {
                EnumValue = FaceDetectionIsProcessedByKind.Device,
                DescriptionKey = Name.Of(() => Resources.FaceDetectionIsProcessedByDetail_ByDevice_Description)
            });
            return ret;
        }
    }
}
