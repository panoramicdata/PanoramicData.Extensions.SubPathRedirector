using AwesomeAssertions;
using PanoramicData.Extensions.SubPathRedirector.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

/// <summary>
/// Tests for the internal CaseInsensitiveResponseUrlCalculator
/// </summary>
public class CaseInsensitiveResponseUrlCalculatorTests
{
	/// <summary>
	/// Prefix slashes and the case of the resource name are normalised as expected
	/// </summary>
	[Theory]
	[InlineData("http://localhost:5000/google/", "http://localhost:5001/notgoogle/", "http://localhost:5000/google/favicon.ico", "http://localhost:5001/notgoogle/favicon.ico")]
	[InlineData("/google/", "/notgoogle/", "/google/favicon.ico", "/notgoogle/favicon.ico")]
	[InlineData("/google/", "notgoogle/", "/google/favicon.ico", "notgoogle/favicon.ico")]
	[InlineData("/google/", "notgoogle", "/google/favicon.ico", "notgoogle/favicon.ico")]
	[InlineData("/google/", "notgoogle", "/google/FavIcon.ico", "notgoogle/FavIcon.ico")]
	[InlineData("/Google/", "notgoogle", "/google/favicon.ico", "notgoogle/favicon.ico")]
	[InlineData("/google/", "notgoogle", "/Google/favicon.ico", "notgoogle/favicon.ico")]
	public void Calculate_Prefixes_ReturnsExpectedUrl(string requestPrefix, string redirectionPrefix, string requestPath, string expected)
	{
		var config = new SubPathRedirection { RequestUrlPrefix = requestPrefix, RedirectionUrlPrefix = redirectionPrefix, RedirectionType = RedirectionTypes.Temporary };

		CaseInsensitiveResponseUrlCalculator.Calculate(requestPath, config).Should().Be(expected);
	}

	/// <summary>
	/// Null arguments are rejected
	/// </summary>
	[Fact]
	public void Calculate_NullArguments_Throw()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/a/", RedirectionUrlPrefix = "/b/" };

		var nullPath = () => CaseInsensitiveResponseUrlCalculator.Calculate(null!, config);
		var nullRedirection = () => CaseInsensitiveResponseUrlCalculator.Calculate("/a/x", null!);

		nullPath.Should().Throw<ArgumentNullException>();
		nullRedirection.Should().Throw<ArgumentNullException>();
	}
}