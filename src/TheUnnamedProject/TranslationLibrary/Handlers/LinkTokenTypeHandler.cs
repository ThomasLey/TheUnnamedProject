namespace TranslationLibrary.Handlers;

internal class LinkTokenTypeHandler : ITokenTypeHandler
{
    public bool CanHandle(string dataType)
    {
        return string.Equals(dataType, "link", StringComparison.OrdinalIgnoreCase);
    }

    public TokenHandlerResult Handle(string key, string value, string additionalInformation, IDictionary<string, string>? @params)
    {
        if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(additionalInformation) || @params == null)
        {
            return new("", false);
        }

        var title = additionalInformation[0] switch
        {
            '@' => @params.TryGetValue(additionalInformation[1..], out var t) ? t : "!@#)(*`",
            _ => additionalInformation
        };

        if ("!@#)(*`".Equals(title))
        {
            return new("", false);
        }

        return new($"<a href='{value}'>{title}</a>");
    }
}