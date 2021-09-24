using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Serilog.Destructure.NamedValuesHandler.Tests.Sinks;
using Serilog.Events;
using Xunit;

namespace Serilog.Destructure.NamedValuesHandler.Tests.DestructuringTests
{
    public class CollectionObjectValuesDestructuringTests : AbstractDestructuringTests
    {
        [Theory]
        [AutoMoqData]
        public void TryDestructureArray_HandleAllValuesShouldBeMasked_ValuesAreMasked(ICollection<DestructibleEntity> values)
        {
            // Arrange
            const string Mask = "******";
            var logger = new LoggerConfiguration()
                .WriteTo.List(out var logEventsProvider)
                .HandleValues(p =>
                    p.Handle<DestructibleEntity>((_, value) =>
                    {
                        value.Name = Mask;
                        return value;
                    }))
                .CreateLogger();

            // Act
            logger.Information("{@Values}", values);

            // Assert
            var logEvents = logEventsProvider.GetLogEvents();
            var eventValues = logEvents.SelectAllPropertyValues("Values");
            eventValues.Should().ContainSingle();

            var eventValue = eventValues.First();
            eventValue.Should().BeOfType<SequenceValue>();

            var logEventPropertyValues = ((SequenceValue)eventValue).Elements;
            logEventPropertyValues.Should().HaveCount(values.Count);
            logEventPropertyValues
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
