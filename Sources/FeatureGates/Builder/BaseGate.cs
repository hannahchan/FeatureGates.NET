namespace FeatureGates.Builder;

using System;
using System.Threading.Tasks;

/// <summary>Represents an incomplete synchronous or asynchronous feature gate. Complete this gate by specifying a 'WhenOpened' operation.</summary>
public class BaseGate : AbstractFeatureGate
{
    internal BaseGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy)
        : base(featureGateKey, instrumentType, fallbackOnException)
    {
        this.ControlledBy = controlledBy;
    }

    /// <summary>Gets the predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</summary>
    public Func<bool> ControlledBy { get; }

    /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
    /// <param name="action">The operation expressed as an action.</param>
    /// <returns>A <see cref="HalfGate" /> for chaining.</returns>
    public HalfGate WhenOpened(Action? action)
    {
        return new HalfGate(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, action);
    }

    /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
    /// <param name="function">The operation expressed as a function.</param>
    /// <typeparam name="TResult">The type of the result returned by the function.</typeparam>
    /// <returns>A <see cref="PartialResultGate{TResult}" /> for chaining.</returns>
    public PartialResultGate<TResult> WhenOpened<TResult>(Func<TResult> function)
    {
        return new PartialResultGate<TResult>(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, function);
    }

    /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
    /// <param name="function">The operation expressed as an asynchronous action.</param>
    /// <returns>A <see cref="HalfGateAsync" /> for chaining.</returns>
    public HalfGateAsync WhenOpened(Func<Task> function)
    {
        return new HalfGateAsync(this.Key, this.InstrumentType, this.FallbackOnException, () => Task.Run(this.ControlledBy), function);
    }

    /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
    /// <param name="function">The operation expressed as an asynchronous function.</param>
    /// <typeparam name="TResult">The type of the result returned by the asynchronous function.</typeparam>
    /// <returns>A <see cref="PartialResultGateAsync{TResult}" /> for chaining.</returns>
    public PartialResultGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
    {
        return new PartialResultGateAsync<TResult>(this.Key, this.InstrumentType, this.FallbackOnException, () => Task.Run(this.ControlledBy), function);
    }
}
