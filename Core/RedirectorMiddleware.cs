using Microsoft.AspNetCore.Http;
using PanoramicData.Extensions.SubPathRedirector.Interfaces;

namespace PanoramicData.Extensions.SubPathRedirector;

/// <summary>
/// The middleware that performs any configured redirections
/// </summary>
internal class RedirectorMiddleware : IMiddleware
{
	private readonly IRedirectorConfigurationProvider _configurationProvider;
	private readonly IRedirectionHandler _redirectionHandler;
	private readonly IRedirectionContextFactory _redirectionContextFactory;

	public RedirectorMiddleware(IRedirectionHandler handler, IRedirectionContextFactory redirectionContextFactory, IRedirectorConfigurationProvider configurationProvider)
	{
		_redirectionHandler = handler;
		_redirectionContextFactory = redirectionContextFactory;
		_configurationProvider = configurationProvider;
	}

	public Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var configuration = _configurationProvider.GetConfiguration();
		var redirectionContext = _redirectionContextFactory.CreateRedirectionContext(context, configuration);
		if (redirectionContext.CanRedirectRequest)
		{
			_redirectionHandler.RedirectRequest(redirectionContext);
			return Task.CompletedTask;
		}

		return next.Invoke(context);
	}
}
