namespace PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

internal static class WaitHelper
{
	internal static async Task<bool> UntilAsync(Func<bool> condition)
	{
		var deadline = DateTime.UtcNow.AddSeconds(10);
		while (!condition())
		{
			if (DateTime.UtcNow > deadline)
			{
				return false;
			}

			await Task.Delay(10, TestContext.Current.CancellationToken);
		}

		return true;
	}
}