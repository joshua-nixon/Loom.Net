using Loom.Net.AspNetCore.Configuration;
using Loom.Net.AspNetCore.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Loom.Net.AspNetCore.Extensions
{
    public static class WebApplicationExtensions
    {
        extension(WebApplication application)
        {
            public WebApplication UseLoom()
            {
                var options = application.Services.GetRequiredService<LoomAspNetCoreConfiguration>();

                application.Map(options.RoutePattern, LoomCatchAllEndpointHandler.HandleAsync);

                return application;
            }
        }
    }
}
