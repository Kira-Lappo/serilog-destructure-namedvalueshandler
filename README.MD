# Serilog.Destructure.NamedValuesHandler

![GitHub Workflow Status (branch)](https://img.shields.io/github/workflow/status/Kira-Lappo/serilog-destructure-namedvalueshandler/Build%20And%20Test/dev?label=dev&style=for-the-badge)
[
![Nuget](https://img.shields.io/nuget/v/Serilog.Destructure.NamedValuesHandler?style=for-the-badge)
](
https://www.nuget.org/packages/Serilog.Destructure.NamedValuesHandler
)

> Work In Progress. No guaranties.

## ToDos

* [ ] Investigate how Serilog handles system values(string, DateTime, etc.) at destructuring
* [ ] Resolve todos in code

## Idea

You use Serilog and structured logging in your project.

You want to mask or even omit some special values from properties of your log events.

This package was created exactly for these purposes.

## Usage

### Log Example

So you have an object to log:

```csharp
var user = new User
{
    Name = "John Watson",
    Email = "dr.john.h.watson@johnwatsonblog.co.uk",
    Age = 42,
};
```

We do want to mask doctor's email for whatever reason, but we want to keep some part of that information. Let's setup logger so we could see just the latest 7
characters of the email:

```csharp
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .HandleValues(p => p
        .Mask("email", visibleCharsAmount:7)
    )
    .CreateLogger();
```

And then let's check how it logs this object:

```csharp
logger.Information("Logged User: {@User}", user);
```

```bash
[01:36:10 INF] Logged User: {"Name": "John Watson", "Email": "******************************g.co.uk", "Age": 42}
```

But we may have more complex situation. For example, we still want to see at least a domain name.

Let's change mask handling:

```csharp
.HandleValues(p => p
    .Handle<string>(
      "email",
      value => string.IsNullOrWhiteSpace(value)
          ? value
          : $"*****@{value.Split("@").Last()}")
    )
)
```

Now we have only domain name in a log message:

```bash
[01:43:25 INF] Logged User: {"Name": "John Watson", "Email": "*****@johnwatsonblog.co.uk", "Age": 42}
```

We can also omit some values so they disappear from log message:

```csharp
.HandleValues(p => p
    .Omit("email", "age")
)
```

```bash
[01:49:19 INF] Logged User: {"Name": "John Watson"}
```

## Special Cases : "Why It Does Not Work?"

### Case 1: a property of a simple type is not masked

Setup:
```csharp
// At logger creation
.HandleValues(p => p
    .Handle<string>((name, value) => "***")
)

// Log event
logger.Information("String property: {@StringValue}", "Sherlock Holmes");

// Log message
[00:17:32 INF] String property: Sherlock Holmes
```

Unfortunately by existing Serilog implementation
Destructure polices can't not affect such properties.

Properties of [simple types](https://github.com/serilog/serilog/blob/13dd8cfc5ee47a841128beaef853b3798464f807/src/Serilog/Capturing/PropertyValueConverter.cs#L37)
are handled by Serilog only and are not passed to a custom Destructuring policy.

Still they can be omitted
