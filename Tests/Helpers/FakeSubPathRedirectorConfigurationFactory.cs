namespace PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

internal static class FakeSubPathRedirectorConfigurationFactory
{
	internal static SubPathRedirectorConfiguration CreateDefaultAbsolutePathConfiguration()
	{
		var configuration = new SubPathRedirectorConfiguration();

		configuration.SubPathRedirections.Add(
			new() { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "https://www.google.com/" }
			);

		return configuration;
	}

	internal static SubPathRedirectorConfiguration CreateDefaultAbsolutePathConfiguration(RedirectionTypes redirectionType)
	{
		var configuration = new SubPathRedirectorConfiguration();

		configuration.SubPathRedirections.Add(
			new() { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "https://www.google.com/", RedirectionType = redirectionType }
		);

		return configuration;
	}

	internal static SubPathRedirectorConfiguration CreateDefaultFullPathConfiguration()
	{
		var configuration = new SubPathRedirectorConfiguration();

		configuration.SubPathRedirections.Add(
			new() { RequestUrlPrefix = "https://localhost:5000/google/", RedirectionUrlPrefix = "https://www.google.com/" }
			);

		return configuration;
	}

	internal static SubPathRedirectorConfiguration CreateDefaultFullPathConfiguration(RedirectionTypes redirectionType)
	{
		var configuration = new SubPathRedirectorConfiguration();

		configuration.SubPathRedirections.Add(
			new() { RequestUrlPrefix = "https://localhost:5000/google/", RedirectionUrlPrefix = "https://www.google.com/", RedirectionType = redirectionType }
		);

		return configuration;
	}
}