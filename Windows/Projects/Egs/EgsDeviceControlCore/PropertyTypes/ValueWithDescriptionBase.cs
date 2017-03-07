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
    using System.Globalization;
    using Egs;
    using Egs.EgsDeviceControlCore.Properties;
    using Egs.DotNetUtility;

    /// <summary>
    /// Value with Description, but only Description is defined in this base class.
    /// </summary>
    [DataContract]
    public abstract class ValueWithDescriptionBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        string _Description;
        /// <summary>
        /// The key to Resource.ResourceManager.GetString.
        /// </summary>
        public string DescriptionKey { get; set; }
        /// <summary>
        /// Unless Description is set, Description returns Resources.ResourceManager.GetString(DescriptionKey, Resources.Culture). 
        /// The list of key and value is described in some excel sheets.  
        /// EgsSourceCodeGeneration.exe runs in a build event, and it converts the excel sheets and generates the multi-lingual "Resource.resx" files.
        /// </summary>
        public virtual string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_Description) == false) { return _Description; }
                return string.IsNullOrEmpty(DescriptionKey) ? "" : Resources.ResourceManager.GetString(DescriptionKey, Resources.Culture);
            }
            set
            {
                _Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public event EventHandler ValueUpdated;
        protected virtual void OnValueUpdated()
        {
            var t = ValueUpdated; if (t != null) { t(this, EventArgs.Empty); }
            OnPropertyChanged("Value");
        }

        public override string ToString() { return Description; }
    }

    /// <summary>
    /// Value with Description.
    /// </summary>
    [DataContract]
    public class ValueWithDescription<T> : ValueWithDescriptionBase
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        T _Value;
        [DataMember]
        public T Value
        {
            get { return _Value; }
            set { _Value = value; OnValueUpdated(); }
        }

        public static implicit operator T(ValueWithDescription<T> self)
        {
            return self.Value;
        }
    }

    [DataContract]
    public class EnumValueWithDescriptionOptions<T> : ValueWithDescriptionBase
        where T : IComparable
    {
        [DataMember]
        public OptionalValue<ValueWithDescription<T>> OptionalValue { get; set; }

        public T Value
        {
            get { return OptionalValue.SelectedItem.Value; }
            set
            {
                var hr = OptionalValue.SelectSingleItemByPredicate(e => e.Value.Equals(value));
                if (hr == false)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static implicit operator T(EnumValueWithDescriptionOptions<T> self)
        {
            return self.Value;
        }

        public EnumValueWithDescriptionOptions()
        {
            OptionalValue = new OptionalValue<ValueWithDescription<T>>();
            OptionalValue.SelectedIndexChanged += delegate { OnValueUpdated(); };
        }

        [Obsolete]
        public static EnumValueWithDescriptionOptions<T> CreateDefaults()
        {
            var ret = new EnumValueWithDescriptionOptions<T>();
            try
            {
                var names = Enum.GetNames(typeof(T));
                foreach (var name in names)
                {
                    var newItem = new ValueWithDescription<T>();
                    newItem.Value = (T)Enum.Parse(typeof(T), name);
                    newItem.DescriptionKey = typeof(T).Name + "_" + name + "_Description";
                    ret.OptionalValue.Options.Add(newItem);
                }
            }
            catch (Exception ex)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Console.WriteLine(ex.Message);
                throw;
            }
            return ret;
        }
    }
}
