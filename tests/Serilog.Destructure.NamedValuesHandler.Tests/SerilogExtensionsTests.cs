using System;
using FluentAssertions;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests
{
    public class SerilogExtensionsTests
    {
        [Fact]
        public void HandleValues_ReturnsSameConfiguration()
        {
            // Arrange
            var originalLoggerConfiguration = new LoggerConfiguration();

            // Act
            var loggerConfiguration = originalLoggerConfiguration.Destructure.HandleValues(_ => { });

            // Arrange
            loggerConfiguration.Should().BeSameAs(originalLoggerConfiguration);
        }

        [Fact]
        public void HandleValues_NullLoggerConfiguration_ExceptionIsThrown()
        {
            // Act
            Action action = () => SerilogExtensions.HandleValues(configuration: null, _ => { });

            // Arrange
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void HandleValues_NullPolicyConfiguration_ExceptionIsThrown()
        {
            // Arrange
            var loggerConfiguration = new LoggerConfiguration();

            var logger = new LoggerConfiguration()
                .Destructure
                .HandleValues(p => p
                    .WithOmitHandler(
                        (name, value, type) => name == "age" && (int)value < 18))
                .CreateLogger();

            // Act
            Action action = () => loggerConfiguration.Destructure.HandleValues(policyConfiguration: null);

            // Arrange
            action.Should().Throw<ArgumentNullException>();
        }
    }
}
