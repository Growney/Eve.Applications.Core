using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.EveAuthTool.Standard.Discord.Configuration
{
    public class DiscordLinkParameterStore : GuidArgumentStore<DiscordLinkParameter>
    {
        private readonly ICommandController m_controller;
        public DiscordLinkParameterStore(ILogger<DiscordLinkParameterStore> logger,IControllerProvider provider,ITenantConfiguration configuration)
            :base(logger)
        {
            m_controller = provider.CreateController(configuration.ControllerKey);
        }
        public override DiscordLinkParameter RecallArguments(Guid guid)
        {
            return DiscordLinkParameter.Recall(m_controller, guid);
        }

        public override void StoreArguments(Guid guid, DiscordLinkParameter arguments)
        {
            arguments.LinkGuid = guid;
            arguments.Save(m_controller);
        }

        public override bool DiscardArguments(Guid guid)
        {
            DiscordLinkParameter.Discard(m_controller, guid);
            return true;
        }
    }
}
