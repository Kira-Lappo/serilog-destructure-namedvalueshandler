using System;
using System.Collections.Generic;

namespace Serilog.Destructure.NamedValuesHandler.Example
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime BirthDate { get; set; }

        public Car Car { get; set; }

        public Dictionary<string, string> Characteristics { get; set; }

        public Dictionary<Car,decimal> CarPayment { get; set; }
    }

    public class Car
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string Model { get; set; }

        public string Manufacturer { get; set; }

        public DateTime ManufactureDate { get; set; }
    }
}
