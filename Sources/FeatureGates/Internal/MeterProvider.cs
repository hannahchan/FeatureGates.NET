namespace FeatureGates.Internal;

using System.Diagnostics.Metrics;

internal static class MeterProvider
{
    public static readonly Meter Meter = new Meter(
        name: Library.Meter.Name,
        version: Library.Meter.Version);
}
