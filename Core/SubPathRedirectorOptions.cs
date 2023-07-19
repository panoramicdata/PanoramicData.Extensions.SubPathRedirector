using PanoramicData.Extensions.SubPathRedirector.ContextFactories;

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
	/// The type of redirection context factory to use in handling requests
	/// </summary>
	internal Type RedirectionContextFactoryType { get; private set; } = typeof(CaseInsensitiveAbsolutePathContextFactory);

	/// <summary>
	/// Set the redirection context factory type for use in handling requests
	/// </summary>
	/// <typeparam name="TRedirectionContextFactory">The type of redirection context factory to be used</typeparam>
	public void UseRedirectionContextFactoryType<TRedirectionContextFactory>() where TRedirectionContextFactory : class, IRedirectionContextFactory
		=> RedirectionContextFactoryType = typeof(TRedirectionContextFactory);
}