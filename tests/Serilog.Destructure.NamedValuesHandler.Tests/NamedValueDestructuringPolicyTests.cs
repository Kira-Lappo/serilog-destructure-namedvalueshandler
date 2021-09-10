using System;
using Moq;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Destructure.NamedValuesHandler.Tests
{
    public partial class NamedValueDestructuringPolicyTests
    {
        public NamedValueDestructuringPolicyTests()
        {
            var factoryMock = new Mock<ILogEventPropertyValueFactory>();
            factoryMock
                .Setup(_ => _.CreatePropertyValue(It.IsAny<object>(), It.IsAny<bool>()))
                .Returns((object o, bool isDestruct) => new ScalarValue(o));

            ScalarOnlyFactory = factoryMock.Object;
        }

        private ILogEventPropertyValueFactory ScalarOnlyFactory { get; }

        public class DestructibleEntity
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public int Age { get; set; }

            public DateTime BirthDate { get; set; }

            public InnerDestructibleEntity InnerDestructibleEntity { get; set; }
        }

        public class InnerDestructibleEntity
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public int Age { get; set; }

            public DateTime BirthDate { get; set; }
        }
    }
}
