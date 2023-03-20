namespace PanoramicData.Extensions.SubPathRedirector;

/// <summary>
/// A redirection that is to be performed
/// </summary>
public class SubPathRedirection
{
	/// <summary>
	/// The Url prefix for which requests are to be redirected
	/// </summary>
	public required string RequestUrlPrefix { get; set; } = string.Empty;

	/// <summary>
	/// The Url prefix to which redirection is to be performed
	/// </summary>
	public required string RedirectionUrlPrefix { get; set; } = string.Empty;

	/// <summary>
	/// The type of redirection to be performed
	/// </summary>
	public RedirectionTypes RedirectionType { get; set; } = RedirectionTypes.Permanent;
}