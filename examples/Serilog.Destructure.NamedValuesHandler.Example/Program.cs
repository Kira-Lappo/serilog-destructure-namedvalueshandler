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
            // [01:57:06 INF] Masked values examples
            // [01:57:06 INF] Object Destructuring: {"Id": "71570f6a-05f4-4884-970b-34918af003a3", "Name": "*******tson", "Age": 35, "BirthDate": "1875-05-06T00:00:00.0000000", "Car": {"Id": "6
            // 1b032f1-a71c-452f-b811-c4c9ff98a2c3", "FullName": "***", "Model": "***", "ManufactureDate": "DateTime.Secured", "$type": "Car"}, "Characteristics": {"goodPartner": "***", "brave"
            // : "***"}, "CarPayment": {"Car { Id: fbd8921c-fd3a-4646-9600-e4b916cbe54f, FullName: \"***\", Model: \"***\", ManufactureDate: 01/01/0001 00:00:00 }": 42000}, "Tags": ["***", "***
            // ", "***"], "$type": "User"}
            // [01:57:06 INF] Dictionary Destructuring 1: {"goodPartner": "***", "brave": "***"}
            // [01:57:06 INF] Dictionary Destructuring 2: {"Car { Id: fbd8921c-fd3a-4646-9600-e4b916cbe54f, FullName: \"***\", Model: \"***\", ManufactureDate: 01/01/0001 00:00:00 }": 42000}
            // [01:57:06 INF] Property value is replaced by its name 'ReplacedStringValue': +++===+++
            // [01:57:06 INF] Property value is omitted by its name 'OmittedStringValue' : {@OmittedStringValue}
            // [01:57:06 INF] ==============================================
            // [01:57:06 INF] The next values will not be deconstructed or masked, custom deconstruct policy/enrichers can't handle such cases.
            // [01:57:06 INF] Property value is NOT replaced by its type 'string': Sherlock Holmes
            // [01:57:06 INF] Property value is NOT replaced by its type 'string' (with cast to object): Sherlock Holmes
            // [01:57:06 INF] Property value is NOT replaced by its type 'DateTime' 1: 12/31/1904 00:00:00
            // [01:57:06 INF] Property value is NOT replaced by its type 'DateTime' 2: 01/02/1905 00:00:00

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
                Tags      = new []{ "detective", "side-kick", "medicine" },
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
