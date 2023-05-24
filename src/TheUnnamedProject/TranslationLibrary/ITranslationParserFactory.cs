using System.Globalization;
using TranslationLibrary.Handlers;

namespace TranslationLibrary;

public interface ITranslationParserFactory
{
    /// <summary>
    /// Create an ITranslationParser instance with the given CultureInfo and an optional list of additional <see cref="ITokenTypeHandler"/> objects
    /// </summary>
    /// <param name="cultureInfo">It is used when formatting <see cref="DateTime"/>, <see cref="DateOnly"/> and <see cref="TimeOnly"/> types.</param>
    /// <param name="additionalHandlers">It is used to override existing or add new <see cref="ITokenTypeHandler"/> objects</param>
    /// <returns>A new ITranslationParser implementation</returns>
    ITranslationParser Create(CultureInfo cultureInfo, IEnumerable<ITokenTypeHandler>? additionalHandlers = null);

    /// <summary>
    /// Create an ITranslationParser instance with the given CultureInfo and an optional list of additional <see cref="ITokenTypeHandler"/> types
    /// </summary>
    /// <param name="cultureInfo">It is used when formatting <see cref="DateTime"/>, <see cref="DateOnly"/> and <see cref="TimeOnly"/> types.</param>
    /// <param name="additionalHandlers">It is used to override existing or add new <see cref="ITokenTypeHandler"/> types</param>
    /// <returns>A new ITranslationParser implementation</returns>
    ITranslationParser Create(CultureInfo cultureInfo, IEnumerable<Type> additionalHandlers);
}
