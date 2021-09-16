using Moq;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Destructure.NamedValuesHandler.Tests
{
    public class ValueFactories
    {
        private ValueFactories()
        {
            var factoryMock = new Mock<ILogEventPropertyValueFactory>();
            factoryMock
                .Setup(_ => _.CreatePropertyValue(It.IsAny<object>(), It.IsAny<bool>()))
                .Returns((object o, bool isDestruct) => new ScalarValue(o));

            ScalarOnlyFactory = factoryMock.Object;
        }

        public static ValueFactories Instance { get; } = new();

        public ILogEventPropertyValueFactory ScalarOnlyFactory { get; }

        internal NamedValueDestructuringPolicy EmptyPolicy => new NamedValueHandlersBuilder().BuildDestructuringPolicy();
    }
}
