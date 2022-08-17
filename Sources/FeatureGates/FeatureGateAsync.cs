namespace FeatureGates;

using System;
using System.Threading.Tasks;

public class FeatureGateAsync : AbstractFeatureGate
{
    public FeatureGateAsync(string featureGateKey, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
        : this(featureGateKey, InstrumentType.Counter, false, controlledBy, whenOpened, whenClosed)
    {
    }

    public FeatureGateAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
        : base(featureGateKey, instrumentType, fallbackOnException)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    public Func<Task<bool>> ControlledBy { get; }

    public Func<Task>? WhenOpened { get; }

    public Func<Task>? WhenClosed { get; }

    public async Task InvokeAsync()
    {
        await InternalFeatureGate.InvokeAsync(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, this.WhenOpened, this.WhenClosed);
    }
}
