using Microsoft.AspNetCore.Http;

namespace PanoramicData.Extensions.SubPathRedirector;

/// <summary>
/// Context for a redirection
/// </summary>
public class RedirectionContext : IRedirectionContext
{
	internal RedirectionContext()
	{
	}

	public required HttpContext HttpContext { get; init; }

	public HttpRequest Request => HttpContext.Request;

	public HttpResponse Response => HttpContext.Response;

	public required string RequestPath { get; set; }

	public required string ModifiedRequestPath { get; set; }

	public required string ResponseUrl { get; init; }

	public bool CanRedirectRequest => SubPathRedirection is not null;

	public SubPathRedirection? SubPathRedirection { get; init; }

	public required SubPathRedirectorOptions Options { get; init; }
}
