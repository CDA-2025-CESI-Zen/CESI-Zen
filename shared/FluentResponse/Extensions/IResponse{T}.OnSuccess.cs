using FluentResponse.Interfaces;

namespace FluentResponse;
public static partial class Extensions {

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TFromValue">Base response's value type.</typeparam>
    /// <typeparam name="TOutValue">Propagated response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <returns>A response for method chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IResponse<TOutValue> OnSuccess<TFromValue, TOutValue>(
        this IResponse<TFromValue> self,
        Func<TFromValue, TOutValue> onSuccess
    ) =>
        self switch {
            ISuccess<TFromValue> success => Response.Success(onSuccess(success.Value)),
            IFailure             failure => Response.Failure<TOutValue>(failure.Exception),
            _                            => throw new InvalidOperationException()
        };

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TFromValue">Base response's value type.</typeparam>
    /// <typeparam name="TOutValue">Propagated response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <returns>A response for method chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IResponse<TOutValue> OnSuccess<TFromValue, TOutValue>(
        this IResponse<TFromValue> self,
        Func<TFromValue, IResponse<TOutValue>> onSuccess
    ) =>
        self switch {
            ISuccess<TFromValue> success => onSuccess(success.Value),
            IFailure             failure => Response.Failure<TOutValue>(failure.Exception),
            _                            => throw new InvalidOperationException()
        };

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <returns>A response for method chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IResponse OnSuccess<TValue>(
        this IResponse<TValue> self,
        Func<TValue, IResponse> onSuccess
    ) =>
        self switch {
            ISuccess<TValue> success => onSuccess(success.Value),
            IFailure         failure => Response.Failure(failure.Exception),
            _                        => throw new InvalidOperationException()
        };

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <returns>A response for method chaining.</returns>
    public static IResponse<TValue> OnSuccess<TValue>(
        this IResponse<TValue> self,
        Action<TValue> onSuccess
    ) {
        if (self is ISuccess<TValue> success) onSuccess(success.Value);
        return self;
    }

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <returns>A response for method chaining.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IResponse<TValue> OnSuccess<TValue>(
        this IResponse<TValue> self,
        Func<TValue, TValue> onSuccess
    ) =>
        self switch {
            ISuccess<TValue> success => Response.Success(onSuccess(success.Value)),
            IFailure         failure => Response.Failure<TValue>(failure.Exception),
            _                        => throw new InvalidOperationException()
        };
}