using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("TranslationLibrary.Tests")]

namespace TranslationLibrary;

public static class ServiceExtensions
{
    public static void AddTranslationParser(this IServiceCollection services)
    {
        services.AddTransient<ITranslationParserFactory, TranslationParserFactory>();
    }
}
