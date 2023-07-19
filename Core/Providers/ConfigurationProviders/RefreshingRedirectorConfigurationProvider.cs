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
	private readonly IHostApplicationLifetime _hostApplicationLifetime;
	private IServiceScopeFactory? _scopeFactory;
	private CancellationToken _lifetimeCancellationToken = new CancellationTokenSource().Token;

	public RefreshingRedirectorConfigurationProvider(Func<IServiceScopeFactory, CancellationToken, Task<SubPathRedirectorConfiguration>> retrievalTask, TimeSpan refreshInterval, IHostApplicationLifetime hostApplicationLifetime)
	{
		_retrievalTask = retrievalTask;
		_refreshInterval = refreshInterval;
		_hostApplicationLifetime = hostApplicationLifetime;

		if (refreshInterval < TimeSpan.FromMinutes(1))
		{
			throw new ArgumentOutOfRangeException(nameof(refreshInterval), "Refreshing more frequently than every minute is not supported!");
		}
	}

	/// <summary>
	/// Async initializer, which makes use of the provided task to get the configuration
	/// </summary>
	protected override async Task InitializeAsync(IServiceScopeFactory scopeFactory)
	{
		_scopeFactory = scopeFactory;
		_lifetimeCancellationToken = _hostApplicationLifetime.ApplicationStopping;

		var startTimestamp = Stopwatch.GetTimestamp();
		try
		{
			_ = await RetrieveAsync(_scopeFactory, _lifetimeCancellationToken);
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
		_ = RunRefreshAsync(timeTaken);
	}

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

	private async Task RunRefreshAsync(TimeSpan timeTaken)
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
				await DelayUntilNextRefreshAsync(timeTaken, _lifetimeCancellationToken);
				timeTaken = await RetrieveAsync(_scopeFactory, _lifetimeCancellationToken);
			}
			catch (TaskCanceledException)
			{
				return;
			}
			catch (Exception ex)
			{
				LogException(ex);
			}
		}
	}

	private async Task DelayUntilNextRefreshAsync(TimeSpan timeTaken, CancellationToken cancellationToken)
	{
		var delayPeriod = _refreshInterval;
		if (timeTaken < _refreshInterval)
		{
			delayPeriod = _refreshInterval - timeTaken;
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
