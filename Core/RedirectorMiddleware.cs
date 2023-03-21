using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace PanoramicData.Extensions.SubPathRedirector;

/// <summary>
/// The middleware that performs any configured redirections
/// </summary>
internal class RedirectorMiddleware : IMiddleware
{
    private readonly SubPathRedirectorOptions _options;
    private readonly IRedirectionHandler _redirectionHandler;

    public RedirectorMiddleware(IRedirectionHandler handler, IOptions<SubPathRedirectorOptions> options)
    {
        _options = options.Value;
        _redirectionHandler = handler;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var redirectionContext = new RedirectionContext(context, _options);
        if (redirectionContext.CanRedirectRequest)
        {
            _redirectionHandler.RedirectRequest(redirectionContext);
            return Task.CompletedTask;
        }

        return next.Invoke(context);
    }
}
