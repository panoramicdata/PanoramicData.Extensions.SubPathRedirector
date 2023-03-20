using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace PanoramicData.Extensions.SubPathRedirector;

/// <summary>
/// The middleware that performs any configured redirections
/// </summary>
internal class RedirectorMiddleware : IMiddleware
{
	private readonly SubPathRedirectorOptions _options;

	public RedirectorMiddleware(IOptions<SubPathRedirectorOptions> options)
	{
		_options = options.Value;
	}

	public Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var redirectionContext = new RedirectionContext(context, _options);
		if (redirectionContext.CanRedirectRequest)
		{
			RedirectionHandler.RedirectRequest(redirectionContext);
			return Task.CompletedTask;
		}

		return next.Invoke(context);
	}
}
