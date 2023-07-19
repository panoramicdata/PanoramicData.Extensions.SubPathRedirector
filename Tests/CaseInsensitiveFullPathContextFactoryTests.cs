using FluentAssertions;
using PanoramicData.Extensions.SubPathRedirector.ContextFactories;
using PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

public class CaseInsensitiveFullPathContextFactoryTests
{
	[Fact]
	public void CreateRedirectionContext_NonMatchedUrl_DoesNotDiscoverSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://localhost:5000/images/favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorOptionsFactory.CreateDefaultFullPathOptions();

		var sut = new CaseInsensitiveFullPathContextFactory();
		var requestContext = sut.CreateRedirectionContext(httpContext, options);

		requestContext.SubPathRedirection.Should().BeNull();
	}

	[Fact]
	public void CreateRedirectionContext_LowerCaseSubPathPrefix_DiscoversSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://localhost:5000/google/favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorOptionsFactory.CreateDefaultFullPathOptions();

		var sut = new CaseInsensitiveFullPathContextFactory();
		var requestContext = sut.CreateRedirectionContext(httpContext, options);

		requestContext.SubPathRedirection.Should().NotBeNull();
		requestContext.SubPathRedirection!.RedirectionUrlPrefix.Should().Be("https://www.google.com/");
	}

	[Fact]
	public void CreateRedirectionContext_MixedCaseSubPathPrefix_DiscoversSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://localhost:5000/GoOgLe/Favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorOptionsFactory.CreateDefaultFullPathOptions();

		var sut = new CaseInsensitiveFullPathContextFactory();
		var requestContext = sut.CreateRedirectionContext(httpContext, options);

		requestContext.SubPathRedirection.Should().NotBeNull();
		requestContext.SubPathRedirection!.RedirectionUrlPrefix.Should().Be("https://www.google.com/");
	}

	[Fact]
	public void CreateRedirectionContext_MixedCaseHost_DiscoversSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://LocalHost:5000/google/favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorOptionsFactory.CreateDefaultFullPathOptions();

		var sut = new CaseInsensitiveFullPathContextFactory();
		var requestContext = sut.CreateRedirectionContext(httpContext, options);

		requestContext.SubPathRedirection.Should().NotBeNull();
		requestContext.SubPathRedirection!.RedirectionUrlPrefix.Should().Be("https://www.google.com/");
	}
}
