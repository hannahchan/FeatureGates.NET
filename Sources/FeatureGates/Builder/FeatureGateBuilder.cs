namespace FeatureGates.Builder;

using System;
using System.Threading.Tasks;

/// <summary>Represents a <see cref="FeatureGateBuilder" />. Chain methods from the <see cref="FeatureGateBuilder" /> to start building your feature gate.</summary>
public class FeatureGateBuilder
{
    internal FeatureGateBuilder(string featureGateKey)
    {
        this.Key = featureGateKey;
    }

    /// <summary>Gets the feature gate key. A user-defined identifier for the feature gate.</summary>
    public string Key { get; }

    /// <summary>Gets the instrument type that the feature gate will used to record executions.</summary>
    public InstrumentType InstrumentType { get; private set; } = InstrumentType.Counter;

    /// <summary>Gets a value indicating whether to execute the 'WhenClosed' operation when an uncaught exception is thrown during execution of the 'WhenOpened' operation.</summary>
    public bool FallbackOnException { get; private set; }

    /// <summary>Configures the feature gate to use a histogram to record executions.</summary>
    /// <returns>The <see cref="FeatureGateBuilder" /> for chaining.</returns>
    public FeatureGateBuilder WithHistogram()
    {
        this.InstrumentType = InstrumentType.Histogram;
        return this;
    }

    /// <summary>Configures the feature gate to execute its 'WhenClosed' operation when an uncaught exception is thrown during execution of its 'WhenOpened' operation.</summary>
    /// <returns>The <see cref="FeatureGateBuilder" /> for chaining.</returns>
    public FeatureGateBuilder WithFallbackOnException()
    {
        this.FallbackOnException = true;
        return this;
    }

    /// <summary>Sets the predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</summary>
    /// <param name="predicate">The predicate expressed as a function.</param>
    /// <returns>A <see cref="BaseGate" /> for chaining.</returns>
    public BaseGate ControlledBy(Func<bool> predicate)
    {
        return new BaseGate(this.Key, this.InstrumentType, this.FallbackOnException, predicate);
    }

    /// <summary>Sets the predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</summary>
    /// <param name="predicate">The predicate expressed as an asynchronous function.</param>
    /// <returns>A <see cref="BaseGateAsync" /> for chaining.</returns>
    public BaseGateAsync ControlledBy(Func<Task<bool>> predicate)
    {
        return new BaseGateAsync(this.Key, this.InstrumentType, this.FallbackOnException, predicate);
    }
}
