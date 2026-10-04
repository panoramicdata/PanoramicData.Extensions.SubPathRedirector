namespace PanoramicData.Extensions.SubPathRedirector.Demo;

/// <summary>
/// Entry point for the demo application
/// </summary>
public static class Program
{
	/// <summary>
	/// Start the demo web application
	/// </summary>
	/// <param name="args">Command-line arguments</param>
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddRazorPages();
		builder.Services.AddServerSideBlazor();

		builder.Services.AddSubPathRedirection(options =>
			options.SubPathRedirections.Add(new() { RequestUrlPrefix = "/google/", RedirectionUrlPrefix = "https://www.google.com/" })
		);

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();

		app.UseStaticFiles();
		app.UseSubPathRedirection();

		app.UseRouting();

		app.MapBlazorHub();
		app.MapFallbackToPage("/_Host");

		app.Run();
	}
}
