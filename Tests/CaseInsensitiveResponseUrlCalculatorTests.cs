using AwesomeAssertions;
using PanoramicData.Extensions.SubPathRedirector.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

/// <summary>
/// Tests for the internal CaseInsensitiveResponseUrlCalculator
/// </summary>
public class CaseInsensitiveResponseUrlCalculatorTests
{
	/// <summary>
	/// A full Url redirection prefix produces a full Url to the resource
	/// </summary>
	[Fact]
	public void Calculate_RedirectionPrefixIsFullUrl_ReturnsFullUrlToCorrectResource()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "http://localhost:5000/google/", RedirectionUrlPrefix = "http://localhost:5001/notgoogle/", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("http://localhost:5000/google/favicon.ico", config);

		result.Should().Be("http://localhost:5001/notgoogle/favicon.ico");
	}

	/// <summary>
	/// A redirection prefix with a leading slash is handled
	/// </summary>
	[Fact]
	public void Calculate_RedirectionPrefixStartsWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "/notgoogle/", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("/notgoogle/favicon.ico");
	}

	/// <summary>
	/// A redirection prefix without a leading slash is handled
	/// </summary>
	[Fact]
	public void Calculate_RedirectionPrefixDoesNotStartWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle/", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	/// <summary>
	/// A redirection prefix with a trailing slash is handled
	/// </summary>
	[Fact]
	public void Calculate_RedirectionPrefixEndsWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle/", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	/// <summary>
	/// A redirection prefix without a trailing slash is handled
	/// </summary>
	[Fact]
	public void Calculate_RedirectionPrefixDoesNotEndWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	/// <summary>
	/// The case of the resource name after the prefix is preserved
	/// </summary>
	[Fact]
	public void Calculate_RequestContainsMixedCaseResourceName_RetainsMixedCaseResourceName()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/FavIcon.ico", config);

		result.Should().Be("notgoogle/FavIcon.ico");
	}

	/// <summary>
	/// A mixed-case request prefix still generates the expected Url
	/// </summary>
	[Fact]
	public void Calculate_RequestUrlPrefixContainsMixedCaseValue_GeneratesAppropriateUrl()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/Google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	/// <summary>
	/// A mixed-case request Url still generates the expected Url
	/// </summary>
	[Fact]
	public void Calculate_RequestUrlContainsMixedCaseValue_GeneratesAppropriateUrl()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/Google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}
}