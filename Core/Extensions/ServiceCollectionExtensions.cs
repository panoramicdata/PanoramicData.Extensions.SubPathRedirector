using PanoramicData.Extensions.SubPathRedirector;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Add the DI registrations necessary to use the sub path redirection middleware
	/// </summary>
	/// <param name="services">The service collection that we are extending</param>
	/// <param name="options">The action that is to be applied to the configuration options</param>
	/// <returns>The service collection being extended, to support method chaining</returns>
	public static IServiceCollection AddSubPathRedirection(this IServiceCollection services, Action<SubPathRedirectorOptions> options)
	{
		_ = services.AddTransient<RedirectorMiddleware>();
		_ = services.AddTransient<IRedirectionHandler, RedirectionHandler>();
		services.Configure(options);

		// Register the context factory type to be used to fulfil requests
		var setupOptions = new SubPathRedirectorOptions();
		options.Invoke(setupOptions);
		services.AddTransient(typeof(IRedirectionContextFactory), setupOptions.RedirectionContextFactoryType);

		return services;
	}
}
