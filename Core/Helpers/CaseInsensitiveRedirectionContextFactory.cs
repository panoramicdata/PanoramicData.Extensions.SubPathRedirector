using Microsoft.AspNetCore.Http;

namespace PanoramicData.Extensions.SubPathRedirector.Helpers;

internal static class CaseInsensitiveRedirectionContextFactory
{
	internal static RedirectionContext Create(HttpContext httpContext, SubPathRedirectorConfiguration configuration, string requestPath)
	{
		var modifiedRequestPath = requestPath.ToLowerInvariant();
		var redirection = configuration.SubPathRedirections.FirstOrDefault(m => modifiedRequestPath.StartsWith(m.RequestUrlPrefix, StringComparison.Ordinal));

		return new RedirectionContext
		{
			HttpContext = httpContext,
			RequestPath = requestPath,
			ModifiedRequestPath = modifiedRequestPath,
			SubPathRedirection = redirection,
			ResponseUrl = redirection is null ? string.Empty : CaseInsensitiveResponseUrlCalculator.Calculate(requestPath, redirection),
		};
	}
}