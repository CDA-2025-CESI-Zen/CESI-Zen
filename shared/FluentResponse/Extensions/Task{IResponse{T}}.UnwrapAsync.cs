using FluentResponse.Interfaces;

namespace FluentResponse;
public static partial class Extensions {

    /// <summary>
    /// Outputs the response's value if it is successful; otherwise throws the response's exception.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <returns>The reponse's value if successful.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<TValue> UnwrapAsync<TValue>(this Task<IResponse<TValue>> task) =>
        (await task).Unwrap();

    /// <summary>
    /// Outputs a value according to the response's state.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    /// <returns>A value based according to the response's state.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<TOut> UnwrapAsync<TValue, TOut>(
        this Task<IResponse<TValue>> task,
        Func<TValue, TOut>           onSuccess,
        Func<Exception, TOut>   onFailure
    ) => (await task).Unwrap(onSuccess, onFailure);

    /// <summary>
    /// Outputs a value according to the response's state.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    /// <returns>A value based according to the response's state.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<TOut> UnwrapAsync<TValue, TOut>(
        this Task<IResponse<TValue>>     task,
        Func<TValue, Task<TOut>>         onSuccess,
        Func<Exception, Task<TOut>> onFailure
    ) => await (await task).UnwrapAsync(onSuccess, onFailure);
}