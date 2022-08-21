namespace FeatureGates;

using System;
using System.Threading.Tasks;

/// <summary>Represents an asynchronous feature gate.</summary>
public class FeatureGateAsync : AbstractFeatureGate
{
    /// <summary>Initializes a new instance of the <see cref="FeatureGateAsync"/> class.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</param>
    /// <param name="whenOpened">The operation to execute when 'ControlledBy' evaluates to 'true'.</param>
    /// <param name="whenClosed">The operation to execute when 'ControlledBy' evaluates to 'false'.</param>
    public FeatureGateAsync(string featureGateKey, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
        : this(featureGateKey, InstrumentType.Counter, false, controlledBy, whenOpened, whenClosed)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="FeatureGateAsync"/> class.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the 'WhenClosed' operation when an uncaught exception is thrown during execution of the 'WhenOpened' operation.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</param>
    /// <param name="whenOpened">The operation to execute when 'ControlledBy' evaluates to 'true'.</param>
    /// <param name="whenClosed">The operation to execute when 'ControlledBy' evaluates to 'false'.</param>
    public FeatureGateAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<Task>? whenOpened, Func<Task>? whenClosed)
        : base(featureGateKey, instrumentType, fallbackOnException)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    /// <summary>Gets the predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</summary>
    public Func<Task<bool>> ControlledBy { get; }

    /// <summary>Gets the operation to execute when 'ControlledBy' evaluates to 'true'.</summary>
    public Func<Task>? WhenOpened { get; }

    /// <summary>Gets the operation to execute when 'ControlledBy' evaluates to 'false'.</summary>
    public Func<Task>? WhenClosed { get; }

    /// <summary>Invokes the feature gate and records the execution.</summary>
    /// <returns>The task object representing the asynchronous execution.</returns>
    public async Task InvokeAsync()
    {
        await InternalFeatureGate.InvokeAsync(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, this.WhenOpened, this.WhenClosed);
    }
}
