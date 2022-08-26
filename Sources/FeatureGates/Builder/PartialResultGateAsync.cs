namespace FeatureGates.Builder;

using System;
using System.Threading.Tasks;

/// <summary>Represents an incomplete asynchronous result gate, a feature gate that returns a result. Complete this gate by specifying a 'WhenClosed' operation.</summary>
/// <typeparam name="TResult">The type of the result returned by the feature gate.</typeparam>
public class PartialResultGateAsync<TResult> : AbstractFeatureGate
{
    internal PartialResultGateAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<Task<TResult>> whenOpened)
        : base(featureGateKey, instrumentType, fallbackOnException)
    {
        this.ControlledBy = controlledBy;
        this.WhenOpened = whenOpened;
    }

    /// <summary>Gets the predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</summary>
    public Func<Task<bool>> ControlledBy { get; }

    /// <summary>Gets the operation to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
    public Func<Task<TResult>> WhenOpened { get; }

    /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>false</c>.</summary>
    /// <param name="function">The operation expressed as an asynchronous function.</param>
    /// <returns>A completed <see cref="FeatureGateAsync{TResult}" />.</returns>
    public FeatureGateAsync<TResult> WhenClosed(Func<Task<TResult>> function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            fallbackOnException: this.FallbackOnException,
            controlledBy: this.ControlledBy,
            whenOpened: this.WhenOpened,
            whenClosed: function);
    }

    /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>false</c>.</summary>
    /// <param name="function">The operation expressed as a function.</param>
    /// <returns>A completed <see cref="FeatureGateAsync{TResult}" />.</returns>
    public FeatureGateAsync<TResult> WhenClosed(Func<TResult> function)
    {
        return new FeatureGateAsync<TResult>(
            featureGateKey: this.Key,
            instrumentType: this.InstrumentType,
            fallbackOnException: this.FallbackOnException,
            controlledBy: this.ControlledBy,
            whenOpened: this.WhenOpened,
            whenClosed: () => Task.Run(function));
    }
}
