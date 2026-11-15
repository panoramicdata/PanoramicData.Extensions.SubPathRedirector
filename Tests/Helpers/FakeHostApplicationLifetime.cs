using Microsoft.Extensions.Hosting;

namespace PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

internal sealed class FakeHostApplicationLifetime : IHostApplicationLifetime, IDisposable
{
	private readonly CancellationTokenSource _started = new();
	private readonly CancellationTokenSource _stopping = new();
	private readonly CancellationTokenSource _stopped = new();

	public CancellationToken ApplicationStarted => _started.Token;

	public CancellationToken ApplicationStopping => _stopping.Token;

	public CancellationToken ApplicationStopped => _stopped.Token;

	public void StopApplication() => _stopping.Cancel();

	public void NotifyStarted() => _started.Cancel();

	public void Dispose()
	{
		_started.Dispose();
		_stopping.Dispose();
		_stopped.Dispose();
	}
}