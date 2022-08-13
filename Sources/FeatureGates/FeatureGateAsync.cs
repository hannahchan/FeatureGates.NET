namespace FeatureGates;

using System;
using System.Threading.Tasks;

public class FeatureGateAsync : AbstractFeatureGate
{
    private readonly Func<Task<bool>> controlledBy;

    private readonly Func<Task>? whenOpened;

    private readonly Func<Task>? whenClosed;

    public FeatureGateAsync(string featureGateKey, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
        : base(featureGateKey, InstrumentType.Counter)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public FeatureGateAsync(string featureGateKey, InstrumentType instrumentType, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
        : base(featureGateKey, instrumentType)
    {
        this.controlledBy = controlledBy;
        this.whenOpened = whenOpened;
        this.whenClosed = whenClosed;
    }

    public async Task Invoke()
    {
        await StaticFeatureGate.Invoke(this.Key, this.InstrumentType, this.controlledBy, this.whenOpened, this.whenClosed);
    }

    public FeatureGateAsync WhenOpened(Func<Task>? function)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: this.controlledBy,
            whenOpened: function,
            whenClosed: this.whenClosed);
    }

    public FeatureGateAsync WhenOpened(Action? action)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: this.controlledBy,
            whenOpened: action == null ? null : () => Task.Run(action),
            whenClosed: this.whenClosed);
    }

    public FeatureGateAsync WhenClosed(Func<Task>? function)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: this.controlledBy,
            whenOpened: this.whenOpened,
            whenClosed: function);
    }

    public FeatureGateAsync WhenClosed(Action? action)
    {
        return new FeatureGateAsync(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            controlledBy: this.controlledBy,
            whenOpened: this.whenOpened,
            whenClosed: action == null ? null : () => Task.Run(action));
    }
}
