namespace FeatureGates;

using System;

public class FeatureGate<TResult> : AbstractFeatureGate
{
    public FeatureGate(string featureGateKey, Func<bool> controlledBy, Func<TResult> whenOpened, Func<TResult> whenClosed)
        : this(featureGateKey, InstrumentType.Counter, false, controlledBy, whenOpened, whenClosed)
    {
    }

    public FeatureGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Func<TResult> whenOpened, Func<TResult> whenClosed)
        : base(featureGateKey, instrumentType, fallbackOnException)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public Func<bool> ControlledBy { get; }

    public Func<TResult> WhenOpened { get; }

    public Func<TResult> WhenClosed { get; }

    public TResult Invoke()
    {
        return InternalFeatureGate.Invoke(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, this.WhenOpened, this.WhenClosed);
    }
}
