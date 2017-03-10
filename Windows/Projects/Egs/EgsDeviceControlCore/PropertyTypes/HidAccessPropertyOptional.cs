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

    public interface ITypeParameterOfHidAccessPropertyOptional
    {
        byte ConvertValueToByte();
    }

    public class HidAccessPropertyOptionalTypeParameterBase<T> : ValueWithDescription<T>, ITypeParameterOfHidAccessPropertyOptional
        where T : IComparable
    {
        byte ITypeParameterOfHidAccessPropertyOptional.ConvertValueToByte() { return Convert.ToByte(Value); }
    }

    [DataContract]
    public class HidAccessPropertyOptional<T, V> : HidAccessPropertyBase
        where T : HidAccessPropertyOptionalTypeParameterBase<V>, ITypeParameterOfHidAccessPropertyOptional, new()
        where V : IComparable
    {
        [DataMember]
        public OptionalValue<T> OptionalValue { get; set; }

        public T SelectedItem
        {
            get { return OptionalValue.SelectedItem; }
            internal set
            {
                var hr = OptionalValue.SelectSingleItemByPredicate(e => e.ConvertValueToByte() == value.ConvertValueToByte());
                if (hr == false)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
        public V Value
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
        internal byte ValueInByteArrayData { get { return ByteArrayData[OneValueOffsetInByteArrayData]; } }

        internal override void RaiseValueUpdatedOnGetHidFeatureReport()
        {
            var hr = OptionalValue.SelectSingleItemByPredicate(e => e.ConvertValueToByte() == ValueInByteArrayData);
            if (hr)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                throw new ArgumentOutOfRangeException();
            }
        }

        public HidAccessPropertyOptional()
        {
            OptionalValue = new OptionalValue<T>();
            OptionalValue.SelectedItemChanged += (sender, e) => { OnOptionalValueSelectedItemChanged(e); };
        }

        protected virtual void OnOptionalValueSelectedItemChanged(EventArgs e)
        {
            if (OptionalValue.Options.Count == 0) { return; }
            if (OptionalValue.SelectedItem == null)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                OptionalValue.SelectedIndex = 0;
            }
            var valueAsByte = SelectedItem.ConvertValueToByte();
            if (ValueInByteArrayData != valueAsByte)
            {
                ByteArrayData[OneValueOffsetInByteArrayData] = valueAsByte;
                OnValueUpdated();
            }
        }
    }
}
