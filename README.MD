# Serilog.Destructure.NamedValuesHandler

> Work In Progress. No guaranties.

Handles values of structured logs. Can mask or omit values from structured logs

## ToDos

* [ ] Cover with tests all possible use cases
* [ ] Add more examples and use-cases
* [ ] Resolve todos in code

## Typical Usage

* Mask some sensitive values
    ```csharp
    var logger = new LoggerConfiguration()
        .Destructure
            .WithPropertyHandler(p => p
                .MaskStringValue("email")                           // *********
                .MaskStringValue("IBAN", visibleCharsAmount: 6)     // **********************560101
            )
        .CreateLogger();
    ```

* Completely remove some values
    ```csharp
        var logger = new LoggerConfiguration()
            .Destructure
                .WithPropertyHandler(p => p
                    .OmitNames("email", "IBAN")
                    .OmitFromNamespace("Business.Domain.Secured", "System")
                    .OmitOfType(typeof(DestructibleEntity))
                )
            .CreateLogger();
    ```
