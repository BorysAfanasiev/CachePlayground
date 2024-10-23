using System.Net;

namespace IntegrationTests.Fixtures.Tools;

internal record HttpResponse<T>
{
    public HttpResponse(HttpStatusCode statusCode, T? content)
    {
        StatusCode = statusCode;
        Content = content;
    }

    public HttpStatusCode StatusCode { get; }

    public T? Content { get; }
}
