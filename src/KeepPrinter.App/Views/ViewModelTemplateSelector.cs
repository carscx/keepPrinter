using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using KeepPrinter.ViewModels;

namespace KeepPrinter.App.Views;

/// <summary>
/// Selector de DataTemplate basado en el tipo de ViewModel.
/// </summary>
public class ViewModelTemplateSelector : DataTemplateSelector
{
    public DataTemplate? SetupTemplate { get; set; }
    public DataTemplate? BatchProgressTemplate { get; set; }
    public DataTemplate? CompletionTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        return item switch
        {
            SetupViewModel => SetupTemplate,
            BatchProgressViewModel => BatchProgressTemplate,
            CompletionViewModel => CompletionTemplate,
            _ => null
        };
    }

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
    {
        return SelectTemplateCore(item);
    }
}
