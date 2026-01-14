using FluentResponse.Interfaces;

namespace FluentResponse;
public static partial class Extensions {

    /// <summary>
    /// Throws the response's exception if it is not successful.
    /// </summary>
    public static async Task UnwrapAsync(this Task<IResponse> task) =>
        (await task).Unwrap();

    /// <summary>
    /// Outputs a value according to the response's state.
    /// </summary>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called when the response is successful and returns the output type.</param>
    /// <param name="onFailure">A function called with the response's exception as argument and returns the output type.</param>
    /// <returns>A value based according to the response's state.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<TOut> UnwrapAsync<TOut>(
        this Task<IResponse>  task,
        Func<TOut>            onSuccess,
        Func<Exception, TOut> onFailure
    ) => (await task).Unwrap(onSuccess, onFailure);

    /// <summary>
    /// Outputs a value according to the response's state.
    /// </summary>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called when the response is successful and returns the output type.</param>
    /// <param name="onFailure">A function called with the response's exception as argument and returns the output type.</param>
    /// <returns>A value based according to the response's state.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<TOut> UnwrapAsync<TOut>(
        this Task<IResponse>        task,
        Func<Task<TOut>>            onSuccess,
        Func<Exception, Task<TOut>> onFailure
    ) => await (await task).UnwrapAsync(onSuccess, onFailure);
}