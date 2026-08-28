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

Version 1.1 of the package introduces the ability to load configuration asynchronously from a data store, so there are now 3 overloads 
of the `AddSubPathRedirection` method. The options for loading configuration are detailed towards the end of this document.

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
> **Warning** Consider your authentication requirements carefully. Failure to apply the correct rules could result in the 
> exposure of sensitive information or functionality to unauthenticated users.

## Configuration loading options

Version 1.1 of the package enhances the options available to you for loading mapping configuration. This is achieved 
by exposing 3 overloads of the `AddSubPathRedirection` method. Understanding these various options is key to making 
best use of the package in your specific circumstances.

### 1. Static configuration in code

The first overload of the `AddSubPathRedirection` method takes static, hard-coded configuration, and is the 
simplest to use. This is the method shown in the first section, and used in the demo.

### 2. Async loading of configuration from a data store (no refreshes)

The second overload of the `AddSubPathRedirection` method takes as its parameter a 
`Func<IServiceScopeFactory, Task<SubPathRedirectorConfiguration>>` which is the code to be called to load configuration 
once the system has started. The loading method is asynchronous to support accessing the config from a slow, external 
data store, and is delayed until application startup has completed to ensure that your data store has finished 
initializing.

Prior to loading the config, the config is empty, resulting in no static files being served. Therefore, you should 
aim to optimise loading of config as much as possible, to reduce to a minimum the period for which there is no 
config available.

### 3. Customised async loading (optionally with refreshes/reloading) by specifying the provider type

The third overload of the `AddSubPathRedirection` method takes a type parameter to specify a custom class 
inheriting from `RedirectorConfigurationProviderBase` that will be used to load redirector configuration. This class 
offers asynchronous startup and loading of the configuration system, enabling lots of flexibility, including the 
ability to reload configuration during the application's lifetime. The implementation of the `InitializeAsync` method 
that the implementing class is required to override is called to load the configuration once the system has started. 
The loading method is asynchronous to support accessing the config from a slow, external data store, and is delayed 
until startup has completed to ensure that your data store initialization will have completed.

One public implementation of `RedirectorConfigurationProviderBase` is provided, namely 
`RefreshingRedirectorConfigurationProvider`. This class can be used to call your asynchronous loading code to a 
predetermined schedule, with the refresh period specified as a parameter of the constructor. Using this as a base 
class for your custom loading logic will make regular refreshes simpler.

Providing a custom type inheriting from `RedirectorConfigurationProviderBase` enables you to implement a 
custom caching scheme making use of cache invalidation to reload configuration as quickly as possible after a change 
is detected, if you consider this useful. It can also offer the ability to inject dependencies into your code to
access the data store, as necessary. Note that the provider is registered as a singleton service, so if you need 
access to any scoped or transient services then you should accept an instance of IServiceScopeFactory and create a 
scope during your configuration loading operations rather than capturing a service with a lower scope directly,
as those captured services would never be disposed; furthermore bugs may be encountered in those services. An 
instance of IServiceScopeFactory is supplied to the InitializeAsync method to enable (and encourage) dependency 
resolution in the correct way, so generally you are unlikely to need to accept many dependencies through the
constructor.

Custom implementations should call the `ApplyConfiguration` method on the base class to apply the new configuration 
to the underlying subsystem each time it has been loaded - but only do so once the configuration is complete.

Developers should note that their code is not directly called by the subsystem to retrieve configuration on each 
request. The middleware in this package sits on a very hot path in the request pipeline, and it is therefore 
inappropriate to allow developers to run a lot of code during the request. Instead, `RedirectorConfigurationProviderBase` 
is responsible for delivering the configuration on request, and developers of custom providers are required to call 
the `ApplyConfiguration` method on the base class to have it swap out its configuration with an updated set in a 
highly optimised, thread-safe way. This affects the design of your provider; you are responsible for wiring up 
tasks to complete any future loading/reloading operations as part of the work done in the `InitializeAsync` method.

Prior to loading the config, the config is empty, resulting in no redirection being performed. Therefore, you should 
aim to optimise loading of config as much as possible, to reduce to a minimum the period for which there is no config 
available.
