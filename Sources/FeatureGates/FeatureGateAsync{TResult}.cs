namespace FeatureGates;

using System;
using System.Threading.Tasks;

public class FeatureGateAsync<TResult> : AbstractFeatureGate
{
    public FeatureGateAsync(string featureGateKey, Func<Task<bool>> controlledBy, Func<Task<TResult>> whenOpened, Func<Task<TResult>> whenClosed)
        : base(featureGateKey, InstrumentType.Counter)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public FeatureGateAsync(string featureGateKey, InstrumentType instrumentType, Func<Task<bool>> controlledBy, Func<Task<TResult>> whenOpened, Func<Task<TResult>> whenClosed)
        : base(featureGateKey, instrumentType)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public Func<Task<bool>> ControlledBy { get; }

    public Func<Task<TResult>> WhenOpened { get; }

    public Func<Task<TResult>> WhenClosed { get; }

    public async Task<TResult> InvokeAsync()
    {
        return await StaticFeatureGate.InvokeAsync(this.Key, this.InstrumentType, this.ControlledBy, this.WhenOpened, this.WhenClosed);
    }
}
