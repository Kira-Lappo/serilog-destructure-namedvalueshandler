using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests
{
    public partial class NamedValueDestructuringPolicyTests
    {
        public static IEnumerable<object[]> NonDestructibleValues => new[]
        {
            new object[] { null },
            new object[] { Enumerable.Empty<int>() },
            new object[] { Environment.NewLine },
            new object[] { Guid.Empty },
            new object[] { DateTime.UtcNow },
        };

        [Theory]
        [MemberData(nameof(NonDestructibleValues))]
        public void TryDestructure_ValueCanNotBeDestructured_NotDestructured(object value)
        {
            // Arrange
            var policy = new NamedValueDestructuringPolicy();

            var logger = new LoggerConfiguration()
                .Destructure
                    .WithPropertyHandler(p => p
                        .OmitNames("email", "IBAN")
                        .OmitFromNamespace("Business.Domain.Secured", "System")
                        .OmitOfType(typeof(DestructibleEntity))
                    )
                .CreateLogger();

            // Act
            var isDestructured = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isDestructured.Should().BeFalse();
            result.Should().BeNull();
        }
    }
}
