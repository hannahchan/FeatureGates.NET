namespace FeatureGates;

using System;

public class FeatureGate : AbstractFeatureGate
{
    public FeatureGate(string featureGateKey, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
        : base(featureGateKey, InstrumentType.Counter)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public FeatureGate(string featureGateKey, InstrumentType instrumentType, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
        : base(featureGateKey, instrumentType)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public Func<bool> ControlledBy { get; }

    public Action? WhenOpened { get; }

    public Action? WhenClosed { get; }

    public static FeatureGateBuilder WithKey(string featureGateKey)
    {
        return new FeatureGateBuilder(featureGateKey);
    }

    public void Invoke()
    {
        StaticFeatureGate.Invoke(this.Key, this.InstrumentType, this.ControlledBy, this.WhenOpened, this.WhenClosed);
    }
}
