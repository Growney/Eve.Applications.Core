using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.EveAuthTool.Standard.Discord.Service.Providers
{
    public class DiscordCommandContextAccessor : IDiscordCommandContextAccessor
    {
        public ICommandContext CommandContext { get; set; }
    }
}
