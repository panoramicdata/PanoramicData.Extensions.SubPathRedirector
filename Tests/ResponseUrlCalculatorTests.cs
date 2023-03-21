using FluentAssertions;

namespace PanoramicData.Extensions.SubPathRedirector.Tests;

public class ResponseUrlCalculatorTests
{
	[Fact]
	public void Calculate_RedirectionPrefixStartsWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "/notgoogle/", RedirectionType = RedirectionTypes.Temporary };
		var result = ResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("/notgoogle/favicon.ico");
	}

	[Fact]
	public void Calculate_RedirectionPrefixDoesNotStartWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle/", RedirectionType = RedirectionTypes.Temporary };
		var result = ResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	[Fact]
	public void Calculate_RedirectionPrefixEndsWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle/", RedirectionType = RedirectionTypes.Temporary };
		var result = ResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	[Fact]
	public void Calculate_RedirectionPrefixDoesNotEndWithSlash_ReturnsUrlContainingAppropriateValue()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = ResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	[Fact]
	public void Calculate_RequestContainsMixedCaseResourceName_RetainsMixedCaseResourceName()
	{
		var config = new SubPathRedirection { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = ResponseUrlCalculator.Calculate("/google/FavIcon.ico", config);

		result.Should().Be("notgoogle/FavIcon.ico");
	}

	[Fact]
	public void Calculate_RequestUrlPrefixContainsMixedCaseValue_GeneratesAppropriateUrl()
	{
		var config = new SubPathRedirection() { RequestUrlPrefix = "/Google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = ResponseUrlCalculator.Calculate("/google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}

	[Fact]
	public void Calculate_RequestUrlContainsMixedCaseValue_GeneratesAppropriateUrl()
	{
		var config = new SubPathRedirection() { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "notgoogle", RedirectionType = RedirectionTypes.Temporary };
		var result = ResponseUrlCalculator.Calculate("/Google/favicon.ico", config);

		result.Should().Be("notgoogle/favicon.ico");
	}
}