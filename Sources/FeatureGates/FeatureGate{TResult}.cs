namespace FeatureGates;

using System;

/// <summary>Represents a feature gate that returns a result.</summary>
/// <typeparam name="TResult">The type of the result returned by the feature gate.</typeparam>
public class FeatureGate<TResult> : AbstractFeatureGate
{
    /// <summary>Initializes a new instance of the <see cref="FeatureGate{Result}"/> class.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</param>
    /// <param name="whenOpened">The operation to execute when 'ControlledBy' evaluates to 'true'.</param>
    /// <param name="whenClosed">The operation to execute when 'ControlledBy' evaluates to 'false'.</param>
    public FeatureGate(string featureGateKey, Func<bool> controlledBy, Func<TResult> whenOpened, Func<TResult> whenClosed)
        : this(featureGateKey, InstrumentType.Counter, false, controlledBy, whenOpened, whenClosed)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="FeatureGate{TResult}"/> class.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the 'WhenClosed' operation when an uncaught exception is thrown during execution of the 'WhenOpened' operation.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</param>
    /// <param name="whenOpened">The operation to execute when 'ControlledBy' evaluates to 'true'.</param>
    /// <param name="whenClosed">The operation to execute when 'ControlledBy' evaluates to 'false'.</param>
    public FeatureGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Func<TResult> whenOpened, Func<TResult> whenClosed)
        : base(featureGateKey, instrumentType, fallbackOnException)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    /// <summary>Gets the predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</summary>
    public Func<bool> ControlledBy { get; }

    /// <summary>Gets the operation to execute when 'ControlledBy' evaluates to 'true'.</summary>
    public Func<TResult> WhenOpened { get; }

    /// <summary>Gets the operation to execute when 'ControlledBy' evaluates to 'false'.</summary>
    public Func<TResult> WhenClosed { get; }

    /// <summary>Invokes the feature gate and records the execution.</summary>
    /// <returns>The TResult value returned from the end of execution.</returns>
    public TResult Invoke()
    {
        return InternalFeatureGate.Invoke(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, this.WhenOpened, this.WhenClosed);
    }
}
