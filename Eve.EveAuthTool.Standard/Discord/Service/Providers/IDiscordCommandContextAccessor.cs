using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.EveAuthTool.Standard.Discord.Service.Providers
{
    public interface IDiscordCommandContextAccessor
    {
        ICommandContext CommandContext { get; set; }
    }
}
