namespace PanoramicData.Extensions.SubPathRedirector;

internal class RedirectionHandler : IRedirectionHandler
{
    public void RedirectRequest(IRedirectionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (!context.CanRedirectRequest)
        {
            throw new InvalidOperationException("This request cannot be redirected!");
        }

        var permanent = context.SubPathRedirection!.RedirectionType switch
        {
            RedirectionTypes.Permanent => true,
            RedirectionTypes.Temporary => false,
            _ => throw new ArgumentOutOfRangeException($"Requested redirection type '{context.SubPathRedirection.RedirectionType}' is not recognised!"),
        };

        context.Response.Redirect(context.ResponseUrl!, permanent);
    }
}
