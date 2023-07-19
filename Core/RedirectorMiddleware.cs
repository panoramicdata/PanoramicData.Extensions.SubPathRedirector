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
	private readonly IRedirectionContextFactory _redirectionContextFactory;

	public RedirectorMiddleware(IRedirectionHandler handler, IRedirectionContextFactory redirectionContextFactory, IOptions<SubPathRedirectorOptions> options)
	{
		_options = options.Value;
		_redirectionHandler = handler;
		_redirectionContextFactory = redirectionContextFactory;
	}

	public Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var redirectionContext = _redirectionContextFactory.CreateRedirectionContext(context, _options);
		if (redirectionContext.CanRedirectRequest)
		{
			_redirectionHandler.RedirectRequest(redirectionContext);
			return Task.CompletedTask;
		}

		return next.Invoke(context);
	}
}
