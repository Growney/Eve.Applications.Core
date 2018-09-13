using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Eve.ESI.Core;
using Eve.ESI.Standard.Authentication.Account;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.ESI.Standard.Authentication.Token;
using Eve.EveAuthTool.Core.Helpers;
using Eve.EveAuthTool.Core.Security.Middleware;
using Eve.EveAuthTool.Core.Security.Routing;
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.ExpireTimeSpan = TimeSpan.FromDays(30);
                        options.LoginPath = "/Registration/Index/";

                    });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IControllerProvider>(new DelegatedControllerProvider(CreateController));
            services.AddSingleton<IStaticDataCache, StaticDataCache>();
            string[] domains = Configuration.GetSection("TenantConfig:Domains").Get<string[]>();
            services.AddSingleton<ITenantConfiguration>(new TenantConfiguration()
            {
                TenantHome = new RedirectToActionResult("Welcome", "Home", null),
                CreateNewResult = new RedirectResult($"{domains[0]}/Home/CreateNewTenant/",false,false),
                NotFoundResult = new RedirectToActionResult("TenantNotFound", "Home", null),
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

                    if (controller is MSSQLCommandController sqlController)
                    {
                        retVal = await sqlController.DeploySchema(Configuration["TenantConfig:SchemaFile"], controller.Name, true);
                    }

                    return retVal;
                },
                SearchIn = new Assembly[]{ typeof(MSSQLCommandController).Assembly }
            });
            services.AddSingleton<IArgumentsStore<OAuthRequestArguments>, ArgumentsStore<OAuthRequestArguments>>();
            services.AddSingleton<IArgumentsStore<ESITokenRequestParameters>, ArgumentsStore<ESITokenRequestParameters>>();
            services.AddSingleton<IESIAuthenticatedConfig, ESIAuthenticatedConfig>();

            services.AddScoped<ITenantControllerProvider, TenantControllerProvider>();
            services.AddScoped<IAllowedCharactersProvider, AllowedCharacterProvider>();
            services.AddScoped<IControllerParameters, ControllerParameters>();
            services.AddScoped<IViewParameterProvider, ViewParameterProvider>();
            services.AddScoped<IScopeGroupProvider, ScopeGroupProvider>();

            services.AddMvc(config =>
            {
                config.Filters.Add(new TenantResolverFilter());
                config.Filters.Add(new TenantVersionCheckingFilter());
#warning TODO Resolve Tenant required if provided redirect issue
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

            app.UseMvc(routes =>
            {
                routes.MapDomainRoutes(
                    domains: configuration.Domains,
                    routeTemplate: "{controller}/{action}/{id?}",
                    defaults: defaults,
                    ignorePorts: configuration.IgnorePorts
                    );

            routes.MapRoute(
                name: "default",
                defaults: defaults,
                template: "{controller}/{action}/{id?}");
            });
        }
    }
}
