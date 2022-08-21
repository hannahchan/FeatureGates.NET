namespace FeatureGates;

using System;

/// <summary>Represents a feature gate.</summary>
public partial class FeatureGate : AbstractFeatureGate
{
    /// <summary>Initializes a new instance of the <see cref="FeatureGate"/> class.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</param>
    /// <param name="whenOpened">The operation to execute when 'ControlledBy' evaluates to 'true'.</param>
    /// <param name="whenClosed">The operation to execute when 'ControlledBy' evaluates to 'false'.</param>
    public FeatureGate(string featureGateKey, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
        : this(featureGateKey, InstrumentType.Counter, false, controlledBy, whenOpened, whenClosed)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="FeatureGate"/> class.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the 'WhenClosed' operation when an uncaught exception is thrown during execution of the 'WhenOpened' operation.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</param>
    /// <param name="whenOpened">The operation to execute when 'ControlledBy' evaluates to 'true'.</param>
    /// <param name="whenClosed">The operation to execute when 'ControlledBy' evaluates to 'false'.</param>
    public FeatureGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
        : base(featureGateKey, instrumentType, fallbackOnException)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    /// <summary>Gets the predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</summary>
    public Func<bool> ControlledBy { get; }

    /// <summary>Gets the operation to execute when 'ControlledBy' evaluates to 'true'.</summary>
    public Action? WhenOpened { get; }

    /// <summary>Gets the operation to execute when 'ControlledBy' evaluates to 'false'.</summary>
    public Action? WhenClosed { get; }

    /// <summary>Invokes the feature gate and records the execution.</summary>
    public void Invoke()
    {
        InternalFeatureGate.Invoke(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, this.WhenOpened, this.WhenClosed);
    }
}
