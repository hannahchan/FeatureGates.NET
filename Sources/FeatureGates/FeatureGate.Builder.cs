namespace FeatureGates;

using System;
using System.Threading.Tasks;

/// <content>Contains the <see cref="Builder" /> and related classes.</content>
public partial class FeatureGate
{
    /// <summary>A static entry point that can be used to create feature gates. Start by specifying a feature gate key and then by chaining methods.</summary>
    /// <param name="featureGateKey">The user-defined identifier for the feature gate.</param>
    /// <returns>A <see cref="Builder" /> for chaining.</returns>
    public static Builder WithKey(string featureGateKey)
    {
        return new Builder(featureGateKey);
    }

    /// <summary>Represents a <see cref="Builder" />. Chain methods from the <see cref="Builder" /> to start building your feature gate.</summary>
    public class Builder
    {
        internal Builder(string featureGateKey)
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
        /// <returns>The <see cref="Builder" /> for chaining.</returns>
        public Builder WithHistogram()
        {
            this.InstrumentType = InstrumentType.Histogram;
            return this;
        }

        /// <summary>Configures the feature gate to execute its 'WhenClosed' operation when an uncaught exception is thrown during execution of its 'WhenOpened' operation.</summary>
        /// <returns>The <see cref="Builder" /> for chaining.</returns>
        public Builder WithFallbackOnException()
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

    /// <summary>Represents an incomplete asynchronous feature gate. Complete this gate by specifying a 'WhenOpened' operation.</summary>
    public class BaseGateAsync : AbstractFeatureGate
    {
        internal BaseGateAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy)
            : base(featureGateKey, instrumentType, fallbackOnException)
        {
            this.ControlledBy = controlledBy;
        }

        /// <summary>Gets the predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</summary>
        public Func<Task<bool>> ControlledBy { get; }

        /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
        /// <param name="function">The operation expressed as an asynchronous action.</param>
        /// <returns>A <see cref="HalfGateAsync" /> for chaining.</returns>
        public HalfGateAsync WhenOpened(Func<Task> function)
        {
            return new HalfGateAsync(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, function);
        }

        /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
        /// <param name="function">The operation expressed as an asynchronous function.</param>
        /// <typeparam name="TResult">The type of the result returned by the asynchronous function.</typeparam>
        /// <returns>A <see cref="PartialResultGateAsync{TResult}" /> for chaining.</returns>
        public PartialResultGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
        {
            return new PartialResultGateAsync<TResult>(this.Key, this.InstrumentType, this.FallbackOnException, this.ControlledBy, function);
        }

        /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
        /// <param name="action">The operation expressed as an action.</param>
        /// <returns>A <see cref="HalfGateAsync" /> for chaining.</returns>
        public HalfGateAsync WhenOpened(Action action)
        {
            return this.WhenOpened(() => Task.Run(action));
        }

        /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
        /// <param name="function">The operation expressed as a function.</param>
        /// <typeparam name="TResult">The type of the result returned by the function.</typeparam>
        /// <returns>A <see cref="PartialResultGateAsync{TResult}" /> for chaining.</returns>
        public PartialResultGateAsync<TResult> WhenOpened<TResult>(Func<TResult> function)
        {
            return this.WhenOpened(() => Task.Run(function));
        }
    }

    /// <summary>Represents a half gate, an invocable feature gate with no 'WhenClosed' operation. Create a full gate by specifying a 'WhenClosed' operation.</summary>
    public class HalfGate : FeatureGate
    {
        internal HalfGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Action? whenOpened)
            : base(featureGateKey, instrumentType, fallbackOnException, controlledBy, whenOpened, null)
        {
        }

        /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>false</c>.</summary>
        /// <param name="action">The operation expressed as an action.</param>
        /// <returns>A completed <see cref="FeatureGate" />.</returns>
        public new FeatureGate WhenClosed(Action? action)
        {
            return new FeatureGate(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                fallbackOnException: this.FallbackOnException,
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: action);
        }

        /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>false</c>.</summary>
        /// <param name="function">The operation expressed as an asynchronous action.</param>
        /// <returns>A completed <see cref="FeatureGateAsync" />.</returns>
        public new FeatureGateAsync WhenClosed(Func<Task>? function)
        {
            return new FeatureGateAsync(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                fallbackOnException: this.FallbackOnException,
                controlledBy: () => Task.Run(this.ControlledBy),
                whenOpened: this.WhenOpened == null ? null : () => Task.Run(this.WhenOpened),
                whenClosed: function);
        }
    }

    /// <summary>Represents an asynchronous half gate, an invocable feature gate with no 'WhenClosed' operation. Create a full gate by specifying a 'WhenClosed' operation.</summary>
    public class HalfGateAsync : FeatureGateAsync
    {
        internal HalfGateAsync(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<Task<bool>> controlledBy, Func<Task>? whenOpened)
            : base(featureGateKey, instrumentType, fallbackOnException, controlledBy, whenOpened, null)
        {
        }

        /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>false</c>.</summary>
        /// <param name="function">The operation expressed as an asynchronous action.</param>
        /// <returns>A completed <see cref="FeatureGateAsync" />.</returns>
        public new FeatureGateAsync WhenClosed(Func<Task>? function)
        {
            return new FeatureGateAsync(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                fallbackOnException: this.FallbackOnException,
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: function);
        }

        /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>false</c>.</summary>
        /// <param name="action">The operation expressed as an action.</param>
        /// <returns>A completed <see cref="FeatureGateAsync" />.</returns>
        public new FeatureGateAsync WhenClosed(Action? action)
        {
            return new FeatureGateAsync(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                fallbackOnException: this.FallbackOnException,
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: action == null ? null : () => Task.Run(action));
        }
    }

    /// <summary>Represents an incomplete result gate, a feature gate that returns a result. Complete this gate by specifying a 'WhenClosed' operation.</summary>
    /// <typeparam name="TResult">The type of the result returned by the feature gate.</typeparam>
    public class PartialResultGate<TResult> : AbstractFeatureGate
    {
        internal PartialResultGate(string featureGateKey, InstrumentType instrumentType, bool fallbackOnException, Func<bool> controlledBy, Func<TResult> whenOpened)
            : base(featureGateKey, instrumentType, fallbackOnException)
        {
            this.ControlledBy = controlledBy;
            this.WhenOpened = whenOpened;
        }

        /// <summary>Gets the predicate that controls whether to execute 'WhenOpened' or 'WhenClosed'.</summary>
        public Func<bool> ControlledBy { get; }

        /// <summary>Gets the operation to execute when 'ControlledBy' evaluates to <c>true</c>.</summary>
        public Func<TResult> WhenOpened { get; }

        /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>false</c>.</summary>
        /// <param name="function">The operation expressed as a function.</param>
        /// <returns>A completed <see cref="FeatureGate{TResult}" />.</returns>
        public FeatureGate<TResult> WhenClosed(Func<TResult> function)
        {
            return new FeatureGate<TResult>(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                fallbackOnException: this.FallbackOnException,
                controlledBy: this.ControlledBy,
                whenOpened: this.WhenOpened,
                whenClosed: function);
        }

        /// <summary>Sets the operation to execute when 'ControlledBy' evaluates to <c>false</c>.</summary>
        /// <param name="function">The operation expressed as an asynchronous function.</param>
        /// <returns>A completed <see cref="FeatureGateAsync{TResult}" />.</returns>
        public FeatureGateAsync<TResult> WhenClosed(Func<Task<TResult>> function)
        {
            return new FeatureGateAsync<TResult>(
                featureGateKey: this.Key,
                instrumentType: this.InstrumentType,
                fallbackOnException: this.FallbackOnException,
                controlledBy: () => Task.Run(this.ControlledBy),
                whenOpened: () => Task.Run(this.WhenOpened),
                whenClosed: function);
        }
    }

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
}
