namespace FeatureGates;

public enum MetricType
{
    /// <summary>Uses a counter to expose metrics.</summary>
    Counter,

    /// <summary>Uses a histogram to expose metrics.</summary>
    Histogram,
}
