using Microsoft.Extensions.DependencyInjection;

namespace PanoramicData.Extensions.SubPathRedirector.Interfaces;

internal interface IRedirectorConfigurationProvider
{
	void StartInitialization(IServiceScopeFactory scopeFactory);

	SubPathRedirectorConfiguration GetConfiguration();
}
