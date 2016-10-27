namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using Egs.DotNetUtility;

    [DataContract]
    public abstract class ButtonModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        [DataMember]
        public string ButtonDescriptionText { get; set; }

        public SimpleDelegateCommand Command { get; private set; }

        public event EventHandler CommandRaised;
        protected virtual void OnCommandRaised(EventArgs e)
        {
            var t = CommandRaised; if (t != null) { t(this, EventArgs.Empty); }
        }

        public ButtonModelBase()
        {
            Command = new SimpleDelegateCommand();
            Command.PerformEventHandler += (sender, e) => { OnCommandRaised(EventArgs.Empty); };
        }
    }

    [DataContract]
    public class TextButtonModel : ButtonModelBase
    {
        [DataMember]
        public string ButtonContentText { get; set; }
        public TextButtonModel()
            : base()
        {
        }
    }

    [DataContract]
    public class CursorSpeedAndPrecisionModeButtonModel : TextButtonModel
    {
        [DataMember]
        public string ModeDescription { get; set; }

        public CursorSpeedAndPrecisionModeButtonModel()
            : base()
        {
        }
    }
}
