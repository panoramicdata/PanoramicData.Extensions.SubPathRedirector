using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using PanoramicData.Extensions.SubPathRedirector;
using PanoramicData.Extensions.SubPathRedirector.Providers.ConfigurationProviders;

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
	{
		app.UseMiddleware<RedirectorMiddleware>();

		// Wire up starting async initialization of the configuration provider once the application has finished starting
		var hostLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
		var configurationProvider = app.ApplicationServices.GetRequiredService<RedirectorConfigurationProviderInitializer>();
		var token = hostLifetime.ApplicationStarted;
		token.Register(configurationProvider.StartInitialization);

		return app;
	}
}
