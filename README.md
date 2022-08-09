# FeatureGates for .NET

Release observable features by using FeatureGates, pre-instrumented abstractions that replace `if` statements.

The aim of FeatureGates is to enable reusable dashboards, tooling and automation by standardizing the way metrics and traces are emitted. These metrics and traces can then be collected and shipped via [OpenTelemetry](https://opentelemetry.io/) or by other means.

FeatureGates are vendor-agnostic and under the hood uses [.NET metrics](https://docs.microsoft.com/dotnet/core/diagnostics/metrics) and [.NET distributed tracing](https://docs.microsoft.com/dotnet/core/diagnostics/distributed-tracing).

Install the package from [NuGet.org](https://www.nuget.org/packages/FeatureGates) by running;

```sh
dotnet add package FeatureGates --version <version>
```

## Usage and Features

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

Create a feature gate and invoke it like the following;

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

Feature gate executions are recorded every time a feature gate is invoked. A feature gate may be invoked multiple times.

### Construction

The recommended way to create a feature gate is to call the static method `FeatureGate.WithKey("MyFeatureGateKey")` which will return a `FeatureGateBuilder`. The `FeatureGateBuilder` will allow you to chain methods to help you correctly create the type of feature gate you need.

Alternatively you can instantiate the type of feature gate you need by calling its constructor. There are only four types;

- `FeatureGate`
- `FeatureGateAsync`
- `FeatureGate<TResult>`
- `FeatureGateAsync<TResult>`

Once created feature gates are immutable. Calling methods that mutate a feature gate will return a new instance of that feature gate with that mutation.

### Feature Gate Key

The feature gate key is a unique string identifier that you define for each of your feature gates. Using the same name as the feature flag that controls your feature gate is recommended. If your feature flag controls more than one feature gate, adding a unique suffix to the feature gate key for each feature gate is recommended.

> **Warning** - Failure to give each of your feature gates in your application a unique feature gate key will result in inaccurate metrics being collected for those feature gates.

### Instrument Types

By default, feature gates are configured to record executions using a counter. If you want to capture timings for your feature, you can change the instrument type of your feature gate during its construction to a histogram. Histograms should be used cautiously as they use more memory than counters.

## Metrics

> **Warning** - Metric names currently unstable.

Depending on the instrument type configured for a feature gate, feature gates will output one of the following metrics.

| Name                      | Instrument Type | Unit         | Description                                                    |
| ------------------------- | --------------- | ------------ | -------------------------------------------------------------- |
| `feature.gate.executions` | Counter         | executions   | Measures the number of times a feature gate has been executed. |
| `feature.gate.duration`   | Histogram       | milliseconds | Measures the duration of feature gate executions.              |

### Metric Attributes

> **Warning** - Metric attributes currently unstable.

The metrics that feature gates output can be aggregated or filtered on the following dimensions.

| Name                     | Description                                                                   |
| -----------------------  | ----------------------------------------------------------------------------- |
| `feature.gate.key`       | The unique string identifier for a feature gate.                              |
| `feature.gate.state`     | Whether a feature gate was executed as `opened` or `closed`.                  |
| `feature.gate.exception` | `true` if an uncaught exception occurred during execution, otherwise `false`. |

### Metric Collection

In order to collect metrics from your feature gates, you will need to subscribe to the `Meter` named `FeatureGates`. If using the [OpenTelemetry .NET](https://github.com/open-telemetry/opentelemetry-dotnet) SDK, you can do this by calling the `AddFeatureGates()` extension method while building your `MeterProvider` in your application. For example;

```C#
using MeterProvider meterProvider = Sdk.CreateMeterProviderBuilder()
    // Other configuration
    .AddFeatureGates()
    // Other configuration
    .Build();
```

It is also possible to subscribe to the metrics from your feature gates by using a [MeterListener](https://docs.microsoft.com/dotnet/api/system.diagnostics.metrics.meterlistener).

To learn more on how to collect metrics, please checkout the [metrics collection tutorial](https://docs.microsoft.com/en-au/dotnet/core/diagnostics/metrics-collection) at the .NET documentation website.

## Traces

Feature gates also output spans that represent feature gate executions to be included in traces. These spans will have the operation name `feature.gate.execution` so that you can easily identify them in a trace.

A span is also known as an [Activity](https://docs.microsoft.com/dotnet/api/system.diagnostics.activity) in .NET.

### Span Attributes

> **Warning** - Span attributes are currently unstable.

Spans emitted by feature gates will have the following attributes.

| Name                      | Description                                                                               |
| ------------------------- | ----------------------------------------------------------------------------------------- |
| `feature.gate.key`        | The unique string identifier for a feature gate.                                          |
| `feature.gate.state`      | Whether a feature gate was executed as `opened` or `closed`.                              |
| `otel.status_code`        | `OK` if the execution was successful otherwise `ERROR` if an uncaught exception occurred. |
| `otel.status_description` | A description that is only present when `otel.status_code` is set to `ERROR`.             |

### Span Events

> **Warning** - Span event attributes are currently unstable.

When an uncaught exception occurs during a feature gate execution, a span event will be added to the span with the details of the exception. The span event will have the following details.

| Name                   | Description                                                |
| ---------------------- | ---------------------------------------------------------- |
| `event`                | The event name which will always be the value `exception`. |
| `exception.message`    | The exception message.                                     |
| `exception.stacktrace` | A stacktrace related to the exception.                     |
| `exception.type`       | The type of the exception including its namespace.         |

### Span Collection

In order to collect spans from your feature gates, you will need to subscribe to the `ActivitySource` named `FeatureGates`. If using the [OpenTelemetry .NET](https://github.com/open-telemetry/opentelemetry-dotnet) SDK, you can do this by calling the `AddFeatureGates()` extension method while building your `TracerProvider` in your application. For example;

```C#
using TracerProvider tracerProvider = Sdk.CreateTracerProviderBuilder()
    // Other configuration
    .AddFeatureGates()
    // Other configuration
    .Build();
```

It is also possible to subscribe to spans from your feature gates by using an [ActivityListener](https://docs.microsoft.com/dotnet/api/system.diagnostics.activitylistener).

To learn more on how to collect spans, please checkout the [trace collection tutorial](https://docs.microsoft.com/dotnet/core/diagnostics/distributed-tracing-collection-walkthroughs) at the .NET documentation website.
