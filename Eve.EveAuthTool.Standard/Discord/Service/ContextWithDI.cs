using Discord;
using Discord.Commands;
using System;

namespace Eve.EveAuthTool.Standard.Discord.Service
{
    public class ContextWithDI<T> : ICommandContext where T : ICommandContext
    {
        private T DiscordContext { get; }
        public IServiceProvider Provider { get; }

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
