using Discord;
using Discord.Commands;
using System;

namespace Eve.EveAuthTool.Standard.Discord.Service
{
    public class ContextWithDI<T> : ICommandContext where T : ICommandContext
    {
        public Action ResetDIProvider;
        private T DiscordContext { get; }
        private IServiceProvider m_serviceProvider;
        public IServiceProvider Provider
        {
            get { return m_serviceProvider; }
            set { m_serviceProvider = value; ResetDIProvider?.Invoke(); }
        }

        public IDiscordClient Client => DiscordContext.Client;

        public IGuild Guild => DiscordContext.Guild;

        public IMessageChannel Channel => DiscordContext.Channel;

        public IUser User => DiscordContext.User;

        public IUserMessage Message => DiscordContext.Message;

        public ContextWithDI(IServiceProvider provider,T originalContext)
        {
            Provider = provider;
            DiscordContext = originalContext;
        }
    }
}
