using Microsoft.AspNetCore.Builder;
using PanoramicData.Extensions.SubPathRedirector;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for the IApplicationBuilder interface
/// </summary>
public static class ApplicationBuilderExtensions
{
	/// <summary>
	/// Add the redirection middleware to the request pipeline
	/// </summary>
	/// <param name="app">The instance of the interface that we are extending</param>
	/// <returns>The instance that was extended, to support method chaining</returns>
	public static IApplicationBuilder UseSubPathRedirection(this IApplicationBuilder app)
		=> app.UseMiddleware<RedirectorMiddleware>();
}
