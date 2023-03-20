using Microsoft.AspNetCore.Http;
using PanoramicData.Extensions.SubPathRedirector.Tests.Fakes;

namespace PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

internal class FakeRedirectionContextBuilder
{
    private string _requestPath = "/google/favicon.ico";
    private SubPathRedirectorOptions _options = FakeSubPathRedirectorOptionsFactory.CreateDefaultOptions();
    private int _redirectionIndex = int.MinValue;
    private HttpContext _httpContext = FakeHttpContextBuilder.CreateBuilder().Build();

    private FakeRedirectionContextBuilder() { }

    public static FakeRedirectionContextBuilder CreateBuilder()
        => new FakeRedirectionContextBuilder();

    public FakeRedirectionContextBuilder ForRequestUrl(string requestPath)
    {
        _requestPath = requestPath;
        return this;
    }

    public FakeRedirectionContextBuilder WithOptions(SubPathRedirectorOptions options)
    {
        _options = options;
        return this;
    }

    public FakeRedirectionContextBuilder WithSubPathRedirectionAtIndex(int index)
    {
        _redirectionIndex = index;
        return this;
    }

    public FakeRedirectionContextBuilder WithHttpContext(HttpContext httpContext)
    {
        _httpContext = httpContext;
        return this;
    }

    public FakeRedirectionContext Build()
    {
        SubPathRedirection? redirection = null;

        if (_redirectionIndex >= 0)
        {
            redirection = _options.SubPathRedirections[_redirectionIndex];
        }

        return new()
        {
            HttpContext = _httpContext,
            RequestPath = _requestPath,
            LoweredRequestPath = _requestPath?.ToLowerInvariant()!,
            Options = _options,
            SubPathRedirection = redirection
        };
    }
}