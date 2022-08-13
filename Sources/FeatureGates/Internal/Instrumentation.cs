namespace FeatureGates.Internal;

using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

internal static class Instrumentation
{
    private static readonly Counter<int> ExecutionCounter = Library.Meter.CreateCounter<int>(
        name: "feature.gate.executions",
        unit: null,
        description: "measures the number of times a feature gate has been executed");

    private static readonly Histogram<double> ExecutionDurationHistogram = Library.Meter.CreateHistogram<double>(
        name: "feature.gate.duration",
        unit: "ms", // milliseconds
        description: "measures the duration of feature gate executions");

    public static void RecordExecution(string featureGateKey, FeatureGateState featureGateState, Action? action, InstrumentType instrumentType)
    {
        bool featureGateException = false;
        using Activity? activity = StartActivity(featureGateKey, featureGateState);
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();

        try
        {
            if (action == null)
            {
                return;
            }

            action();
        }
        catch (Exception exception)
        {
            featureGateException = true;
            activity.RecordException(exception);
            throw;
        }
        finally
        {
            RecordActivityStatus(activity, featureGateException);
            RecordMeasurement(instrumentType, stopwatch.GetElapsedTime(), CreateTags(featureGateKey, featureGateState, featureGateException));
        }
    }

    public static TResult RecordExecution<TResult>(string featureGateKey, FeatureGateState featureGateState, Func<TResult> function, InstrumentType instrumentType)
    {
        bool featureGateException = false;
        using Activity? activity = StartActivity(featureGateKey, featureGateState);
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();

        try
        {
            return function();
        }
        catch (Exception exception)
        {
            featureGateException = true;
            activity.RecordException(exception);
            throw;
        }
        finally
        {
            RecordActivityStatus(activity, featureGateException);
            RecordMeasurement(instrumentType, stopwatch.GetElapsedTime(), CreateTags(featureGateKey, featureGateState, featureGateException));
        }
    }

    public static async Task RecordExecutionAsync(string featureGateKey, FeatureGateState featureGateState, Func<Task>? function, InstrumentType instrumentType)
    {
        bool featureGateException = false;
        using Activity? activity = StartActivity(featureGateKey, featureGateState);
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();

        try
        {
            if (function == null)
            {
                return;
            }

            await function();
        }
        catch (Exception exception)
        {
            featureGateException = true;
            activity.RecordException(exception);
            throw;
        }
        finally
        {
            RecordActivityStatus(activity, featureGateException);
            RecordMeasurement(instrumentType, stopwatch.GetElapsedTime(), CreateTags(featureGateKey, featureGateState, featureGateException));
        }
    }

    public static async Task<TResult> RecordExecutionAsync<TResult>(string featureGateKey, FeatureGateState featureGateState, Func<Task<TResult>> function, InstrumentType instrumentType)
    {
        bool featureGateException = false;
        using Activity? activity = StartActivity(featureGateKey, featureGateState);
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();

        try
        {
            return await function();
        }
        catch (Exception exception)
        {
            featureGateException = true;
            activity.RecordException(exception);
            throw;
        }
        finally
        {
            RecordActivityStatus(activity, featureGateException);
            RecordMeasurement(instrumentType, stopwatch.GetElapsedTime(), CreateTags(featureGateKey, featureGateState, featureGateException));
        }
    }

    private static Activity? StartActivity(string featureGateKey, FeatureGateState featureGateState)
    {
        return Library.ActivitySource.StartActivity("feature.gate.execution", ActivityKind.Internal)
            ?.SetTag(SemanticConventions.AttributeFeatureGateKey, featureGateKey)
            .SetTag(SemanticConventions.AttributeFeatureGateState, featureGateState == FeatureGateState.Opened ? "opened" : "closed");
    }

    private static void RecordActivityStatus(Activity? activity, bool featureGateException)
    {
        if (featureGateException)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "An uncaught exception occurred during feature gate execution.");
            return;
        }

        activity?.SetStatus(ActivityStatusCode.Ok);
    }

    private static TagList CreateTags(string featureGateKey, FeatureGateState featureGateState, bool featureGateException)
    {
        return new TagList
        {
            { SemanticConventions.AttributeFeatureGateKey, featureGateKey },
            { SemanticConventions.AttributeFeatureGateState, featureGateState == FeatureGateState.Opened ? "opened" : "closed" },
            { SemanticConventions.AttributeFeatureGateException, featureGateException ? "true" : "false" },
        };
    }

    private static void RecordMeasurement(InstrumentType instrumentType, TimeSpan elapsed, TagList tags)
    {
        switch (instrumentType)
        {
            case InstrumentType.Counter:
                ExecutionCounter.Add(1, tags);
                break;

            case InstrumentType.Histogram:
                ExecutionDurationHistogram.Record(elapsed.TotalMilliseconds, tags);
                break;

            default:
                break;
        }
    }
}
