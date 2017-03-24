namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using System.ComponentModel;
    using System.Diagnostics;

    public class SimpleDelegateCommand : ICommand//, INotifyPropertyChanged
    {
        //public event EventHandler CanExecuteChanged;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public event EventHandler PerformEventHandler;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }
        bool _CanPerform = true;
        public bool CanPerform
        {
            get { return _CanPerform && (_IsPerforming == false); }
            set
            {
                _CanPerform = value;
                CommandManager.InvalidateRequerySuggested();
                //var t = CanExecuteChanged; if (t != null) { t(this, EventArgs.Empty); }
                //OnPropertyChanged(nameof(CanPerform));
                //OnPropertyChanged(nameof(CanExecute));
            }
        }
        bool _IsPerforming = false;
        public bool IsPerforming
        {
            get { return _IsPerforming; }
            protected set
            {
                _IsPerforming = value;
                CommandManager.InvalidateRequerySuggested();
                //var t = CanExecuteChanged; if (t != null) { t(this, EventArgs.Empty); }
                //OnPropertyChanged(nameof(IsPerforming));
                //OnPropertyChanged(nameof(CanPerform));
                //OnPropertyChanged(nameof(CanExecute));
            }
        }

        public bool CanExecute(object parameter)
        {
            return CanPerform;
        }
        public void Execute(object parameter)
        {
            IsPerforming = true;
            var t = PerformEventHandler;
            if (t != null) { t(this, EventArgs.Empty); }
            IsPerforming = false;
        }

        public SimpleDelegateCommand()
        {
        }

        public SimpleDelegateCommand(Action action)
        {
            PerformEventHandler += (sender, e) => action();
        }
    }
}
