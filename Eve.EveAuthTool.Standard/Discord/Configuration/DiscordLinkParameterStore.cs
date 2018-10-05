using Gware.Standard.Collections.Generic;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.EveAuthTool.Standard.Discord.Configuration
{
    public class DiscordLinkParameterStore : GuidArgumentStore<DiscordLinkParameter>
    {
        private readonly ICommandController m_controller;
        public DiscordLinkParameterStore(IControllerProvider provider,ITenantConfiguration configuration)
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

        public override void DiscardArguments(Guid guid)
        {
            DiscordLinkParameter.Discard(m_controller, guid);
        }
    }
}
