using Microsoft.Extensions.DependencyInjection;
using PanoramicData.Extensions.SubPathRedirector.Interfaces;

namespace PanoramicData.Extensions.SubPathRedirector.Providers.ConfigurationProviders;

/// <summary>
/// Simple configuration provider for serving static config provided in code
/// </summary>
internal class StaticRedirectorConfigurationProvider : IRedirectorConfigurationProvider
{
	private readonly SubPathRedirectorConfiguration _configuration;

	public StaticRedirectorConfigurationProvider(SubPathRedirectorOptions options)
	{
		_configuration = new SubPathRedirectorConfiguration
		{
			SubPathRedirections = options.SubPathRedirections
		};
	}

	void IRedirectorConfigurationProvider.StartInitialization(IServiceScopeFactory scopeFactory)
	{
		// There is no initialization to be performed for this provider
	}

	public SubPathRedirectorConfiguration GetConfiguration()
		=> _configuration;
}
