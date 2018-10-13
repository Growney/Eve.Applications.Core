using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Discord.Service.Providers;
using Gware.Standard.Web;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service
{
    public class DiscordSyncService : IHostedService,IDisposable
    {
        private readonly IDiscordBot m_bot;
        private readonly ILogger<DiscordSyncService> m_logger;
        private readonly IServiceScopeFactory m_scopeFactory;
        private readonly ITenantConfiguration m_tenantConfiguration;
        private readonly IDiscordBotConfiguration m_botConfiguration;

        private Timer m_timer;

        public DiscordSyncService(ILogger<DiscordSyncService> logger,IDiscordBot bot,IServiceScopeFactory scopeFactory,ITenantConfiguration tenantConfiguration,IDiscordBotConfiguration botConfiguration)
        {
            m_logger = logger;
            m_bot = bot;
            m_scopeFactory = scopeFactory;
            m_tenantConfiguration = tenantConfiguration;
            m_botConfiguration = botConfiguration;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            m_logger.LogInformation("Discord Sync service is starting");
            m_timer = new Timer(DoWork,null,TimeSpan.Zero,m_botConfiguration.SyncCycleTime);

            return Task.CompletedTask;
        }
        private async void DoWork(object state)
        {
            List<LinkedTenant> tenants = m_tenantConfiguration.AllWithLink((byte)eTenantLinkType.Discord);
            List<Task<IServiceScope>> tasks = new List<Task<IServiceScope>>();
            foreach(LinkedTenant tenant in tenants)
            {
                IServiceScope tenantScope = m_scopeFactory.CreateScope();
                ConfigureServiceScope(tenantScope, tenant);
                tasks.Add(SyncTenant(tenantScope));
            }

            while (tasks.Count > 0)
            {
                Task<IServiceScope> completed = await Task<IServiceScope>.WhenAny(tasks.ToArray());
                tasks.Remove(completed);
                IServiceScope scope = await completed;
                scope.Dispose();
            }
            

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            m_timer?.Change(Timeout.Infinite, 0);
            m_logger.LogInformation("Discord Sync service is stopping");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            m_timer.Dispose();
        }

        public void ConfigureServiceScope(IServiceScope scope, Tenant tenant)
        {
            ITenantStorage storage = scope.ServiceProvider.GetService<ITenantStorage>();
            storage.Tenant = tenant;
        }

        public async Task<IServiceScope> SyncTenant(IServiceScope scope)
        {
            IDiscordSyncProvider syncProvider = scope.ServiceProvider.GetService<IDiscordSyncProvider>();
            await syncProvider.SyncDiscordTenant();
            return scope;
        }
    }
}
