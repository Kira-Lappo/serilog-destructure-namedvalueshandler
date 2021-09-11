using System.Linq;
using FluentAssertions;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests.DestructuringTests
{
    public class ObjectDestructuringTests
    {
        private ILogEventPropertyValueFactory ScalarOnlyFactory { get; } = ValueFactories.Instance.ScalarOnlyFactory;

        [Theory]
        [AutoMoqData]
        public void TryDestructureObject_ObjectHasProperties_PropertiesAreDestructured(DestructibleEntity value)
        {
            // Arrange
            var expectedProperties = value.GetType().GetProperties();
            var policy = new NamedValueDestructuringPolicy();

            // Act
            var isDestructured = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isDestructured.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<StructureValue>();

            var structuredResult = (StructureValue)result;

            structuredResult.Properties.Should().NotBeEmpty();
            structuredResult.Properties.Should().HaveSameCount(expectedProperties);

            structuredResult.Properties.Select(p => p.Name)
                .Should()
                .BeEquivalentTo(expectedProperties.Select(p => p.Name));

            structuredResult.Properties.Select(p => p.Value)
                .Should()
                .BeEquivalentTo(expectedProperties.Select(p => new ScalarValue(p.GetValue(value))));
        }

        [Theory]
        [AutoMoqData]
        public void TryDestructureObject_Properties_PropertiesAreDestructured(DestructibleEntity value)
        {
            // Arrange
            var expectedProperties = value.GetType().GetProperties();
            var policy = new NamedValueDestructuringPolicy();

            // Act
            var isDestructured = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isDestructured.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<StructureValue>();

            var structuredResult = (StructureValue)result;

            structuredResult.Properties.Should().NotBeEmpty();
            structuredResult.Properties.Should().HaveSameCount(expectedProperties);

            structuredResult.Properties.Select(p => p.Name)
                .Should()
                .BeEquivalentTo(expectedProperties.Select(p => p.Name));

            structuredResult.Properties.Select(p => p.Value)
                .Should()
                .BeEquivalentTo(expectedProperties.Select(p => new ScalarValue(p.GetValue(value))));
        }
    }
}
