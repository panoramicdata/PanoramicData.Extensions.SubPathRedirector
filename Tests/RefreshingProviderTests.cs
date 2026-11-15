using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using PanoramicData.Extensions.SubPathRedirector.Interfaces;
using PanoramicData.Extensions.SubPathRedirector.Providers.ConfigurationProviders;
using PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

/// <summary>
/// Tests for <see cref="RefreshingRedirectorConfigurationProvider"/>
/// </summary>
public class RefreshingProviderTests
{
	private static Task<SubPathRedirectorConfiguration> Retrieve(IServiceScopeFactory _, CancellationToken __)
		=> Task.FromResult(FakeSubPathRedirectorConfigurationFactory.CreateDefaultAbsolutePathConfiguration());

	/// <summary>
	/// Intervals below the supported minimums are rejected
	/// </summary>
	[Theory]
	[InlineData(14, 30)]
	[InlineData(15, 4)]
	public void Constructor_IntervalTooShort_Throws(int refreshSeconds, int retrySeconds)
	{
		using var lifetime = new FakeHostApplicationLifetime();

		var action = () => new RefreshingRedirectorConfigurationProvider(Retrieve, TimeSpan.FromSeconds(refreshSeconds), TimeSpan.FromSeconds(retrySeconds), lifetime);

		action.Should().Throw<ArgumentOutOfRangeException>();
	}

	/// <summary>
	/// The default retry interval is accepted
	/// </summary>
	[Fact]
	public void Constructor_DefaultRetryInterval_DoesNotThrow()
	{
		using var lifetime = new FakeHostApplicationLifetime();

		using var provider = new RefreshingRedirectorConfigurationProvider(Retrieve, TimeSpan.FromSeconds(15), lifetime);

		((IRedirectorConfigurationProvider)provider).GetConfiguration().SubPathRedirections.Should().BeEmpty();
	}

	/// <summary>
	/// Initialisation applies the retrieved configuration
	/// </summary>
	[Fact]
	public async Task StartInitialization_RetrievalSucceeds_AppliesConfiguration()
	{
		using var lifetime = new FakeHostApplicationLifetime();
		using var provider = new RefreshingRedirectorConfigurationProvider(Retrieve, TimeSpan.FromSeconds(15), lifetime);
		await using var services = new ServiceCollection().BuildServiceProvider();
		var configurationProvider = (IRedirectorConfigurationProvider)provider;

		configurationProvider.StartInitialization(services.GetRequiredService<IServiceScopeFactory>());
		var applied = await WaitHelper.UntilAsync(() => configurationProvider.GetConfiguration().SubPathRedirections.Count == 1);
		lifetime.StopApplication();

		applied.Should().BeTrue();
	}

	/// <summary>
	/// A failed initial retrieval applies an empty configuration
	/// </summary>
	[Fact]
	public async Task StartInitialization_RetrievalFails_AppliesEmptyConfiguration()
	{
		using var lifetime = new FakeHostApplicationLifetime();
		var attempted = false;
		Task<SubPathRedirectorConfiguration> Fail(IServiceScopeFactory _, CancellationToken __)
		{
			attempted = true;
			throw new InvalidOperationException("boom");
		}

		using var provider = new RefreshingRedirectorConfigurationProvider(Fail, TimeSpan.FromSeconds(15), lifetime);
		await using var services = new ServiceCollection().BuildServiceProvider();
		var configurationProvider = (IRedirectorConfigurationProvider)provider;

		configurationProvider.StartInitialization(services.GetRequiredService<IServiceScopeFactory>());
		var tried = await WaitHelper.UntilAsync(() => attempted);
		lifetime.StopApplication();

		tried.Should().BeTrue();
		configurationProvider.GetConfiguration().SubPathRedirections.Should().BeEmpty();
	}
}