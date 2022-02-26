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
            var loggerConfiguration = originalLoggerConfiguration.HandleValues(_ => { });

            // Arrange
            loggerConfiguration.Should().BeSameAs(originalLoggerConfiguration);
        }

        [Fact]
        public void HandleValues_NullLoggerConfiguration_ExceptionIsThrown()
        {
            // Act
            Action action = () => ((LoggerConfiguration)null).HandleValues(_ => { });

            // Arrange
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void HandleValues_NullPolicyConfiguration_ExceptionIsThrown()
        {
            // Arrange
            var loggerConfiguration = new LoggerConfiguration();

            // Act
            Action action = () => loggerConfiguration.HandleValues(destructureConfiguration: null);

            // Arrange
            action.Should().NotThrow<ArgumentNullException>("we accept null options for destructure handler");
        }
    }
}
