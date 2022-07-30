namespace FeatureGates;

using System;
using System.Threading.Tasks;

public class FeatureGateBuilder
{
    public FeatureGateBuilder(string featureGateKey)
    {
        this.Key = featureGateKey;
    }

    public string Key { get; }

    public InstrumentType InstrumentType { get; private set; } = InstrumentType.Counter;

    public FeatureGateBuilder WithHistogram()
    {
        this.InstrumentType = InstrumentType.Histogram;
        return this;
    }

    public PartialFeatureGate ControlledBy(Func<bool> predicate)
    {
        return new PartialFeatureGate(this.Key, this.InstrumentType, predicate);
    }

    public PartialFeatureGateAsync ControlledBy(Func<Task<bool>> predicate)
    {
        return new PartialFeatureGateAsync(this.Key, this.InstrumentType, predicate);
    }

    public class PartialFeatureGate
    {
        public PartialFeatureGate(string key, InstrumentType instrumentType, Func<bool> controlledBy)
        {
            this.Key = key;
            this.InstrumentType = instrumentType;
            this.ControlledBy = controlledBy;
        }

        public string Key { get; }

        public InstrumentType InstrumentType { get; }

        public Func<bool> ControlledBy { get; }

        public FeatureGate WhenOpened(Action? action)
        {
            return new FeatureGate(this.Key, this.InstrumentType, this.ControlledBy, action, null);
        }

        public FeatureGate<TResult> WhenOpened<TResult>(Func<TResult> function)
        {
            return new FeatureGate<TResult>(this.Key, this.InstrumentType, this.ControlledBy, function, null);
        }

        public FeatureGateAsync WhenOpened(Func<Task> function)
        {
            return new FeatureGateAsync(this.Key, this.InstrumentType, () => Task.Run(this.ControlledBy), function, null);
        }

        public FeatureGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
        {
            return new FeatureGateAsync<TResult>(this.Key, this.InstrumentType, () => Task.Run(this.ControlledBy), function, null);
        }
    }

    public class PartialFeatureGateAsync
    {
        public PartialFeatureGateAsync(string key, InstrumentType instrumentType, Func<Task<bool>> controlledBy)
        {
            this.Key = key;
            this.InstrumentType = instrumentType;
            this.ControlledBy = controlledBy;
        }

        public string Key { get; }

        public InstrumentType InstrumentType { get; }

        public Func<Task<bool>> ControlledBy { get; }

        public FeatureGateAsync WhenOpened(Func<Task> function)
        {
            return new FeatureGateAsync(this.Key, this.InstrumentType, this.ControlledBy, function, null);
        }

        public FeatureGateAsync<TResult> WhenOpened<TResult>(Func<Task<TResult>> function)
        {
            return new FeatureGateAsync<TResult>(this.Key, this.InstrumentType, this.ControlledBy, function, null);
        }

        public FeatureGateAsync WhenOpened(Action action)
        {
            return this.WhenOpened(() => Task.Run(action));
        }

        public FeatureGateAsync<TResult> WhenOpened<TResult>(Func<TResult> function)
        {
            return this.WhenOpened(() => Task.Run(function));
        }
    }
}
