namespace PanoramicData.Extensions.SubPathRedirector;

internal interface IRedirectionHandler
{
    void RedirectRequest(IRedirectionContext context);
}