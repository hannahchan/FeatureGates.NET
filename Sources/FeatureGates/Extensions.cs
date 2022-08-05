namespace FeatureGates;

using FeatureGates.Internal;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

public static class Extensions
{
    public static MeterProviderBuilder AddFeatureGates(this MeterProviderBuilder builder)
    {
        return builder.AddMeter(Library.Meter.Name);
    }

    public static TracerProviderBuilder AddFeatureGates(this TracerProviderBuilder builder)
    {
        return builder.AddSource(Library.ActivitySource.Name);
    }
}
