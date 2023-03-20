namespace PanoramicData.Extensions.SubPathRedirector;

/// <summary>
/// Options for the configuration of sub-path redirection
/// </summary>
public class SubPathRedirectorOptions
{
	/// <summary>
	/// The list of redirections that are to be performed
	/// </summary>
	public List<SubPathRedirection> SubPathRedirections { get; set; } = new();

	/// <summary>
	/// Get the redirection associated with the requested path, or null if no mapping exists
	/// </summary>
	/// <param name="loweredRequestPath">The lowercase path of the request whose mapping we are trying to find</param>
	/// <returns>The redirection configuration that is to be used in interception, or null if no interception is appropriate</returns>
	internal SubPathRedirection? GetRedirection(string loweredRequestPath)
	{
		// Do very fast lookups of mappings in a case-insensitive manner by using lowercase Url
		var host = GetHost(loweredRequestPath);
		var redirection = SubPathRedirections.FirstOrDefault(m => loweredRequestPath.StartsWith(m.RequestUrlPrefix, StringComparison.Ordinal));
		return redirection;
	}

	/// <summary>
	/// Extract the host name from a request path
	/// </summary>
	/// <param name="requestPath">The request path</param>
	/// <returns>The host name, or the full path if the format is not recognised</returns>
	private static string GetHost(string requestPath)
	{
		var firstSeparatorIndex = requestPath.IndexOf('/');
		return firstSeparatorIndex < 0
			? requestPath
			: requestPath[..firstSeparatorIndex];
	}
}