using System;
using FluentAssertions;
using Serilog.Events;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests.DestructuringTests
{
    public class HandlerNegativeCasesTests : AbstractDestructuringTests
    {
        [Theory]
        [AutoMoqData]
        public void TryDestructure_ValueHandlerThrowsException_ValueIsNotModified(DestructibleEntity value)
        {
            // Arrange
            var maskedName = nameof(value.Name);
            var maskedValue = new ScalarValue(value.Name);

            var policy = new NamedValueHandlersBuilder()
                .Handle(
                    maskedName,
                    new Func<string, string>(
                        _ =>
                            throw new Exception(nameof(TryDestructure_ValueHandlerThrowsException_ValueIsNotModified))))
                .BuildDestructuringPolicy();

            // Act
            LogEventPropertyValue result = null;
            Func<bool> action = () => policy.TryDestructure(value, ScalarOnlyFactory, out result);

            // Assert
            action.Should().NotThrow();
            result.Should().NotBeNull();

            var structureResult = (StructureValue)result;
            structureResult.Properties.Should()
                .Contain(p => p.Name == maskedName && Equals(p.Value, maskedValue));
        }

        [Theory]
        [AutoMoqData]
        public void TryDestructure_OmitterThrowsException_ValueIsNotOmitted(DestructibleEntity value)
        {
            // Arrange
            var maskedName = nameof(value.Name);
            var maskedValue = new ScalarValue(value.Name);

            var policy = new NamedValueHandlersBuilder()
                .Omit(
                    _ =>
                        throw new Exception(nameof(TryDestructure_OmitterThrowsException_ValueIsNotOmitted)))
                .BuildDestructuringPolicy();

            // Act
            LogEventPropertyValue result = null;
            Func<bool> action = () => policy.TryDestructure(value, ScalarOnlyFactory, out result);

            // Assert
            action.Should().NotThrow();
            result.Should().NotBeNull();

            var structureResult = (StructureValue)result;
            structureResult.Properties.Should()
                .Contain(p => p.Name == maskedName && Equals(p.Value, maskedValue));
        }

        [Theory]
        [AutoMoqData]
        public void TryDestructure_HandlerReturnsNullAsANewValue_NullIsAccepted(DestructibleEntity value)
        {
            // Arrange
            var maskedName = nameof(value.Name);
            var expectedMaskedValue = new ScalarValue(value: null);

            var policy = new NamedValueHandlersBuilder()
                .Handle(
                    maskedName,
                    new Func<string, string>(_ => null))
                .BuildDestructuringPolicy();

            // Act
            LogEventPropertyValue result = null;
            Func<bool> action = () => policy.TryDestructure(value, ScalarOnlyFactory, out result);

            // Assert
            action.Should().NotThrow();
            result.Should().NotBeNull();

            var structureResult = (StructureValue)result;
            structureResult.Properties.Should()
                .Contain(p => p.Name == maskedName && Equals(p.Value, expectedMaskedValue));
        }
    }
}
