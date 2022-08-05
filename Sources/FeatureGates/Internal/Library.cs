namespace FeatureGates.Internal;

using System;

internal static class Library
{
    public static class Assembly
    {
        public static readonly string Name = typeof(Library).Assembly.GetName().Name!;

        public static readonly Version Version = typeof(Library).Assembly.GetName().Version!;
    }

    public static class ActivitySource
    {
        public static readonly string Name = Assembly.Name;

        public static readonly string Version = Assembly.Version.ToString();
    }

    public static class Meter
    {
        public static readonly string Name = Assembly.Name;

        public static readonly string Version = Assembly.Version.ToString();
    }
}
