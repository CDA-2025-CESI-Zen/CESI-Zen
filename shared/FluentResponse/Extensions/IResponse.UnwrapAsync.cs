using FluentResponse.Interfaces;

namespace FluentResponse;
public static partial class Extensions {

    /// <summary>
    /// Outputs a value according to the response's state.
    /// </summary>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called when the response is successful and returns the output type.</param>
    /// <param name="onFailure">A function called with the response's exception as argument and returns the output type.</param>
    /// <returns>A value based according to the response's state.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<TOut> UnwrapAsync<TOut>(
        this IResponse              self,
        Func<Task<TOut>>            onSuccess,
        Func<Exception, Task<TOut>> onFailure
    ) =>
        self switch {
            ISuccess         => await onSuccess(),
            IFailure failure => await onFailure(failure.Exception),
            _                => throw new InvalidOperationException()
        };
}