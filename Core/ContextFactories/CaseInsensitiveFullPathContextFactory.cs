using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using PanoramicData.Extensions.SubPathRedirector.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.ContextFactories;

/// <summary>
/// Redirection context factory that uses case-insensitive matching
/// using the full path of the incoming request
/// </summary>
public class CaseInsensitiveFullPathContextFactory : IRedirectionContextFactory
{
	/// <summary>
	/// Create a redirection context that determines its redirection responsibility
	/// based upon a case-insensitive matching of the incoming request on its full path
	/// </summary>
	/// <param name="httpContext">The HttpContext of the incoming request</param>
	/// <param name="options">The configuration of the subsystem on which to base the behaviour</param>
	/// <returns>A RedirectionContext suitably configured for the current request</returns>
	public RedirectionContext CreateRedirectionContext(HttpContext httpContext, SubPathRedirectorOptions options)
	{
		var requestPath = GetFullPath(httpContext.Request);
		var modifiedRequestPath = requestPath.ToLowerInvariant();

		var responseUrl = string.Empty;
		var redirection = GetRedirection(modifiedRequestPath, options);
		if (redirection is not null)
		{
			responseUrl = CaseInsensitiveResponseUrlCalculator.Calculate(requestPath, redirection);
		}

		return new RedirectionContext
		{
			HttpContext = httpContext,
			Options = options,
			RequestPath = requestPath,
			ModifiedRequestPath = modifiedRequestPath,
			SubPathRedirection = redirection,
			ResponseUrl = responseUrl,
		};
	}

	/// <summary>
	/// Get the redirection associated with the requested path, or null if no mapping exists
	/// </summary>
	/// <param name="loweredRequestPath">The lowercase path of the request whose mapping we are trying to find</param>
	/// <returns>The redirection configuration that is to be used in interception, or null if no interception is appropriate</returns>
	private static SubPathRedirection? GetRedirection(string loweredRequestPath, SubPathRedirectorOptions options) =>
		// Do very fast lookups of mappings in a case-insensitive manner by using lowercase Url
		options.SubPathRedirections.FirstOrDefault(m => loweredRequestPath.StartsWith(m.RequestUrlPrefix, StringComparison.Ordinal));

	private static string GetFullPath(HttpRequest httpRequest)
		=> UriHelper.BuildAbsolute(httpRequest.Scheme, httpRequest.Host, httpRequest.PathBase, httpRequest.Path);
}
