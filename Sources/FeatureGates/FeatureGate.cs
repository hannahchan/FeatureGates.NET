namespace FeatureGates;

using System;
using FeatureGates.Builder;

/// <summary>Represents an instrumented feature gate.</summary>
public class FeatureGate : AbstractFeatureGate
{
    /// <summary>Initializes a new instance of the <see cref="FeatureGate" /> class.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <see cref="WhenOpened" /> or <see cref="WhenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <see cref="ControlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <see cref="ControlledBy" /> evaluates to <c>false</c>.</param>
    public FeatureGate(string featureGateKey, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
        : this(featureGateKey, InstrumentType.Counter, false, controlledBy, whenOpened, whenClosed)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="FeatureGate" /> class.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <param name="instrumentType">The instrument type that the feature gate will used to record executions.</param>
    /// <param name="fallbackOnException">Whether to execute the <see cref="WhenClosed" /> delegate when an uncaught exception is thrown during execution of the <see cref="WhenOpened" /> delegate.</param>
    /// <param name="controlledBy">The predicate that controls whether to execute <see cref="WhenOpened" /> or <see cref="WhenClosed" />.</param>
    /// <param name="whenOpened">The delegate to execute when <see cref="ControlledBy" /> evaluates to <c>true</c>.</param>
    /// <param name="whenClosed">The delegate to execute when <see cref="ControlledBy" /> evaluates to <c>false</c>.</param>
    public FeatureGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Action? whenOpened, Action? whenClosed)
        : base(featureGateKey, instrumentType, fallbackOnException)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
        this.WhenClosed = whenClosed;
    }

    /// <summary>Gets the predicate that controls whether to execute <see cref="WhenOpened" /> or <see cref="WhenClosed" />.</summary>
    public Func<bool> ControlledBy { get; }

    /// <summary>Gets the delegate to execute when <see cref="ControlledBy" /> evaluates to <c>true</c>.</summary>
    public Action? WhenOpened { get; }

    /// <summary>Gets the delegate to execute when <see cref="ControlledBy" /> evaluates to <c>false</c>.</summary>
    public Action? WhenClosed { get; }

    /// <summary>A static entry point that can be used to create feature gates. Start by specifying a feature gate key and then by chaining the next available methods.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <returns>A <see cref="FeatureGateBuilder" /> for chaining.</returns>
    public static FeatureGateBuilder WithKey(string featureGateKey)
    {
        return new FeatureGateBuilder(featureGateKey);
    }

    /// <summary>Invokes the feature gate and records the execution.</summary>
    public void Invoke()
    {
        StaticFeatureGate.Invoke(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, this.WhenOpened, this.WhenClosed);
    }
}
