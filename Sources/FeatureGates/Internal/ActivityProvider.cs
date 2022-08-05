namespace FeatureGates.Internal;

using System.Diagnostics;

internal static class ActivityProvider
{
    private static readonly ActivitySource ActivitySource = new ActivitySource(
        name: Library.ActivitySource.Name,
        version: Library.ActivitySource.Version);

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
