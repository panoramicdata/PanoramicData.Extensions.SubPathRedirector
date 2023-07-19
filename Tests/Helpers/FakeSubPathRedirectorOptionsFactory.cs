namespace PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

internal static class FakeSubPathRedirectorOptionsFactory
{
	internal static SubPathRedirectorOptions CreateDefaultAbsolutePathOptions()
	{
		var options = new SubPathRedirectorOptions();

		options.SubPathRedirections.Add(
			new() { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "https://www.google.com/" }
			);

		return options;
	}

	internal static SubPathRedirectorOptions CreateDefaultAbsolutePathOptions(RedirectionTypes redirectionType)
	{
		var options = new SubPathRedirectorOptions();

		options.SubPathRedirections.Add(
			new() { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "https://www.google.com/", RedirectionType = redirectionType }
		);

		return options;
	}

	internal static SubPathRedirectorOptions CreateDefaultFullPathOptions()
	{
		var options = new SubPathRedirectorOptions();

		options.SubPathRedirections.Add(
			new() { RequestUrlPrefix = "https://localhost:5000/google/", RedirectionUrlPrefix = "https://www.google.com/" }
			);

		return options;
	}

	internal static SubPathRedirectorOptions CreateDefaultFullPathOptions(RedirectionTypes redirectionType)
	{
		var options = new SubPathRedirectorOptions();

		options.SubPathRedirections.Add(
			new() { RequestUrlPrefix = "https://localhost:5000/google/", RedirectionUrlPrefix = "https://www.google.com/", RedirectionType = redirectionType }
		);

		return options;
	}
}