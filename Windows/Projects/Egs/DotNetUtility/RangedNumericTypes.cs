namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;

    public sealed class RangedInt : RangedNumericType<int>
    {
        public RangedInt() : this(0) { }
        public RangedInt(int value) : this(value, int.MinValue, int.MaxValue, 1, 10, 10) { }
        public RangedInt(int value, int minimum, int maximum) : this(value, minimum, maximum, (maximum - minimum) / 100, (maximum - minimum) / 10, (maximum - minimum) / 10) { }
        public RangedInt(int value, int minimum, int maximum, int smallChange, int largeChange, int tickFrequency)
            : base(value, minimum, maximum, smallChange, largeChange, tickFrequency)
        {
        }

        public List<int> GetRangeBySmallChange()
        {
            var ret = new List<int>();
            for (int i = Minimum; i <= Maximum; i += SmallChange) { ret.Add(i); }
            return ret;
        }
        public List<double> GetRatioBySmallChange()
        {
            var ret = new List<double>();
            for (int i = Minimum; i <= Maximum; i += SmallChange) { ret.Add((double)i / (double)DivisionCount); }
            return ret;
        }

        /// <summary>Maximum - Minimum + 1</summary>
        public int PositionCount { get { return Maximum - Minimum + 1; } }
        /// <summary>Maximum - Minimum</summary>
        public int DivisionCount { get { return Maximum - Minimum; } }

        public void SetValueIfChanged(int value)
        {
            if (Value.Equals(value) == false) { Value = value; }
        }
    }

    public sealed class RangedLong : RangedNumericType<long>
    {
        public RangedLong() : this(0) { }
        public RangedLong(long value) : this(value, long.MinValue, long.MaxValue, 1, 10, 10) { }
        public RangedLong(long value, long minimum, long maximum) : this(value, minimum, maximum, (maximum - minimum) / 100, (maximum - minimum) / 10, (maximum - minimum) / 10) { }
        public RangedLong(long value, long minimum, long maximum, long smallChange, long largeChange, long tickFrequency)
            : base(value, minimum, maximum, smallChange, largeChange, tickFrequency)
        {
        }

        public List<long> GetRangeBySmallChange()
        {
            var ret = new List<long>();
            for (long i = Minimum; i <= Maximum; i += SmallChange) { ret.Add(i); }
            return ret;
        }
        public List<double> GetRatioBySmallChange()
        {
            var ret = new List<double>();
            for (long i = Minimum; i <= Maximum; i += SmallChange) { ret.Add((double)i / (double)DivisionCount); }
            return ret;
        }

        /// <summary>Maximum - Minimum + 1</summary>
        public long PositionCount { get { return Maximum - Minimum + 1; } }
        /// <summary>Maximum - Minimum</summary>
        public long DivisionCount { get { return Maximum - Minimum; } }

        public void SetValueIfChanged(long value)
        {
            if (Value.Equals(value) == false) { Value = value; }
        }
    }

    public sealed class RangedDouble : RangedNumericType<double>
    {
        public RangedDouble() : this(0) { }
        public RangedDouble(double value) : this(value, double.NegativeInfinity, double.PositiveInfinity, 0.01, 0.1, 0.1) { }
        public RangedDouble(double value, double minimum, double maximum) : this(value, minimum, maximum, (maximum - minimum) / 100, (maximum - minimum) / 10, (maximum - minimum) / 10) { }
        public RangedDouble(double value, double minimum, double maximum, double smallChange, double largeChange, double tickFrequency)
            : base(value, minimum, maximum, smallChange, largeChange, tickFrequency)
        {
        }

        /// <summary>Maximum - Minimum</summary>
        public double Distance { get { return Maximum - Minimum; } }

        public void SetValueIfChanged(double value)
        {
            Trace.Assert(double.IsNaN(value) == false);
            if (Value.Equals(value) == false) { Value = value; }
        }
    }

    public sealed class RangedFloat : RangedNumericType<float>
    {
        public RangedFloat() : this(0) { }
        public RangedFloat(float value) : this(value, float.NegativeInfinity, float.PositiveInfinity, 0.01f, 0.1f, 0.1f) { }
        public RangedFloat(float value, float minimum, float maximum) : this(value, minimum, maximum, (maximum - minimum) / 100, (maximum - minimum) / 10, (maximum - minimum) / 10) { }
        public RangedFloat(float value, float minimum, float maximum, float smallChange, float largeChange, float tickFrequency)
            : base(value, minimum, maximum, smallChange, largeChange, tickFrequency)
        {
        }
        public RangedFloat(double value, double minimum, double maximum, double smallChange, double largeChange, double tickFrequency)
            : base((float)value, (float)minimum, (float)maximum, (float)smallChange, (float)largeChange, (float)tickFrequency)
        {
        }

        /// <summary>Maximum - Minimum</summary>
        public float Distance { get { return Maximum - Minimum; } }

        public void SetValueIfChanged(float value)
        {
            Trace.Assert(float.IsNaN(value) == false);
            if (Value.Equals(value) == false) { Value = value; }
        }
    }
}