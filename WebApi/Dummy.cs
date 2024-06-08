using System.Net;

namespace WebApi;

public static class Dummy
{
    public static readonly IReadOnlyList<HttpStatusCode> StatusCodes =
    [
        HttpStatusCode.BadRequest,  // Polly won't retry for this.
        HttpStatusCode.NotFound,    // Polly won't retry for this.
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.OK,
        HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.OK,
    ];

    public static HttpStatusCode GetRandomStatusCode() => StatusCodes[Random.Shared.Next(StatusCodes.Count)];
}
