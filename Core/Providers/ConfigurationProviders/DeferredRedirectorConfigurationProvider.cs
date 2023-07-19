using Microsoft.Extensions.DependencyInjection;

namespace PanoramicData.Extensions.SubPathRedirector.Providers.ConfigurationProviders;

/// <summary>
/// Deferred configuration provider, which enables the running of an async
/// task to get the configuration options just once, during initialization
/// </summary>
internal class DeferredRedirectorConfigurationProvider : RedirectorConfigurationProviderBase
{
	private readonly Func<IServiceScopeFactory, Task<SubPathRedirectorConfiguration>> _initializationTask;

	public DeferredRedirectorConfigurationProvider(Func<IServiceScopeFactory, Task<SubPathRedirectorConfiguration>> initializationTask)
	{
		_initializationTask = initializationTask;
	}

	/// <summary>
	/// Async initializer, which makes use of the provided task to get the configuration options
	/// </summary>
	protected override async Task InitializeAsync(IServiceScopeFactory scopeFactory)
	{
		SubPathRedirectorConfiguration? configuration = null;

		try
		{
			configuration = await _initializationTask(scopeFactory);
		}
		catch (Exception ex)
		{
			LogException(ex);
		}

		ApplyConfiguration(configuration ?? new SubPathRedirectorConfiguration());
	}
}
