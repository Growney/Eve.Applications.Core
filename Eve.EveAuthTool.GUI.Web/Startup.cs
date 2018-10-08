using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Eve.ESI.Standard;
using Eve.ESI.Standard.Authentication.Account;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.ESI.Standard.Authentication.Token;
using Eve.EveAuthTool.Standard.Helpers;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.Standard.Security.Routing;
using Eve.EveAuthTool.GUI.Web.Controllers.Helpers;
using Eve.EveAuthTool.Standard.Security;
using Eve.Static.Standard;
using Gware.Core.MSSQL.Storage.Controller;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.OAuth;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Discord.Service;

namespace Eve.EveAuthTool.GUI.Web
{
    public class Startup
    {
        public const string c_defaultController = "Home";
        public const string c_defaultAction = "AboutESA";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
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
        public void ConfigureServices(IServiceCollection services)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Configuration["SyncFusionLicense"]);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.ExpireTimeSpan = TimeSpan.FromDays(30);
                        options.LoginPath = "/Registration/EveLogin/";
                        options.AccessDeniedPath = "/Registration/Denied/";

                    });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDelegatedControllerProvider(CreateController);

            services.AddSingleton<IArgumentsStore<OAuthRequestArguments>, ArgumentsStore<OAuthRequestArguments>>();
            services.AddSingleton<IArgumentsStore<ESITokenRequestParameters>, ArgumentsStore<ESITokenRequestParameters>>();
            services.AddSingleton<IArgumentsStore<DiscordLinkParameter>, DiscordLinkParameterStore>();

            services.AddSingleton<IESIAuthenticatedConfig, ESIAuthenticatedConfig>();
            services.AddSingleton<IDiscordBotConfiguration, DiscordBotConfiguration>();
            services.AddSingleton<IStaticDataCache, StaticDataCache>();

            services.AddTransient<IDiscordBot, DiscordBot>();
            services.AddScoped<IAllowedCharactersProvider, AllowedCharacterProvider>();
            services.AddScoped<IControllerParameters, ControllerParameters>();
            services.AddScoped<IViewParameterProvider, ViewParameterProvider>();
            services.AddScoped<IScopeGroupProvider, ScopeGroupProvider>();
            

            RouteTemplateDomain[] domains = Configuration.GetSection("TenantConfig:Domains").Get<RouteTemplateDomain[]>();
            services.AddTenantMVC(x =>
            {
                x.Controller = CreateController("TenantDB");
                x.SchemaFile = Configuration["TenantConfig:SchemaFile"];
                x.DBNameFormat = Configuration["TenantConfig:DBNameFormat"];
                x.Domains = domains;
                x.CreateComposite = true;
                x.OnDeployTenantSchema = async (ICommandController controller) =>
                {
                    bool retVal = false;

                    if (controller is MSSQLCommandController sqlController)
                    {
                        retVal = await sqlController.DeploySchema(Configuration["TenantConfig:SchemaFile"], controller.Name, true);
                    }

                    return retVal;
                };
                x.SearchIn = new Assembly[] { typeof(MSSQLCommandController).Assembly };
            },
            x =>
            {
                x.TenantHome = new RedirectToActionResult("Welcome", "Home", null);
                x.CreateNewResult = new RedirectResult($"{domains[0].GetAddress()}/Home/CreateNewTenant", false, false);
                x.NotFoundResult = new RedirectToActionResult("TenantNotFound", "Home", null);
                x.Upgrading = context =>
                {
                    return new RedirectToActionResult("TenantUpgrade", "Registration", new { returnUrl = context.HttpContext.Request.Path });
                };
            },
            config =>
            {
                config.Filters.Add(new TenantRequiredIfProvidedAttribute());
                config.Filters.Add(new AllowedCharactersResolverFilter());
                config.Filters.Add(new ViewParameterResolverFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1); 

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ITenantConfiguration configuration, IHostingEnvironment env)
        {
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            Microsoft.AspNetCore.Routing.RouteValueDictionary defaults = new Microsoft.AspNetCore.Routing.RouteValueDictionary { { "controller", c_defaultController }, { "action", c_defaultAction } };

            app.UseTenantMvc(configuration.Domains, defaults);
        }
    }
}
