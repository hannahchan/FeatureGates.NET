namespace FeatureGates;

using System;
using System.Threading.Tasks;
using FeatureGates.Internal;

// In future when we are confident that these APIs will be stable, we will make these public for high-performance scenarios.
// The intention is to roll these static APIs into FeatureGate.cs. Suggested file name: FeatureGate.Invokers
internal static class InternalFeatureGate
{
    public static void Invoke(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
    {
        if (controlledBy())
        {
            try
            {
                Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (fallbackOnException)
                {
                    Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
                }
                else
                {
                    throw;
                }
            }
        }
        else
        {
            Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
        }
    }

    public static TResult Invoke<TResult>(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Func<TResult> whenOpened, Func<TResult> whenClosed)
    {
        if (controlledBy())
        {
            try
            {
                return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (fallbackOnException)
                {
                    return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
                }
                else
                {
                    throw;
                }
            }
        }
        else
        {
            return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
        }
    }

    public static async Task InvokeAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
    {
        if (await controlledBy())
        {
            try
            {
                await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (fallbackOnException)
                {
                    await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
                }
                else
                {
                    throw;
                }
            }
        }
        else
        {
            await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
        }
    }

    public static async Task<TResult> InvokeAsync<TResult>(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<Task<TResult>> whenOpened, Func<Task<TResult>> whenClosed)
    {
        if (await controlledBy())
        {
            try
            {
                return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (fallbackOnException)
                {
                    return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
                }
                else
                {
                    throw;
                }
            }
        }
        else
        {
            return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
        }
    }
}
