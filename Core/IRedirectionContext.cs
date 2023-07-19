using Microsoft.AspNetCore.Http;

namespace PanoramicData.Extensions.SubPathRedirector;

internal interface IRedirectionContext
{
	/// <summary>
	/// Whether the current request can be redirected
	/// </summary>
	bool CanRedirectRequest { get; }

	/// <summary>
	/// The HttpContext of the current request
	/// </summary>
	HttpContext HttpContext { get; }

	/// <summary>
	/// The current request
	/// </summary>
	HttpRequest Request { get; }

	/// <summary>
	/// The response associated with the current request
	/// </summary>
	HttpResponse Response { get; }

	/// <summary>
	/// The options with which redirection is to be performefd
	/// </summary>
	SubPathRedirectorOptions Options { get; }

	/// <summary>
	/// The relative path of the current request
	/// </summary>
	string RequestPath { get; set; }

	/// <summary>
	/// A lowercase version of the path of the current request
	/// </summary>
	string ModifiedRequestPath { get; set; }

	/// <summary>
	/// The Url to which to redirect, if redirection is supported
	/// </summary>
	string? ResponseUrl { get; }

	/// <summary>
	/// The redirection configuration matching the current request
	/// </summary>
	SubPathRedirection? SubPathRedirection { get; }
}