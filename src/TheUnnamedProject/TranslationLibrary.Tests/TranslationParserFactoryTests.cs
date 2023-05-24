﻿using System.Globalization;
using FluentAssertions;
using TranslationLibrary.Handlers;

namespace TranslationLibrary.Tests;

public class TranslationParserFactoryTests
{
    [Test]
    public void Pass_Null_CultureInfo()
    {
        var factory = new TranslationParserFactory();

        var action = () => factory.Create(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Can_Find_Default_TokenType_Handlers()
    {
        var factory = new TranslationParserFactory();

        var parser = factory.Create(CultureInfo.InvariantCulture) as TranslationParser;

        parser.Should().NotBeNull();
        parser!.TokenTypeHandlers.Count().Should().BeGreaterThan(0);
    }

    [Test]
    public void Create_Using_Handler_With_CultureInfo_Constructor_As_Object()
    {
        var factory = new TranslationParserFactory();
        var replacements = new Dictionary<string, string>
        {
            { "key", "test" }
        };

        var parser = factory.Create(CultureInfo.InvariantCulture, new[] { new MockTokenTypeHandler_CultureInfoConstructor(CultureInfo.InvariantCulture) });
        var translation = parser.Parse("Here some replaced string: {string|key}", replacements);

        translation.Should().Be("Here some replaced string: cultureInfo_key_test");
    }

    [Test]
    public void Create_Using_Handler_With_CultureInfo_Constructor_As_Type()
    {
        var factory = new TranslationParserFactory();
        var replacements = new Dictionary<string, string>
        {
            { "key", "test" }
        };

        var parser = factory.Create(CultureInfo.InvariantCulture, new[] { typeof(MockTokenTypeHandler_CultureInfoConstructor) });
        var translation = parser.Parse("Here some replaced string: {string|key}", replacements);

        translation.Should().Be("Here some replaced string: cultureInfo_key_test");
    }

    [Test]
    public void Create_Using_Handler_With_Empty_Constructor_As_Object()
    {
        var factory = new TranslationParserFactory();
        var replacements = new Dictionary<string, string>
        {
            { "key", "test" }
        };

        var parser = factory.Create(CultureInfo.InvariantCulture, new[] { new MockTokenTypeHandler_EmptyConstructor() });
        var translation = parser.Parse("Here some replaced string: {string|key}", replacements);

        translation.Should().Be("Here some replaced string: empty_key_test");
    }

    [Test]
    public void Create_Using_Handler_With_Empty_Constructor_As_Type()
    {
        var factory = new TranslationParserFactory();
        var replacements = new Dictionary<string, string>
        {
            { "key", "test" }
        };

        var parser = factory.Create(CultureInfo.InvariantCulture, new[] { typeof(MockTokenTypeHandler_EmptyConstructor) });
        var translation = parser.Parse("Here some replaced string: {string|key}", replacements);

        translation.Should().Be("Here some replaced string: empty_key_test");
    }

    [Test]
    public void Create_Using_Handler_With_Unusable_Constructor_As_Object()
    {
        var factory = new TranslationParserFactory();
        var replacements = new Dictionary<string, string>
        {
            { "key", "test" }
        };

        var parser = factory.Create(CultureInfo.InvariantCulture, new[] { new MockTokenTypeHandler_UnusableConstructor_For_Reflection("mock") });
        var translation = parser.Parse("Here some replaced string: {string|key}", replacements);

        translation.Should().Be("Here some replaced string: unusable_for_reflection_key_test");
    }

    [Test]
    public void Create_Using_Handler_With_Unusable_Constructor_As_Type()
    {
        var factory = new TranslationParserFactory();

        var action = () => factory.Create(CultureInfo.InvariantCulture, new[] { typeof(MockTokenTypeHandler_UnusableConstructor_For_Reflection) });

        action.Should().Throw<MissingMemberException>();
    }

    private class MockTokenTypeHandler_CultureInfoConstructor : ITokenTypeHandler
    {
        public MockTokenTypeHandler_CultureInfoConstructor(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }
        }

        public bool CanHandle(string dataType)
        {
            // Override dataTypeHandler "string"
            return string.Equals(dataType, "string", StringComparison.OrdinalIgnoreCase);
        }

        public TokenHandlerResult Handle(string key, string value, string additionalInformation, IDictionary<string, string>? @params)
        {
            return new($"cultureInfo_{key}_{value}");
        }
    }

    private class MockTokenTypeHandler_EmptyConstructor : ITokenTypeHandler
    {
        public bool CanHandle(string dataType)
        {
            // Override dataTypeHandler "string"
            return string.Equals(dataType, "string", StringComparison.OrdinalIgnoreCase);
        }

        public TokenHandlerResult Handle(string key, string value, string additionalInformation, IDictionary<string, string>? @params)
        {
            return new($"empty_{key}_{value}");
        }
    }

    private class MockTokenTypeHandler_UnusableConstructor_For_Reflection : ITokenTypeHandler
    {
        public MockTokenTypeHandler_UnusableConstructor_For_Reflection(string mockParameter) { }

        public bool CanHandle(string dataType)
        {
            // Override dataTypeHandler "string"
            return string.Equals(dataType, "string", StringComparison.OrdinalIgnoreCase);
        }

        public TokenHandlerResult Handle(string key, string value, string additionalInformation, IDictionary<string, string>? @params)
        {
            return new($"unusable_for_reflection_{key}_{value}");
        }
    }
}
