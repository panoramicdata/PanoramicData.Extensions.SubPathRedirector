using AwesomeAssertions;
using PanoramicData.Extensions.SubPathRedirector.ContextFactories;
using PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

/// <summary>
/// Tests for <see cref="ContextFactories.CaseInsensitiveFullPathContextFactory"/>
/// </summary>
public class CaseInsensitiveFullPathContextFactoryTests
{
	/// <summary>
	/// A Url matching no prefix yields no redirection
	/// </summary>
	[Fact]
	public void CreateRedirectionContext_NonMatchedUrl_DoesNotDiscoverSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://localhost:5000/images/favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorConfigurationFactory.CreateDefaultFullPathConfiguration();

		var sut = new CaseInsensitiveFullPathContextFactory();
		var requestContext = sut.CreateRedirectionContext(httpContext, options);

		requestContext.SubPathRedirection.Should().BeNull();
	}

	/// <summary>
	/// A lowercase prefix is matched
	/// </summary>
	[Fact]
	public void CreateRedirectionContext_LowerCaseSubPathPrefix_DiscoversSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://localhost:5000/google/favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorConfigurationFactory.CreateDefaultFullPathConfiguration();

		var sut = new CaseInsensitiveFullPathContextFactory();
		var requestContext = sut.CreateRedirectionContext(httpContext, options);

		requestContext.SubPathRedirection.Should().NotBeNull();
		requestContext.SubPathRedirection!.RedirectionUrlPrefix.Should().Be("https://www.google.com/");
	}

	/// <summary>
	/// A mixed-case prefix is matched case-insensitively
	/// </summary>
	[Fact]
	public void CreateRedirectionContext_MixedCaseSubPathPrefix_DiscoversSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://localhost:5000/GoOgLe/Favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorConfigurationFactory.CreateDefaultFullPathConfiguration();

		var sut = new CaseInsensitiveFullPathContextFactory();
		var requestContext = sut.CreateRedirectionContext(httpContext, options);

		requestContext.SubPathRedirection.Should().NotBeNull();
		requestContext.SubPathRedirection!.RedirectionUrlPrefix.Should().Be("https://www.google.com/");
	}

	/// <summary>
	/// A mixed-case host does not prevent a match
	/// </summary>
	[Fact]
	public void CreateRedirectionContext_MixedCaseHost_DiscoversSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://LocalHost:5000/google/favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorConfigurationFactory.CreateDefaultFullPathConfiguration();

		var sut = new CaseInsensitiveFullPathContextFactory();
		var requestContext = sut.CreateRedirectionContext(httpContext, options);

		requestContext.SubPathRedirection.Should().NotBeNull();
		requestContext.SubPathRedirection!.RedirectionUrlPrefix.Should().Be("https://www.google.com/");
	}
}
