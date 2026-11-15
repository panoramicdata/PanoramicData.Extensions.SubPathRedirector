using AwesomeAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PanoramicData.Extensions.SubPathRedirector.ContextFactories;
using PanoramicData.Extensions.SubPathRedirector.Interfaces;
using PanoramicData.Extensions.SubPathRedirector.Providers.ConfigurationProviders;
using PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

/// <summary>
/// Tests for service registration and the middleware it wires up
/// </summary>
public class ServiceRegistrationTests
{
	private const string GoogleUrl = "https://localhost:5000/google/favicon.ico";

	private static void AddGoogleRedirection(SubPathRedirectorOptions options)
		=> options.SubPathRedirections.Add(new() { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "https://www.google.com/" });

	private static SubPathRedirectorConfiguration GoogleConfiguration()
		=> FakeSubPathRedirectorConfigurationFactory.CreateDefaultAbsolutePathConfiguration();

	private static async Task<HttpContext> InvokeAsync(ServiceProvider provider, string url, Action? onNext = null)
	{
		var httpContext = FakeHttpContextBuilder.CreateBuilder(url).Build();
		var middleware = provider.GetRequiredService<RedirectorMiddleware>();

		await middleware.InvokeAsync(httpContext, _ =>
		{
			onNext?.Invoke();
			return Task.CompletedTask;
		});

		return httpContext;
	}

	/// <summary>
	/// A static registration redirects a matching request
	/// </summary>
	[Fact]
	public async Task AddSubPathRedirection_Static_RedirectsMatchingRequest()
	{
		var services = new ServiceCollection();
		services.AddSubPathRedirection(AddGoogleRedirection);
		await using var provider = services.BuildServiceProvider();

		var httpContext = await InvokeAsync(provider, GoogleUrl);

		httpContext.Response.StatusCode.Should().Be(StatusCodes.Status301MovedPermanently);
		httpContext.Response.Headers.Location.ToString().Should().Be("https://www.google.com/favicon.ico");
	}

	/// <summary>
	/// A request with no matching redirection is passed to the next middleware
	/// </summary>
	[Fact]
	public async Task AddSubPathRedirection_Static_PassesThroughNonMatchingRequest()
	{
		var services = new ServiceCollection();
		services.AddSubPathRedirection(AddGoogleRedirection);
		await using var provider = services.BuildServiceProvider();
		var nextCalled = false;

		var httpContext = await InvokeAsync(provider, "https://localhost:5000/images/favicon.ico", () => nextCalled = true);

		nextCalled.Should().BeTrue();
		httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
	}

	/// <summary>
	/// The context factory type can be replaced
	/// </summary>
	[Fact]
	public void UseRedirectionContextFactoryType_ReplacesDefault()
	{
		var services = new ServiceCollection();
		services.AddSubPathRedirection(options => options.UseRedirectionContextFactoryType<CaseInsensitiveFullPathContextFactory>());
		using var provider = services.BuildServiceProvider();

		provider.GetRequiredService<IRedirectionContextFactory>().Should().BeOfType<CaseInsensitiveFullPathContextFactory>();
	}

	/// <summary>
	/// A deferred registration applies the configuration it retrieves
	/// </summary>
	[Fact]
	public async Task AddSubPathRedirection_Deferred_AppliesRetrievedConfiguration()
	{
		var services = new ServiceCollection();
		services.AddSubPathRedirection(_ => { }, _ => Task.FromResult(GoogleConfiguration()));
		await using var provider = services.BuildServiceProvider();
		var configurationProvider = provider.GetRequiredService<IRedirectorConfigurationProvider>();

		provider.GetRequiredService<RedirectorConfigurationProviderInitializer>().StartInitialization();
		var applied = await WaitHelper.UntilAsync(() => configurationProvider.GetConfiguration().SubPathRedirections.Count == 1);

		applied.Should().BeTrue();
		(await InvokeAsync(provider, GoogleUrl)).Response.StatusCode.Should().Be(StatusCodes.Status301MovedPermanently);
	}

	/// <summary>
	/// A deferred registration whose retrieval fails applies an empty configuration
	/// </summary>
	[Fact]
	public async Task AddSubPathRedirection_DeferredFailure_AppliesEmptyConfiguration()
	{
		var services = new ServiceCollection();
		services.AddSubPathRedirection(_ => { }, _ => throw new InvalidOperationException("boom"));
		await using var provider = services.BuildServiceProvider();
		var nextCalled = false;

		provider.GetRequiredService<RedirectorConfigurationProviderInitializer>().StartInitialization();
		await Task.Delay(200, TestContext.Current.CancellationToken);
		_ = await InvokeAsync(provider, GoogleUrl, () => nextCalled = true);

		nextCalled.Should().BeTrue();
	}

	/// <summary>
	/// A custom provider type is resolved from the container
	/// </summary>
	[Fact]
	public void AddSubPathRedirection_CustomProvider_RegistersProviderType()
	{
		var services = new ServiceCollection();
		services.AddSubPathRedirection<FixedConfigurationProvider>(_ => { });
		using var provider = services.BuildServiceProvider();

		provider.GetRequiredService<IRedirectorConfigurationProvider>().Should().BeOfType<FixedConfigurationProvider>();
	}

	/// <summary>
	/// The application started token triggers initialisation of the provider
	/// </summary>
	[Fact]
	public async Task UseSubPathRedirection_ApplicationStarted_InitialisesProvider()
	{
		using var lifetime = new FakeHostApplicationLifetime();
		var services = new ServiceCollection();
		services.AddSingleton<IHostApplicationLifetime>(lifetime);
		services.AddSubPathRedirection<FixedConfigurationProvider>(_ => { });
		await using var provider = services.BuildServiceProvider();
		var configurationProvider = provider.GetRequiredService<IRedirectorConfigurationProvider>();

		var app = new ApplicationBuilder(provider);
		app.UseSubPathRedirection().Should().BeSameAs(app);
		lifetime.NotifyStarted();
		var applied = await WaitHelper.UntilAsync(() => configurationProvider.GetConfiguration().SubPathRedirections.Count == 1);

		applied.Should().BeTrue();
	}

	private sealed class FixedConfigurationProvider : RedirectorConfigurationProviderBase
	{
		protected override Task InitializeAsync(IServiceScopeFactory scopeFactory)
		{
			ArgumentNullException.ThrowIfNull(scopeFactory);
			ApplyConfiguration(GoogleConfiguration());
			return Task.CompletedTask;
		}
	}
}