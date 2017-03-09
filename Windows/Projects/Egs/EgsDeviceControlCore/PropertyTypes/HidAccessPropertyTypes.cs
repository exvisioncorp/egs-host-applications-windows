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
    /// The classes derived from this class have information to set settings and get state of the device.
    /// </summary>
    [DataContract]
    public abstract class HidAccessPropertyBase : ValueWithDescriptionBase
    {
        internal static Dictionary<string, HidAccessPropertyPrimitiveTypeIds> TypeAbbreviationName_EnumValue_Dict { get; private set; }
        internal static Dictionary<Type, string> Type_TypeAbbreviationName_Dict { get; private set; }
        internal const string TypeNameStringBool = "bool";
        internal const string TypeNameStringByte = "byte";
        internal const string TypeNameStringShort = "short";
        internal const string TypeNameStringUshort = "ushort";
        internal const string TypeNameStringInt = "int";
        internal const string TypeNameStringUint = "uint";
        internal const string TypeNameStringFloat = "float";
        static HidAccessPropertyBase()
        {
            TypeAbbreviationName_EnumValue_Dict = new Dictionary<string, HidAccessPropertyPrimitiveTypeIds>();
            TypeAbbreviationName_EnumValue_Dict[TypeNameStringBool] = HidAccessPropertyPrimitiveTypeIds.TypeId_bool;
            TypeAbbreviationName_EnumValue_Dict[TypeNameStringByte] = HidAccessPropertyPrimitiveTypeIds.TypeId_byte;
            TypeAbbreviationName_EnumValue_Dict[TypeNameStringShort] = HidAccessPropertyPrimitiveTypeIds.TypeId_short;
            TypeAbbreviationName_EnumValue_Dict[TypeNameStringUshort] = HidAccessPropertyPrimitiveTypeIds.TypeId_ushort;
            TypeAbbreviationName_EnumValue_Dict[TypeNameStringInt] = HidAccessPropertyPrimitiveTypeIds.TypeId_int;
            TypeAbbreviationName_EnumValue_Dict[TypeNameStringUint] = HidAccessPropertyPrimitiveTypeIds.TypeId_uint;
            TypeAbbreviationName_EnumValue_Dict[TypeNameStringFloat] = HidAccessPropertyPrimitiveTypeIds.TypeId_float;
            Type_TypeAbbreviationName_Dict = new Dictionary<Type, string>();
            Type_TypeAbbreviationName_Dict[typeof(System.Boolean)] = TypeNameStringBool;
            Type_TypeAbbreviationName_Dict[typeof(System.Byte)] = TypeNameStringByte;
            Type_TypeAbbreviationName_Dict[typeof(System.Int16)] = TypeNameStringShort;
            Type_TypeAbbreviationName_Dict[typeof(System.UInt16)] = TypeNameStringUshort;
            Type_TypeAbbreviationName_Dict[typeof(System.Int32)] = TypeNameStringInt;
            Type_TypeAbbreviationName_Dict[typeof(System.UInt32)] = TypeNameStringUint;
            Type_TypeAbbreviationName_Dict[typeof(System.Single)] = TypeNameStringFloat;
        }

        internal const int ByteArrayDataLength = 64;
        internal const int TypeIdOffsetInByteArrayData = 4;
        internal const int DataLengthOffsetInByteArrayData = 8;
        internal const int SendingDataOffsetInDataOffsetInByteArrayData = 12;
        internal const int OneValueOffsetInByteArrayData = 16;

        // NOTE: About from ReportId to SendingDataOffsetInData, initial values (when this object is constructed) are used.  When users access get property of the value data, ByteArrayData is always converted and returned.
        internal byte[] ByteArrayData { get; set; }

        HidReportIds _ReportIdAsHidReportKind;
        internal HidReportIds ReportIdAsHidReportKind
        {
            get { return _ReportIdAsHidReportKind; }
            set { ByteArrayData[0] = (byte)value; _ReportIdAsHidReportKind = value; }
        }
        byte _ReportId;
        internal byte ReportId
        {
            get { return _ReportId; }
            set { ByteArrayData[0] = value; _ReportId = value; }
        }
        byte _MessageId;
        internal byte MessageId
        {
            get { return _MessageId; }
            set { ByteArrayData[1] = value; _MessageId = value; }
        }
        byte _CategoryId;
        internal byte CategoryId
        {
            get { return _CategoryId; }
            set { ByteArrayData[2] = value; _CategoryId = value; }
        }
        byte _PropertyId;
        internal byte PropertyId
        {
            get { return _PropertyId; }
            set { ByteArrayData[3] = value; _PropertyId = value; }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        string _ValueTypeOnDevice;
        internal string ValueTypeOnDevice
        {
            get { return _ValueTypeOnDevice; }
            set
            {
                var typeIdAsInt = (int)TypeAbbreviationName_EnumValue_Dict[value];
                var bytes = BitConverter.GetBytes(typeIdAsInt);
                bytes.CopyTo(ByteArrayData, TypeIdOffsetInByteArrayData);
                _ValueTypeOnDevice = value;
            }
        }
        internal int TypeId
        {
            get { return (int)TypeAbbreviationName_EnumValue_Dict[_ValueTypeOnDevice]; }
        }
        int _DataLength;
        internal int DataLength
        {
            get { return _DataLength; }
            set { var bytes = BitConverter.GetBytes(value); bytes.CopyTo(ByteArrayData, DataLengthOffsetInByteArrayData); _DataLength = value; }
        }
        int _SendingDataOffsetInData;
        internal int SendingDataOffsetInData
        {
            get { return _SendingDataOffsetInData; }
            set { var bytes = BitConverter.GetBytes(value); bytes.CopyTo(ByteArrayData, SendingDataOffsetInDataOffsetInByteArrayData); _SendingDataOffsetInData = value; }
        }

        internal bool IsReadOnly { get; set; }
        internal string NameOfProperty { get; set; }
        public Version AvailableFirmwareVersion { get; internal set; }

        internal virtual void RaiseValueUpdatedOnGetHidFeatureReport()
        {
            OnValueUpdated();
        }

        internal HidAccessPropertyBase()
        {
            ByteArrayData = new byte[ByteArrayDataLength];
        }
    }

    [DataContract]
    public class HidAccessPropertyBoolean : HidAccessPropertyBase
    {
        [DataMember]
        public bool Value
        {
            get { return BitConverter.ToBoolean(ByteArrayData, OneValueOffsetInByteArrayData); }
            set
            {
                var bytes = BitConverter.GetBytes(value);
                bytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData);
                OnValueUpdated();
            }
        }
    }

    [DataContract]
    public class HidAccessPropertyByte : HidAccessPropertyBase
    {
        [DataMember]
        public byte Value
        {
            get { return ByteArrayData[OneValueOffsetInByteArrayData]; }
            set
            {
                ByteArrayData[OneValueOffsetInByteArrayData] = value;
                OnValueUpdated();
            }
        }
    }

    [DataContract]
    public class HidAccessPropertyInt16 : HidAccessPropertyBase
    {
        [DataMember]
        public short Value
        {
            get { return BitConverter.ToInt16(ByteArrayData, OneValueOffsetInByteArrayData); }
            set
            {
                var bytes = BitConverter.GetBytes(value);
                bytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData);
                OnValueUpdated();
            }
        }
    }

    [DataContract]
    public class HidAccessPropertyUInt16 : HidAccessPropertyBase
    {
        [DataMember]
        public ushort Value
        {
            get { return BitConverter.ToUInt16(ByteArrayData, OneValueOffsetInByteArrayData); }
            set
            {
                var bytes = BitConverter.GetBytes(value);
                bytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData);
                OnValueUpdated();
            }
        }
    }

    [DataContract]
    public class HidAccessPropertyInt32 : HidAccessPropertyBase
    {
        [DataMember]
        public int Value
        {
            get { return BitConverter.ToInt32(ByteArrayData, OneValueOffsetInByteArrayData); }
            set
            {
                var bytes = BitConverter.GetBytes(value);
                bytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData);
                OnValueUpdated();
            }
        }
    }

    [DataContract]
    public class HidAccessPropertyInt32Array : HidAccessPropertyBase
    {
        [DataMember]
        public int[] Value
        {
            get
            {
                var ret = new int[DataLength];
                for (int i = 0; i < DataLength; i++)
                {
                    ret[i] = BitConverter.ToInt32(ByteArrayData, OneValueOffsetInByteArrayData + i * 4);
                }
                return ret;
            }
            set
            {
                for (int i = 0; i < DataLength; i++)
                {
                    var bytes = BitConverter.GetBytes(value[i]);
                    bytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + i * 4);
                }
                OnValueUpdated();
            }
        }
    }

    [DataContract]
    public class HidAccessPropertyUInt32 : HidAccessPropertyBase
    {
        [DataMember]
        public uint Value
        {
            get { return BitConverter.ToUInt32(ByteArrayData, OneValueOffsetInByteArrayData); }
            set
            {
                var bytes = BitConverter.GetBytes(value);
                bytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData);
                OnValueUpdated();
            }
        }
    }

    [DataContract]
    public class HidAccessPropertySingle : HidAccessPropertyBase
    {
        [DataMember]
        public float Value
        {
            get { return BitConverter.ToSingle(ByteArrayData, OneValueOffsetInByteArrayData); }
            set
            {
                var bytes = BitConverter.GetBytes(value);
                bytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData);
                OnValueUpdated();
            }
        }
    }

    [DataContract]
    public class HidAccessPropertySingleArray : HidAccessPropertyBase
    {
        [DataMember]
        public float[] Value
        {
            get
            {
                var ret = new float[DataLength];
                for (int i = 0; i < DataLength; i++)
                {
                    ret[i] = BitConverter.ToSingle(ByteArrayData, OneValueOffsetInByteArrayData + i * 4);
                }
                return ret;
            }
            set
            {
                for (int i = 0; i < DataLength; i++)
                {
                    var bytes = BitConverter.GetBytes(value[i]);
                    bytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + i * 4);
                }
                OnValueUpdated();
            }
        }
        public float Value00 { get { return Value[0]; } set { var temp = Value; temp[0] = value; Value = temp; } }
        public float Value01 { get { return Value[1]; } set { var temp = Value; temp[1] = value; Value = temp; } }
        public float Value02 { get { return Value[2]; } set { var temp = Value; temp[2] = value; Value = temp; } }
        public float Value03 { get { return Value[3]; } set { var temp = Value; temp[3] = value; Value = temp; } }
        public float Value04 { get { return Value[4]; } set { var temp = Value; temp[4] = value; Value = temp; } }
        public float Value05 { get { return Value[5]; } set { var temp = Value; temp[5] = value; Value = temp; } }
        public float Value06 { get { return Value[6]; } set { var temp = Value; temp[6] = value; Value = temp; } }
        public float Value07 { get { return Value[7]; } set { var temp = Value; temp[7] = value; Value = temp; } }
        public float Value08 { get { return Value[8]; } set { var temp = Value; temp[8] = value; Value = temp; } }
        public float Value09 { get { return Value[9]; } set { var temp = Value; temp[9] = value; Value = temp; } }
        public float Value10 { get { return Value[10]; } set { var temp = Value; temp[10] = value; Value = temp; } }
        public float Value11 { get { return Value[11]; } set { var temp = Value; temp[11] = value; Value = temp; } }
    }

    [DataContract]
    public class HidAccessPropertyString : HidAccessPropertyBase
    {
        [DataMember]
        public string Value
        {
            get
            {
                var stringBytes = new byte[DataLength];
                for (int i = OneValueOffsetInByteArrayData; i < OneValueOffsetInByteArrayData + DataLength; i++)
                {
                    stringBytes[i - OneValueOffsetInByteArrayData] = ByteArrayData[i];
                }
                var ret = Encoding.ASCII.GetString(stringBytes);
                return ret;
            }
            set
            {
                // TODO: MUSTDO: test
                byte[] stringBytes;
                stringBytes = Encoding.ASCII.GetBytes(value);
                Trace.Assert(stringBytes.Length == DataLength);
                stringBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData);
                OnValueUpdated();
            }
        }
    }

    [DataContract]
    public class HidAccessPropertyPoint : HidAccessPropertyBase
    {
        [DataMember(Order = 10)]
        public RangedInt XRange { get; private set; }
        public int X
        {
            get
            {
                XRange.Value = BitConverter.ToInt32(ByteArrayData, OneValueOffsetInByteArrayData + 0);
                return XRange.Value;
            }
            set
            {
                XRange.Value = value;
                var XBytes = BitConverter.GetBytes(XRange.Value);
                XBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 0);
                OnValueUpdated();
                OnPropertyChanged(nameof(X));
            }
        }
        [DataMember(Order = 10)]
        public RangedInt YRange { get; private set; }
        public int Y
        {
            get
            {
                YRange.Value = BitConverter.ToInt32(ByteArrayData, OneValueOffsetInByteArrayData + 4);
                return YRange.Value;
            }
            set
            {
                YRange.Value = value;
                var YBytes = BitConverter.GetBytes(YRange.Value);
                YBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 4);
                OnValueUpdated();
                OnPropertyChanged(nameof(Y));
            }
        }

        [DataMember(Order = 100)]
        public System.Drawing.Point Value
        {
            get { return new System.Drawing.Point(X, Y); }
            internal set
            {
                // TODO: MUSTDO: test
                var XBytes = BitConverter.GetBytes((int)value.X);
                var YBytes = BitConverter.GetBytes((int)value.Y);
                XBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 0);
                YBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 4);
                XRange.Value = (int)value.X;
                YRange.Value = (int)value.Y;
                OnValueUpdated();
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Value));
            }
        }

        public HidAccessPropertyPoint()
        {
            XRange = new RangedInt(0, short.MinValue, short.MaxValue);
            YRange = new RangedInt(0, short.MinValue, short.MaxValue);
        }
    }

    [DataContract]
    public class HidAccessPropertySize : HidAccessPropertyBase
    {
        [DataMember(Order = 10)]
        public RangedInt WidthRange { get; private set; }
        public int Width
        {
            get
            {
                WidthRange.Value = BitConverter.ToInt32(ByteArrayData, OneValueOffsetInByteArrayData + 0);
                return WidthRange.Value;
            }
            set
            {
                WidthRange.Value = value;
                var widthBytes = BitConverter.GetBytes(WidthRange.Value);
                widthBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 0);
                OnValueUpdated();
                OnPropertyChanged(nameof(Width));
            }
        }
        [DataMember(Order = 10)]
        public RangedInt HeightRange { get; private set; }
        public int Height
        {
            get
            {
                HeightRange.Value = BitConverter.ToInt32(ByteArrayData, OneValueOffsetInByteArrayData + 4);
                return HeightRange.Value;
            }
            set
            {
                HeightRange.Value = value;
                var heightBytes = BitConverter.GetBytes(HeightRange.Value);
                heightBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 4);
                OnValueUpdated();
                OnPropertyChanged(nameof(Height));
            }
        }

        [DataMember(Order = 100)]
        public System.Drawing.Size Value
        {
            get { return new System.Drawing.Size(Width, Height); }
            internal set
            {
                // TODO: MUSTDO: test
                var WBytes = BitConverter.GetBytes((int)value.Width);
                var HBytes = BitConverter.GetBytes((int)value.Height);
                WBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 0);
                HBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 4);
                WidthRange.Value = (int)value.Width;
                HeightRange.Value = (int)value.Height;
                OnValueUpdated();
                OnPropertyChanged(nameof(Width));
                OnPropertyChanged(nameof(Height));
                OnPropertyChanged(nameof(Value));
            }
        }

        public HidAccessPropertySize()
        {
            WidthRange = new RangedInt(1, 1, short.MaxValue);
            HeightRange = new RangedInt(1, 1, short.MaxValue);
        }
    }

    [DataContract]
    public class HidAccessPropertyRect : HidAccessPropertyBase
    {
        [DataMember(Order = 10)]
        public RangedInt XRange { get; private set; }
        public int X
        {
            get
            {
                XRange.Value = BitConverter.ToInt32(ByteArrayData, OneValueOffsetInByteArrayData + 0);
                return XRange.Value;
            }
            set
            {
                XRange.Value = value;
                var XBytes = BitConverter.GetBytes(XRange.Value);
                XBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 0);
                OnValueUpdated();
                OnPropertyChanged(nameof(X));
            }
        }
        [DataMember(Order = 10)]
        public RangedInt YRange { get; private set; }
        public int Y
        {
            get
            {
                YRange.Value = BitConverter.ToInt32(ByteArrayData, OneValueOffsetInByteArrayData + 4);
                return YRange.Value;
            }
            set
            {
                YRange.Value = value;
                var YBytes = BitConverter.GetBytes(YRange.Value);
                YBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 4);
                OnValueUpdated();
                OnPropertyChanged(nameof(Y));
            }
        }
        [DataMember(Order = 20)]
        public RangedInt WidthRange { get; private set; }
        public int Width
        {
            get
            {
                WidthRange.Value = BitConverter.ToInt32(ByteArrayData, OneValueOffsetInByteArrayData + 8);
                return WidthRange.Value;
            }
            set
            {
                WidthRange.Value = value;
                var widthBytes = BitConverter.GetBytes(WidthRange.Value);
                widthBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 8);
                OnValueUpdated();
                OnPropertyChanged(nameof(Width));
            }
        }
        [DataMember(Order = 20)]
        public RangedInt HeightRange { get; private set; }
        public int Height
        {
            get
            {
                HeightRange.Value = BitConverter.ToInt32(ByteArrayData, OneValueOffsetInByteArrayData + 12);
                return HeightRange.Value;
            }
            set
            {
                HeightRange.Value = value;
                var heightBytes = BitConverter.GetBytes(HeightRange.Value);
                heightBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 12);
                OnValueUpdated();
                OnPropertyChanged(nameof(Height));
            }
        }

        [DataMember(Order = 100)]
        public System.Drawing.Rectangle Value
        {
            get { return new System.Drawing.Rectangle(X, Y, Width, Height); }
            internal set
            {
                // TODO: MUSTDO: test
                var XBytes = BitConverter.GetBytes((int)value.X);
                var YBytes = BitConverter.GetBytes((int)value.Y);
                var WBytes = BitConverter.GetBytes((int)value.Width);
                var HBytes = BitConverter.GetBytes((int)value.Height);
                XBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 0);
                YBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 4);
                WBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 8);
                HBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 12);
                XRange.Value = (int)value.X;
                YRange.Value = (int)value.Y;
                WidthRange.Value = (int)value.Width;
                HeightRange.Value = (int)value.Height;
                OnValueUpdated();
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Width));
                OnPropertyChanged(nameof(Height));
                OnPropertyChanged(nameof(Value));
            }
        }

        public HidAccessPropertyRect()
            : this(new RangedInt(0, short.MinValue, short.MaxValue),
                new RangedInt(0, short.MinValue, short.MaxValue),
                new RangedInt(1, 1, short.MaxValue),
                new RangedInt(1, 1, short.MaxValue))
        {
        }
        public HidAccessPropertyRect(RangedInt newXRange, RangedInt newYRange, RangedInt newWidthRange, RangedInt newHeightRange)
        {
            XRange = newXRange;
            YRange = newYRange;
            WidthRange = newWidthRange;
            HeightRange = newHeightRange;
            var XBytes = BitConverter.GetBytes((int)newXRange.Value);
            var YBytes = BitConverter.GetBytes((int)newYRange.Value);
            var WBytes = BitConverter.GetBytes((int)newWidthRange.Value);
            var HBytes = BitConverter.GetBytes((int)newHeightRange.Value);
            XBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 0);
            YBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 4);
            WBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 8);
            HBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 12);
        }
    }

    [DataContract]
    public class HidAccessPropertyRangedInt : HidAccessPropertyBase
    {
        [DataMember]
        public RangedInt RangedValue { get; internal set; }

        public HidAccessPropertyRangedInt()
        {
            RangedValue = new RangedInt();
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            var valueBytes = BitConverter.GetBytes(RangedValue.Value);
            valueBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData);
            OnValueUpdated();
        }

        public void InitializeOnceAtStartup()
        {
            RangedValue.ValueChanged += (sender, e) => { OnValueChanged(e); };
            OnValueChanged(EventArgs.Empty);
        }
    }

    [DataContract]
    public class HidAccessPropertyRangedSingle : HidAccessPropertyBase
    {
        [DataMember]
        public RangedFloat RangedValue { get; internal set; }

        public HidAccessPropertyRangedSingle()
        {
            RangedValue = new RangedFloat();
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            var valueBytes = BitConverter.GetBytes(RangedValue.Value);
            valueBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData);
            OnValueUpdated();
        }

        public void InitializeOnceAtStartup()
        {
            RangedValue.ValueChanged += (sender, e) => { OnValueChanged(e); };
            OnValueChanged(EventArgs.Empty);
        }
    }

    [DataContract]
    public class HidAccessPropertyRangedIntRange : HidAccessPropertyBase
    {
        [DataMember]
        public RangedIntRange RangedRange { get; internal set; }

        public HidAccessPropertyRangedIntRange()
        {
            RangedRange = new RangedIntRange();
        }

        protected virtual void OnValueFromChanged(EventArgs e)
        {
            var fromBytes = BitConverter.GetBytes(RangedRange.From);
            fromBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData);
            OnValueUpdated();
        }

        protected virtual void OnValueToChanged(EventArgs e)
        {
            var toBytes = BitConverter.GetBytes(RangedRange.To);
            toBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 4);
            OnValueUpdated();
        }

        public void InitializeOnceAtStartup()
        {
            RangedRange.FromChanged += (sender, e) => { OnValueFromChanged(e); };
            RangedRange.ToChanged += (sender, e) => { OnValueToChanged(e); };
            OnValueFromChanged(EventArgs.Empty);
            OnValueToChanged(EventArgs.Empty);
        }
    }

    [DataContract]
    public class HidAccessPropertyRatioRect : HidAccessPropertyBase
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        RatioRect _Value;

        void UpdateWidthBytes()
        {
            var widthRatio = _Value.XRange.To - _Value.XRange.From;
            var widthBytes = BitConverter.GetBytes(widthRatio);
            widthBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 8);
        }
        void UpdateLeftBytes()
        {
            var leftBytes = BitConverter.GetBytes(_Value.XRange.From);
            leftBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 0);
            UpdateWidthBytes();
        }
        public float Left
        {
            get { return (float)_Value.XRange.From; }
            set { _Value.XRange.From = value; UpdateLeftBytes(); OnValueUpdated(); OnPropertyChanged(nameof(Left)); }
        }
        public float Right
        {
            get { return (float)_Value.XRange.To; }
            set { _Value.XRange.To = value; UpdateWidthBytes(); OnValueUpdated(); OnPropertyChanged(nameof(Right)); }
        }
        void UpdateHeightBytes()
        {
            var heightRatio = _Value.YRange.To - _Value.YRange.From;
            var heightBytes = BitConverter.GetBytes(heightRatio);
            heightBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 12);
        }
        void UpdateTopBytes()
        {
            var topBytes = BitConverter.GetBytes(_Value.YRange.From);
            topBytes.CopyTo(ByteArrayData, OneValueOffsetInByteArrayData + 4);
            UpdateHeightBytes();
        }
        public float Top
        {
            get { return (float)_Value.YRange.From; }
            set { _Value.YRange.From = value; UpdateTopBytes(); OnValueUpdated(); OnPropertyChanged(nameof(Top)); }
        }
        public float Bottom
        {
            get { return (float)_Value.YRange.To; }
            set { _Value.YRange.To = value; UpdateHeightBytes(); OnValueUpdated(); OnPropertyChanged(nameof(Bottom)); }
        }

        [DataMember]
        public RatioRect Value
        {
            get { return _Value; }
            set
            {
                _Value.XRange.From = value.XRange.From;
                _Value.YRange.From = value.YRange.From;
                _Value.XRange.To = value.XRange.To;
                _Value.YRange.To = value.YRange.To;
                UpdateLeftBytes();
                UpdateTopBytes();
                // need not to call UpdateWidthBytes and UpdateHeightBytes
                OnValueUpdated();
                OnPropertyChanged(nameof(Left));
                OnPropertyChanged(nameof(Right));
                OnPropertyChanged(nameof(Top));
                OnPropertyChanged(nameof(Bottom));
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(ValueAsString));
            }
        }

        /// <summary>
        /// "Left, Top, Right, Bottom"
        /// </summary>
        public string ValueAsString
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", Left, Top, Right, Bottom); }
            set
            {
                if (string.IsNullOrEmpty(value)) { throw new ArgumentNullException(); }
                string[] values = value.Split(',');
                Debug.Assert(values.Length == 4);
                if (values.Length != 4) { throw new ArgumentException("values.Length != 4"); }
                _Value.XRange.From = float.Parse(values[0], System.Globalization.CultureInfo.InvariantCulture);
                _Value.YRange.From = float.Parse(values[1], System.Globalization.CultureInfo.InvariantCulture);
                _Value.XRange.To = float.Parse(values[2], System.Globalization.CultureInfo.InvariantCulture);
                _Value.YRange.To = float.Parse(values[3], System.Globalization.CultureInfo.InvariantCulture);
                UpdateLeftBytes();
                UpdateTopBytes();
                // need not to call UpdateWidthBytes and UpdateHeightBytes
                OnValueUpdated();
                OnPropertyChanged(nameof(Left));
                OnPropertyChanged(nameof(Right));
                OnPropertyChanged(nameof(Top));
                OnPropertyChanged(nameof(Bottom));
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(ValueAsString));
            }
        }

        public HidAccessPropertyRatioRect()
        {
            _Value = new RatioRect();
            UpdateLeftBytes();
            UpdateTopBytes();
            // need not to call UpdateWidthBytes and UpdateHeightBytes
            OnValueUpdated();
        }

        public void InitializeOnceAtStartup()
        {
        }
    }
}
