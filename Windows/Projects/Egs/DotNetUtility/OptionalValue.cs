namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    [DataContract]
    public class OptionalValue<T> : INotifyPropertyChanged
        where T : class, new()
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        List<T> _Options = new List<T>();
        public event EventHandler OptionsChanged;
        protected virtual void OnOptionsChanged(EventArgs e)
        {
            var t = OptionsChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(Options));
        }

        /// <summary>
        /// ObservableCollection of T.
        /// [NOTE]: SelectedIndex has [DataMember], but Options does not have [DataMember].
        /// Because Json.NET Deserialize / PopulateObject methods DO NOT "RE"SET items but "ADD" items to constructed objects of List, Collection and so on.
        /// Array is reset, but not used.
        /// </summary>
        public List<T> Options
        {
            get { return _Options; }
            set
            {
                _Options = value;
                OnOptionsChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _SelectedIndex = 0;
        public event EventHandler SelectedIndexChanged;
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            var t = SelectedIndexChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(SelectedIndex));
        }
        [DataMember]
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                _SelectedIndex = value;
                OnSelectedIndexChanged(EventArgs.Empty);
                OnSelectedItemChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
            var t = SelectedItemChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(SelectedItem));
        }
        public T SelectedItem
        {
            get
            {
                if (_Options == null) { return null; }
                if (_Options.Count == 0) { return null; }
                if (_SelectedIndex < 0) { return null; }
                return _Options[_SelectedIndex];
            }
        }

        public bool SelectSingleItemByPredicate(Func<T, bool> predicate)
        {
            var result = Options.Single(predicate);
            if (result == null) { return false; }
            SelectedIndex = Options.IndexOf(result);
#if DEBUG
            if (SelectedItem != result) { System.Diagnostics.Debugger.Break(); }
#endif
            return true;
        }
    }
}