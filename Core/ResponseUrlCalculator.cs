namespace PanoramicData.Extensions.SubPathRedirector;

internal static class ResponseUrlCalculator
{
	internal static string Calculate(string requestPath, SubPathRedirection redirection)
	{
		ArgumentNullException.ThrowIfNull(requestPath);
		ArgumentNullException.ThrowIfNull(redirection);

		string responseUrl = requestPath[redirection.RequestUrlPrefix.Length..];
		responseUrl = CombinePath(redirection.RedirectionUrlPrefix, responseUrl);
		return responseUrl;
	}

	private static string CombinePath(string requestUrlPrefix, string responseUrl)
	{
		if (!requestUrlPrefix.EndsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			requestUrlPrefix = requestUrlPrefix + "/";
		}

		if (responseUrl.StartsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			responseUrl = responseUrl[1..];
		}

		return requestUrlPrefix + responseUrl;
	}
}
