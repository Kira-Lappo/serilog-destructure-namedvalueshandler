using System;
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
    }
}
