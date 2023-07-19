using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

public class RedirectionHandlerTests
{
	[Fact]
	public void Redirect_NullRedirectionContext_ThrowsArgumentNullException()
	{
		var handler = new RedirectionHandler();
		var action = () => handler.RedirectRequest(null!);

		action.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void Redirect_NoMatchingSubPathRedirection_ThrowsInvalidOperationException()
	{
		var context = RedirectionContextBuilder
			.CreateBuilder("https://localhost:5000/notgoogle/favicon.ico")
			.Build();
		var handler = new RedirectionHandler();

		var action = () => handler.RedirectRequest(context);

		action.Should().Throw<InvalidOperationException>();
	}

	[Fact]
	public void Redirect_WithSubPathRedirection_Succeeds()
	{
		var context = RedirectionContextBuilder
			.CreateBuilder("https://localhost:5000/google/favicon.ico")
			.WithSubPathRedirectionAtIndex(0)
			.Build();
		var handler = new RedirectionHandler();

		var action = () => handler.RedirectRequest(context);

		action.Should().NotThrow();
	}

	[Fact]
	public void Redirect_ForTemporaryRedirection_SetsResultCode302()
	{
		var context = RedirectionContextBuilder
			.CreateBuilder("https://localhost:5000/google/favicon.ico")
			.WithConfiguration(FakeSubPathRedirectorConfigurationFactory.CreateDefaultAbsolutePathConfiguration(RedirectionTypes.Temporary))
			.WithSubPathRedirectionAtIndex(0)
			.Build();
		var handler = new RedirectionHandler();

		handler.RedirectRequest(context);

		context.Response.StatusCode.Should().Be(StatusCodes.Status302Found);
	}

	[Fact]
	public void Redirect_ForPermanentRedirection_SetsResultCode301()
	{
		var context = RedirectionContextBuilder
			.CreateBuilder("https://localhost:5000/google/favicon.ico")
			.WithSubPathRedirectionAtIndex(0)
			.Build();
		var handler = new RedirectionHandler();

		handler.RedirectRequest(context);

		context.Response.StatusCode.Should().Be(StatusCodes.Status301MovedPermanently);
	}
}
