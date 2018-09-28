using Discord.Commands;
using System;

namespace Eve.EveAuthTool.Standard.Discord.Service
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
