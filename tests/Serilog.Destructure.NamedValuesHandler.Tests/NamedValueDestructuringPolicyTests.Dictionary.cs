﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Serilog.Events;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests
{
    public partial class NamedValueDestructuringPolicyTests
    {
        [Fact]
        public void TryDestructureDictionary_ValueIsDestructed()
        {
            // Arrange
            var maskedKey = Guid.NewGuid().ToString();
            var value = new Dictionary<string, string>
            {
                { maskedKey, maskedKey + ":value" }
            };

            var policy = new NamedValueDestructuringPolicy();

            // Act
            var isHandled = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<DictionaryValue>();

            var dictionaryResult = (DictionaryValue)result;
            dictionaryResult.Elements.Keys.Select(k => k.Value.ToString())
                .Should()
                .BeEquivalentTo(value.Keys);

            dictionaryResult.Elements.Values.Should()
                .BeEquivalentTo(value.Values.Select(v => new ScalarValue(v)));
        }

        [Fact]
        public void TryDestructureDictionary_ValueIsMasked_MaskedValueIsDestructured()
        {
            // Arrange
            var maskedKey = Guid.NewGuid().ToString();
            var maskedValue = $"{maskedKey}:value";
            var expectedMaskedValue = maskedValue.MaskValue();

            var value = new Dictionary<string, string>
            {
                { maskedKey, maskedValue }
            };

            var policy = new NamedValueDestructuringPolicy.NamedValuePolicyBuilder()
                .MaskStringValue(maskedKey)
                .Build();

            // Act
            var isHandled = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();

            var dictionaryResult = (DictionaryValue)result;
            dictionaryResult.Elements.Keys.Should().Contain(e => e.Value.ToString() == maskedKey);

            dictionaryResult.Elements[new ScalarValue(maskedKey)]
                .ToString()
                .Should()
                .Contain(expectedMaskedValue);
        }

        [Fact]
        public void TryDestructureDictionary_ValueIsOmitted_ByName_ValueIsRemoved()
        {
            // Arrange
            var omittedKey = Guid.NewGuid().ToString();
            var omittedValue = $"{omittedKey}:value";

            var value = new Dictionary<string, string>
            {
                { omittedKey, omittedValue }
            };

            var policy = new NamedValueDestructuringPolicy.NamedValuePolicyBuilder()
                .OmitNames(omittedKey)
                .Build();

            // Act
            var isHandled = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();

            var dictionaryResult = (DictionaryValue)result;
            dictionaryResult.Elements.Keys.Should().NotContain(e => e.Value.ToString() == omittedKey);
        }

        [Fact]
        public void TryDestructureDictionary_ValueIsOmitted_ByType_ValueIsRemoved()
        {
            // Arrange
            var omittedKey = Guid.NewGuid().ToString();
            var omittedValue = $"{omittedKey}:value";

            var value = new Dictionary<string, string>
            {
                { omittedKey, omittedValue }
            };

            var policy = new NamedValueDestructuringPolicy.NamedValuePolicyBuilder()
                .OmitOfType(omittedValue.GetType())
                .Build();

            // Act
            var isHandled = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();

            var dictionaryResult = (DictionaryValue)result;
            dictionaryResult.Elements.Keys.Should().NotContain(e => e.Value.ToString() == omittedKey);
        }

        [Fact]
        public void TryDestructureDictionary_ValueIsOmitted_ByNamespace_ValueIsRemoved()
        {
            // Arrange
            var omittedKey = Guid.NewGuid().ToString();
            var omittedValue = $"{omittedKey}:value";
            var omittedNamespace = omittedValue.GetType().Namespace?.Split(".").First();

            var value = new Dictionary<string, string>
            {
                { omittedKey, omittedValue }
            };

            var policy = new NamedValueDestructuringPolicy.NamedValuePolicyBuilder()
                .OmitFromNamespace(omittedNamespace)
                .Build();

            // Act
            var isHandled = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();

            var dictionaryResult = (DictionaryValue)result;
            dictionaryResult.Elements.Keys.Should().NotContain(e => e.Value.ToString() == omittedKey);
        }

        [Fact]
        public void TryDestructureDictionary_ValueIsNotSetInAnyWay_ValueIsNotChanged()
        {
            // Arrange
            var notModifiedKey = Guid.NewGuid().ToString();
            var notModifiedValue = $"{notModifiedKey}:value";

            var value = new Dictionary<string, string>
            {
                { notModifiedKey, notModifiedValue }
            };

            var policy = new NamedValueDestructuringPolicy.NamedValuePolicyBuilder()
                .MaskStringValue($"{notModifiedKey}:masked")
                .OmitFromNamespace("Special.Namespace", "Legacy")
                .OmitOfType(typeof(int))
                .OmitNames($"{notModifiedKey}:omitted")
                .Build();

            // Act
            var isHandled = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();

            var dictionaryResult = (DictionaryValue)result;
            dictionaryResult.Elements.Keys.Should().Contain(e => e.Value.ToString() == notModifiedKey);

            dictionaryResult.Elements[new ScalarValue(notModifiedKey)]
                .ToString()
                .Should()
                .Contain(notModifiedValue);
        }
    }
}
