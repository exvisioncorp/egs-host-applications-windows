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
    public class HidAccessPropertyOptional<T> : HidAccessPropertyBase
        where T : HidAccessPropertyOptionalTypeParameterBase, new()
    {
        [DataMember]
        public OptionalValue<T> OptionalValue { get; set; }

        public byte ValueOfSelectedItem
        {
            get { return OptionalValue.SelectedItem.Value; }
            set
            {
                var hr = OptionalValue.SelectSingleItemByPredicate(e => e.Value == valueInByteArrayData);
                if (hr)
                {
                    ByteArrayData[OneValueOffsetInByteArrayData] = ValueOfSelectedItem;
                    RaiseValueUpdatedOnGetHidFeatureReport();
                }
            }
        }

        byte valueInByteArrayData { get { return ByteArrayData[OneValueOffsetInByteArrayData]; } }

        internal override void RaiseValueUpdatedOnGetHidFeatureReport()
        {
            OptionalValue.SelectSingleItemByPredicate(e => e.Value == valueInByteArrayData);
            // OnValueUpdated is called by the above line.
        }

        public HidAccessPropertyOptional()
        {
            OptionalValue = new OptionalValue<T>();
        }

        protected virtual void OnOptionalValueSelectedItemChanged(EventArgs e)
        {
            if (OptionalValue.Options.Count == 0) { return; }
            if (OptionalValue.SelectedItem == null) { OptionalValue.SelectedIndex = 0; }
            if (valueInByteArrayData != ValueOfSelectedItem)
            {
                ByteArrayData[OneValueOffsetInByteArrayData] = OptionalValue.SelectedItem.Value;
                OnValueUpdated();
            }
        }

        public void InitializeOnceAtStartup()
        {
            OptionalValue.SelectedItemChanged += (sender, e) => { OnOptionalValueSelectedItemChanged(e); };
            OnOptionalValueSelectedItemChanged(EventArgs.Empty);
        }
    }
}
