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

            logger.Information("Masked values examples");
            logger.Information("Object Destructuring: {@User}",                                                        user);
            logger.Information("Array of Objects Destructuring: {@UserArray}",                                         new[] { user });
            logger.Information("Dictionary Destructuring 1: {@Characteristics}",                                       user.Characteristics);
            logger.Information("Dictionary Destructuring 2: {@CarPayments}",                                           user.CarPayment);
            logger.Information("Property value is replaced by its name 'ReplacedStringValue': {@ReplacedStringValue}", "Sherlock Holmes");
            logger.Information("Property value is omitted by its name 'OmittedStringValue' : {@OmittedStringValue}",   "Sherlock Holmes");

            logger.Information("==============================================");

            logger.Information("The next values will not be deconstructed or masked, custom deconstruct policy/enrichers can't handle such cases.");
            logger.Information("Property value is NOT replaced by its type 'string': {@StringValue}",                       "Sherlock Holmes");
            logger.Information("Property value is NOT replaced by its type 'string' (with cast to object): {@StringValue}", (object)"Sherlock Holmes");
            logger.Information("Property value is NOT replaced by its type 'DateTime' 1: {@BeforeSpecialDate}",             SpecialDate.AddDays(value: -1));
            logger.Information("Property value is NOT replaced by its type 'DateTime' 2: {@AfterSpecialDate}",              SpecialDate.AddDays(value: +1));

            // Output
            // [16:12:34 INF] Masked values examples
            // [16:12:34 INF] Object Destructuring: {"Id": "705c97d8-98a7-484d-ad72-b9b6f124cc2b", "Name": "*******tson", "Age": 35, "BirthDate": "1875-05-06T00:00:00.0000000", "Car": {"Id": "8
            // 18f2664-00b2-462e-b86f-05fa36f859fd", "FullName": "***", "Model": "***", "ManufactureDate": "DateTime.Secured", "$type": "Car"}, "Characteristics": {"goodPartner": "***", "brave"
            // : "***"}, "CarPayment": {"Car { Id: 30d52acb-75b3-4537-b5ad-0769490014bc, FullName: \"***\", Model: \"***\", ManufactureDate: 01/01/0001 00:00:00 }": 42000}, "Tags": ["***", "***
            // ", "***"], "$type": "User"}
            // [16:12:34 INF] Object Destructuring: [{"Id": "705c97d8-98a7-484d-ad72-b9b6f124cc2b", "Name": "*******tson", "Age": 35, "BirthDate": "1875-05-06T00:00:00.0000000", "Car": {"Id": "
            // 818f2664-00b2-462e-b86f-05fa36f859fd", "FullName": "***", "Model": "***", "ManufactureDate": "DateTime.Secured", "$type": "Car"}, "Characteristics": {"goodPartner": "***", "brave
            // ": "***"}, "CarPayment": {"Car { Id: null, FullName: null, Model: null, ManufactureDate: null }": 42000}, "Tags": ["***", "***", "***"], "$type": "User"}]
            // [16:12:34 INF] Dictionary Destructuring 1: {"goodPartner": "***", "brave": "***"}
            // [16:12:34 INF] Dictionary Destructuring 2: {"Car { Id: 30d52acb-75b3-4537-b5ad-0769490014bc, FullName: \"***\", Model: \"***\", ManufactureDate: 01/01/0001 00:00:00 }": 42000}
            // [16:12:34 INF] Property value is replaced by its name 'ReplacedStringValue': +++===+++
            // [16:12:34 INF] Property value is omitted by its name 'OmittedStringValue' : {@OmittedStringValue}
            // [16:12:34 INF] ==============================================
            // [16:12:34 INF] The next values will not be deconstructed or masked, custom deconstruct policy/enrichers can't handle such cases.
            // [16:12:34 INF] Property value is NOT replaced by its type 'string': Sherlock Holmes
            // [16:12:34 INF] Property value is NOT replaced by its type 'string' (with cast to object): Sherlock Holmes
            // [16:12:34 INF] Property value is NOT replaced by its type 'DateTime' 1: 12/31/1904 00:00:00
            // [16:12:34 INF] Property value is NOT replaced by its type 'DateTime' 2: 01/02/1905 00:00:00
        }

        private static Logger CreateLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .HandleValues(
                    p => p
                        .Mask("name", visibleCharsAmount: 4)
                        .Handle("ReplacedStringValue", (o, type) => "+++===+++")
                        .Handle<string>((name, value) => "***")
                        .Handle<DateTime>(
                            (name, value) => value > SpecialDate
                                ? "DateTime.Secured"
                                : value)
                        .Omit("badAddictions", "manufacturer", "OmittedStringValue"))
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
                Tags = new HashSet<string>
                {
                    "detective",
                    "side-kick",
                    "medicine"
                },
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
