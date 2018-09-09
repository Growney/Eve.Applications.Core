using Eve.ESI.Core;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.ESI.Standard.Token;
using Eve.EveAuthTool.Blazor.Server.Controllers;
using Eve.EveAuthTool.Core.Discord;
using Eve.EveAuthTool.Core.Discord.Configuration;
using Eve.EveAuthTool.Core.Security.Middleware;
using Eve.EveAuthTool.Core.Security.Routing;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Core.MSSQL.Storage.Controller;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.OAuth;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net.Mime;

namespace Eve.EveAuthTool.Blazor.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public ICommandController CreateController(string key)
        {
            bool isTrusted = Configuration[$"Controllers:{key}:Trusted"]?.ToLower().ToString() == "true";
            if (isTrusted)
            {
                return new MSSQLCommandController(Configuration[$"Controllers:{key}:Server"], Configuration[$"Controllers:{key}:Databasename"]);
            }
            else
            {
                return new MSSQLCommandController(Configuration[$"Controllers:{key}:Server"], Configuration[$"Controllers:{key}:Databasename"], Configuration[$"Controllers:{key}:Username"], Configuration[$"Controllers:{key}:Password"]);
            }
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.ExpireTimeSpan = TimeSpan.FromDays(30);
                        options.LoginPath = "/Registration/Index/";

                    });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IControllerProvider>(new DelegatedControllerProvider(CreateController));
            services.AddSingleton<IStaticDataCache, StaticDataCache>();
            services.AddSingleton<ITenantConfiguration>(new TenantConfiguration()
            {
                TenantHome = new RedirectToActionResult("Index", "Character", null),
                CreateNewResult = new RedirectToActionResult("CreateTenant", "Home", null),
                NotFoundResult = new RedirectToActionResult("NoTenantFound", "Home", null),
                Controller = CreateController("TenantDB"),
                SchemaFile = Configuration["TenantConfig:SchemaFile"],
                DBNameFormat = Configuration["TenantConfig:DBNameFormat"],
                Domains = Configuration.GetSection("TenantConfig:Domains").Get<string[]>(),
                CreateComposite = true,
                Upgrading = new RedirectToActionResult("Upgrading", "Tenant", null),
                IgnorePorts = Configuration["TenantConfig:IgnorePorts"]?.ToLower().Equals("true") ?? false,
                OnDeployTenantSchema = async (ICommandController controller) =>
                {
                    bool retVal = false;

                    if(controller is MSSQLCommandController sqlController)
                    {
                        retVal = await sqlController.DeploySchema(Configuration["TenantConfig:SchemaFile"], controller.Name, true);
                    }

                    return retVal;
                }
            });

            services.AddSingleton<IArgumentsStore<OAuthRequestArguments>, ArgumentsStore<OAuthRequestArguments>>();
            services.AddSingleton<IArgumentsStore<ESIToken>, ArgumentsStore<ESIToken>>();
            services.AddSingleton<IArgumentsStore<DiscordLinkParameters>, ArgumentsStore<DiscordLinkParameters>>();

            services.AddSingleton<IESIAuthenticatedConfig, ESIAuthenticatedConfig>();
            services.AddSingleton<IDiscordBotConfiguration, DiscordBotConfiguration>();
            services.AddScoped<ITenantControllerProvider, TenantControllerProvider>();
            services.AddScoped<IAllowedCharactersProvider, AllowedCharacterProvider>();
            services.AddScoped<ControllerParameters, ControllerParameters>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, DiscordBotService>();
            services.AddMvc(config =>
            {
                config.Filters.Add(new TenantResolverFilter());
                config.Filters.Add(new TenantVersionCheckingFilter());
                config.Filters.Add(new AllowedCharactersResolverFilter());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            }); 

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ITenantConfiguration configuration, IHostingEnvironment env)
        {
            app.UseResponseCompression();

            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseMvc(routes =>
            {
                routes.MapDomainRoutes(
                    domains: configuration.Domains,
                    routeTemplate: "api/{controller}/{action}/{id?}",
                    defaults: new Microsoft.AspNetCore.Routing.RouteValueDictionary { { "controller", "Character" }, { "action", "Index" } },
                    ignorePorts: configuration.IgnorePorts
                    );

                routes.MapRoute(
                    name: "default",
                    template: "api/{controller}/{action}/{id?}"
                    );
            });
            app.UseBlazor<Client.Program>();

      
        }
    }
}
