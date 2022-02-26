using System.Collections.Generic;
using AutoFixture;
using Serilog.Core;

namespace Serilog.Destructure.NamedValuesHandler.Tests
{
    public abstract class AbstractDestructuringTests
    {
        protected ILogEventPropertyValueFactory ScalarOnlyFactory { get; } = ValueFactories.Instance.ScalarOnlyFactory;

        protected static IEnumerable<T> GenerateSequence<T>(int length = 3)
        {
            return new Fixture().CreateMany<T>(length);
        }
    }
}
