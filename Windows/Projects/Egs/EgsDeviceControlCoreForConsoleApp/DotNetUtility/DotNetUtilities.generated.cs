// NOTE: impossible: include file = "../../DotNetUtility/" + "DuplicatedProcessStartBlocking.cs"
namespace Egs
{
namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Linq.Expressions;
    using System.ComponentModel;

    public static class Name
    {
        public static string Of<T, TResult>(this Expression<Func<T, TResult>> accessor)
        {
            return GetNameOfExpression(accessor.Body);
        }

        public static string Of<T>(this Expression<Func<T>> accessor)
        {
            return GetNameOfExpression(accessor.Body);
        }

        public static string Of<T, TResult>(this T obj, Expression<Func<T, TResult>> propertyAccessor)
        {
            return GetNameOfExpression(propertyAccessor.Body);
        }

        static string GetNameOfExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = expression as MemberExpression;
                if (memberExpression == null) { return ""; }
                return memberExpression.Member.Name;
            }
            return "";
        }
    }
}

namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    [DataContract]
    public class OptionalValue<T> : INotifyPropertyChanged
        where T : class, new()
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        List<T> _Options = new List<T>();
        public event EventHandler OptionsChanged;
        protected virtual void OnOptionsChanged(EventArgs e)
        {
            var t = OptionsChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("Options");
        }

        /// <summary>
        /// ObservableCollection of T.
        /// [NOTE]: SelectedIndex has [DataMember], but Options does not have [DataMember].
        /// Because Json.NET Deserialize / PopulateObject methods DO NOT "RE"SET items but "ADD" items to constructed objects of List, Collection and so on.
        /// Array is reset, but not used.
        /// </summary>
        public List<T> Options
        {
            get { return _Options; }
            set
            {
                _Options = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _SelectedIndex = 0;
        public event EventHandler SelectedIndexChanged;
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            var t = SelectedIndexChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("SelectedIndex");
        }
        [DataMember]
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                _SelectedIndex = value;
                OnSelectedIndexChanged(EventArgs.Empty);
                OnSelectedItemChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
            var t = SelectedItemChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("SelectedItem");
        }
        public T SelectedItem
        {
            get
            {
                if (_Options == null) { return null; }
                if (_Options.Count == 0) { return null; }
                if (_SelectedIndex < 0) { return null; }
                return _Options[_SelectedIndex];
            }
        }

        public bool SelectSingleItemByPredicate(Func<T, bool> predicate)
        {
            var result = Options.Single(predicate);
            if (result == null) { return false; }
            SelectedIndex = Options.IndexOf(result);
            return true;
        }
    }
}
namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    public class RangedNumericType<T> : IComparable<T>, IEquatable<T>, INotifyPropertyChanged
        where T : IComparable<T>, IEquatable<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        [DataMember]
        public bool CanRaiseDebbugerBreak { get; set; }


        [EditorBrowsable(EditorBrowsableState.Never)]
        T _Value;
        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged(EventArgs e)
        {
            var t = ValueChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("Value");
        }
        [DataMember]
        public T Value
        {
            get { return _Value; }
            set
            {
                if (value.CompareTo(_Minimum) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] newValue(={0}) < Minimum(={1}).  Value = Minimum = {1}", value, _Minimum));
                    value = _Minimum;
                }
                else if (_Maximum.CompareTo(value) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] Maximum(={1}) < newValue(={0}).  Value = Maximum = {1}", value, _Maximum));
                    value = _Maximum;
                }
                _Value = value;
                OnValueChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        T _Minimum;
        [DataMember]
        public T Minimum
        {
            get { return _Minimum; }
            set
            {
                // Latest setting has priority.  The set value does not depend on the previous object state.
                if (_Maximum.CompareTo(value) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] Maximum(={1}) < newMinimum(={0}).  Minimum = Value = Maximum = newMinimum = {0}", value, _Maximum));
                    if (CanRaiseDebbugerBreak) { Debugger.Break(); }
                    _Minimum = _Value = _Maximum = value;
                    OnPropertyChanged("Maximum");
                    OnPropertyChanged("Minimum");
                    OnValueChanged(EventArgs.Empty);
                }
                else if (_Value.CompareTo(value) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] Value(={1}) < newMinimum(={0}).  Minimum = Value = newMinimum = {0}", value, _Value));
                    _Minimum = _Value = value;
                    OnPropertyChanged("Minimum");
                    OnValueChanged(EventArgs.Empty);
                }
                else
                {
                    _Minimum = value;
                    OnPropertyChanged("Minimum");
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        T _Maximum;
        [DataMember]
        public T Maximum
        {
            get { return _Maximum; }
            set
            {
                // Latest setting has priority.  The set value does not depend on the previous object state.
                if (value.CompareTo(_Minimum) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] newMaximum(={0}) < Minimum(={1}).  Maximum = Value = Minimum = newMaximum = {0}", value, _Minimum));
                    if (CanRaiseDebbugerBreak) { Debugger.Break(); }
                    _Maximum = _Value = _Minimum = value;
                    OnPropertyChanged("Minimum");
                    OnPropertyChanged("Maximum");
                    OnValueChanged(EventArgs.Empty);
                }
                else if (value.CompareTo(_Value) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] newMaximum(={0}) < Value(={1}).  Maximum = Value = newMaximum = {0}", value, _Value));
                    _Maximum = _Value = value;
                    OnPropertyChanged("Maximum");
                    OnValueChanged(EventArgs.Empty);
                }
                else
                {
                    _Maximum = value;
                    OnPropertyChanged("Maximum");
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        T _SmallChange;
        [DataMember]
        public T SmallChange
        {
            get { return _SmallChange; }
            set { _SmallChange = value; OnPropertyChanged("SmallChange"); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        T _LargeChange;
        [DataMember]
        public T LargeChange
        {
            get { return _LargeChange; }
            set { _LargeChange = value; OnPropertyChanged("LargeChange"); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        T _TickFrequency;
        [DataMember]
        public T TickFrequency
        {
            get { return _TickFrequency; }
            set { _TickFrequency = value; OnPropertyChanged("TickFrequency"); }
        }

        public RangedNumericType()
        {
            CanRaiseDebbugerBreak = false;
        }
        public RangedNumericType(T value)
        {
            _Minimum = _Value = _Maximum = value;
            CanRaiseDebbugerBreak = false;
        }
        public RangedNumericType(T value, T minimum, T maximum, T smallChange, T largeChange, T tickFrequency)
        {
            Trace.Assert(minimum.CompareTo(maximum) <= 0);
            Trace.Assert(minimum.CompareTo(value) <= 0);
            Trace.Assert(value.CompareTo(maximum) <= 0);
            _Value = value;
            _Minimum = minimum;
            _Maximum = maximum;
            _SmallChange = smallChange;
            _LargeChange = largeChange;
            _TickFrequency = tickFrequency;
            CanRaiseDebbugerBreak = false;
        }
        public static implicit operator T(RangedNumericType<T> self)
        {
            return self.Value;
        }

        /// <summary>Only Value is compared.</summary>
        public bool Equals(T other) { return _Value.Equals(other); }
        /// <summary>Only Value is compared.</summary>
        public int CompareTo(T other) { return _Value.CompareTo(other); }
        /// <summary>Get the hashcode of the Value.</summary>
        public override int GetHashCode() { return _Value.GetHashCode(); }
        /// <summary>Value, Minimum, Maximum, SmallChange, LargeChange and TickFrequency are compared.</summary>
        public override bool Equals(object obj)
        {
            if (obj == null || (this.GetType() != obj.GetType())) { return false; }
            try
            {
                var objAsT = (RangedNumericType<T>)obj;
                if (Value.Equals(objAsT.Value)) { return false; }
                if (Minimum.Equals(objAsT.Minimum)) { return false; }
                if (Maximum.Equals(objAsT.Maximum)) { return false; }
                if (SmallChange.Equals(objAsT.SmallChange)) { return false; }
                if (LargeChange.Equals(objAsT.LargeChange)) { return false; }
                if (TickFrequency.Equals(objAsT.TickFrequency)) { return false; }
                return true;
            }
            catch (Exception ex)
            {
                if (CanRaiseDebbugerBreak) { Debugger.Break(); Console.WriteLine(ex.Message); }
                return false;
            }
        }

        public static bool operator ==(RangedNumericType<T> left, RangedNumericType<T> right)
        {
            if (object.ReferenceEquals(left, null)) { return object.ReferenceEquals(right, null); }
            return left.Equals(right);
        }
        public static bool operator !=(RangedNumericType<T> left, RangedNumericType<T> right)
        {
            return !(left == right);
        }
        public static bool operator <(RangedNumericType<T> left, RangedNumericType<T> right)
        {
            return (left.CompareTo(right) < 0);
        }
        public static bool operator >(RangedNumericType<T> left, RangedNumericType<T> right)
        {
            return (left.CompareTo(right) > 0);
        }

        public override string ToString()
        {
            return "Value=" + _Value + ", Minimum=" + _Minimum + ", Maximum=" + _Maximum;
        }
    }
}
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
    public class RangedRangeType<T> : IEquatable<RangedRangeType<T>>, INotifyPropertyChanged
        where T : IComparable<T>, IEquatable<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }


        [DataMember]
        public bool CanRaiseDebbugerBreak { get; set; }


        [EditorBrowsable(EditorBrowsableState.Never)]
        T _From;
        public event EventHandler FromChanged;
        protected virtual void OnFromChanged(EventArgs e)
        {
            var t = FromChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("From");
        }
        [DataMember]
        public T From
        {
            get { return _From; }
            set
            {
                if (value.CompareTo(_Minimum) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] newFrom(={0}) < Minimum(={1}).  From = Minimum = {1}", value, _Minimum));
                    value = _Minimum;
                }
                else if (_To.CompareTo(value) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] To(={1}) < newFrom(={0}).  From = To = {1}", value, _To));
                    value = _To;
                }
                _From = value;
                OnFromChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        T _To;
        public event EventHandler ToChanged;
        protected virtual void OnToChanged(EventArgs e)
        {
            var t = ToChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("To");
        }
        [DataMember]
        public T To
        {
            get { return _To; }
            set
            {
                if (value.CompareTo(_From) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] newTo(={0}) < From(={1}).  To = From = {1}", value, _From));
                    value = _From;
                }
                else if (_Maximum.CompareTo(value) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] Maximum(={1}) < newTo(={0}).  To = Maximum = {1}", value, _Maximum));
                    value = _Maximum;
                }
                _To = value;
                OnToChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        T _Minimum;
        [DataMember]
        public T Minimum
        {
            get { return _Minimum; }
            set
            {
                // Latest setting has priority.  The set value does not depend on the previous object state.
                if (_Maximum.CompareTo(value) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] Maximum(={1}) < newMinimum(={0}).  Minimum = From = To = Maximum = newMinimum = {0}", value, _Maximum));
                    if (CanRaiseDebbugerBreak) { Debugger.Break(); }
                    _Minimum = _From = _To = _Maximum = value;
                    OnPropertyChanged("Maximum");
                    OnPropertyChanged("Minimum");
                    OnToChanged(EventArgs.Empty);
                    OnFromChanged(EventArgs.Empty);
                }
                else if (_To.CompareTo(value) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] To(={1}) < newMinimum(={0}).  Minimum = From = To = newMinimum = {0}", value, _To));
                    _Minimum = _From = _To = value;
                    OnPropertyChanged("Minimum");
                    OnToChanged(EventArgs.Empty);
                    OnFromChanged(EventArgs.Empty);
                }
                else if (_From.CompareTo(value) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] From(={1}) < newMinimum(={0}).  Minimum = From = newMinimum = {0}", value, _From));
                    _Minimum = _From = value;
                    OnPropertyChanged("Minimum");
                    OnFromChanged(EventArgs.Empty);
                }
                else
                {
                    _Minimum = value;
                    OnPropertyChanged("Minimum");
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        T _Maximum;
        [DataMember]
        public T Maximum
        {
            get { return _Maximum; }
            set
            {
                // Latest setting has priority.  The set value does not depend on the previous object state.
                if (value.CompareTo(_Minimum) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] newMaximum(={0}) < Minimum(={1}).  Maximum = To = From = Minimum = newMaximum = {0}", value, _Minimum));
                    if (CanRaiseDebbugerBreak) { Debugger.Break(); }
                    _Maximum = _To = _From = _Minimum = value;
                    OnPropertyChanged("Minimum");
                    OnPropertyChanged("Maximum");
                    OnFromChanged(EventArgs.Empty);
                    OnToChanged(EventArgs.Empty);
                }
                else if (value.CompareTo(_From) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] newMaximum(={0}) < From(={1}).  Maximum = To = From = newMaximum = {0}", value, _From));
                    _Maximum = _To = _From = value;
                    OnPropertyChanged("Maximum");
                    OnFromChanged(EventArgs.Empty);
                    OnToChanged(EventArgs.Empty);
                }
                else if (value.CompareTo(_To) < 0)
                {
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Warning] newMaximum(={0}) < To(={1}).  Maximum = To = newMaximum = {0}", value, _To));
                    _Maximum = _To = value;
                    OnPropertyChanged("Maximum");
                    OnToChanged(EventArgs.Empty);
                }
                else
                {
                    _Maximum = value;
                    OnPropertyChanged("Maximum");
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        T _SmallChange;
        [DataMember]
        public T SmallChange
        {
            get { return _SmallChange; }
            set { _SmallChange = value; OnPropertyChanged("SmallChange"); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        T _LargeChange;
        [DataMember]
        public T LargeChange
        {
            get { return _LargeChange; }
            set { _LargeChange = value; OnPropertyChanged("LargeChange"); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        T _TickFrequency;
        [DataMember]
        public T TickFrequency
        {
            get { return _TickFrequency; }
            set { _TickFrequency = value; OnPropertyChanged("TickFrequency"); }
        }

        public RangedRangeType()
        {
            CanRaiseDebbugerBreak = false;
        }
        public RangedRangeType(T from, T to)
        {
            _Minimum = _From = from;
            _To = _Maximum = to;
            CanRaiseDebbugerBreak = false;
        }
        public RangedRangeType(T from, T to, T minimum, T maximum, T smallChange, T largeChange, T tickFrequency)
        {
            Trace.Assert(minimum.CompareTo(maximum) <= 0);
            Trace.Assert(minimum.CompareTo(from) <= 0);
            Trace.Assert(from.CompareTo(to) <= 0);
            Trace.Assert(to.CompareTo(maximum) <= 0);
            _From = from;
            _To = to;
            _Minimum = minimum;
            _Maximum = maximum;
            _SmallChange = smallChange;
            _LargeChange = largeChange;
            _TickFrequency = tickFrequency;
            CanRaiseDebbugerBreak = false;
        }

        /// <summary>Only Value is compared.</summary>
        public bool Equals(RangedRangeType<T> other) { return _From.Equals(other._From) && _To.Equals(other._To); }
        /// <summary>Get the hashcode of the Value.</summary>
        public override int GetHashCode() { return _From.GetHashCode() ^ _To.GetHashCode(); }
        /// <summary>Value, Minimum, Maximum, SmallChange, LargeChange and TickFrequency are compared.</summary>
        public override bool Equals(object obj)
        {
            if (obj == null || (this.GetType() != obj.GetType())) { return false; }
            try
            {
                var objAsT = (RangedRangeType<T>)obj;
                if (From.Equals(objAsT.From)) { return false; }
                if (To.Equals(objAsT.To)) { return false; }
                if (Minimum.Equals(objAsT.Minimum)) { return false; }
                if (Maximum.Equals(objAsT.Maximum)) { return false; }
                if (SmallChange.Equals(objAsT.SmallChange)) { return false; }
                if (LargeChange.Equals(objAsT.LargeChange)) { return false; }
                if (TickFrequency.Equals(objAsT.TickFrequency)) { return false; }
                return true;
            }
            catch (Exception ex)
            {
                if (CanRaiseDebbugerBreak) { Debugger.Break(); Console.WriteLine(ex.Message); }
                return false;
            }
        }
        public override string ToString()
        {
            return "From=" + _From + ", To=" + _To + ", Minimum=" + _Minimum + ", Maximum=" + _Maximum;
        }
    }
}
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
namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.ComponentModel;

    class VelocityFilter
    {
        public double FramesPerSecond { get; set; }
        public double XDotEmaCoefficient { get; set; }
        public double X { get; private set; }
        public double XDot { get; private set; }
        double XPrevious;
        double XDotPrevious;

        public VelocityFilter()
        {
            FramesPerSecond = 100.0;
            XDotEmaCoefficient = 1.0;
            X = 0;
            XDot = 0;
            XPrevious = 0;
            XDotPrevious = 0;
        }

        public void Initialize(double initialX)
        {
            X = initialX;
            XDot = 0;
            XPrevious = initialX;
            XDotPrevious = 0;
        }

        public void Update(double newX)
        {
            XPrevious = X;
            XDotPrevious = XDot;
            X = newX;
            var measuredXDot = (X - XPrevious) * FramesPerSecond;
            XDot = XDotEmaCoefficient * measuredXDot + (1.0 - XDotEmaCoefficient) * XDotPrevious;
        }
    }
}

}
