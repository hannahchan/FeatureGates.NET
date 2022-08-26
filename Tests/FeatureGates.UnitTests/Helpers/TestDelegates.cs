namespace FeatureGates.UnitTests.Helpers;

using System;
using System.Threading.Tasks;

public static class TestDelegates
{
    public static bool BooleanFunction()
    {
        return new Random().NextDouble() >= 0.5;
    }

    public static Task<bool> BooleanFunctionAsync()
    {
        return Task.FromResult(new Random().NextDouble() >= 0.5);
    }

    public static void Action()
    {
        // Do nothing
    }

    public static TResult Function<TResult>()
    {
        return default;
    }

    public static Task ActionAsync()
    {
        return Task.CompletedTask;
    }

    public static Task<TResult> FunctionAsync<TResult>()
    {
        return Task.FromResult<TResult>(default);
    }
}
