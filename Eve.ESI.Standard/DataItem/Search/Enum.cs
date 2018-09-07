using System;

namespace Eve.ESI.Standard.DataItem.Search
{
    [Flags]
    public enum eSearchEntity
    {
        agent = 0x01,
        alliance = 0x02,
        character = 0x04,
        constellation = 0x08,
        corporation = 0x10,
        faction = 0x20,
        inventory_type = 0x40,
        region = 0x80,
        solar_system = 0x0100,
        station = 0x0200
    }
}
