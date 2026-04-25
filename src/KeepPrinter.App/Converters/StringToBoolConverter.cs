using System;
using Microsoft.UI.Xaml.Data;

namespace KeepPrinter.App.Converters;

/// <summary>
/// Convierte un string a bool (true si no es null/empty).
/// </summary>
public class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return !string.IsNullOrWhiteSpace(value as string);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
