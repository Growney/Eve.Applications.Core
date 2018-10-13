using Eve.ESI.Standard;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Configuration;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Discord.Service;
using Eve.EveAuthTool.Standard.Discord.Service.Providers;
using Eve.EveAuthTool.Standard.Helpers;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.Static.Standard;
using Gware.Core.MSSQL.Storage.Controller;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Configuration;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
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
                //configure NLog

            })
            .ConfigureServices(ConfigureServices)
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
                NLog.LogManager.LoadConfiguration("nlog.config");
            });

            await builder.RunConsoleAsync();

        }

        public static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddDelegatedControllerProvider<MSSQLCommandController>(
                (l, x) => CreateController(l, hostContext.Configuration, x),
                hostContext.Configuration["Controllers:PublicDataController"]);

            services.AddTransient(typeof(ICache<,>), typeof(FixedTimeCache<,>));

            services.AddSingleton<IStaticDataCache, StaticDataCache>();
            services.AddSingleton<IESIAuthenticatedConfig, ESIAuthenticatedConfig>();
            services.AddSingleton<IPublicDataProvider, PublicDataProvider>();
            services.AddSingleton<IDiscordBotConfiguration, DiscordBotConfiguration>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, DiscordBotService>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, DiscordSyncService>();
            services.AddSingleton<IArgumentsStore<DiscordLinkParameter>, DiscordLinkParameterStore>();
            services.AddSingleton<ISingleParameters, SingleParameters>();
            services.AddSingleton<IDiscordBot, DiscordBot>();

            services.AddScoped<IDiscordSyncProvider, DiscordSyncProvider>();
            services.AddScoped<IDiscordCommandContextAccessor, DiscordCommandContextAccessor>();
            services.AddScoped<IAllowedCharactersProvider, AllowedCharacterProvider>();
            services.AddScoped<IScopeParameters, DiscordCommandContextScopeParameters>();
            services.AddScoped<IScopeParameters, DiscordSyncScopeParameters>();
            services.AddScoped<IDiscordLinkProvider, DiscordLinkProvider>();
            services.AddScoped<ITypeSafeConfigurationProvider<eUserSetting>, UserConfigurationProvider>();


            services.AddTenantConfiguration<TenantConfiguration<MSSQLCommandController>>(x =>
            {
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
            });

            services.AddTenant();
        }

        public static MSSQLCommandController CreateController(ILogger<MSSQLCommandController> logger, IConfiguration config, string key)
        {
            bool isTrusted = config[$"Controllers:{key}:Trusted"]?.ToLower().ToString() == "true";
            if (isTrusted)
            {
                return new MSSQLCommandController(logger, config[$"Controllers:{key}:Server"], config[$"Controllers:{key}:Databasename"]);
            }
            else
            {
                return new MSSQLCommandController(logger, config[$"Controllers:{key}:Server"], config[$"Controllers:{key}:Databasename"], config[$"Controllers:{key}:Username"], config[$"Controllers:{key}:Password"]);
            }
        }

    }
}
