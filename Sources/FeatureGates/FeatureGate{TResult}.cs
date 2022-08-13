namespace FeatureGates;

using System;
using System.Threading.Tasks;

public class FeatureGate<TResult> : AbstractFeatureGate
{
    private readonly Func<bool> controlledBy;

    private readonly Func<TResult> whenOpened;

    private readonly Func<TResult> whenClosed;

    public FeatureGate(string featureGateKey, Func<bool> controlledBy, Func<TResult> whenOpened, Func<TResult> whenClosed)
        : base(featureGateKey, InstrumentType.Counter)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public FeatureGate(string featureGateKey, InstrumentType instrumentType, Func<bool> controlledBy, Func<TResult> whenOpened, Func<TResult> whenClosed)
        : base(featureGateKey, instrumentType)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public TResult Invoke()
    {
        return StaticFeatureGate.Invoke(this.Key, this.InstrumentType, this.controlledBy, this.whenOpened, this.whenClosed);
    }

    public FeatureGate<TResult> WhenOpened(Func<TResult> function)
    {
        return new FeatureGate<TResult>(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: this.controlledBy,
            whenOpened: function,
            whenClosed: this.whenClosed);
    }

    public FeatureGateAsync<TResult> WhenOpened(Func<Task<TResult>> function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: () => Task.Run(this.controlledBy),
            whenOpened: function,
            whenClosed: () => Task.Run(this.whenClosed));
    }

    public FeatureGate<TResult> WhenClosed(Func<TResult> function)
    {
        return new FeatureGate<TResult>(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: this.controlledBy,
            whenOpened: this.whenOpened,
            whenClosed: function);
    }

    public FeatureGateAsync<TResult> WhenClosed(Func<Task<TResult>> function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: () => Task.Run(this.controlledBy),
            whenOpened: () => Task.Run(this.whenOpened),
            whenClosed: function);
    }
}
