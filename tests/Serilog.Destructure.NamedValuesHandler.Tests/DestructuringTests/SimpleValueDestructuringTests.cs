using FluentAssertions;
using Serilog.Events;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests.DestructuringTests
{
    public class SimpleValueDestructuringTests : AbstractDestructuringTests
    {
        [Theory]
        [AutoMoqData]
        public void TryDestructureString_HappyPath_StringValueIsMasked(string value)
        {
            // Arrange
            const string Mask = "******";
            var policy = new NamedValueHandlersBuilder()
                .Handle<string>((_, _) => Mask)
                .BuildDestructuringPolicy();

            // Act
            var isHandled = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();

            var dictionaryResult = (ScalarValue)result;
            dictionaryResult.Value.Should().Be(Mask);
        }
    }
}
