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

	/// <summary>
	/// The HttpContext of the current request
	/// </summary>
	public required HttpContext HttpContext { get; init; }

	/// <summary>
	/// The current request
	/// </summary>
	public HttpRequest Request => HttpContext.Request;

	/// <summary>
	/// The response associated with the current request
	/// </summary>
	public HttpResponse Response => HttpContext.Response;

	/// <summary>
	/// The path of the current request
	/// </summary>
	public required string RequestPath { get; set; }

	/// <summary>
	/// The request path as normalised for matching, for example lowercased
	/// </summary>
	public required string ModifiedRequestPath { get; set; }

	/// <summary>
	/// The Url to which to redirect, or an empty string if no redirection matches
	/// </summary>
	public required string ResponseUrl { get; init; }

	/// <summary>
	/// Whether the current request matches a configured redirection
	/// </summary>
	public bool CanRedirectRequest => SubPathRedirection is not null;

	/// <summary>
	/// The redirection matching the current request, or null if there is none
	/// </summary>
	public SubPathRedirection? SubPathRedirection { get; init; }
}
