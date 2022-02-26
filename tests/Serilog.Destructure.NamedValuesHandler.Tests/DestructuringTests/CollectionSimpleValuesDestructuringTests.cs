using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Serilog.Events;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests.DestructuringTests
{
    public class CollectionSimpleValuesDestructuringTests : AbstractDestructuringTests
    {
        public static IEnumerable<object[]> CollectionExamples => new[]
        {
            new object[] { GenerateSequence<string>().ToList() },
            new object[] { GenerateSequence<string>().ToArray() },
            new object[] { GenerateSequence<string>().ToHashSet() },
            new object[] { new Queue(GenerateSequence<string>().ToList()) },
            new object[] { new Stack(GenerateSequence<string>().ToList()) },
        };

        [Theory]
        [MemberData(nameof(CollectionExamples))]
        public void TryDestructureArray_HandleAllValuesShouldBeMasked_ValuesAreMasked(IEnumerable values)
        {
            // Arrange
            const string Mask = "******";
            var maskedScalarValue = new ScalarValue(Mask);
            var policy = new NamedValueHandlersBuilder()
                .Handle<string>((_, _) => Mask)
                .BuildDestructuringPolicy();

            // Act
            var isHandled = policy.TryDestructure(values, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<SequenceValue>();

            var sequenceResult = (SequenceValue)result;
            sequenceResult.Elements.Should()
                .AllBeEquivalentTo(maskedScalarValue);
        }
    }
}
