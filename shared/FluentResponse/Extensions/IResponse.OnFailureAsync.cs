using FluentResponse.Interfaces;

namespace FluentResponse;
public static partial class Extensions {

    /// <summary>
    /// Runs a function with the response's exception as argument if it is not successful and propagates the return value.
    /// </summary>
    /// <param name="self">The current response.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<T> OnFailureAsync<T>(
        this T self,
        Func<Exception, Task<T>> onFailure
    ) where T : IResponse =>
        self is IFailure failure
            ? await onFailure(failure.Exception)
            : self;

    /// <summary>
    /// Runs a function with the response's exception as argument if it is not successful.
    /// </summary>
    /// <param name="self">The current response.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<T> OnFailureAsync<T>(
        this T self,
        Func<Exception, Task> onFailure
    ) where T : IResponse {
        if (self is IFailure failure) await onFailure(failure.Exception);
        return self;
    }
}