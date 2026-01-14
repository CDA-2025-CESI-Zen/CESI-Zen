using FluentResponse.Interfaces;

namespace FluentResponse;
public static partial class Extensions {

    /// <summary>
    /// Outputs a value according to the response's state.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    /// <returns>A value based according to the response's state.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<TOut> UnwrapAsync<TValue, TOut>(
        this IResponse<TValue>      self,
        Func<TValue, Task<TOut>>    onSuccess,
        Func<Exception, Task<TOut>> onFailure
    ) =>
        self switch {
            ISuccess<TValue> success => await onSuccess(success.Value),
            IFailure         failure => await onFailure(failure.Exception),
            _                        => throw new InvalidOperationException()
        };
}