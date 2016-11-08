namespace DotNetUtility.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Globalization;

    public class BooleanConverter<T> : IValueConverter
    {
        public BooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        public T True { get; set; }
        public T False { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && ((bool)value) ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }
    }

    /// <summary>
    /// Convert true to Visibility.Collapsed and false to Visibility.Visible
    /// </summary>
    public sealed class TrueToCollapsedFalseToVisibleConverter : BooleanConverter<Visibility>
    {
        public TrueToCollapsedFalseToVisibleConverter() : base(Visibility.Collapsed, Visibility.Visible) { }
    }

    public class BoolToOppositeBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool)) { throw new InvalidOperationException("The target must be a bool"); }
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null) { return System.Windows.DependencyProperty.UnsetValue; }
            if (Enum.IsDefined(value.GetType(), value) == false) { return System.Windows.DependencyProperty.UnsetValue; }
            object parameterValue = Enum.Parse(value.GetType(), parameterString);
            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null || value.Equals(false)) { return System.Windows.DependencyProperty.UnsetValue; }
            return Enum.Parse(targetType, parameterString);
        }
    }

    internal class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null) { return System.Windows.DependencyProperty.UnsetValue; }
            if (Enum.IsDefined(value.GetType(), value) == false) { return System.Windows.DependencyProperty.UnsetValue; }
            object parameterValue = Enum.Parse(value.GetType(), parameterString);
            return (parameterValue.Equals(value)) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null || value.Equals(System.Windows.Visibility.Collapsed) || value.Equals(System.Windows.Visibility.Hidden)) { return System.Windows.DependencyProperty.UnsetValue; }
            return Enum.Parse(targetType, parameterString);
        }
    }

    public class Int32IndexToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int integer = (int)value;
            if (integer == int.Parse(parameter.ToString(), System.Globalization.CultureInfo.InvariantCulture))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
