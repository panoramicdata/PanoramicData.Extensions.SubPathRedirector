using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PanoramicData.Extensions.SubPathRedirector.Interfaces;

namespace PanoramicData.Extensions.SubPathRedirector.Providers.ConfigurationProviders;

public abstract class RedirectorConfigurationProviderBase : IRedirectorConfigurationProvider, IDisposable
{
	private readonly object _updateLock = new();
	private Task? _initializationTask;
	private IServiceScopeFactory? _scopeFactory;
	private SubPathRedirectorConfiguration _configuration = new();
	private bool _configurationApplied;

	/// <summary>
	/// Start async initialization of configuration
	/// </summary>
	public void StartInitialization(IServiceScopeFactory scopeFactory)
	{
		_scopeFactory = scopeFactory;

		// Set up for async initialization, but don't start the operation yet
		_initializationTask = new Task(async () => await InitializeAsync(scopeFactory));
		_initializationTask.ContinueWith(InitializationCompletedHandler);

		// All set up; now start the operation; it will continue after we return
		_initializationTask.Start();
	}

	/// <summary>
	/// Method to be implemented by a provider to asynchronously initialize the configuration
	/// </summary>
	/// <param name="scopeFactory">A service scope factory that can be used to start a service scope, if needed</param>
	/// <remarks>
	/// The implementer MUST call ApplyConfiguration to provide the options to the 
	/// underlying system before the completion of the initialization operation.
	/// </remarks>
	protected abstract Task InitializeAsync(IServiceScopeFactory scopeFactory);

	/// <summary>
	/// Handler that is run when async initialization has been completed
	/// </summary>
	/// <param name="task">The task that was performed</param>
	/// <exception cref="InvalidOperationException">The configuration provider did not apply config during initialization</exception>
	private void InitializationCompletedHandler(Task task)
	{
		if (!_configurationApplied)
		{
			throw new InvalidOperationException(
				"Configuration providers MUST call ApplyConfiguration during initialization!");
		}
	}

	/// <summary>
	/// Exposed method for configuration providers to call to supply config
	/// </summary>
	/// <param name="configuration">The configuration options to be applied</param>
	protected void ApplyConfiguration(SubPathRedirectorConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

		// Apply the new configuration
		lock (_updateLock)
		{
			_configuration = configuration;
			_configurationApplied = true;
		}
	}

	/// <summary>
	/// Method used by the subsystem to get the current configuration
	/// </summary>
	/// <returns>The current configuration, as it exists</returns>
	SubPathRedirectorConfiguration IRedirectorConfigurationProvider.GetConfiguration()
		=> _configuration;

	/// <summary>
	/// Handle logging of an exception, if logging has been enabled
	/// </summary>
	/// <param name="ex">The exception that is to be logged</param>
	/// <param name="scopeFactory">The service scope factory from which we can create scopes for accessing DI</param>
	protected void LogException(Exception ex)
	{
		if (_scopeFactory is null)
		{
			return;
		}

		try
		{
			using var scope = _scopeFactory.CreateScope();
			var logger = scope.ServiceProvider.GetService<ILogger<RedirectorConfigurationProviderBase>>();
			logger?.LogError(ex, "Exception during configuration retrieval. Message: {Message}", ex.Message);
		}
		catch (Exception)
		{
			// Not much we can do about a logging failure, other than avoid it being rethrown
		}
	}

	public void Dispose()
	{
		_initializationTask?.Dispose();
		GC.SuppressFinalize(this);
	}
}
