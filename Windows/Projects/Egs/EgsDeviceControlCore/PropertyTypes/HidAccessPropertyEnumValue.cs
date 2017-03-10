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

    [DataContract]
    public class HidAccessPropertyEnumValue<T> : HidAccessPropertyBase
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
        internal byte ValueInByteArrayData { get { return ByteArrayData[OneValueOffsetInByteArrayData]; } }

        internal override void RaiseValueUpdatedOnGetHidFeatureReport()
        {
            var hr = OptionalValue.SelectSingleItemByPredicate(e => Convert.ToByte(e.Value) == ValueInByteArrayData);
            if (hr)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                throw new ArgumentOutOfRangeException();
            }
        }

        public static implicit operator T(HidAccessPropertyEnumValue<T> self)
        {
            return self.Value;
        }

        public HidAccessPropertyEnumValue()
        {
            OptionalValue = new OptionalValue<ValueWithDescription<T>>();
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
            var valueAsByte = Convert.ToByte(Value);
            if (ValueInByteArrayData != valueAsByte)
            {
                ByteArrayData[OneValueOffsetInByteArrayData] = valueAsByte;
                OnValueUpdated();
            }
        }
    }
}
