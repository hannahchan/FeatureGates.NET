namespace FeatureGates.Builder;

using System;
using System.Threading.Tasks;

/// <summary>Represents an incomplete asynchronous feature gate. Complete this gate by specifying a 'WhenOpened' delegate.</summary>
public class BaseGateAsync : AbstractFeatureGate
{
    internal BaseGateAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy)
        : base(featureGateKey, instrumentType, fallbackOnException)
    {
        this.ControlledBy = controlledBy;
    }

    /// <summary>Gets the predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</summary>
    public Func<Task<bool>> ControlledBy { get; }

    /// <summary>Sets the delegate to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
    /// <param name="function">The delegate expressed as an asynchronous action.</param>
    /// <returns>A <see cref="HalfGateAsync" /> for chaining.</returns>
    public HalfGateAsync WhenOpened(Func<Task> function)
    {
        return new HalfGateAsync(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, function);
    }

    /// <summary>Sets the delegate to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
    /// <param name="function">The delegate expressed as an asynchronous function.</param>
    /// <typeparam name="TResult">The type of the result returned by the asynchronous function.</typeparam>
    /// <returns>A <see cref="PartialResultGateAsync{TResult}" /> for chaining.</returns>
    public PartialResultGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
    {
        return new PartialResultGateAsync<TResult>(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, function);
    }

    /// <summary>Sets the delegate to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
    /// <param name="action">The delegate expressed as an action.</param>
    /// <returns>A <see cref="HalfGateAsync" /> for chaining.</returns>
    public HalfGateAsync WhenOpened(Action action)
    {
        return this.WhenOpened(() => Task.Run(action));
    }

    /// <summary>Sets the delegate to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
    /// <param name="function">The delegate expressed as a function.</param>
    /// <typeparam name="TResult">The type of the result returned by the function.</typeparam>
    /// <returns>A <see cref="PartialResultGateAsync{TResult}" /> for chaining.</returns>
    public PartialResultGateAsync<TResult> WhenOpened<TResult>(Func<TResult> function)
    {
        return this.WhenOpened(() => Task.Run(function));
    }
}
