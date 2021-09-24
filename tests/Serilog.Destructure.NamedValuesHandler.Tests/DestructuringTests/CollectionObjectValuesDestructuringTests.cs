using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Serilog.Events;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests.DestructuringTests
{
    public class CollectionObjectValuesDestructuringTests : AbstractDestructuringTests
    {
        [Theory]
        [AutoMoqData]
        public void TryDestructureArray_HandleAllValuesShouldBeMasked_ValuesAreMasked(IEnumerable<DestructibleEntity> values)
        {
            // Arrange
            const string Mask = "******";
            var policy = new NamedValueHandlersBuilder()
                .Handle<DestructibleEntity>((_, value) =>
                {
                    value.Name = Mask;
                    return value;
                })
                .BuildDestructuringPolicy();

            // Act
            var isHandled = policy.TryDestructure(values, ScalarOnlyFactory, out var result);

            // Assert
            isHandled.Should().BeTrue();
            result.Should().NotBeNull();
            result.Should().BeOfType<SequenceValue>();

            var sequenceResult = (SequenceValue)result;
            sequenceResult.Elements.Should().HaveCount(values.Count());
            sequenceResult.Elements
                .Select(e =>
                {
                    using var sw = new StringWriter();
                    e.Render(sw);
                    sw.Flush();
                    return sw.ToString();
                })
                .Should()
                .OnlyContain(s => s.Contains(Mask));
        }
    }
}
