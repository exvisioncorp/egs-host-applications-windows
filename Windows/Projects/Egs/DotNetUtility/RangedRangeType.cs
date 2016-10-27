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
                if (CanRaiseDebbugerBreak) { Debugger.Break(); System.Windows.Forms.MessageBox.Show(ex.Message); }
                return false;
            }
        }
        public override string ToString()
        {
            return "From=" + _From + ", To=" + _To + ", Minimum=" + _Minimum + ", Maximum=" + _Maximum;
        }
    }
}