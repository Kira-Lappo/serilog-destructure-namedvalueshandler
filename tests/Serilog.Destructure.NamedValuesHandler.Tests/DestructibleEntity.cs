using System;
using System.Collections.Generic;

namespace Serilog.Destructure.NamedValuesHandler.Tests
{
    public class DestructibleEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime BirthDate { get; set; }

        public InnerDestructibleEntity InnerDestructibleEntity { get; set; }

        public Dictionary<string, string> KeyValues { get; set; }
    }

    public class InnerDestructibleEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
