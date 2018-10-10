using Discord;
using Discord.Commands;
using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Client;
using Eve.ESI.Standard.Authentication.Configuration;
using Eve.EveAuthTool.Standard.Discord.Configuration;
using Eve.EveAuthTool.Standard.Discord.Service.Providers;
using Eve.EveAuthTool.Standard.Helpers;
using Eve.Static.Standard;
using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Standard.Discord.Service.Module
{
    public abstract class EveAuthToolModuleBase<T> : ModuleBase<ContextWithDI<SocketCommandContext>> where T : EveAuthToolModuleBase<T>
    {
        protected IScopeParameters ScopeParameters { get; private set; }
        protected ISingleParameters SingleParameters { get; private set; }
        protected IDiscordBot Bot { get; private set; }
        protected IArgumentsStore<DiscordLinkParameter> DiscordLinkStore { get; private set; }
        protected ILogger<T> Logger { get; private set; }
        protected IDiscordLinkProvider LinkProvider { get; private set; }
        protected IServiceScopeFactory ScopeFactory { get; private set; }
        public EveAuthToolModuleBase()
        {

        }
        protected override void BeforeExecute(CommandInfo command)
        {
            Context.ResetDIProvider += SetDIContextItems;
            SetDIContextItems();
        }

        protected void SetDIContextItems()
        {
            ScopeParameters = Context.Provider.GetService(typeof(IScopeParameters)) as IScopeParameters;
            SingleParameters = Context.Provider.GetService(typeof(ISingleParameters)) as ISingleParameters;
            Bot = Context.Provider.GetService(typeof(IDiscordBot)) as IDiscordBot;
            DiscordLinkStore = Context.Provider.GetService(typeof(IArgumentsStore<DiscordLinkParameter>)) as IArgumentsStore<DiscordLinkParameter>;
            Logger = Context.Provider.GetService(typeof(ILogger<T>)) as ILogger<T>;
            LinkProvider = Context.Provider.GetService(typeof(IDiscordLinkProvider)) as IDiscordLinkProvider;
            ScopeFactory = Context.Provider.GetService(typeof(IServiceScopeFactory)) as IServiceScopeFactory;
        }
    }
}
