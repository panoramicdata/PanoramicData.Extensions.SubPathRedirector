using Microsoft.AspNetCore.Http;

namespace PanoramicData.Extensions.SubPathRedirector;

/// <summary>
/// The contract that subpath redirection selectors must implement
/// </summary>
public interface IRedirectionContextFactory
{
	/// <summary>
	/// Create a redirection context using the incoming HttpContext and the configuration of the subsystem
	/// </summary>
	/// <param name="httpContext">The context of the current request for which a context is being created</param>
	/// <param name="options">The configuration of the subsystem on which creation is based</param>
	/// <returns>A RedirectionContext that can be used to fulfil the current request</returns>
	RedirectionContext CreateRedirectionContext(HttpContext httpContext, SubPathRedirectorOptions options);
}
