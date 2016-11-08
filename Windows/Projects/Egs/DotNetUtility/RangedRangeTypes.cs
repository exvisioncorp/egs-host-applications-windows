namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class RangedIntRange : RangedRangeType<int>
    {
        public RangedIntRange() : base(0, 100) { }
        public RangedIntRange(int from, int to) : base(from, to) { }
    }

    [DataContract]
    public sealed class RangedLongRange : RangedRangeType<long>
    {
        public RangedLongRange() : base(0, 100) { }
        public RangedLongRange(long from, long to) : base(from, to) { }
    }

    [DataContract]
    public sealed class RangedFloatRange : RangedRangeType<float>
    {
        public RangedFloatRange() : base(0, 1.0f) { }
        public RangedFloatRange(float from, float to) : base(from, to) { }
    }

    [DataContract]
    public sealed class RangedDoubleRange : RangedRangeType<double>
    {
        public RangedDoubleRange() : base(0, 1.0) { }
        public RangedDoubleRange(double from, double to) : base(from, to) { }
    }
}