# Serilog.Destructure.NamedValuesHandler

![GitHub Workflow Status (branch)](https://img.shields.io/github/workflow/status/Kira-Lappo/serilog-destructure-namedvalueshandler/Build%20And%20Test/dev?label=dev&style=for-the-badge)
![Nuget](https://img.shields.io/nuget/v/Serilog.Destructure.NamedValuesHandler?style=for-the-badge)

> Work In Progress. No guaranties.

Handles values of structured logs. Can mask or omit values from structured logs

## ToDos

* [ ] Cover with tests all possible use cases
* [ ] Add more examples and use-cases
* [ ] Resolve todos in code

## Typical Usage

* Mask some sensitive values:
    ```csharp
    var logger = new LoggerConfiguration()
        .Destructure
            .HandleValues(p => p
                .MaskStringValue("email")                           // *********
                .MaskStringValue("IBAN", visibleCharsAmount: 6)     // **********************560101
            )
        .CreateLogger();
    ```

* Completely remove some values:
    ```csharp
        var logger = new LoggerConfiguration()
            .Destructure
                .HandleValues(p => p
                    .OmitNames("email", "IBAN")
                    .OmitFromNamespace("Business.Domain.Secured", "System")
                    .OmitOfType(typeof(DestructibleEntity))
                )
            .CreateLogger();
    ```

* Do your own value modification:
    ```csharp
    var logger = new LoggerConfiguration()
                  .Destructure
                  .HandleValues(p => p
                      .WithNamedValueHandler(
                          "email",
                          (value, type) => $"*****@{value.Split("@").Last()}")) // *****@gmail.com
                  .CreateLogger();
    ```

* Omit by your own rules:
    ```csharp
    var logger = new LoggerConfiguration()
        .Destructure
        .HandleValues(p => p
            .WithOmitHandler(
                (name, value, type) => name == "age" && (int)value < 18))
        .CreateLogger();
    ```
