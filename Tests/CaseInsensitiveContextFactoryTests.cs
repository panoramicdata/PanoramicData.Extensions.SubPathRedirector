using AwesomeAssertions;
using PanoramicData.Extensions.SubPathRedirector.ContextFactories;
using PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

/// <summary>
/// Tests for the case-insensitive context factories
/// </summary>
public class CaseInsensitiveContextFactoryTests
{
	private static RedirectionContext Create(bool fullPath, string url)
	{
		var httpContext = FakeHttpContextBuilder.CreateBuilder(url).Build();
		return fullPath
			? new CaseInsensitiveFullPathContextFactory().CreateRedirectionContext(httpContext, FakeSubPathRedirectorConfigurationFactory.CreateDefaultFullPathConfiguration())
			: new CaseInsensitiveAbsolutePathContextFactory().CreateRedirectionContext(httpContext, FakeSubPathRedirectorConfigurationFactory.CreateDefaultAbsolutePathConfiguration());
	}

	/// <summary>
	/// A Url matching no prefix yields no redirection
	/// </summary>
	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public void CreateRedirectionContext_NonMatchedUrl_DoesNotDiscoverSubPathRedirection(bool fullPath)
	{
		var context = Create(fullPath, "https://localhost:5000/images/favicon.ico");

		context.SubPathRedirection.Should().BeNull();
		context.CanRedirectRequest.Should().BeFalse();
		context.ResponseUrl.Should().BeEmpty();
	}

	/// <summary>
	/// Prefixes and hosts are matched case-insensitively
	/// </summary>
	[Theory]
	[InlineData(false, "https://localhost:5000/google/favicon.ico")]
	[InlineData(false, "https://localhost:5000/GoOgLe/Favicon.ico")]
	[InlineData(false, "https://LocalHost:5000/google/favicon.ico")]
	[InlineData(true, "https://localhost:5000/google/favicon.ico")]
	[InlineData(true, "https://localhost:5000/GoOgLe/Favicon.ico")]
	[InlineData(true, "https://LocalHost:5000/google/favicon.ico")]
	public void CreateRedirectionContext_MatchingUrl_DiscoversSubPathRedirection(bool fullPath, string url)
	{
		var context = Create(fullPath, url);

		context.SubPathRedirection.Should().NotBeNull();
		context.SubPathRedirection!.RedirectionUrlPrefix.Should().Be("https://www.google.com/");
		context.ResponseUrl.Should().StartWith("https://www.google.com/");
	}

	/// <summary>
	/// The request path keeps its case while the modified path is lowercase
	/// </summary>
	[Theory]
	[InlineData(false, "/GoOgLe/Favicon.ico")]
	[InlineData(true, "https://localhost:5000/GoOgLe/Favicon.ico")]
	public void CreateRedirectionContext_MixedCaseUrl_PreservesRequestPathAndLowersModifiedPath(bool fullPath, string expectedPath)
	{
		var context = Create(fullPath, "https://localhost:5000/GoOgLe/Favicon.ico");

		context.RequestPath.Should().Be(expectedPath);
		context.ModifiedRequestPath.Should().Be(expectedPath.ToLowerInvariant());
		context.ResponseUrl.Should().Be("https://www.google.com/Favicon.ico");
	}
}