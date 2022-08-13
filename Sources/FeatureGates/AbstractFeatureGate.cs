namespace FeatureGates;

using System;
using System.Threading.Tasks;
using FeatureGates.Internal;

public abstract class AbstractFeatureGate
{
    protected AbstractFeatureGate(string featureGateKey, InstrumentType instrumentType)
    {
        this.Key = featureGateKey;
        this.InstrumentType = instrumentType;
    }

    public string Key { get; }

    public InstrumentType InstrumentType { get; }

    protected void Invoke(FeatureGateState featureGateState, Action? action)
    {
        Instrumentation.RecordExecution(this.Key, featureGateState, action, this.InstrumentType);
    }

    protected TResult Invoke<TResult>(FeatureGateState featureGateState, Func<TResult> function)
    {
        return Instrumentation.RecordExecution(this.Key, featureGateState, function, this.InstrumentType);
    }

    protected async Task Invoke(FeatureGateState featureGateState, Func<Task>? function)
    {
        await Instrumentation.RecordExecution(this.Key, featureGateState, function, this.InstrumentType);
    }

    protected async Task<TResult> Invoke<TResult>(FeatureGateState featureGateState, Func<Task<TResult>> function)
    {
        return await Instrumentation.RecordExecution(this.Key, featureGateState, function, this.InstrumentType);
    }
}
