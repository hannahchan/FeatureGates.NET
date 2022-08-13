namespace FeatureGates;

public abstract class AbstractFeatureGate
{
    protected AbstractFeatureGate(string featureGateKey, InstrumentType instrumentType)
    {
        this.Key = featureGateKey;
        this.InstrumentType = instrumentType;
    }

    public string Key { get; }

    public InstrumentType InstrumentType { get; }
}
