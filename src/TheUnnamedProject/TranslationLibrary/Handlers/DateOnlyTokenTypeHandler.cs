using System.Globalization;
using Humanizer;

namespace TranslationLibrary.Handlers;

internal class DateOnlyTokenTypeHandler : ITokenTypeHandler
{
    private readonly CultureInfo _cultureInfo;
    public DateOnlyTokenTypeHandler(CultureInfo cultureInfo)
    {
        _cultureInfo = cultureInfo;
    }

    public bool CanHandle(string dataType)
    {
        return string.Equals(dataType, "dateonly", StringComparison.OrdinalIgnoreCase);
    }

    public TokenHandlerResult Handle(string key, string value, string additionalInformation, IDictionary<string, string>? @params)
    {
        if (string.IsNullOrWhiteSpace(value) || !DateOnly.TryParseExact(value, "o", _cultureInfo, DateTimeStyles.None, out var dateTimeValue))
        {
            return new("", false);
        }


        if (string.IsNullOrWhiteSpace(additionalInformation))
        {
            return new(dateTimeValue.ToString(_cultureInfo));
        }

        return new(additionalInformation switch
        {
            "h" => dateTimeValue.Humanize(),
            _ => dateTimeValue.ToString(additionalInformation, _cultureInfo)
        });
    }
}