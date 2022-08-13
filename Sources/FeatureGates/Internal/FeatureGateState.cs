namespace FeatureGates.Internal;

/// <summary>The state of the feature gate.</summary>
internal enum FeatureGateState
{
    /// <summary>Represents a closed feature gate.</summary>
    Closed,

    /// <summary>Represents an opened feature gate.</summary>
    Opened,
}
