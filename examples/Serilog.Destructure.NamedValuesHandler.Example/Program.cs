using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Serilog.Core;

namespace Serilog.Destructure.NamedValuesHandler.Example
{
    public static class Program
    {
        private static readonly DateTime SpecialDate = new(year: 1905, month: 1, day: 1);

        public static void Main(string[] args)
        {
            var configuration = CreateConfiguration();
            var logger = CreateLogger(configuration);

            var user = GetUser();
            logger.Information("Created user: {@User}", user);
            logger.Information("The next values will not be deconstructed, deconstruct policy does not handle that.");
            logger.Information("String property: {@StringValue}",          "Sherlock Holmes");
            logger.Information("DateTime Example 1: {@BeforeSpecialDate}", SpecialDate.AddDays(value: -1));
            logger.Information("DateTime Example 2: {@AfterSpecialDate}",  SpecialDate.AddDays(value: +1));
        }

        private static Logger CreateLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .HandleValues(
                    p => p
                        .Mask("name", visibleCharsAmount: 4)
                        .Handle<string>((name, value) => "***")
                        .Handle<DateTime>(
                            (name, value) => value > SpecialDate
                                ? "DateTime.Secured"
                                : value)
                        .Omit("badAddictions", "manufacturer"))
                .CreateLogger();
        }

        private static User GetUser()
        {
            return new User
            {
                Id        = Guid.NewGuid(),
                Name      = "John Watson",
                Age       = 35,
                BirthDate = new DateTime(year: 1875, month: 5, day: 6),
                Car = new Car
                {
                    Id              = Guid.NewGuid(),
                    FullName        = "Bolt V8",
                    Model           = "V8 Cherry",
                    Manufacturer    = "Bolt",
                    ManufactureDate = new DateTime(year: 1933, month: 4, day: 5)
                },
                Characteristics = new Dictionary<string, string>
                {
                    { "goodPartner", "yes" },
                    { "brave", "yes" },
                    { "badAddictions", "yes" }
                },
                CarPayment = new Dictionary<Car, decimal>
                {
                    {
                        new Car
                        {
                            Id = Guid.NewGuid()
                        },
                        42000M
                    }
                }
            };
        }

        private static IConfiguration CreateConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }
}
