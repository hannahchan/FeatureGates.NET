namespace FeatureGates;

public abstract class AbstractFeatureGate
{
    protected AbstractFeatureGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException)
    {
        this.Key = featureGateKey;
        this.InstrumentType = instrumentType;
        this.FallbackOnException = fallbackOnException;
    }

    public string Key { get; }

    public InstrumentType InstrumentType { get; }

    public bool FallbackOnException { get; }
}
