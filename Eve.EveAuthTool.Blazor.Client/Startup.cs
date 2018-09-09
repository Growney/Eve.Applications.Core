using Eve.EveAuthTool.Blazor.Client.Services;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Eve.EveAuthTool.Blazor.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AppState>();
            services.AddScoped<SessionState>();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<Client.App>("app");
        }
    }
}
