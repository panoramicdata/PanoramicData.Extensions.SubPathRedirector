using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace PanoramicData.Extensions.SubPathRedirector.Providers.ConfigurationProviders;

/// <summary>
/// Refreshing configuration provider, which enables the running of an async
/// task to get the configuration options on a regular schedule, including at initialization
/// </summary>
public class RefreshingRedirectorConfigurationProvider : RedirectorConfigurationProviderBase, IDisposable
{
	private readonly Func<IServiceScopeFactory, CancellationToken, Task<SubPathRedirectorConfiguration>> _retrievalTask;
	private readonly TimeSpan _refreshInterval;
	private readonly TimeSpan _retryInterval;
	private readonly IHostApplicationLifetime _hostApplicationLifetime;
	private IServiceScopeFactory? _scopeFactory;
	private CancellationToken _lifetimeCancellationToken = new CancellationTokenSource().Token;

	/// <summary>
	/// A simpler constructor for the refreshing provider
	/// </summary>
	/// <param name="retrievalTask">The task to be used to retrieve the configuration</param>
	/// <param name="refreshInterval">The interval we wish to have between retrieval operations</param>
	/// <param name="hostApplicationLifetime">The host application lifetime, from which our own lifetime is determined</param>
	/// <exception cref="ArgumentOutOfRangeException">The refresh or retry intervals are shorter than are permitted</exception>
	/// <remarks>Use of this constructor results in a retry interval of 30 seconds, used when the retrieval fails</remarks>
	public RefreshingRedirectorConfigurationProvider(
		Func<IServiceScopeFactory, CancellationToken, Task<SubPathRedirectorConfiguration>> retrievalTask,
		TimeSpan refreshInterval,
		IHostApplicationLifetime hostApplicationLifetime) : this(retrievalTask, refreshInterval, TimeSpan.FromSeconds(30), hostApplicationLifetime)
	{
		// All work is delegated to the main constructor
	}

	/// <summary>
	/// The main constructor for the refreshing provider
	/// </summary>
	/// <param name="retrievalTask">The task to be used to retrieve the configuration</param>
	/// <param name="refreshInterval">The interval we wish to have between retrieval operations</param>
	/// <param name="retryInterval">The interval between retrievals when the retrieval operation fails</param>
	/// <param name="hostApplicationLifetime">The host application lifetime, from which our own lifetime is determined</param>
	/// <exception cref="ArgumentOutOfRangeException">The refresh or retry intervals are shorter than are permitted</exception>
	public RefreshingRedirectorConfigurationProvider(
		Func<IServiceScopeFactory, CancellationToken, Task<SubPathRedirectorConfiguration>> retrievalTask,
		TimeSpan refreshInterval,
		TimeSpan retryInterval,
		IHostApplicationLifetime hostApplicationLifetime)
	{
		_retrievalTask = retrievalTask;
		_refreshInterval = refreshInterval;
		_retryInterval = retryInterval;
		_hostApplicationLifetime = hostApplicationLifetime;

		if (refreshInterval < TimeSpan.FromSeconds(15))
		{
			throw new ArgumentOutOfRangeException(nameof(refreshInterval), "Refreshing more frequently than every 15 seconds is not supported!");
		}

		if (retryInterval < TimeSpan.FromSeconds(5))
		{
			throw new ArgumentOutOfRangeException(nameof(refreshInterval), "Retrying failed retrievals more frequently than every 5 seconds is not supported!");
		}
	}

	/// <summary>
	/// Async initializer, which makes use of the provided task to get the configuration
	/// </summary>
	protected override async Task InitializeAsync(IServiceScopeFactory scopeFactory)
	{
		_scopeFactory = scopeFactory;
		_lifetimeCancellationToken = _hostApplicationLifetime.ApplicationStopping;

		var initializationSucceeded = false;
		var startTimestamp = Stopwatch.GetTimestamp();
		try
		{
			_ = await RetrieveAsync(_scopeFactory, _lifetimeCancellationToken);
			initializationSucceeded = true;
		}
		catch (TaskCanceledException)
		{
			return;
		}
		catch (Exception ex)
		{
			LogException(ex);
			// Apply an empty configuration class so that we have applied something; this is a requirement
			// of the base class on which we are built, to prove we are using it correctly
			ApplyConfiguration(new SubPathRedirectorConfiguration());
		}

		// Start the process of performing regular refreshes
		var timeTaken = Stopwatch.GetElapsedTime(startTimestamp);
		_ = RunRefreshAsync(timeTaken, initializationSucceeded);
	}

	/// <summary>
	/// The retrieval operation, which performs retrieval of the data and applies it to the base
	/// </summary>
	/// <param name="scopeFactory">The service scope factory that can be used to gain access to DI</param>
	/// <param name="cancellationToken">The token used to manage cancellation</param>
	/// <returns>The time taken to perform the retrieval operation</returns>
	private async Task<TimeSpan> RetrieveAsync(IServiceScopeFactory scopeFactory, CancellationToken cancellationToken)
	{
		if (_scopeFactory is null)
		{
			// Initialization is not yet done; we can do no work here
			return TimeSpan.Zero;
		}

		var startTimestamp = Stopwatch.GetTimestamp();

		var options = await _retrievalTask(scopeFactory, cancellationToken);
		ApplyConfiguration(options);

		return Stopwatch.GetElapsedTime(startTimestamp);
	}

	/// <summary>
	/// The refresh loop, which manages the running of the retrieval process
	/// to the required schedule, until the host initiates shutdown
	/// </summary>
	/// <param name="timeTaken">The time taken to perform the initialization task</param>
	private async Task RunRefreshAsync(TimeSpan timeTaken, bool operationSucceeded)
	{
		if (_scopeFactory is null)
		{
			// Initialization has not completed, so we cannot refresh
			return;
		}

		while (!_lifetimeCancellationToken.IsCancellationRequested)
		{
			try
			{
				await DelayUntilNextRefreshAsync(timeTaken, operationSucceeded, _lifetimeCancellationToken);
				timeTaken = await RetrieveAsync(_scopeFactory, _lifetimeCancellationToken);
				operationSucceeded = true;
			}
			catch (TaskCanceledException)
			{
				return;
			}
			catch (Exception ex)
			{
				LogException(ex);
				operationSucceeded = false;
			}
		}
	}

	/// <summary>
	/// Pause the task's execution until the next refresh is required
	/// </summary>
	/// <param name="timeTaken">The time taken to perform the last operation</param>
	/// <param name="operationSucceeded">Whether the last attempt succeeded</param>
	/// <param name="cancellationToken">The token used to manage cancellation</param>
	private async Task DelayUntilNextRefreshAsync(TimeSpan timeTaken, bool operationSucceeded, CancellationToken cancellationToken)
	{
		var delayPeriod = operationSucceeded ? _refreshInterval : _retryInterval;
		if (timeTaken < delayPeriod)
		{
			delayPeriod -= timeTaken;
		}

		try
		{
			await Task.Delay(delayPeriod, cancellationToken);
		}
		catch (TaskCanceledException)
		{
			// No need to do anything; cancellation is supported behaviour
		}
	}
}
