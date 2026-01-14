using FluentResponse.Interfaces;

namespace FluentResponse;
public static partial class Extensions {

    /// <summary>
    /// Runs a function with the response's exception as argument if it is not successful and propagates the return value.
    /// </summary>
    /// <param name="task">The current async response.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<T> OnFailureAsync<T>(
        this Task<T> task,
        Func<Exception, T> onFailure
    ) where T : IResponse => (await task).OnFailure(onFailure);
        
    /// <summary>
    /// Runs a function with the response's exception as argument if it is not successful.
    /// </summary>
    /// <param name="task">The current async response.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<T> OnFailureAsync<T>(
        this Task<T> task,
        Action<Exception> onFailure
    ) where T : IResponse => (await task).OnFailure(onFailure);

    /// <summary>
    /// Runs a function with the response's exception as argument if it is not successful and propagates the return value.
    /// </summary>
    /// <param name="task">The current async response.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<T> OnFailureAsync<T>(
        this Task<T> task,
        Func<Exception, Task<T>> onFailure
    ) where T : IResponse => await (await task).OnFailureAsync(onFailure);
        
    /// <summary>
    /// Runs a function with the response's exception as argument if it is not successful.
    /// </summary>
    /// <param name="task">The current async response.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<T> OnFailureAsync<T>(
        this Task<T> task,
        Func<Exception, Task> onFailure
    ) where T : IResponse => await (await task).OnFailureAsync(onFailure);
}