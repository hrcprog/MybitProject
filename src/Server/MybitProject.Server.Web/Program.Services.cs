﻿using System.IO.Compression;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.ResponseCompression;
using MybitProject.Server.Api;
using MybitProject.Client.Web;
using MybitProject.Server.Web.Services;
using MybitProject.Client.Core.Services.Contracts;

namespace MybitProject.Server.Web;

public static partial class Program
{
    public static void AddServerWebProjectServices(this WebApplicationBuilder builder)
    {
        // Services being registered here can get injected in server project only.
        var services = builder.Services;
        var configuration = builder.Configuration;

        if (AppEnvironment.IsDev())
        {
            builder.Logging.AddDiagnosticLogger();
        }

        services.AddClientWebProjectServices(configuration);

        services.AddSingleton(sp =>
        {
            ServerWebSettings settings = new();
            configuration.Bind(settings);
            return settings;
        });

        builder.AddServerApiProjectServices();

        services.AddOptions<ServerWebSettings>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        AddBlazor(builder);
    }

    private static void AddBlazor(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddScoped<IAuthTokenProvider, ServerSideAuthTokenProvider>();
        services.AddScoped(sp =>
        {
            // This HTTP client is utilized during pre-rendering and within Blazor Auto/Server sessions for API calls. 
            // Key headers such as Authorization and AcceptLanguage headers are added in Client/Core/Services/HttpMessageHandlers. 
            // Additionally, forwarded headers are handled to ensure proper forwarding, if the backend is hosted behind a CDN. 
            // User agent and referrer headers are also included to provide the API with necessary request context. 

            Uri.TryCreate(configuration.GetServerAddress(), UriKind.RelativeOrAbsolute, out var serverAddress);
            var currentRequest = sp.GetRequiredService<IHttpContextAccessor>().HttpContext!.Request;
            if (serverAddress!.IsAbsoluteUri is false)
            {
                serverAddress = new Uri(currentRequest.GetBaseUrl(), serverAddress);
            }

            var httpClient = new HttpClient(sp.GetRequiredService<HttpMessageHandler>())
            {
                BaseAddress = serverAddress
            };

            var forwardedHeadersOptions = sp.GetRequiredService<ServerWebSettings>().ForwardedHeaders;

            foreach (var xHeader in currentRequest.Headers.Where(h => h.Key.StartsWith("X-", StringComparison.InvariantCultureIgnoreCase)))
            {
                httpClient.DefaultRequestHeaders.Add(xHeader.Key, string.Join(',', xHeader.Value.AsEnumerable()));
            }

            if (httpClient.DefaultRequestHeaders.Contains(forwardedHeadersOptions.ForwardedForHeaderName) is false &&
                currentRequest.HttpContext.Connection.RemoteIpAddress is not null)
            {
                httpClient.DefaultRequestHeaders.Add(forwardedHeadersOptions.ForwardedForHeaderName,
                                                     currentRequest.HttpContext.Connection.RemoteIpAddress.ToString());
            }

            if (currentRequest.Headers.TryGetValue(HeaderNames.UserAgent, out var headerValues))
            {
                foreach (var ua in currentRequest.Headers.UserAgent)
                {
                    httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(ua);
                }
            }

            if (currentRequest.Headers.TryGetValue(HeaderNames.Referer, out headerValues))
            {
                httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, string.Join(',', headerValues.AsEnumerable()));
            }

            return httpClient;
        });

        services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        services.AddMvc();
    }
}
