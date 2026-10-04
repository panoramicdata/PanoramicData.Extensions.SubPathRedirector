using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace PanoramicData.Extensions.SubPathRedirector.Demo.Pages;

/// <summary>
/// Model for the error page
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    /// <summary>
    /// The identifier of the failed request, if known
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Whether a request identifier is available to show
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    /// <summary>
    /// Create the error page model
    /// </summary>
    /// <param name="logger">The logger for this page</param>
    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Capture the identifier of the current request
    /// </summary>
    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
