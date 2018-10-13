using Gware.Standard.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.EveAuthTool.Standard.Configuration
{
    public enum eUserSetting : long
    {
        [ConfigurationDefault(false)]
        UpdateDiscordName,

        [ConfigurationDefault(false)]
        UpdateDiscordImage,
    }
}
