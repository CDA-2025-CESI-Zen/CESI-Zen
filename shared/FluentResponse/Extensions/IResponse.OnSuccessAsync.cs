using FluentResponse.Interfaces;

namespace FluentResponse;
public static partial class Extensions {

    /// <summary>
    /// Runs a function if it is successful and propagates the return value.
    /// </summary>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called when the response is successful and returns a response of the same type.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<IResponse> OnSuccessAsync(
        this IResponse self,
        Func<Task<IResponse>> onSuccess
    ) => self is ISuccess
            ? await onSuccess()
            : self;

    /// <summary>
    /// Runs a function if it is successful.
    /// </summary>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called when the response is successful.</param>
    /// <returns>A response for method chaining.</returns>
    public static async Task<IResponse> OnSuccessAsync(
        this IResponse self,
        Func<Task> onSuccess
    ) {
        if (self is ISuccess) await onSuccess();
        return self;
    }

    /// <summary>
    /// Runs a function if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called when the response is successful and returns a response of the same type.</param>
    /// <returns>A response for method chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this IResponse self,
        Func<Task<IResponse<TValue>>> onSuccess
    ) => self switch {
            ISuccess         => await onSuccess(),
            IFailure failure => Response.Failure<TValue>(failure.Exception),
            _                => throw new InvalidOperationException()
        };

    /// <summary>
    /// Runs a function if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <returns>A response for method chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static  async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this IResponse self,
        Func<Task<TValue>> onSuccess
    ) => self switch {
            ISuccess         => Response.Success(await onSuccess()),
            IFailure failure => Response.Failure<TValue>(failure.Exception),
            _                => throw new InvalidOperationException()
        };
}