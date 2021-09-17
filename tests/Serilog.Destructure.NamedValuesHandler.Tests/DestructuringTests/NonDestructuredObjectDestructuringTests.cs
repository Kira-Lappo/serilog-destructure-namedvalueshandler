using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests.DestructuringTests
{
    public class NonDestructuredObjectDestructuringTests : AbstractDestructuringTests
    {
        public static IEnumerable<object[]> NonDestructibleValues => new[]
        {
            new object[] { null },
            new object[] { Enumerable.Empty<int>() },
            new object[] { Environment.NewLine },
            new object[] { Guid.Empty },
            new object[] { DateTime.UtcNow },
            new object[] { HttpStatusCode.InternalServerError },
            new object[] { 42M },
            new object[] { 42D },
            new object[] { 42L },
            new object[] { 42 },
            new object[] { (short)42 },
            new object[] { '*' },
            new object[] { true }
        };

        [Theory]
        [MemberData(nameof(NonDestructibleValues))]
        public void TryDestructure_ValueCanNotBeDestructured_NotDestructured(object value)
        {
            // Arrange
            var policy = ValueFactories.Instance.EmptyPolicy;

            // Act
            var isDestructured = policy.TryDestructure(value, ScalarOnlyFactory, out var result);

            // Assert
            isDestructured.Should().BeFalse();
            result.Should().BeNull();
        }
    }
}
