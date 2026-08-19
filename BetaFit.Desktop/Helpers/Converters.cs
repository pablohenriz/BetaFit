using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BetaFit.Desktop.Helpers;

/// <summary>
/// Converte a seção atual (enum) em Visibility, comparando com um parâmetro string.
/// Usado no XAML para mostrar/ocultar cada painel (Dashboard/Produtos/Categorias) sem code-behind.
/// </summary>
public class EnumToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null)
            return Visibility.Collapsed;

        return value.ToString() == parameter.ToString()
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Converte bool, string (vazia/nula) ou null em Visibility. Aceita "Invert" como ConverterParameter.
/// Usado tanto para flags (IsEditing) quanto para mensagens (ErrorMessage/SuccessMessage).
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool truthy = value switch
        {
            bool b => b,
            string s => !string.IsNullOrWhiteSpace(s),
            null => false,
            _ => true
        };

        if (string.Equals(parameter as string, "Invert", StringComparison.OrdinalIgnoreCase))
            truthy = !truthy;

        return truthy ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Converte true/false de IsActive em um texto amigável ("Ativo" / "Inativo").
/// </summary>
public class ActiveStatusTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? "Ativo" : "Inativo";

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
