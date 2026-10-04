namespace PanoramicData.Extensions.SubPathRedirector;

/// <summary>
/// Enum of the redirection types that can be performed
/// </summary>
public enum RedirectionTypes : byte
{
	/// <summary>
	/// A temporary redirect (HTTP 302)
	/// </summary>
	Temporary = 0,

	/// <summary>
	/// A permanent redirect (HTTP 301)
	/// </summary>
	Permanent = 1,
}
