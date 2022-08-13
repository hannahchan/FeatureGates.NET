namespace FeatureGates;

using System;
using System.Threading.Tasks;

public class FeatureGateAsync<TResult> : AbstractFeatureGate
{
    private readonly Func<Task<bool>> controlledBy;

    private readonly Func<Task<TResult>> whenOpened;

    private readonly Func<Task<TResult>> whenClosed;

    public FeatureGateAsync(string featureGateKey, Func<Task<bool>> controlledBy, Func<Task<TResult>> whenOpened, Func<Task<TResult>> whenClosed)
        : base(featureGateKey, InstrumentType.Counter)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public FeatureGateAsync(string featureGateKey, InstrumentType instrumentType, Func<Task<bool>> controlledBy, Func<Task<TResult>> whenOpened, Func<Task<TResult>> whenClosed)
        : base(featureGateKey, instrumentType)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public async Task<TResult> Invoke()
    {
        return await StaticFeatureGate.Invoke(this.Key, this.InstrumentType, this.controlledBy, this.whenOpened, this.whenClosed);
    }

    public FeatureGateAsync<TResult> WhenOpened(Func<Task<TResult>> function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: this.controlledBy,
            whenOpened: function,
            whenClosed: this.whenClosed);
    }

    public FeatureGateAsync<TResult> WhenOpened(Func<TResult> function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: this.controlledBy,
            whenOpened: () => Task.Run(function),
            whenClosed: this.whenClosed);
    }

    public FeatureGateAsync<TResult> WhenClosed(Func<Task<TResult>> function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: this.controlledBy,
            whenOpened: this.whenOpened,
            whenClosed: function);
    }

    public FeatureGateAsync<TResult> WhenClosed(Func<TResult> function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: this.controlledBy,
            whenOpened: this.whenOpened,
            whenClosed: () => Task.Run(function));
    }
}
