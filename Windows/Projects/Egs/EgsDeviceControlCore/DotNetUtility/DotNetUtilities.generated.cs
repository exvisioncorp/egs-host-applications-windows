// NOTE: impossible: include file = "../../DotNetUtility/" + "DuplicatedProcessStartBlocking.cs"
namespace Egs
{
namespace DotNetUtility
{
    using System;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    /// <summary>Information about DPI of a display</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct Dpi
    {
        /// <summary>new Dpi(96, 96)</summary>
        public static readonly Dpi Default = new Dpi(96, 96);
        public double X { get; set; }
        public double Y { get; set; }
        public static bool operator ==(Dpi dpi1, Dpi dpi2) { return dpi1.X == dpi2.X && dpi1.Y == dpi2.Y; }
        public static bool operator !=(Dpi dpi1, Dpi dpi2) { return !(dpi1 == dpi2); }
        public bool Equals(Dpi other) { return X == other.X && Y == other.Y; }
        public override bool Equals(object obj) { if (ReferenceEquals(null, obj)) { return false; } else { return obj is Dpi && Equals((Dpi)obj); } }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }
        public override string ToString() { return string.Format(System.Globalization.CultureInfo.CurrentCulture, "({0},{1})", X, Y); }

        public Dpi(double x, double y)
            : this()
        {
            X = x;
            Y = y;
        }

        public static Dpi DpiFromHdcForTheEntireScreen
        {
            get
            {
                var ret = new Dpi();
                var hdcForTheEntireScreen = Win32.NativeMethods.GetDC(IntPtr.Zero);
                using (var g = System.Drawing.Graphics.FromHdc(hdcForTheEntireScreen))
                {
                    ret.X = g.DpiX;
                    ret.Y = g.DpiY;
                }
                Win32.NativeMethods.ReleaseDC(IntPtr.Zero, hdcForTheEntireScreen);
                return ret;
            }
        }

        static Dpi GetDpiFromGetDpiForMonitor(IntPtr hMonitor, Win32.MonitorDpiType dpiType)
        {
            try
            {
                var ret = new Dpi();
                uint dpiX = 0, dpiY = 0;
                Win32.NativeMethods.GetDpiForMonitor(hMonitor, dpiType, ref dpiX, ref dpiY);
                ret.X = dpiX;
                ret.Y = dpiY;
                return ret;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                Debug.WriteLine(ex.Message);
                return Dpi.Default;
            }
        }

        public static Dpi DpiFromGetDpiForMonitorOfPrimaryMonitorWithEffectiveDpi
        {
            get
            {
                var hMonitor = Win32.NativeMethods.MonitorFromWindow(IntPtr.Zero, Win32.NativeMethods.MonitorDefaultTo.MONITOR_DEFAULTTOPRIMARY);
                return GetDpiFromGetDpiForMonitor(hMonitor, Win32.MonitorDpiType.EffectiveDpi);
            }
        }

        public static Dpi DpiFromGetDpiForMonitorOfNearestMonitorWithAngularDpi
        {
            get
            {
                var hMonitor = Win32.NativeMethods.MonitorFromWindow(IntPtr.Zero, Win32.NativeMethods.MonitorDefaultTo.MONITOR_DEFAULTTOPRIMARY);
                return GetDpiFromGetDpiForMonitor(hMonitor, Win32.MonitorDpiType.AngularDpi);
            }
        }

        public static Dpi DpiFromGetDpiForMonitorOfNearestMonitorWithRawDpi
        {
            get
            {
                var hMonitor = Win32.NativeMethods.MonitorFromWindow(IntPtr.Zero, Win32.NativeMethods.MonitorDefaultTo.MONITOR_DEFAULTTOPRIMARY);
                return GetDpiFromGetDpiForMonitor(hMonitor, Win32.MonitorDpiType.RawDpi);
            }
        }

        public static System.Drawing.Size GetPrimaryScreenPhysicalPixelResolution()
        {
            var ret = new System.Drawing.Size();
            var hdcForTheEntireScreen = Win32.NativeMethods.GetDC(IntPtr.Zero);
            const int DESKTOPVERTRES = 117;
            const int DESKTOPHORZRES = 118;
            ret.Width = Win32.NativeMethods.GetDeviceCaps(hdcForTheEntireScreen, DESKTOPHORZRES);
            ret.Height = Win32.NativeMethods.GetDeviceCaps(hdcForTheEntireScreen, DESKTOPVERTRES);
            Win32.NativeMethods.ReleaseDC(IntPtr.Zero, hdcForTheEntireScreen);
            return ret;
        }

        public System.Drawing.Point ScaledCursorPosition
        {
            get
            {
                var winFormsCursorPosition = System.Windows.Forms.Cursor.Position;
                var ret = new System.Drawing.Point((int)(winFormsCursorPosition.X * Default.X / X), (int)(winFormsCursorPosition.Y * Default.Y / Y));
                return ret;
            }
        }

        public System.Drawing.Rectangle ScaledPrimaryScreenBounds
        {
            get
            {
                var winFormsScreenPrimaryScreenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                var ret = new System.Drawing.Rectangle(
                    (int)(winFormsScreenPrimaryScreenBounds.X * Default.X / X),
                    (int)(winFormsScreenPrimaryScreenBounds.Y * Default.Y / Y),
                    (int)(winFormsScreenPrimaryScreenBounds.Width * Default.X / X),
                    (int)(winFormsScreenPrimaryScreenBounds.Height * Default.Y / Y));
                return ret;
            }
        }

        public System.Drawing.Rectangle ScaledPrimaryScreenWorkindArea
        {
            get
            {
                var winFormsScreenPrimaryScreenWorkingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                var ret = new System.Drawing.Rectangle(
                    (int)(winFormsScreenPrimaryScreenWorkingArea.X * Default.X / X),
                    (int)(winFormsScreenPrimaryScreenWorkingArea.Y * Default.Y / Y),
                    (int)(winFormsScreenPrimaryScreenWorkingArea.Width * Default.X / X),
                    (int)(winFormsScreenPrimaryScreenWorkingArea.Height * Default.Y / Y));
                return ret;
            }
        }
    }
}

namespace DotNetUtility
{
    using System;
    using System.Runtime.InteropServices;
    using System.Reflection;
    using System.Threading;
    using System.Security.AccessControl;
    using System.Security.Principal;

    public static class DuplicatedProcessStartBlocking
    {
        static System.Threading.Mutex mutex = null;
        //static System.Diagnostics.Process currentProcess = null;

        /// <summary>
        /// If it return false, exit program.  (ex. call if (Application.Current != null) { Application.Current.Shutdown(); })
        /// </summary>
        public static bool TryGetMutexOnTheBeginningOfApplicationConstructor()
        {
            var entryAssemblyFullName = Assembly.GetEntryAssembly().FullName;

            mutex = new System.Threading.Mutex(false, entryAssemblyFullName);
            // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
            // edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            mutex.SetAccessControl(securitySettings);

            if (mutex.WaitOne(TimeSpan.Zero, false))
            {
                //currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                //currentProcess.Exited += (sender, e) => { ReleaseMutex(); };
                return true;
            }

            mutex.Close();
            mutex = null;
            return false;
        }

        public static void ReleaseMutex()
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Close();
                mutex = null;
            }
        }
    }
}
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

namespace Win32
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Identifies dots per inch (dpi) type.  Refer to https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
    /// </summary>
    public enum MonitorDpiType
    {
        /// <summary>
        /// MDT_Effective_DPI
        /// <para>Effective DPI that incorporates accessibility overrides and matches what Desktop Window Manage (DWM) uses to scale desktop applications.</para>
        /// </summary>
        EffectiveDpi = 0,
        /// <summary>
        /// MDT_Angular_DPI
        /// <para>DPI that ensures rendering at a compliant angular resolution on the screen, without incorporating accessibility overrides.</para>
        /// </summary>
        AngularDpi = 1,
        /// <summary>
        /// MDT_Raw_DPI
        /// <para>Linear DPI of the screen as measures on the screen itself.</para>
        /// </summary>
        RawDpi = 2,
        /// <summary>
        /// MDT_Default
        /// </summary>
        Default = EffectiveDpi,
    }

    internal static partial class NativeMethods
    {
        /// <summary>
        /// The GetDC function retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen.  
        /// You can use the returned handle in subsequent GDI functions to draw in the DC.  
        /// The device context is an opaque data structure, whose values are used internally by GDI.  
        /// The GetDCEx function is an extension to GetDC, which gives an application more control over how and whether clipping occurs in the client area.
        /// </summary>
        /// <param name="hWnd">
        /// hWnd [in] A handle to the window whose DC is to be retrieved.  If this value is NULL, GetDC retrieves the DC for the entire screen.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the DC for the specified window's client area.  
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        extern internal static IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        extern internal static int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll", ExactSpelling = true)]
        extern internal static int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        internal enum GetWindowCmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }
        [DllImport("user32.dll")]
        extern internal static IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

        /// <summary>
        /// Refer to https://msdn.microsoft.com/ja-jp/library/cc411206.aspx
        /// </summary>
        [Flags]
        internal enum SetWindowPosFlags : uint
        {
            // NOTE: not completed
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_FRAMECHANGED = 0x0020,
            SWP_SHOWWINDOW = 0x0040,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,

            IgnoreResize = 0x0001,
            IgnoreMove = 0x0002,
            IgnoreZOrder = 0x0004,
            DoNotRedraw = 0x0008,
            DoNotActivate = 0x0010,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            ShowWindow = 0x0040,
            HideWindow = 0x0080,
            DoNotCopyBits = 0x0100,
            DoNotChangeOwnerZOrder = 0x0200,
            DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            DeferErase = 0x2000,
            SynchronousWindowPosition = 0x4000,
        }
        /// <summary>
        ///     Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered
        ///     according to their appearance on the screen. The topmost window receives the highest rank and is the first window
        ///     in the Z order.
        ///     <para>See https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545%28v=vs.85%29.aspx for more information.</para>
        /// </summary>
        /// <param name="hWnd">C++ ( hWnd [in]. Type: HWND )<br />A handle to the window.</param>
        /// <param name="hWndInsertAfter">
        ///     C++ ( hWndInsertAfter [in, optional]. Type: HWND )<br />A handle to the window to precede the positioned window in
        ///     the Z order. This parameter must be a window handle or one of the following values.
        ///     <list type="table">
        ///     <itemheader>
        ///         <term>HWND placement</term><description>Window to precede placement</description>
        ///     </itemheader>
        ///     <item>
        ///         <term>HWND_BOTTOM ((HWND)1)</term>
        ///         <description>
        ///         Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost
        ///         window, the window loses its topmost status and is placed at the bottom of all other windows.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>HWND_NOTOPMOST ((HWND)-2)</term>
        ///         <description>
        ///         Places the window above all non-topmost windows (that is, behind all topmost windows). This
        ///         flag has no effect if the window is already a non-topmost window.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>HWND_TOP ((HWND)0)</term><description>Places the window at the top of the Z order.</description>
        ///     </item>
        ///     <item>
        ///         <term>HWND_TOPMOST ((HWND)-1)</term>
        ///         <description>
        ///         Places the window above all non-topmost windows. The window maintains its topmost position
        ///         even when it is deactivated.
        ///         </description>
        ///     </item>
        ///     </list>
        ///     <para>For more information about how this parameter is used, see the following Remarks section.</para>
        /// </param>
        /// <param name="x">C++ ( X [in]. Type: int )<br />The new position of the left side of the window, in client coordinates.</param>
        /// <param name="y">C++ ( Y [in]. Type: int )<br />The new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">C++ ( cx [in]. Type: int )<br />The new width of the window, in pixels.</param>
        /// <param name="cy">C++ ( cy [in]. Type: int )<br />The new height of the window, in pixels.</param>
        /// <param name="uFlags">
        ///     C++ ( uFlags [in]. Type: UINT )<br />The window sizing and positioning flags. This parameter can be a combination
        ///     of the following values.
        ///     <list type="table">
        ///     <itemheader>
        ///         <term>HWND sizing and positioning flags</term>
        ///         <description>Where to place and size window. Can be a combination of any</description>
        ///     </itemheader>
        ///     <item>
        ///         <term>SWP_ASYNCWINDOWPOS (0x4000)</term>
        ///         <description>
        ///         If the calling thread and the thread that owns the window are attached to different input
        ///         queues, the system posts the request to the thread that owns the window. This prevents the calling
        ///         thread from blocking its execution while other threads process the request.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_DEFERERASE (0x2000)</term>
        ///         <description>Prevents generation of the WM_SYNCPAINT message. </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_DRAWFRAME (0x0020)</term>
        ///         <description>Draws a frame (defined in the window's class description) around the window.</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_FRAMECHANGED (0x0020)</term>
        ///         <description>
        ///         Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message
        ///         to the window, even if the window's size is not being changed. If this flag is not specified,
        ///         WM_NCCALCSIZE is sent only when the window's size is being changed
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_HIDEWINDOW (0x0080)</term><description>Hides the window.</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOACTIVATE (0x0010)</term>
        ///         <description>
        ///         Does not activate the window. If this flag is not set, the window is activated and moved to
        ///         the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
        ///         parameter).
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOCOPYBITS (0x0100)</term>
        ///         <description>
        ///         Discards the entire contents of the client area. If this flag is not specified, the valid
        ///         contents of the client area are saved and copied back into the client area after the window is sized or
        ///         repositioned.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOMOVE (0x0002)</term>
        ///         <description>Retains the current position (ignores X and Y parameters).</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOOWNERZORDER (0x0200)</term>
        ///         <description>Does not change the owner window's position in the Z order.</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOREDRAW (0x0008)</term>
        ///         <description>
        ///         Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies
        ///         to the client area, the nonclient area (including the title bar and scroll bars), and any part of the
        ///         parent window uncovered as a result of the window being moved. When this flag is set, the application
        ///         must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOREPOSITION (0x0200)</term><description>Same as the SWP_NOOWNERZORDER flag.</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOSENDCHANGING (0x0400)</term>
        ///         <description>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOSIZE (0x0001)</term>
        ///         <description>Retains the current size (ignores the cx and cy parameters).</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOZORDER (0x0004)</term>
        ///         <description>Retains the current Z order (ignores the hWndInsertAfter parameter).</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_SHOWWINDOW (0x0040)</term><description>Displays the window.</description>
        ///     </item>
        ///     </list>
        /// </param>
        /// <returns><c>true</c> or nonzero if the function succeeds, <c>false</c> or zero otherwise or if function fails.</returns>
        /// <remarks>
        ///     <para>
        ///     As part of the Vista re-architecture, all services were moved off the interactive desktop into Session 0.
        ///     hwnd and window manager operations are only effective inside a session and cross-session attempts to manipulate
        ///     the hwnd will fail. For more information, see The Windows Vista Developer Story: Application Compatibility
        ///     Cookbook.
        ///     </para>
        ///     <para>
        ///     If you have changed certain window data using SetWindowLong, you must call SetWindowPos for the changes to
        ///     take effect. Use the following combination for uFlags: SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER |
        ///     SWP_FRAMECHANGED.
        ///     </para>
        ///     <para>
        ///     A window can be made a topmost window either by setting the hWndInsertAfter parameter to HWND_TOPMOST and
        ///     ensuring that the SWP_NOZORDER flag is not set, or by setting a window's position in the Z order so that it is
        ///     above any existing topmost windows. When a non-topmost window is made topmost, its owned windows are also made
        ///     topmost. Its owners, however, are not changed.
        ///     </para>
        ///     <para>
        ///     If neither the SWP_NOACTIVATE nor SWP_NOZORDER flag is specified (that is, when the application requests that
        ///     a window be simultaneously activated and its position in the Z order changed), the value specified in
        ///     hWndInsertAfter is used only in the following circumstances.
        ///     </para>
        ///     <list type="bullet">
        ///     <item>Neither the HWND_TOPMOST nor HWND_NOTOPMOST flag is specified in hWndInsertAfter. </item>
        ///     <item>The window identified by hWnd is not the active window. </item>
        ///     </list>
        ///     <para>
        ///     An application cannot activate an inactive window without also bringing it to the top of the Z order.
        ///     Applications can change an activated window's position in the Z order without restrictions, or it can activate
        ///     a window and then move it to the top of the topmost or non-topmost windows.
        ///     </para>
        ///     <para>
        ///     If a topmost window is repositioned to the bottom (HWND_BOTTOM) of the Z order or after any non-topmost
        ///     window, it is no longer topmost. When a topmost window is made non-topmost, its owners and its owned windows
        ///     are also made non-topmost windows.
        ///     </para>
        ///     <para>
        ///     A non-topmost window can own a topmost window, but the reverse cannot occur. Any window (for example, a
        ///     dialog box) owned by a topmost window is itself made a topmost window, to ensure that all owned windows stay
        ///     above their owner.
        ///     </para>
        ///     <para>
        ///     If an application is not in the foreground, and should be in the foreground, it must call the
        ///     SetForegroundWindow function.
        ///     </para>
        ///     <para>
        ///     To use SetWindowPos to bring a window to the top, the process that owns the window must have
        ///     SetForegroundWindow permission.
        ///     </para>
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, [MarshalAs(UnmanagedType.Bool)]bool bRepaint);

        internal enum MonitorDefaultTo
        {
            MONITOR_DEFAULTTONULL = 0,
            MONITOR_DEFAULTTOPRIMARY = 1,
            MONITOR_DEFAULTTONEAREST = 2,
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        extern internal static IntPtr MonitorFromWindow(IntPtr hWnd, MonitorDefaultTo dwFlags);

        [DllImport("SHCore.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        extern internal static void GetDpiForMonitor(IntPtr hMonitor, MonitorDpiType dpiType, ref uint dpiX, ref uint dpiY);

        // Winuser.h
        internal const int WM_DPICHANGED = 0x02E0;
        internal const int WM_INPUT = 0x000000FF;
        internal const int WM_DEVICECHANGE = 0x0219;
        internal static ushort ToLoWord(this IntPtr dword) { return (ushort)((uint)dword & 0xffff); }
        internal static ushort ToHiWord(this IntPtr dword) { return (ushort)((uint)dword >> 16); }
    }
}

}
