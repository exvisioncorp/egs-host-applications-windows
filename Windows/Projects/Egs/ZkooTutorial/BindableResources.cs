namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Globalization;

    public class BindableResources : INotifyPropertyChanged
    {
        static readonly BindableResources _Current = new BindableResources();
        public static BindableResources Current { get { return _Current; } }

        readonly Properties.Resources _Resources = new Properties.Resources();
        public Properties.Resources Resources { get { return _Resources; } }

        readonly Properties.NarrationTexts _NarrationTexts = new Properties.NarrationTexts();
        public Properties.NarrationTexts NarrationTexts { get { return _NarrationTexts; } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }
        public event EventHandler CultureChanged;
        public void ChangeCulture(string name)
        {
            var newCultureInfo = (string.IsNullOrEmpty(name)) ? CultureInfo.InstalledUICulture : CultureInfo.GetCultureInfo(name);
            Properties.Resources.Culture = newCultureInfo;
            Properties.NarrationTexts.Culture = newCultureInfo;
            OnPropertyChanged(nameof(Resources));
            OnPropertyChanged(nameof(NarrationTexts));
            var t = CultureChanged; if (t != null) { t(this, EventArgs.Empty); }
        }
    }
}
