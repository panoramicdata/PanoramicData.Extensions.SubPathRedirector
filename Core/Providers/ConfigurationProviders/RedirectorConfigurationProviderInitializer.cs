using Microsoft.Extensions.DependencyInjection;
using PanoramicData.Extensions.SubPathRedirector.Interfaces;

namespace PanoramicData.Extensions.SubPathRedirector.Providers.ConfigurationProviders;

internal class RedirectorConfigurationProviderInitializer
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IRedirectorConfigurationProvider _RedirectorOptionsProvider;

	public RedirectorConfigurationProviderInitializer(IServiceScopeFactory scopeFactory, IRedirectorConfigurationProvider RedirectorOptionsProvider)
	{
		_scopeFactory = scopeFactory;
		_RedirectorOptionsProvider = RedirectorOptionsProvider;
	}

	internal void StartInitialization()
		=> _RedirectorOptionsProvider.StartInitialization(_scopeFactory);
}
