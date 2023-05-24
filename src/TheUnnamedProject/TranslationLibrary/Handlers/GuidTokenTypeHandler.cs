namespace TranslationLibrary.Handlers;

internal class GuidTokenTypeHandler : ITokenTypeHandler
{
    public bool CanHandle(string dataType)
    {
        return string.Equals(dataType, "guid", StringComparison.OrdinalIgnoreCase);
    }

    public TokenHandlerResult Handle(string key, string value, string additionalInformation, IDictionary<string, string>? @params)
    {
        if (string.IsNullOrWhiteSpace(value) || !Guid.TryParse(value, out var guidValue))
        {
            return new("", false);
        }

        if (string.IsNullOrWhiteSpace(additionalInformation))
        {
            return new(guidValue.ToString());
        }

        return new(guidValue.ToString(additionalInformation));
    }
}