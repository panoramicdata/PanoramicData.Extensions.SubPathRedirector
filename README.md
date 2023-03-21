# PanoramicData.Extensions.SubPathRedirector
ASP.NET Core middleware for redirecting the requests of a subpath to another application; can be useful for handling cross-application requests 
in development.

It is common in cloud-hosted environments to aggregate multiple sites together using an aggregation service, such as Azure Application Gateway. 
Whilst this works well for hosted environments, it can cause developers a problem when they run the code locally. Any inter-site links that 
are routed to the correct site through the aggregation service when hosted on the cloud, may not route to the correct application in the local 
development environment, where no such aggregation is available. If this causes you difficulty replicating the behaviour of production for
developers, this middleware might be for you!

## Example usage scenario
As an example, consider the following setup:
www.contoso.com/marketing/ routes to the marketing website, developed in ASP.NET Core
www.contoso.com/shop/ routes to a separate commerce site developed in PHP

A relative link from the marketing site of /shop/products/1 works correctly in production, but not in development. In development, this 
request is intercepted by the marketing site itself, and to replicate what happens in production we need to redirect it to another site.
This middleware supports redirecting any request to /shop/ to a different domain, such as another localhost address with a different port.

## Usage
There are two tasks to do in startup of an ASP.NET Core site to make this middleware function as intended.

### Step 1 - Add services (and the associated config)
Add the services that are used by the subsystem into DI in Program.cs (or Startup.cs in older systems) using code similar to the following:

```
builder.Services.AddSubPathRedirection(options =>
	options.SubPathRedirections.Add( new() { RequestUrlPrefix = "/shop/", RedirectionUrlPrefix = "https://localhost:5001/" })
);
```

Here we make use of the AddSubPathRedirection extension method to add the necessary services, as well as defining the action that is 
used to apply modifications to the options on which the redirection of requests is based.

### Step 2 - add the middleware to the pipeline
Next, we must include the middleware in the pipeline in the correct place.
```
app.UseSubPathRedirection();
```
The order in which pipline registration methods are called is important, as the order of execution of middleware is determined by the 
order in which they are specified at startup. If we want to serve files only for users who are authenticated then this call should come 
after the call to UseAuthorization(). However, if files are to be served to all users, even those who are unauthenticated, we should 
add our redirection middleware before the application of authorization.

You may choose to have redirection take place even for unauthorized users. If you wish that to work, call UseSubPathRedirection() 
before calling UseAuthorization(), as shown below:
```
// In this example, redirections do not have any authorization applied.
app.UseStaticFiles();
app.UseSubPathRedirection();

app.UseRouting();

// Turn on authentication
app.UseAuthentication();
app.UseAuthorization();
```
Alternatively, to force users to authenticate before their requests can be redirected, instead call UseSubPathRedirection() 
after you call UseAuthorization(), as shown in this alternative code sample:
```
app.UseStaticFiles();

app.UseRouting();

// Turn on authentication
app.UseAuthentication();
app.UseAuthorization();

// In this example, we restrict redirection to users who are logged in
app.UseSubPathRedirection();
```
Use the correct code sample to meet your specific requirements.
> CAUTION: Consider your authentication requirements carefully. Failure to apply the correct rules could result in the exposure 
> of sensitive information or functionality to unauthenticated users.
