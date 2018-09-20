using System;

namespace Eve.ESI.Standard.DataItem
{
    public enum eRoleLocation
    {
        General,
        Base,
        HQ,
        Other
    }
    public enum eESIRoleGroup : long
    {
        Accountant = eESIRole.Account_Take_1 | eESIRole.Account_Take_2 | eESIRole.Account_Take_3 | eESIRole.Account_Take_4 | eESIRole.Account_Take_5 | 
            eESIRole.Account_Take_6 | eESIRole.Account_Take_7 | eESIRole.Accountant | eESIRole.Junior_Accountant,
        Containers = eESIRole.Container_Take_1 | eESIRole.Container_Take_2 | eESIRole.Container_Take_3 | eESIRole.Container_Take_4 | eESIRole.Container_Take_5 |
            eESIRole.Container_Take_6 | eESIRole.Container_Take_7,
        Hangers = eESIRole.Hangar_Query_1 | eESIRole.Hangar_Query_2 | eESIRole.Hangar_Query_3 | eESIRole.Hangar_Query_4 | eESIRole.Hangar_Query_5 | eESIRole.Hangar_Query_6 | eESIRole.Hangar_Query_7 |
        eESIRole.Hangar_Take_1 | eESIRole.Hangar_Take_2 | eESIRole.Hangar_Take_3 | eESIRole.Hangar_Take_4 | eESIRole.Hangar_Take_5 | eESIRole.Hangar_Take_6 | eESIRole.Hangar_Take_7,
        Structures = eESIRole.Config_Equipment | eESIRole.Config_Starbase_Equipment | eESIRole.Starbase_Defense_Operator | eESIRole.Starbase_Fuel_Technician | eESIRole.Station_Manager,

        Misc = 0x7FFF_FFFF_FFFF_FFFF & ~Accountant & ~Containers & ~Hangers & ~Structures

    }

    [Flags]
    public enum eESIRole : long
    {
        None = 0x00,

        Account_Take_1 = 0x0000_0001,
        Account_Take_2 = 0x0000_0002,
        Account_Take_3 = 0x0000_0004,
        Account_Take_4 = 0x0000_0008,

        Account_Take_5 = 0x0000_0010,
        Account_Take_6 = 0x0000_0020,
        Account_Take_7 = 0x0000_0040,
        Accountant = 0x0000_0080,

        Auditor = 0x0000_0100,
        Communications_Officer = 0x0000_0200,
        Config_Equipment = 0x0000_0400,
        Config_Starbase_Equipment = 0x0000_0800,

        Container_Take_1 = 0x0000_1000,
        Container_Take_2 = 0x0000_2000,
        Container_Take_3 = 0x0000_4000,
        Container_Take_4 = 0x0000_8000,

        Container_Take_5 = 0x0001_0000,
        Container_Take_6 = 0x0002_0000,
        Container_Take_7 = 0x0004_0000,
        Contract_Manager = 0x0008_0000,

        Diplomat = 0x0010_0000,
        Director = 0x0020_0000,
        Factory_Manager = 0x0040_0000,
        Fitting_Manager = 0x0080_0000,

        Hangar_Query_1 = 0x0100_0000,
        Hangar_Query_2 = 0x0200_0000,
        Hangar_Query_3 = 0x0400_0000,
        Hangar_Query_4 = 0x0800_0000,

        Hangar_Query_5 = 0x1000_0000,
        Hangar_Query_6 = 0x2000_0000,
        Hangar_Query_7 = 0x4000_0000,
        Hangar_Take_1 = 0x8000_0000,

        Hangar_Take_2 = 0x0001_0000_0000,
        Hangar_Take_3 = 0x0002_0000_0000,
        Hangar_Take_4 = 0x0004_0000_0000,
        Hangar_Take_5 = 0x0008_0000_0000,

        Hangar_Take_6 = 0x0010_0000_0000,
        Hangar_Take_7 = 0x0020_0000_0000,
        Junior_Accountant = 0x0040_0000_0000,
        Personnel_Manager = 0x0080_0000_0000,

        Rent_Factory_Facility = 0x0100_0000_0000,
        Rent_Office = 0x0200_0000_0000,
        Rent_Research_Facility = 0x0400_0000_0000,
        Security_Officer = 0x0800_0000_0000,

        Starbase_Defense_Operator = 0x1000_0000_0000,
        Starbase_Fuel_Technician = 0x2000_0000_0000,
        Station_Manager = 0x4000_0000_0000,
        Terrestrial_Combat_Officer = 0x8000_0000_0000,

        Terrestrial_Logistics_Officer = 0x0001_0000_0000_0000,
        Trader = 0x0002_0000_0000_0000
    }
}
