using System;
using FluentAssertions;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests.DestructuringTests
{
    public class HandlerNegativeCasesTests
    {
        private ILogEventPropertyValueFactory ScalarOnlyFactory { get; } = ValueFactories.Instance.ScalarOnlyFactory;

        [Theory]
        [AutoMoqData]
        public void TryDestructure_ValueHandlerThrowsException_ValueIsNotModified(DestructibleEntity value)
        {
            // Arrange
            var maskedName = nameof(value.Name);
            var maskedValue = new ScalarValue(value.Name);

            var policy = new NamedValueDestructuringPolicy.NamedValuePolicyBuilder()
                .HandleNamedValue(
                    maskedName,
                    new Func<string, string>(
                        _ =>
                            throw new Exception(nameof(TryDestructure_ValueHandlerThrowsException_ValueIsNotModified))))
                .Build();

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

            var policy = new NamedValueDestructuringPolicy.NamedValuePolicyBuilder()
                .WithOmitHandler(
                    (_, _, _) =>
                        throw new Exception(nameof(TryDestructure_OmitterThrowsException_ValueIsNotOmitted)))
                .Build();

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
    }
}