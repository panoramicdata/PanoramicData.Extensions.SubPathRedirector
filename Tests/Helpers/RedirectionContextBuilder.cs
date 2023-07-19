using Microsoft.AspNetCore.Http;
using PanoramicData.Extensions.SubPathRedirector.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

internal class RedirectionContextBuilder
{
	private readonly Uri _requestUri;
	private SubPathRedirectorConfiguration _configuration = FakeSubPathRedirectorConfigurationFactory.CreateDefaultAbsolutePathConfiguration();
	private int _redirectionIndex = int.MinValue;
	private HttpContext? _httpContext;

	private RedirectionContextBuilder(string requestUrl)
	{
		_requestUri = new Uri(requestUrl);
	}

	public static RedirectionContextBuilder CreateBuilder(string requestUrl)
		=> new(requestUrl);

	public RedirectionContextBuilder WithConfiguration(SubPathRedirectorConfiguration configuration)
	{
		_configuration = configuration;
		return this;
	}

	public RedirectionContextBuilder WithSubPathRedirectionAtIndex(int index)
	{
		_redirectionIndex = index;
		return this;
	}

	public RedirectionContextBuilder WithHttpContext(HttpContext httpContext)
	{
		_httpContext = httpContext;
		return this;
	}

	public RedirectionContext Build()
	{
		SubPathRedirection? redirection = null;

		_httpContext ??= FakeHttpContextBuilder.CreateBuilder(_requestUri.ToString()).Build();

		var modifiedRequestPath = _requestUri.AbsolutePath.ToLowerInvariant()!;
		var responseUrl = string.Empty;
		if (_redirectionIndex >= 0)
		{
			redirection = _configuration.SubPathRedirections[_redirectionIndex];
			responseUrl = CaseInsensitiveResponseUrlCalculator.Calculate(modifiedRequestPath, redirection);
		}

		return new()
		{
			HttpContext = _httpContext,
			RequestPath = _requestUri.AbsolutePath,
			ModifiedRequestPath = modifiedRequestPath,
			SubPathRedirection = redirection,
			ResponseUrl = responseUrl,
		};
	}
}