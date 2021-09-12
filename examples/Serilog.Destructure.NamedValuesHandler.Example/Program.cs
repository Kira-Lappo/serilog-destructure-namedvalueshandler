using System;
using System.Collections.Generic;
using Serilog.Core;

namespace Serilog.Destructure.NamedValuesHandler.Example
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var logger = CreateLogger();

            var user = GetUser();

            logger.Information("Created user: {@User}", user);
        }

        private static Logger CreateLogger()
        {
            return new LoggerConfiguration()
                .WriteTo.Console()
                .Destructure
                    .HandleValues(p => p
                        .MaskStringValue("name", visibleCharsAmount:4)
                        .HandleNamedValue<string>((name, value) => "***")
                        .HandleNamedValue<DateTime>((name, value) => "DateTime.Secured")
                        .OmitNames("badAddictions", "manufacturer")
                    )
                .CreateLogger();
        }

        private static User GetUser()
        {
            return new User
            {
                Id        = Guid.NewGuid(),
                Name      = "John Watson",
                Age       = 35,
                BirthDate = new DateTime(1875, 5, 6),
                Car = new Car
                {
                    Id              = Guid.NewGuid(),
                    FullName        = "Bolt V8",
                    Model           = "V8 Cherry",
                    Manufacturer    = "Bolt",
                    ManufactureDate = new DateTime(1933),
                },
                Characteristics = new Dictionary<string, string>
                {
                    { "goodPartner", "yes" },
                    { "brave", "yes" },
                    { "badAddictions", "yes" },
                },
                CarPayment = new Dictionary<Car, decimal>
                {
                    { new Car{Id = Guid.NewGuid()}, 42000M }
                }
            };
        }
    }
}
