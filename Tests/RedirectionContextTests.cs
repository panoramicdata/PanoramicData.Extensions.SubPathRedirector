using FluentAssertions;
using PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

public class RedirectionContextTests
{
	[Fact]
	public void Constructor_NonMatchedUrl_DoesNotDiscoverSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://localhost:5000/images/favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorOptionsFactory.CreateDefaultOptions();

		var context = new RedirectionContext(httpContext, options);

		context.SubPathRedirection.Should().BeNull();
	}

	[Fact]
	public void Constructor_LowerCaseSubPathPrefix_DiscoversSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://localhost:5000/google/favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorOptionsFactory.CreateDefaultOptions();

		var context = new RedirectionContext(httpContext, options);

		context.SubPathRedirection.Should().NotBeNull();
		context.SubPathRedirection!.RedirectionUrlPrefix.Should().Be("https://www.google.com/");
	}

	[Fact]
	public void Constructor_MixedCaseSubPathPrefix_DiscoversSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://localhost:5000/GoOgLe/Favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorOptionsFactory.CreateDefaultOptions();

		var context = new RedirectionContext(httpContext, options);

		context.SubPathRedirection.Should().NotBeNull();
		context.SubPathRedirection!.RedirectionUrlPrefix.Should().Be("https://www.google.com/");
	}

	[Fact]
	public void Constructor_MixedCaseHost_DiscoversSubPathRedirection()
	{
		var httpContext = FakeHttpContextBuilder
			.CreateBuilder("https://LocalHost:5000/google/favicon.ico")
			.Build();

		var options = FakeSubPathRedirectorOptionsFactory.CreateDefaultOptions();

		var context = new RedirectionContext(httpContext, options);

		context.SubPathRedirection.Should().NotBeNull();
		context.SubPathRedirection!.RedirectionUrlPrefix.Should().Be("https://www.google.com/");
	}
}
