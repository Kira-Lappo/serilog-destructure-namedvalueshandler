using System.Linq;
using FluentAssertions;
using Moq;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests
{
    public partial class NamedValueDestructuringPolicyTests
    {
        [Theory]
        [AutoMoqData]
        public void TryDestructureObject_ObjectHasProperties_PropertiesAreDestructured(DestructibleEntity @object)
        {
            // Arrange
            var factoryMock = new Mock<ILogEventPropertyValueFactory>();
            factoryMock
                .Setup(_ => _.CreatePropertyValue(It.IsAny<object>(), It.IsAny<bool>()))
                .Returns((object o, bool isDestruct) => new ScalarValue(o));

            var factory = factoryMock.Object;
            var expectedProperties = @object.GetType().GetProperties();
            var policy = new NamedValueDestructuringPolicy();

            // Act
            var isDestructured = policy.TryDestructure(@object, factory, out var result);

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
                .BeEquivalentTo(expectedProperties.Select(p => new ScalarValue(p.GetValue(@object))));
        }
    }
}
