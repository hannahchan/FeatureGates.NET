namespace FeatureGates.Internal;

using System.Diagnostics;

internal static class ActivityProvider
{
    private static readonly ActivitySource ActivitySource = new ActivitySource(
        name: typeof(MeterProvider).Assembly.GetName().Name!,
        version: typeof(MeterProvider).Assembly.GetName().Version!.ToString());

    public static void AddActivityListener(ActivityListener listener)
    {
        ActivitySource.AddActivityListener(listener);
    }

    public static bool HasListeners()
    {
        return ActivitySource.HasListeners();
    }

    public static Activity? StartActivity(string name)
    {
        return ActivitySource.StartActivity(name, ActivityKind.Internal);
    }
}
