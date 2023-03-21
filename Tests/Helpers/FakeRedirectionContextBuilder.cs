using Microsoft.AspNetCore.Http;
using PanoramicData.Extensions.SubPathRedirector.Tests.Fakes;

namespace PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

internal class FakeRedirectionContextBuilder
{
	private readonly Uri _requestUri;
	private SubPathRedirectorOptions _options = FakeSubPathRedirectorOptionsFactory.CreateDefaultOptions();
	private int _redirectionIndex = int.MinValue;
	private HttpContext? _httpContext;

	private FakeRedirectionContextBuilder(string requestUrl)
	{
		_requestUri = new Uri(requestUrl);
	}

	public static FakeRedirectionContextBuilder CreateBuilder(string requestUrl)
		=> new(requestUrl);

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

		_httpContext ??= FakeHttpContextBuilder.CreateBuilder(_requestUri.ToString()).Build();

		if (_redirectionIndex >= 0)
		{
			redirection = _options.SubPathRedirections[_redirectionIndex];
		}

		return new()
		{
			HttpContext = _httpContext,
			RequestPath = _requestUri.AbsolutePath,
			LoweredRequestPath = _requestUri.AbsolutePath.ToLowerInvariant()!,
			Options = _options,
			SubPathRedirection = redirection
		};
	}
}