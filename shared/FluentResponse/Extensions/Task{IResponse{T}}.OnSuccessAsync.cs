using FluentResponse.Interfaces;

namespace FluentResponse;
public static partial class Extensions {

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TFromValue">Base response's value type.</typeparam>
    /// <typeparam name="TOutValue">Propagated response's value type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <returns>A response for method chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<IResponse<TOutValue>> OnSuccessAsync<TFromValue, TOutValue>(
        this Task<IResponse<TFromValue>> task,
        Func<TFromValue, IResponse<TOutValue>> onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <returns>A response for method chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<IResponse> OnSuccessAsync<TValue>(
        this Task<IResponse<TValue>> task,
        Func<TValue, IResponse> onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this Task<IResponse<TValue>> task,
        Action<TValue> onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TFromValue">Base response's value type.</typeparam>
    /// <typeparam name="TOutValue">Propagated response's value type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <returns>A response for method chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<IResponse<TOutValue>> OnSuccessAsync<TFromValue, TOutValue>(
        this Task<IResponse<TFromValue>> task,
        Func<TFromValue, Task<IResponse<TOutValue>>> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <returns>A response for method chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<IResponse> OnSuccessAsync<TValue>(
        this Task<IResponse<TValue>> task,
        Func<TValue, Task<IResponse>> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this Task<IResponse<TValue>> task,
        Func<TValue, Task> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);
}