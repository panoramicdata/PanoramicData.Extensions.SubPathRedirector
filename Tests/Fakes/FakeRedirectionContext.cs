using Microsoft.AspNetCore.Http;

namespace PanoramicData.Extensions.SubPathRedirector.Tests.Fakes;

internal class FakeRedirectionContext : IRedirectionContext
{
    public bool CanRedirectRequest => SubPathRedirection is not null;

    public required HttpContext HttpContext { get; set; }

    public HttpRequest Request => HttpContext.Request;

    public HttpResponse Response => HttpContext.Response;

    public required SubPathRedirectorOptions Options { get; set; }

    public required string RequestPath { get; set; }

    public required string LoweredRequestPath { get; set; }

    public string? ResponseUrl { get; set; }

    public SubPathRedirection? SubPathRedirection { get; set; }
}
