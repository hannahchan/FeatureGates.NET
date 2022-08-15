namespace FeatureGates;

using System;

public partial class FeatureGate : AbstractFeatureGate
{
    public FeatureGate(string featureGateKey, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
        : this(featureGateKey, InstrumentType.Counter, controlledBy, whenOpened, whenClosed)
    {
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

    public void Invoke()
    {
        StaticFeatureGate.Invoke(this.Key, this.InstrumentType, this.ControlledBy, this.WhenOpened, this.WhenClosed);
    }
}
