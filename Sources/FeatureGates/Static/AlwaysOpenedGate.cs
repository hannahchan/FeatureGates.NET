namespace FeatureGates;

using System;
using System.Threading.Tasks;
using FeatureGates.Internal;

internal static class AlwaysOpenedGate
{
    /// <summary>Immediately execute a delegate and record its execution as an 'opened' feature gate execution.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="action">The delegate to execute and record.</param>
    public static void RecordExecution(string featureGateKey, InstrumentType instrumentType, Action? action)
    {
        Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, action, instrumentType);
    }

    /// <summary>Immediately execute a delegate and record its execution as an 'opened' feature gate execution.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="function">The delegate to execute and record.</param>
    /// <typeparam name="TResult">The type of the result returned at the end of execution.</typeparam>
    /// <returns>The <typeparamref name="TResult" /> value returned from the end of execution.</returns>
    public static TResult RecordExecution<TResult>(string featureGateKey, InstrumentType instrumentType, Func<TResult> function)
    {
        return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, function, instrumentType);
    }

    /// <summary>Immediately execute a delegate and record its execution as an 'opened' feature gate execution.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="function">The delegate to execute and record.</param>
    /// <returns>The task object representing the asynchronous execution.</returns>
    public static async Task RecordExecutionAsync(string featureGateKey, InstrumentType instrumentType, Func<Task> function)
    {
        await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Opened, function, instrumentType);
    }

    /// <summary>Immediately execute a delegate and record its execution as an 'opened' feature gate execution.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="function">The delegate to execute and record.</param>
    /// <typeparam name="TResult">The type of the result returned at the end of execution.</typeparam>
    /// <returns>The <typeparamref name="TResult" /> value wrapped in a task object representing the asynchronous execution.</returns>
    public static async Task<TResult> RecordExecutionAsync<TResult>(string featureGateKey, InstrumentType instrumentType, Func<Task<TResult>> function)
    {
        return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Opened, function, instrumentType);
    }
}
