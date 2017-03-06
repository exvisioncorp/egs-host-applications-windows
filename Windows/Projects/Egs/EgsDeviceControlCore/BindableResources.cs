namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Globalization;
    using System.Diagnostics;

    /// <summary>
    /// Wrapper of EgsHostAppCore.Properties.Resources class for DataBinding on XAML.
    /// You can use this like "{Binding Source={x:Static deviceControl:BindableResources.Current}, Path=Resources.EgsHostSettings_...}" in data binding in XAML files.
    /// </summary>
    public class BindableResources : INotifyPropertyChanged
    {
        static readonly BindableResources _Current = new BindableResources();
        /// <summary>
        /// static readonly instance of this class.
        /// </summary>
        public static BindableResources Current { get { return _Current; } }

        Egs.EgsDeviceControlCore.Properties.Resources _Resources = new Egs.EgsDeviceControlCore.Properties.Resources();
        /// <summary>
        /// Readonly instance of Properties.Resources.
        /// </summary>
        public Egs.EgsDeviceControlCore.Properties.Resources Resources { get { return _Resources; } }

        /// <summary>Implementation of INotifyPropertyChanged.PropertyChanged.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>OnPropertyChanged to raise PropertyChanged event</summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }
        /// <summary>
        /// When ChangeCulture() is called, this event raises.
        /// </summary>
        public event EventHandler CultureChanged;
        /// <summary>
        /// Set Properties.Resources.Culture by CultureInfo.GetCultureInfo(name)
        /// </summary>
        /// <param name="name">CultureInfo.Name.  e.g. "en", "ja" and "zh-Hans"...  If you set this value to null or empty, CultureInfo.InstalledUICulture will be used.</param>
        public void ChangeCulture(string name)
        {
            var newCultureInfo = (string.IsNullOrEmpty(name)) ? CultureInfo.InstalledUICulture : CultureInfo.GetCultureInfo(name);
            Egs.EgsDeviceControlCore.Properties.Resources.Culture = newCultureInfo;
            OnPropertyChanged(nameof(Resources));
            var t = CultureChanged; if (t != null) { t(this, EventArgs.Empty); }
        }
    }
}
