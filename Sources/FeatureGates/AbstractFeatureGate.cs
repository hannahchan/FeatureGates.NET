namespace FeatureGates;

/// <summary>The base class for all feature gates.</summary>
public abstract class AbstractFeatureGate
{
    /// <summary>Initializes a new instance of the <see cref="AbstractFeatureGate"/> class.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the 'WhenClosed' operation when an uncaught exception is thrown during execution of the 'WhenOpened' operation.</param>
    protected AbstractFeatureGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException)
    {
        this.Key = featureGateKey;
        this.InstrumentType = instrumentType;
        this.FallbackOnException = fallbackOnException;
    }

    /// <summary>Gets the feature gate key. A user-defined identifier for the feature gate.</summary>
    public string Key { get; }

    /// <summary>Gets the instrument type that the feature gate will used to record executions.</summary>
    public InstrumentType InstrumentType { get; }

    /// <summary>Gets a value indicating whether to execute the 'WhenClosed' operation when an uncaught exception is thrown during execution of the 'WhenOpened' operation.</summary>
    public bool FallbackOnException { get; }
}
