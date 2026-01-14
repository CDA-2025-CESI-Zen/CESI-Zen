using FluentResponse.Interfaces;

namespace FluentResponse;
public static partial class Extensions {

    /// <summary>
    /// Runs a function if it is successful and propagates the return value.
    /// </summary>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called when the response is successful and returns a response of the same type.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<IResponse> OnSuccessAsync(
        this Task<IResponse> task,
        Func<IResponse> onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <summary>
    /// Runs a function if it is successful.
    /// </summary>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called when the response is successful.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<IResponse> OnSuccessAsync(
        this Task<IResponse> task,
        Action onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <summary>
    /// Runs a function if it is successful and propagates the return value.
    /// </summary>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called when the response is successful and returns a response of the same type.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<IResponse> OnSuccessAsync(
        this Task<IResponse> task,
        Func<Task<IResponse>> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);

    /// <summary>
    /// Runs a function if it is successful.
    /// </summary>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called when the response is successful.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<IResponse> OnSuccessAsync(
        this Task<IResponse> task,
        Func<Task> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);

    /// <summary>
    /// Runs a function if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="task">The current async response.</param>
    /// <param name="onSuccess">A function called when the response is successful and returns a response of the same type.</param>
    /// <returns>A response for method chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this Task<IResponse>          task,
        Func<Task<IResponse<TValue>>> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);
}