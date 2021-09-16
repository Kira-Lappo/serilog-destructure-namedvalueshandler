using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests.DestructuringTests
{
    public class DictionaryDestructuringTests
    {
        private ILogEventPropertyValueFactory ScalarOnlyFactory { get; } = ValueFactories.Instance.ScalarOnlyFactory;

        [Theory]
        [AutoMoqData]
        public void TryDestructureDictionary_HappyPath_ValueIsDestructed(Dictionary<string, string> value)
        {
            // Arrange
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

        [Theory]
        [AutoMoqData]
        public void TryDestructureDictionary_ValueIsMasked_MaskedValueIsDestructured(Dictionary<string, string> value)
        {
            // Arrange
            var maskedKey = value.Keys.First();
            var maskedValue = value[maskedKey];
            var expectedMaskedValue = maskedValue.Mask();

            var policy = new NamedValueDestructuringPolicyBuilder()
                .Mask(maskedKey)
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

        [Theory]
        [AutoMoqData]
        public void TryDestructureDictionary_ValueIsOmitted_ByName_ValueIsRemoved(Dictionary<string, string> value)
        {
            // Arrange
            var omittedKey = value.Keys.First();

            var policy = new NamedValueDestructuringPolicyBuilder()
                .Omit(omittedKey)
                .Build();

            // Act
            var isHandled = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();

            var dictionaryResult = (DictionaryValue)result;
            dictionaryResult.Elements.Keys.Should().NotContain(e => e.Value.ToString() == omittedKey);
        }

        [Theory]
        [AutoMoqData]
        public void TryDestructureDictionary_ValueIsOmitted_ByType_ValueIsRemoved(Dictionary<string, string> value)
        {
            // Arrange
            var omittedKey = value.Keys.First();
            var omittedValue = value[omittedKey];

            var policy = new NamedValueDestructuringPolicyBuilder()
                .OmitType(omittedValue.GetType())
                .Build();

            // Act
            var isHandled = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();

            var dictionaryResult = (DictionaryValue)result;
            dictionaryResult.Elements.Keys.Should().NotContain(e => e.Value.ToString() == omittedKey);
        }

        [Theory]
        [AutoMoqData]
        public void TryDestructureDictionary_ValueIsOmitted_ByNamespace_ValueIsRemoved(Dictionary<string, string> value)
        {
            // Arrange
            var omittedKey = value.Keys.First();
            var omittedValue = value[omittedKey];
            var omittedNamespace = omittedValue.GetType().Namespace?.Split(".").First();

            var policy = new NamedValueDestructuringPolicyBuilder()
                .OmitNamespace(omittedNamespace)
                .Build();

            // Act
            var isHandled = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();

            var dictionaryResult = (DictionaryValue)result;
            dictionaryResult.Elements.Keys.Should().NotContain(e => e.Value.ToString() == omittedKey);
        }

        [Theory]
        [AutoMoqData]
        public void TryDestructureDictionary_ValueIsNotMaskedOrOmitted_ValueIsNotChanged(Dictionary<string, string> value)
        {
            // Arrange
            var notModifiedKey = value.Keys.First();
            var notModifiedValue = value[notModifiedKey];

            var policy = new NamedValueDestructuringPolicyBuilder()
                .Mask($"{notModifiedKey}:masked")
                .OmitNamespace("Special.Namespace", "Legacy")
                .OmitType(typeof(int))
                .Omit($"{notModifiedKey}:omitted")
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
