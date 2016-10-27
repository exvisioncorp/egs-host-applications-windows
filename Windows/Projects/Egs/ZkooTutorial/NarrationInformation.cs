namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.Threading;
    using System.IO;
    using System.Net;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;
    using System.Windows;
    using Egs;
    using Egs.DotNetUtility;
    using System.Globalization;

    class NarrationInformation
    {
        public int ViewIndex { get; set; }
        public int MessageIndex { get; set; }
        public int SubIndex { get; set; }
        public string OggAudioFileName
        {
            get
            {
                var ret = "tutorial_";
                ret += ViewIndex.ToString("D03", CultureInfo.InvariantCulture);
                ret += "_";
                ret += MessageIndex.ToString("D03", CultureInfo.InvariantCulture);
                ret += SubIndex.ToString("D02", CultureInfo.InvariantCulture);
                ret += ".ogg";
                return ret;
            }
        }
        public string WavAudioFileName
        {
            get
            {
                var ret = "";
                ret += ViewIndex.ToString("D03", CultureInfo.InvariantCulture);
                ret += "-";
                ret += MessageIndex.ToString("D03", CultureInfo.InvariantCulture);
                ret += SubIndex.ToString("D02", CultureInfo.InvariantCulture);
                ret += ".wav";
                return ret;
            }
        }
        public string TextKey { get; set; }
        public string Text
        {
            get
            {
                return Egs.ZkooTutorial.Properties.NarrationTexts.ResourceManager.GetString(TextKey,
                    Egs.ZkooTutorial.Properties.NarrationTexts.Culture);
            }
        }

        public bool Equals(NarrationInformation other)
        {
            if (other == null) { return false; }
            return ViewIndex.Equals(other.ViewIndex)
                && MessageIndex.Equals(other.MessageIndex)
                && SubIndex.Equals(other.SubIndex);
        }
        /// <summary>Get the hashcode of the Value.</summary>
        public override int GetHashCode()
        {
            return ViewIndex.GetHashCode()
                ^ MessageIndex.GetHashCode()
                ^ SubIndex.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null || (this.GetType() != obj.GetType())) { return false; }
            return this.Equals((NarrationInformation)obj);
        }
    }
}
