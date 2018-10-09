using Eve.ESI.Standard;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Discord.Service;
using Eve.Static.Standard;
using Gware.Core.MSSQL.Storage.Controller;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;

namespace Eve.EveAuthTool.Service
{
    class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            var builder = new HostBuilder()
            .ConfigureHostConfiguration(config =>
            {
                config.AddEnvironmentVariables(prefix: "EVESWAGGERAUTH_");
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile($"Configuration//appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices(ConfigureServices)
            .ConfigureLogging((hostingContext, logging) =>
            {

            });

            await builder.RunConsoleAsync();
            
        }

        public static void ConfigureServices(HostBuilderContext hostContext,IServiceCollection services)
        {
            services.AddDelegatedControllerProvider(x =>
            {
                return CreateController(hostContext.Configuration, x);
            });

            services.AddSingleton<IStaticDataCache, StaticDataCache>();
            services.AddSingleton<IESIAuthenticatedConfig, ESIAuthenticatedConfig>();
            services.AddSingleton<IPublicDataProvider, PublicDataProvider>();
            services.AddSingleton<IDiscordBotConfiguration, DiscordBotConfiguration>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, DiscordBotService>();
            services.AddSingleton<IArgumentsStore<DiscordLinkParameter>, DiscordLinkParameterStore>();
            services.AddTenant(x =>
            {
                
                x.SchemaFile = hostContext.Configuration["TenantConfig:SchemaFile"];
                x.DBNameFormat = hostContext.Configuration["TenantConfig:DBNameFormat"];
                x.Domains = hostContext.Configuration.GetSection("TenantConfig:Domains").Get<RouteTemplateDomain[]>();
                x.CreateComposite = true;
                x.OnDeployTenantSchema = async (ICommandController controller) =>
                {
                    bool retVal = false;

                    if (controller is MSSQLCommandController sqlController)
                    {
                        retVal = await sqlController.DeploySchema(hostContext.Configuration["TenantConfig:SchemaFile"], controller.Name, true);
                    }

                    return retVal;
                };
                x.Controller = CreateController(hostContext.Configuration,"TenantDB");
                x.SearchIn = new Assembly[] { typeof(MSSQLCommandController).Assembly };
            });
        }

        public static ICommandController CreateController(IConfiguration config,string key)
        {
            bool isTrusted = config[$"Controllers:{key}:Trusted"]?.ToLower().ToString() == "true";
            if (isTrusted)
            {
                return new MSSQLCommandController(config[$"Controllers:{key}:Server"], config[$"Controllers:{key}:Databasename"]);
            }
            else
            {
                return new MSSQLCommandController(config[$"Controllers:{key}:Server"], config[$"Controllers:{key}:Databasename"], config[$"Controllers:{key}:Username"], config[$"Controllers:{key}:Password"]);
            }
        }
        
    }
}
