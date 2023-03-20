using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace PanoramicData.Extensions.SubPathRedirector.Tests.Helpers;

internal class FakeHttpContextBuilder
{
    private Dictionary<string, StringValues> _formValues = new()
    {
        { "email", "fred.smith@hotmail.com" },
        { "name", "Fred Smith" }
    };

    private FakeHttpContextBuilder() { }

    public FakeHttpContextBuilder WithFormValues(Dictionary<string, StringValues> formValues)
    {
        _formValues = formValues;
        return this;
    }

    public static FakeHttpContextBuilder CreateBuilder()
        => new FakeHttpContextBuilder();

    public HttpContext Build()
    {
        var features = new FeatureCollection();

        // Request
        var formCollection = new FormCollection(_formValues);
        var form = new FormFeature(formCollection);
        features.Set<IFormFeature>(form);

        // Response
        var response = new HttpResponseFeature();
        features.Set<IHttpResponseFeature>(response);

        var context = new DefaultHttpContext(features);

        // Response Cookies
        _ = new ResponseCookiesFeature(context.Features);

        return context;
    }
}