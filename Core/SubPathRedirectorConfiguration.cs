namespace PanoramicData.Extensions.SubPathRedirector;

/// <summary>
/// The set of redirections that the redirector applies
/// </summary>
public class SubPathRedirectorConfiguration
{
	/// <summary>
	/// The redirections to apply; the first whose prefix matches a request is used
	/// </summary>
	public IList<SubPathRedirection> SubPathRedirections { get; set; } = new List<SubPathRedirection>();
}
