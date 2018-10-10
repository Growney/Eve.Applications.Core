using Gware.Standard.Web;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service.Providers
{
    public interface IDiscordSyncProvider
    {
        Task SyncDiscordTenant();
    }
}
