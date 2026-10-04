using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

internal class FakeHttpContextBuilder
{
	private Dictionary<string, StringValues> _formValues = new();
	private readonly Uri _requestUri;
	private IHeaderDictionary _headers = new HeaderDictionary();
	private string _method = "GET";
	private string _protocol = "HTTP/1.1";
	private string _pathBase = string.Empty;

	private FakeHttpContextBuilder() : this("https://localhost:5000/") { }

	private FakeHttpContextBuilder(string requestUrl)
	{
		_requestUri = new Uri(requestUrl);
	}

	public FakeHttpContextBuilder WithBaseUrl(string baseUrl)
	{
		_pathBase = baseUrl;
		return this;
	}

	public FakeHttpContextBuilder WithHttpMethod(string httpMethod)
	{
		_method = httpMethod;
		return this;
	}

	public FakeHttpContextBuilder WithHttpProtocol(string httpProtocol)
	{
		_protocol = httpProtocol;
		return this;
	}

	public FakeHttpContextBuilder WithRequestHeaders(IHeaderDictionary headers)
	{
		_headers = headers;
		return this;
	}

	public FakeHttpContextBuilder WithFormValues(Dictionary<string, StringValues> formValues)
	{
		_formValues = formValues;
		return this;
	}

	public static FakeHttpContextBuilder CreateBuilder(string requestUrl)
		=> new(requestUrl);

	public HttpContext Build()
	{
		var features = new FeatureCollection();

		// Form collection
		if (_formValues.Count > 0)
		{
			var formCollection = new FormCollection(_formValues);
			var form = new FormFeature(formCollection);
			features.Set<IFormFeature>(form);
		}

		if (!_headers.ContainsKey("Host"))
		{
			var host = _requestUri.IsDefaultPort
				? _requestUri.Host
				: _requestUri.Host + ":" + _requestUri.Port;
			_headers["Host"] = host;
		}

		// Request
		var request = new HttpRequestFeature
		{
			PathBase = _pathBase,
			Path = _requestUri.AbsolutePath,
			Scheme = _requestUri.Scheme,
			Headers = _headers,
			Method = _method,
			QueryString = _requestUri.Query,
			Protocol = _protocol
		};
		features.Set<IHttpRequestFeature>(request);

		// Response
		var response = new HttpResponseFeature();
		features.Set<IHttpResponseFeature>(response);

		var context = new DefaultHttpContext(features);

		// Response Cookies
		_ = new ResponseCookiesFeature(context.Features);

		return context;
	}
}