using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace KeepPrinter.App.Converters;

/// <summary>
/// Convierte int a Visibility (> 0 = Visible, 0 = Collapsed).
/// </summary>
public class IntToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int intValue)
        {
            return intValue > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
