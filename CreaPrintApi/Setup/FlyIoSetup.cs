using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;

namespace CreaPrintApi.Setup
{
 public static class FlyIoSetup
 {
 /// <summary>
 /// Configure hosting settings appropriate for fly.io.
 /// - Bind Kestrel to0.0.0.0:{PORT} when PORT is provided by the platform
 /// - Set ASPNETCORE_URLS fallback
 /// - Configure forwarded headers options to trust the proxy
 /// </summary>
 public static void Configure(WebApplicationBuilder builder)
 {
 var port = Environment.GetEnvironmentVariable("PORT");
 if (!string.IsNullOrWhiteSpace(port) && int.TryParse(port, out var p))
 {
 // Ensure Kestrel will listen on the external interface
 builder.WebHost.UseUrls($"http://0.0.0.0:{p}");
 // Also set ASPNETCORE_URLS so other components relying on it behave as expected
 Environment.SetEnvironmentVariable("ASPNETCORE_URLS", $"http://0.0.0.0:{p}");
 }

 // Configure forwarded headers so HttpContext.Request.Scheme and IP are correct behind fly proxy
 builder.Services.Configure<ForwardedHeadersOptions>(options =>
 {
 options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
 // Clear the default networks/proxies so the proxy is accepted (fly proxies have dynamic addresses)
 options.KnownNetworks.Clear();
 options.KnownProxies.Clear();
 });
 }
 }
}
