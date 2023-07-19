using PanoramicData.Extensions.SubPathRedirector;
using PanoramicData.Extensions.SubPathRedirector.Interfaces;
using PanoramicData.Extensions.SubPathRedirector.Providers.ConfigurationProviders;

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
		// Register a static configuration provider for use in retrieval of redirection config
		var configuration = new SubPathRedirectorOptions();
		options.Invoke(configuration);
		_ = services.AddSingleton<IRedirectorConfigurationProvider>(new StaticRedirectorConfigurationProvider(configuration));

		RegisterDefaultServices(services, configuration);

		return services;
	}

	/// <summary>
	/// Add the DI registrations necessary to use the sub path redirection middleware
	/// </summary>
	/// <param name="services">The service collection that we are extending</param>
	/// <param name="options">The action that is to be applied to the configuration options</param>
	/// <param name="configurationAction">The action to perform to retrieve redirector configuration</param>
	/// <returns>The service collection being extended, to support method chaining</returns>
	public static IServiceCollection AddSubPathRedirection(this IServiceCollection services, Action<SubPathRedirectorOptions> options, Func<IServiceScopeFactory, Task<SubPathRedirectorConfiguration>> configurationAction)
	{
		var configuration = new SubPathRedirectorOptions();
		options.Invoke(configuration);

		// Register a static configuration provider for use in retrieval of redirection config
		_ = services.AddSingleton<IRedirectorConfigurationProvider>(new DeferredRedirectorConfigurationProvider(configurationAction));
		RegisterDefaultServices(services, configuration);

		return services;
	}

	/// <summary>
	/// Add the DI registrations necessary to use the sub path redirection middleware
	/// </summary>
	/// <typeparam name="TConfigurationProvider">The type used to provide redirector configuration</typeparam>
	/// <param name="services">The service collection that we are extending</param>
	/// <param name="options">The action that is to be applied to the configuration options</param>
	/// <returns>The service collection being extended, to support method chaining</returns>
	public static IServiceCollection AddSubPathRedirection<TConfigurationProvider>(this IServiceCollection services, Action<SubPathRedirectorOptions> options) where TConfigurationProvider : RedirectorConfigurationProviderBase
	{
		var configuration = new SubPathRedirectorOptions();
		options.Invoke(configuration);

		// Register a custom configuration provider for use in retrieval of redirection config
		_ = services.AddSingleton(typeof(IRedirectorConfigurationProvider), typeof(TConfigurationProvider));
		RegisterDefaultServices(services, configuration);

		return services;
	}

	/// <summary>
	/// Register all of the services used in all scenarios
	/// </summary>
	/// <param name="services">The service collection into which to register services</param>
	/// <param name="options">The options used to specify the required configuration</param>
	private static void RegisterDefaultServices(IServiceCollection services, SubPathRedirectorOptions options)
	{
		_ = services.AddTransient<RedirectorMiddleware>();
		_ = services.AddTransient<IRedirectionHandler, RedirectionHandler>();
		_ = services.AddSingleton<RedirectorConfigurationProviderInitializer>();

		// Register the context factory type to be used to fulfil requests
		services.AddTransient(typeof(IRedirectionContextFactory), options.RedirectionContextFactoryType);
	}
}
