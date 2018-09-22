using Gware.Standard.Data;
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

        [EnumDisplay("Account 1 Take")]
        Account_Take_1 = 0x0000_0001,
        [EnumDisplay("Account 2 Take")]
        Account_Take_2 = 0x0000_0002,
        [EnumDisplay("Account 3 Take")]
        Account_Take_3 = 0x0000_0004,
        [EnumDisplay("Account 4 Take")]
        Account_Take_4 = 0x0000_0008,

        [EnumDisplay("Account 5 Take")]
        Account_Take_5 = 0x0000_0010,
        [EnumDisplay("Account 6 Take")]
        Account_Take_6 = 0x0000_0020,
        [EnumDisplay("Account 7 Take")]
        Account_Take_7 = 0x0000_0040,
        Accountant = 0x0000_0080,

        Auditor = 0x0000_0100,
        [EnumDisplay("Communications Officer")]
        Communications_Officer = 0x0000_0200,
        [EnumDisplay("Config Equipment")]
        Config_Equipment = 0x0000_0400,
        [EnumDisplay("Config Starbase Equipment")]
        Config_Starbase_Equipment = 0x0000_0800,

        [EnumDisplay("Container 1 Take")]
        Container_Take_1 = 0x0000_1000,
        [EnumDisplay("Container 2 Take")]
        Container_Take_2 = 0x0000_2000,
        [EnumDisplay("Container 3 Take")]
        Container_Take_3 = 0x0000_4000,
        [EnumDisplay("Container 4 Take")]
        Container_Take_4 = 0x0000_8000,

        [EnumDisplay("Container 5 Take")]
        Container_Take_5 = 0x0001_0000,
        [EnumDisplay("Container 6 Take")]
        Container_Take_6 = 0x0002_0000,
        [EnumDisplay("Container 7 Take")]
        Container_Take_7 = 0x0004_0000,
        [EnumDisplay("Contract Manager")]
        Contract_Manager = 0x0008_0000,

        Diplomat = 0x0010_0000,
        Director = 0x0020_0000,
        [EnumDisplay("Factory Manager")]
        Factory_Manager = 0x0040_0000,
        [EnumDisplay("Fitting Manager")]
        Fitting_Manager = 0x0080_0000,


        [EnumDisplay("Hanger 1 Query")]
        Hangar_Query_1 = 0x0100_0000,
        [EnumDisplay("Hanger 2 Query")]
        Hangar_Query_2 = 0x0200_0000,
        [EnumDisplay("Hanger 3 Query")]
        Hangar_Query_3 = 0x0400_0000,
        [EnumDisplay("Hanger 4 Query")]
        Hangar_Query_4 = 0x0800_0000,

        [EnumDisplay("Hanger 5 Query")]
        Hangar_Query_5 = 0x1000_0000,
        [EnumDisplay("Hanger 6 Query")]
        Hangar_Query_6 = 0x2000_0000,
        [EnumDisplay("Hanger 7 Query")]
        Hangar_Query_7 = 0x4000_0000,
        [EnumDisplay("Hanger 1 Take")]
        Hangar_Take_1 = 0x8000_0000,

        [EnumDisplay("Hanger 2 Take")]
        Hangar_Take_2 = 0x0001_0000_0000,
        [EnumDisplay("Hanger 3 Take")]
        Hangar_Take_3 = 0x0002_0000_0000,
        [EnumDisplay("Hanger 4 Take")]
        Hangar_Take_4 = 0x0004_0000_0000,
        [EnumDisplay("Hanger 5 Take")]
        Hangar_Take_5 = 0x0008_0000_0000,

        [EnumDisplay("Hanger 6 Take")]
        Hangar_Take_6 = 0x0010_0000_0000,
        [EnumDisplay("Hanger 7 Take")]
        Hangar_Take_7 = 0x0020_0000_0000,
        [EnumDisplay("Junior Accountant")]
        Junior_Accountant = 0x0040_0000_0000,
        [EnumDisplay("Personnel Manager")]
        Personnel_Manager = 0x0080_0000_0000,

        [EnumDisplay("Rent Factory Facility")]
        Rent_Factory_Facility = 0x0100_0000_0000,
        [EnumDisplay("Rent Office")]
        Rent_Office = 0x0200_0000_0000,
        [EnumDisplay("Rent Research Facility")]
        Rent_Research_Facility = 0x0400_0000_0000,
        [EnumDisplay("Security Officer")]
        Security_Officer = 0x0800_0000_0000,

        [EnumDisplay("Starbase Defense Operator")]
        Starbase_Defense_Operator = 0x1000_0000_0000,
        [EnumDisplay("Starbase Fuel Technician")]
        Starbase_Fuel_Technician = 0x2000_0000_0000,
        [EnumDisplay("Station Manager")]
        Station_Manager = 0x4000_0000_0000,
        [EnumDisplay("Terrestrial Combat Officer")]
        Terrestrial_Combat_Officer = 0x8000_0000_0000,
        
        [EnumDisplay("Terrestrial Logistics Officer")]
        Terrestrial_Logistics_Officer = 0x0001_0000_0000_0000,
        Trader = 0x0002_0000_0000_0000
    }
}
