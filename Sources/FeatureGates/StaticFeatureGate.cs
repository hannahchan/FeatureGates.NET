namespace FeatureGates;

using System;
using System.Threading.Tasks;
using FeatureGates.Internal;

public static class StaticFeatureGate
{
    public static void Invoke(string featureGateKey, InstrumentType instrumentType, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
    {
        if (controlledBy())
        {
            Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
        }
        else
        {
            Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
        }
    }

    public static TResult Invoke<TResult>(string featureGateKey, InstrumentType instrumentType, Func<bool> controlledBy, Func<TResult> whenOpened, Func<TResult> whenClosed)
    {
        return controlledBy()
            ? Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType)
            : Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
    }

    public static async Task Invoke(string featureGateKey, InstrumentType instrumentType, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
    {
        if (await controlledBy())
        {
            await Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
        }
        else
        {
            await Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
        }
    }

    public static async Task<TResult> Invoke<TResult>(string featureGateKey, InstrumentType instrumentType, Func<Task<bool>> controlledBy, Func<Task<TResult>> whenOpened, Func<Task<TResult>> whenClosed)
    {
        return await controlledBy()
            ? await Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType)
            : await Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
    }
}
