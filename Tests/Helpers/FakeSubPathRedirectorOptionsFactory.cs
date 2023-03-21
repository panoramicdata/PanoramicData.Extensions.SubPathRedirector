namespace PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

internal static class FakeSubPathRedirectorOptionsFactory
{
	internal static SubPathRedirectorOptions CreateDefaultOptions()
	{
		var options = new SubPathRedirectorOptions();

		options.SubPathRedirections.Add(
			new() { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "https://www.google.com/" }
			);

		return options;
	}

	internal static SubPathRedirectorOptions CreateDefaultTemporaryRedirectOptions()
	{
		var options = new SubPathRedirectorOptions();

		options.SubPathRedirections.Add(
			new() { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "https://www.google.com/", RedirectionType = RedirectionTypes.Temporary }
		);

		return options;
	}
}