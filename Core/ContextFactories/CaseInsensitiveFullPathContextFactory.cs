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
	/// <param name="configuration">The configuration of the subsystem on which to base the behaviour</param>
	/// <returns>A RedirectionContext suitably configured for the current request</returns>
	public RedirectionContext CreateRedirectionContext(HttpContext httpContext, SubPathRedirectorConfiguration configuration)
	{
		var request = httpContext.Request;
		var requestPath = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path);
		return CaseInsensitiveRedirectionContextFactory.Create(httpContext, configuration, requestPath);
	}
}