using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace PanoramicData.Extensions.SubPathRedirector;

/// <summary>
/// Context for a redirection
/// </summary>
internal class RedirectionContext : IRedirectionContext
{
	public RedirectionContext(HttpContext context, SubPathRedirectorOptions options)
	{
		HttpContext = context;
		Options = options;
		RequestPath = GetRequestPath(context);
		LoweredRequestPath = RequestPath.ToLowerInvariant();
		SubPathRedirection = GetRedirection(LoweredRequestPath, options);
		if (SubPathRedirection is not null)
		{
			ResponseUrl = ResponseUrlCalculator.Calculate(RequestPath, SubPathRedirection);
		}
	}

	public HttpContext HttpContext { get; }

	public HttpRequest Request => HttpContext.Request;

	public HttpResponse Response => HttpContext.Response;

	public string RequestPath { get; set; }

	public string LoweredRequestPath { get; set; }

	public string? ResponseUrl { get; }

	public bool CanRedirectRequest => SubPathRedirection is not null;

	public SubPathRedirection? SubPathRedirection { get; }

	public SubPathRedirectorOptions Options { get; }

	private static string GetRequestPath(HttpContext context)
	{
		var requestUrl = context.Request.GetEncodedUrl();
		var requestUri = new Uri(requestUrl);
		return requestUri.AbsolutePath;
	}

	private static SubPathRedirection? GetRedirection(string loweredRequestPath, SubPathRedirectorOptions options)
		=> options.GetRedirection(loweredRequestPath);
}
