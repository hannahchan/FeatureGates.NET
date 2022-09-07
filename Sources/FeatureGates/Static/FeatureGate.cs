namespace FeatureGates.Static;

using System;
using System.Threading.Tasks;
using FeatureGates.Internal;

/// <summary>Intended for high-performance or memory-intensive scenarios, the static feature gate removes the need to <c>new</c> up feature gate instances by providing static methods that perform the same functionality.</summary>
public static class FeatureGate
{
    /*
        The following four static methods are core to the functionality of:

            - FeatureGate.cs
            - FeatureGateAsync.cs
            - FeatureGate{TResult}.cs
            - FeatureGateAsync{TResult}.cs
    */

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
                if (!fallbackOnException)
                {
                    throw;
                }

                Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }

            return;
        }

        Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
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
                if (!fallbackOnException)
                {
                    throw;
                }

                return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }
        }

        return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
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
                if (!fallbackOnException)
                {
                    throw;
                }

                await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }

            return;
        }

        await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
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
                if (!fallbackOnException)
                {
                    throw;
                }

                return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }
        }

        return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
    }

    /*
        Other permutations of the InvokeAsync() where one of the parameters is an asynchronous delegate.

        | controlledBy     | whenOpened  | whenClosed  |
        |------------------|-------------|-------------|
        | Func<Task<bool>> | Action?     | Action?     |
        | Func<bool>       | Func<Task>? | Action?     |
        | Func<bool>       | Action?     | Func<Task>? |
    */

    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
    /// <returns>The task object representing the asynchronous execution.</returns>
    public static async Task InvokeAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Action? whenOpened, Action? whenClosed)
    {
        if (await controlledBy())
        {
            try
            {
                Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }

            return;
        }

        Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
    }

    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
    /// <returns>The task object representing the asynchronous execution.</returns>
    public static async Task InvokeAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Func<Task>? whenOpened, Action? whenClosed)
    {
        if (controlledBy())
        {
            try
            {
                await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }

            return;
        }

        Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
    }

    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
    /// <returns>The task object representing the asynchronous execution.</returns>
    public static async Task InvokeAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Action? whenOpened, Func<Task>? whenClosed)
    {
        if (controlledBy())
        {
            try
            {
                Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }

            return;
        }

        await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
    }

    /*
        Other permutations of the InvokeAsync() where two of the parameters is an asynchronous delegate.

        | controlledBy     | whenOpened  | whenClosed  |
        |------------------|-------------|-------------|
        | Func<bool>       | Func<Task>? | Func<Task>? |
        | Func<Task<bool>> | Action?     | Func<Task>? |
        | Func<Task<bool>> | Func<Task>? | Action?     |
    */

    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
    /// <returns>The task object representing the asynchronous execution.</returns>
    public static async Task InvokeAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
    {
        if (controlledBy())
        {
            try
            {
                await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }

            return;
        }

        await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
    }

    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
    /// <returns>The task object representing the asynchronous execution.</returns>
    public static async Task InvokeAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Action? whenOpened, Func<Task>? whenClosed)
    {
        if (await controlledBy())
        {
            try
            {
                Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }

            return;
        }

        await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
    }

    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
    /// <returns>The task object representing the asynchronous execution.</returns>
    public static async Task InvokeAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Action? whenClosed)
    {
        if (await controlledBy())
        {
            try
            {
                await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }

            return;
        }

        Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
    }

    /*
        Other permutations of the InvokeAsync<TResult>() where one of the parameters is an asynchronous delegate.

        | controlledBy     | whenOpened          | whenClosed          |
        |------------------|---------------------|---------------------|
        | Func<Task<bool>> | Func<TResult>       | Func<TResult>       |
        | Func<bool>       | Func<Task<TResult>> | Func<TResult>       |
        | Func<bool>       | Func<TResult>       | Func<Task<TResult>> |
    */

    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
    /// <typeparam name="TResult">The type of the result returned at the end of execution.</typeparam>
    /// <returns>The <typeparamref name="TResult" /> value wrapped in a task object representing the asynchronous execution.</returns>
    public static async Task<TResult> InvokeAsync<TResult>(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<TResult> whenOpened, Func<TResult> whenClosed)
    {
        if (await controlledBy())
        {
            try
            {
                return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }
        }

        return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
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
    public static async Task<TResult> InvokeAsync<TResult>(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Func<Task<TResult>> whenOpened, Func<TResult> whenClosed)
    {
        if (controlledBy())
        {
            try
            {
                return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }
        }

        return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
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
    public static async Task<TResult> InvokeAsync<TResult>(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Func<TResult> whenOpened, Func<Task<TResult>> whenClosed)
    {
        if (controlledBy())
        {
            try
            {
                return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }
        }

        return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
    }

    /*
        Other permutations of the InvokeAsync<TResult>() where two of the parameters is an asynchronous delegate.

        | controlledBy     | whenOpened          | whenClosed          |
        |------------------|---------------------|---------------------|
        | Func<bool>       | Func<Task<TResult>> | Func<Task<TResult>> |
        | Func<Task<bool>> | Func<TResult>       | Func<Task<TResult>> |
        | Func<Task<bool>> | Func<Task<TResult>> | Func<TResult>       |
    */

    /// <summary>Invokes the <paramref name="controlledBy" /> predicate and then based on the result will invoke the <paramref name="whenOpened" /> or <paramref name="whenClosed" /> delegate.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <paramref name="whenClosed" /> delegate when an uncaught exception is thrown during execution of the <paramref name="whenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <paramref name="whenOpened" /> or <paramref name="whenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <paramref name="controlledBy" /> evaluates to <c>false</c>.</param>
    /// <typeparam name="TResult">The type of the result returned at the end of execution.</typeparam>
    /// <returns>The <typeparamref name="TResult" /> value wrapped in a task object representing the asynchronous execution.</returns>
    public static async Task<TResult> InvokeAsync<TResult>(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Func<Task<TResult>> whenOpened, Func<Task<TResult>> whenClosed)
    {
        if (controlledBy())
        {
            try
            {
                return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }
        }

        return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
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
    public static async Task<TResult> InvokeAsync<TResult>(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<TResult> whenOpened, Func<Task<TResult>> whenClosed)
    {
        if (await controlledBy())
        {
            try
            {
                return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }
        }

        return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
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
    public static async Task<TResult> InvokeAsync<TResult>(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<Task<TResult>> whenOpened, Func<TResult> whenClosed)
    {
        if (await controlledBy())
        {
            try
            {
                return await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Opened, whenOpened, instrumentType);
            }
            catch
            {
                if (!fallbackOnException)
                {
                    throw;
                }

                return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
            }
        }

        return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Closed, whenClosed, instrumentType);
    }

    /*
        Below here are static convenience methods that immediately execute code and records the execution.
    */

    /// <summary>Immediately executes a delegate and records its execution as an 'opened' feature gate execution.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="action">The delegate to execute and record.</param>
    public static void RecordExecution(string featureGateKey, InstrumentType instrumentType, Action? action)
    {
        Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, action, instrumentType);
    }

    /// <summary>Immediately executes a delegate and records its execution as an 'opened' feature gate execution.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="function">The delegate to execute and record.</param>
    /// <typeparam name="TResult">The type of the result returned at the end of execution.</typeparam>
    /// <returns>The <typeparamref name="TResult" /> value returned from the end of execution.</returns>
    public static TResult RecordExecution<TResult>(string featureGateKey, InstrumentType instrumentType, Func<TResult> function)
    {
        return Instrumentation.RecordExecution(featureGateKey, FeatureGateState.Opened, function, instrumentType);
    }

    /// <summary>Immediately executes a delegate and records its execution as an 'opened' feature gate execution.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="function">The delegate to execute and record.</param>
    /// <returns>The task object representing the asynchronous execution.</returns>
    public static async Task RecordExecutionAsync(string featureGateKey, InstrumentType instrumentType, Func<Task> function)
    {
        await Instrumentation.RecordExecutionAsync(featureGateKey, FeatureGateState.Opened, function, instrumentType);
    }

    /// <summary>Immediately executes a delegate and records its execution as an 'opened' feature gate execution.</summary>
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
