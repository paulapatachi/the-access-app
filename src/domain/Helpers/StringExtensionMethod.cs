using System.Globalization;

namespace MyApp.Domain.Helpers;

public static class StringExtensionMethod
{
    private const string DefaultDateFormat = "yyyyMMdd";
    private static readonly DateTime DefaultDateTime = DateTime.MinValue;
    
    public static DateTime GetDate(this string? stringDate)
    {
        if (string.IsNullOrWhiteSpace(stringDate))
            return DefaultDateTime;
        
        if (DateTime.TryParseExact(
                stringDate,
                DefaultDateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result))
        {
            return result;
        }

        return DefaultDateTime;
    }
}