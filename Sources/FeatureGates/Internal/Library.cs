namespace FeatureGates.Internal;

using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;

internal static class Library
{
    public static readonly string Name = typeof(Library).Assembly.GetName().Name!;

    public static readonly Version Version = typeof(Library).Assembly.GetName().Version!;

    public static readonly ActivitySource ActivitySource = new ActivitySource(
        name: Name,
        version: Version.ToString());

    public static readonly Meter Meter = new Meter(
        name: Name,
        version: Version.ToString());
}
