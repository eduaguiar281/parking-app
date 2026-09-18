namespace ParkingApp.Api.Infrastructure;

public sealed record ErrorBody(string Code, string Message);

public static class HttpError
{
    public static IResult Json(int status, string code, string message) =>
        Results.Json(new ErrorBody(code, message), statusCode: status);
}
