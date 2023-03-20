using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

public class RedirectionHandlerTests
{
    [Fact]
    public void Redirect_NullRedirectionContext_ThrowsArgumentNullException()
    {
        var action = () => RedirectionHandler.RedirectRequest(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Redirect_NoMatchingSubPathRedirection_ThrowsInvalidOperationException()
    {
        var context = FakeRedirectionContextBuilder
            .CreateBuilder()
            .ForRequestUrl("/notgoogle/favicon.ico")
            .Build();

        var action = () => RedirectionHandler.RedirectRequest(context);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Redirect_WithSubPathRedirection_Succeeds()
    {
        var context = FakeRedirectionContextBuilder
            .CreateBuilder()
            .ForRequestUrl("/notgoogle/favicon.ico")
            .WithSubPathRedirectionAtIndex(0)
            .Build();

        var action = () => RedirectionHandler.RedirectRequest(context);

        action.Should().NotThrow();
    }

    [Fact]
    public void Redirect_ForTemporaryRedirection_SetsResultCode302()
    {
        var context = FakeRedirectionContextBuilder
            .CreateBuilder()
            .ForRequestUrl("/notgoogle/favicon.ico")
            .WithOptions(FakeSubPathRedirectorOptionsFactory.CreateDefaultTemporaryRedirectOptions())
            .WithSubPathRedirectionAtIndex(0)
            .Build();

        RedirectionHandler.RedirectRequest(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status302Found);
    }

    [Fact]
    public void Redirect_ForPermanentRedirection_SetsResultCode301()
    {
        var context = FakeRedirectionContextBuilder
            .CreateBuilder()
            .ForRequestUrl("/notgoogle/favicon.ico")
            .WithSubPathRedirectionAtIndex(0)
            .Build();

        RedirectionHandler.RedirectRequest(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status301MovedPermanently);
    }
}
