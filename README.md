# FeatureGates for .NET

Release observable features by using FeatureGates, pre-instrumented abstractions that replace `if` statements.

The aim of FeatureGates is to enable reusable dashboards and other tooling by standardizing the way metrics are emitted. These metrics can then be collected and shipped via [OpenTelemetry](https://opentelemetry.io/) or by any other means.

FeatureGates are vendor-agnostic and under the hood uses [.NET metrics](https://docs.microsoft.com/dotnet/core/diagnostics/metrics).

Install the package from [NuGet.org](https://www.nuget.org/packages/FeatureGates) by running;

    dotnet add package FeatureGates --version <version>

## Usage

Instead of surrounding your feature with an `if` statement like the code below;

```C#
string myFeatureFlag = "MyCoolNewFeature";
bool IsEnabled() => myFeatureManager.Evaluate(myFeatureFlag);

if (IsEnabled())
{
    // My code when IsEnabled() returns 'true'.
}
else
{
    // Optional: My code when IsEnabled() returns 'false'.
}
```

Create a FeatureGate and invoke it like the following;

```C#
string myFeatureFlag = "MyCoolNewFeature";
bool IsEnabled() => myFeatureManager.Evaluate(myFeatureFlag);

var featureGate = FeatureGate
    .WithKey(myFeatureFlag)
    .ControlledBy(IsEnabled)
    .WhenOpened(() =>
    {
        // My code when IsEnabled() returns 'true'.
    })
    .WhenClosed(() =>
    {
        // Optional: My code when IsEnabled() returns 'false'.
    });

featureGate.Invoke();
```

Once created FeatureGates are immutable. Calling methods that mutate a FeatureGate will return a new instance of that FeatureGate with the mutation.

The recommended way to create a FeatureGate is to call the static method `FeatureGate.WithKey(string featureGateKey)` which will return a `FeatureGateBuilder`. The `FeatureGateBuilder` will allow you to chain methods to help you correctly create the type of FeatureGate you need.

Alternatively you can instantiate the type of FeatureGate you need by calling its constructor. There are only four types;

- `FeatureGate`
- `FeatureGateAsync`
- `FeatureGate<TResult>`
- `FeatureGateAsync<TResult>`

The `featureGateKey` should be a unique identifier you define for your FeatureGate.

A FeatureGate can be invoked multiple times. Every time a FeatureGate is invoked, metrics are emitted.
