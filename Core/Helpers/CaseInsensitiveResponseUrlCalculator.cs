namespace PanoramicData.Extensions.SubPathRedirector.Helpers;

/// <summary>
/// Case-insensitive response URL calculator
/// </summary>
internal static class CaseInsensitiveResponseUrlCalculator
{
	internal static string Calculate(string requestPath, SubPathRedirection redirection)
	{
		ArgumentNullException.ThrowIfNull(requestPath);
		ArgumentNullException.ThrowIfNull(redirection);

		var responseUrl = requestPath[redirection.RequestUrlPrefix.Length..];
		responseUrl = CombinePath(redirection.RedirectionUrlPrefix, responseUrl);
		return responseUrl;
	}

	private static string CombinePath(string requestUrlPrefix, string responseUrl)
	{
		if (!requestUrlPrefix.EndsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			requestUrlPrefix += "/";
		}

		if (responseUrl.StartsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			responseUrl = responseUrl[1..];
		}

		return requestUrlPrefix + responseUrl;
	}
}
