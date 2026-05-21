using Microsoft.AspNetCore.Http;
using SharedKernel.Common;

namespace Auth.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemResult(this Result result) =>
        result.ErrorType switch
        {
            ErrorType.NotFound     => Results.NotFound(result.Error),
            ErrorType.Validation   => Results.BadRequest(result.Error),
            ErrorType.Conflict     => Results.Conflict(result.Error),
            ErrorType.Unauthorized => Results.Unauthorized(),
            _                      => Results.Problem(result.Error)
        };

    public static IResult ToProblemResult<T>(this Result<T> result) =>
        result.ErrorType switch
        {
            ErrorType.NotFound     => Results.NotFound(result.Error),
            ErrorType.Validation   => Results.BadRequest(result.Error),
            ErrorType.Conflict     => Results.Conflict(result.Error),
            ErrorType.Unauthorized => Results.Unauthorized(),
            _                      => Results.Problem(result.Error)
        };
}
