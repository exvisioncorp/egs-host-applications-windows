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
                if (CanRaiseDebbugerBreak) { Debugger.Break(); System.Windows.Forms.MessageBox.Show(ex.Message); }
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