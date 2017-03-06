namespace Egs.EgsSourceCodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OptionalByteValueAndDescription
    {
        public byte Value { get; set; }
        public string Description { get; set; }
    }

    public class DescriptionAndOptionsInOneLanguage
    {
        public string PropertyDescription { get; set; }
        public List<OptionalByteValueAndDescription> OptionalByteValueAndDescriptionList { get; set; }
        public DescriptionAndOptionsInOneLanguage()
        {
            OptionalByteValueAndDescriptionList = new List<OptionalByteValueAndDescription>();
        }
    }

    public class EgsDeviceHidAccessPropertyOneRecord
    {
        public string OwnerClass { get; set; }
        public string ValueTypeOnHost { get; set; }
        public string ValueNameOnHost { get; set; }
        public string ReportId { get; set; }
        public string MessageId { get; set; }
        public string CategoryId { get; set; }
        public string PropertyId { get; set; }
        public string ValueTypeOnDevice { get; set; }
        public int DataLength { get; set; }
        public bool IsDataMember { get; set; }
        public string AccessModifierInFuture { get; set; }
        public bool IsReadOnly { get; set; }
        public string AvailableFirmwareVersion { get; set; }
        public string AccessModifierInLatestSdkForWindows { get; set; }
        public string PropertyInitializationOnWindows { get; set; }
        public Dictionary<string, DescriptionAndOptionsInOneLanguage> Language_DescriptionAndOptions_Dict { get; set; }

        public string DescriptionKey
        {
            get { return OwnerClass + "_" + ValueNameOnHost + "_Description"; }
        }
        public bool IsHidAccessPropertyOptional
        {
            get { return ValueTypeOnHost == "HidAccessPropertyOptional"; }
        }
        public bool IsEnumValueWithDescription
        {
            get { return ValueTypeOnHost.Contains("EnumValueWithDescription"); }
        }
        public string DetailTypeName
        {
            get { return ValueNameOnHost + "Detail"; }
        }
        public string ModifiedValueTypeOnHost
        {
            get { return IsHidAccessPropertyOptional ? ("HidAccessPropertyOptional<" + DetailTypeName + ">") : ValueTypeOnHost; }
        }


        public string GetCodeOfPropertyDefinition()
        {
            var ret = "        ";
            ret += IsDataMember ? "[DataMember]" + Environment.NewLine + "        " : "";
            ret += AccessModifierInLatestSdkForWindows + " " + ModifiedValueTypeOnHost + " " + ValueNameOnHost + " { get; ";
            ret += (AccessModifierInLatestSdkForWindows != "private") ? "private " : "";
            ret += "set; }";
            ret += Environment.NewLine;
            return ret;
        }

        readonly string[] ModifiedValueTypeOnHostTypesWhichCallInitializeOnceAtStartup = new string[]
        {
            "HidAccessPropertyRangedInt", "HidAccessPropertyRangedSingle", "HidAccessPropertyRangedIntRange", "HidAccessPropertyOptional", "HidAccessPropertyRatioRect"
        };

        public string GetCodeOfCreatingPropertyObject()
        {
            var ret = string.Format(System.Globalization.CultureInfo.InvariantCulture, "            {0} = new {1}() {{ ", ValueNameOnHost, ModifiedValueTypeOnHost);
            if (ModifiedValueTypeOnHost.Contains("HidAccessProperty")
                || ModifiedValueTypeOnHost.Contains("ValueWithDescription"))
            {
                ret += "DescriptionKey = nameof(Resources." + DescriptionKey + ")";
            }
            if (ModifiedValueTypeOnHost.Contains("HidAccessProperty"))
            {
                ret += string.IsNullOrEmpty(ReportId) ? "" : ", ReportId = " + ReportId;
                ret += string.IsNullOrEmpty(MessageId) ? "" : ", MessageId = " + MessageId;
                ret += string.IsNullOrEmpty(CategoryId) ? "" : ", CategoryId = " + CategoryId;
                ret += string.IsNullOrEmpty(PropertyId) ? "" : ", PropertyId = " + PropertyId;
                ret += string.IsNullOrEmpty(ValueTypeOnDevice) ? "" : ", ValueTypeOnDevice = \"" + ValueTypeOnDevice + "\"";
                ret += ", DataLength = " + DataLength;
                ret += ", IsReadOnly = " + (IsReadOnly ? "true" : "false");
                ret += ", NameOfProperty = \"" + ValueNameOnHost + "\"";
                ret += ", AvailableFirmwareVersion = new Version(\"" + AvailableFirmwareVersion + "\")";
            }
            ret += " };";
            if (IsHidAccessPropertyOptional)
            {
                ret += string.Format(System.Globalization.CultureInfo.InvariantCulture, " {0}.OptionalValue.Options = {1}.GetDefaultList();", ValueNameOnHost, DetailTypeName);
            }
            if (ModifiedValueTypeOnHostTypesWhichCallInitializeOnceAtStartup.Any(e => ModifiedValueTypeOnHost.Contains(e)))
            {
                ret += string.Format(System.Globalization.CultureInfo.InvariantCulture, " {0}.InitializeOnceAtStartup();", ValueNameOnHost);
            }
            ret += Environment.NewLine;
            return ret;
        }

        public string GetCodeOfAddingPropertiesToPropertyList()
        {
            var ret = "";
            if (ModifiedValueTypeOnHost.Contains("HidAccessProperty"))
            {
                ret += "            HidAccessPropertyList.Add(" + ValueNameOnHost + ");" + Environment.NewLine;
            }
            return ret;
        }

        public string GetCodeOfInitializationByDefaultValue()
        {
            var ret = "";
            if (string.IsNullOrEmpty(PropertyInitializationOnWindows)) { return ret; }
            if (IsHidAccessPropertyOptional)
            {
                ret += string.Format(System.Globalization.CultureInfo.InvariantCulture, "            {0}.OptionalValue.SelectSingleItemByPredicate(e => e.{1});", ValueNameOnHost, PropertyInitializationOnWindows) + Environment.NewLine;
            }
            else
            {
                foreach (var initializationCode in PropertyInitializationOnWindows.Split(';'))
                {
                    ret += @"            " + ValueNameOnHost + "." + initializationCode.Trim() + ";" + Environment.NewLine;
                }
            }
            return ret;
        }
    }
}
