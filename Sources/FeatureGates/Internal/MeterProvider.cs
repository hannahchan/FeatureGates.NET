namespace FeatureGates.Internal;

using System.Diagnostics.Metrics;

internal static class MeterProvider
{
    public static readonly Meter Meter = new Meter(
        name: typeof(MeterProvider).Assembly.GetName().Name!,
        version: typeof(MeterProvider).Assembly.GetName().Version!.ToString());
}
