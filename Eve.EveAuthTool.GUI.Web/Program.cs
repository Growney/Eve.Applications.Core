﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;
using NLog.Config;
using NLog.Internal.Fakeables;
using NLog.Targets;
namespace Eve.EveAuthTool.GUI.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile(Path.Combine(AppContext.BaseDirectory, $"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json"), optional: false, reloadOnChange: true);                 
                })
                .ConfigureLogging((hostingContext,logging) =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();

    }
}
