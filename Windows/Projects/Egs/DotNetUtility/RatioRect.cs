namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    public class RatioRect
    {
        [DataMember]
        public RangedFloatRange XRange { get; set; }
        [DataMember]
        public RangedFloatRange YRange { get; set; }

        public RatioRect()
        {
            XRange = new RangedFloatRange(0, 1);
            YRange = new RangedFloatRange(0, 1);
        }
    }
}