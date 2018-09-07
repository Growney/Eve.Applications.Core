using Discord.Commands;
using System;

namespace Eve.EveAuthTool.Core.Discord
{
    public class DILinkedCommandService : CommandService
    {
        public IServiceProvider Provider { get; }

        public DILinkedCommandService(IServiceProvider provider)
        {
            Provider = provider;
        }
    }
}
