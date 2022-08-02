namespace FeatureGates;

using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Threading.Tasks;
using FeatureGates.Internal;

public abstract class AbstractFeatureGate
{
    private static readonly Counter<int> ExecutionCounter = MeterProvider.Meter.CreateCounter<int>(
        name: "feature.gate.executions",
        unit: null,
        description: "measures the number of times a feature gate has been executed");

    private static readonly Histogram<double> ExecutionDurationHistogram = MeterProvider.Meter.CreateHistogram<double>(
        name: "feature.gate.duration",
        unit: "ms", // milliseconds
        description: "measures the duration of feature gate executions");

    protected AbstractFeatureGate(string featureGateKey, InstrumentType instrumentType)
    {
        this.Key = featureGateKey;
        this.InstrumentType = instrumentType;
    }

    /// <summary>The state of the feature gate.</summary>
    protected enum FeatureGateState
    {
        /// <summary>Represents a closed feature gate.</summary>
        Closed,

        /// <summary>Represents an opened feature gate.</summary>
        Opened,
    }

    public string Key { get; }

    public InstrumentType InstrumentType { get; }

    protected void Invoke(FeatureGateState featureGateState, Action? action)
    {
        bool featureGateException = false;
        using Activity? activity = StartActivity(this.Key, featureGateState);
        Stopwatch stopwatch = Stopwatch.StartNew();

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
            activity?.RecordException(exception);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            this.Record(stopwatch.Elapsed, CreateTags(this.Key, featureGateState, featureGateException));
        }
    }

    protected TResult? Invoke<TResult>(FeatureGateState featureGateState, Func<TResult>? function)
    {
        bool featureGateException = false;
        using Activity? activity = StartActivity(this.Key, featureGateState);
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            return function == null ? default : function();
        }
        catch (Exception exception)
        {
            featureGateException = true;
            activity?.RecordException(exception);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            this.Record(stopwatch.Elapsed, CreateTags(this.Key, featureGateState, featureGateException));
        }
    }

    protected async Task Invoke(FeatureGateState featureGateState, Func<Task>? function)
    {
        bool featureGateException = false;
        using Activity? activity = StartActivity(this.Key, featureGateState);
        Stopwatch stopwatch = Stopwatch.StartNew();

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
            activity?.RecordException(exception);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            this.Record(stopwatch.Elapsed, CreateTags(this.Key, featureGateState, featureGateException));
        }
    }

    protected async Task<TResult?> Invoke<TResult>(FeatureGateState featureGateState, Func<Task<TResult>>? function)
    {
        bool featureGateException = false;
        using Activity? activity = StartActivity(this.Key, featureGateState);
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            return function == null ? default : await function();
        }
        catch (Exception exception)
        {
            featureGateException = true;
            activity?.RecordException(exception);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            this.Record(stopwatch.Elapsed, CreateTags(this.Key, featureGateState, featureGateException));
        }
    }

    private static Activity? StartActivity(string featureGateKey, FeatureGateState featureGateState)
    {
        return ActivityProvider.StartActivity("FeatureGate")
            ?.AddTag(TraceConventions.AttributeFeatureGateKey, featureGateKey)
            .AddTag(TraceConventions.AttributeFeatureGateState, featureGateState);
    }

    private static TagList CreateTags(string featureGateKey, FeatureGateState featureGateState, bool featureGateException)
    {
        return new TagList
        {
            { MetricConventions.AttributeFeatureGateKey, featureGateKey },
            { MetricConventions.AttributeFeatureGateState, featureGateState.ToString().ToLower(CultureInfo.InvariantCulture) },
            { MetricConventions.AttributeFeatureGateException, featureGateException.ToString().ToLower(CultureInfo.InvariantCulture) },
        };
    }

    private void Record(TimeSpan elapsed, TagList tags)
    {
        switch (this.InstrumentType)
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
