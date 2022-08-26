namespace FeatureGates;

using System;
using System.Threading.Tasks;
using FeatureGates.Internal;

/// <summary>Intended for high-performance or memory-intensive scenarios, the static feature gate removes the need to <c>new</c> up feature gate instances by providing static methods that perform the same functionality.</summary>
internal static class StaticFeatureGate
{
    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
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

    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
    /// <typeparam name="TResult">The type of the result returned at the end of execution.</typeparam>
    /// <returns>The <typeparamref name="TResult" /> value returned from the end of execution.</returns>
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

    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
    /// <returns>The task object representing the asynchronous execution.</returns>
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

    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
    /// <typeparam name="TResult">The type of the result returned at the end of execution.</typeparam>
    /// <returns>The <typeparamref name="TResult" /> value wrapped in a task object representing the asynchronous execution.</returns>
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
