using AwesomeAssertions;
using PanoramicData.Extensions.SubPathRedirector.Helpers;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

public class CaseInsensitiveResponseUrlCalculatorTests
{
	[Fact]
	public void Calculate_RedirectionPrefixIsFullUrl_ReturnsFullUrlToCorrectResource()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "http://localhost:5000/google/", RedirectionUrlPrefix = "http://localhost:5001/notgoogle/", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("http://localhost:5000/google/favicon.ico", config);

		result.Should().Be("http://localhost:5001/notgoogle/favicon.ico");
	}

	[Fact]
	public void Calculate_RedirectionPrefixStartsWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "/notgoogle/", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("/notgoogle/favicon.ico");
	}

	[Fact]
	public void Calculate_RedirectionPrefixDoesNotStartWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle/", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	[Fact]
	public void Calculate_RedirectionPrefixEndsWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle/", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	[Fact]
	public void Calculate_RedirectionPrefixDoesNotEndWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	[Fact]
	public void Calculate_RequestContainsMixedCaseResourceName_RetainsMixedCaseResourceName()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/FavIcon.ico", config);

		result.Should().Be("notgoogle/FavIcon.ico");
	}

	[Fact]
	public void Calculate_RequestUrlPrefixContainsMixedCaseValue_GeneratesAppropriateUrl()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/Google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	[Fact]
	public void Calculate_RequestUrlContainsMixedCaseValue_GeneratesAppropriateUrl()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = CaseInsensitiveResponseUrlCalculator.Calculate("/Google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}
}