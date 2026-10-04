using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using PanoramicData.Extensions.SubPathRedirector.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.ContextFactories;

/// <summary>
/// Redirection context factory that uses case-insensitive matching
/// using the absolute subpath of the incoming request
/// </summary>
public class CaseInsensitiveAbsolutePathContextFactory : IRedirectionContextFactory
{
	/// <summary>
	/// Create a redirection context that determines its redirection responsibility
	/// based upon a case-insensitive matching of the incoming request on its absolute path
	/// </summary>
	/// <param name="httpContext">The HttpContext of the incoming request</param>
	/// <param name="configuration">The configuration of the subsystem on which to base the behaviour</param>
	/// <returns>A RedirectionContext suitably configured for the current request</returns>
	public RedirectionContext CreateRedirectionContext(HttpContext httpContext, SubPathRedirectorConfiguration configuration)
	{
		var requestUrl = httpContext.Request.GetEncodedUrl();
		var requestUri = new Uri(requestUrl);
		var requestPath = requestUri.AbsolutePath;
		var modifiedRequestPath = requestPath.ToLowerInvariant();

		var responseUrl = string.Empty;
		var redirection = GetRedirection(modifiedRequestPath, configuration);
		if (redirection is not null)
		{
			responseUrl = CaseInsensitiveResponseUrlCalculator.Calculate(requestPath, redirection);
		}

		return new RedirectionContext
		{
			HttpContext = httpContext,
			RequestPath = requestPath,
			ModifiedRequestPath = modifiedRequestPath,
			SubPathRedirection = redirection,
			ResponseUrl = responseUrl,
		};
	}

	/// <summary>
	/// Get the redirection associated with the requested path, or null if no redirections exists
	/// </summary>
	/// <param name="loweredRequestPath">The lowercase path of the request whose redirection we are trying to find</param>
	/// <param name="configuration">The configuration containing the redirections to search</param>
	/// <returns>The redirection configuration that is to be used in interception, or null if no interception is appropriate</returns>
	private static SubPathRedirection? GetRedirection(string loweredRequestPath, SubPathRedirectorConfiguration configuration) =>
		// Do very fast lookups of redirections in a case-insensitive manner by using lowercase Url
		configuration.SubPathRedirections.FirstOrDefault(m => loweredRequestPath.StartsWith(m.RequestUrlPrefix, StringComparison.Ordinal));
}
