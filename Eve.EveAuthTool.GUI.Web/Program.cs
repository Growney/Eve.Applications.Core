using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
                .ConfigureAppConfiguration((hostingContext,config) =>
                {

                    var env = hostingContext.HostingEnvironment;
                    var sharedFolder = Path.Combine(env.ContentRootPath, "..", "Eve.EveAuthTool.Configuration");
                    config.AddJsonFile(Path.Combine(env.ContentRootPath, sharedFolder, "appsettings.{ ctx.HostingEnvironment.EnvironmentName}.json"), optional: false, reloadOnChange: true);
                });
    }
}
