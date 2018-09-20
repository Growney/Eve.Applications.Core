using Gware.Standard.Data;

namespace Eve.ESI.Standard
{
    public enum eESIEntityType : int
    {
        character = 0,
        corporation = 1,
        alliance = 2,
        faction = 3,

        role = 4,
        fleetFC = 5,
    }
    
    public enum eESIEntityRelationship : int
    {
        None = 0,

        //Things that entities are in
        
        Fleet = 101,
        [EnumDisplayAttribute("Neutral Standing")]
        NeutralStanding = 102,
        [EnumDisplayAttribute("Terrible Standing")]
        TerribleStanding = 103,
        [EnumDisplayAttribute("Bad Standing")]
        BadStanding = 104,
        [EnumDisplayAttribute("Good Standing")]
        GoodStanding = 105,
        [EnumDisplayAttribute("Excellent Standing")]
        ExcellentStanding = 107,
        WarEnemy = 108,
        WarAlly = 109,

        //Things that entities have
        Roles = 201,
        [EnumDisplayAttribute("Roles At Base")]
        RolesAtBase = 202,
        [EnumDisplayAttribute("Roles At HQ")]
        RolesAtHQ = 203,
        [EnumDisplayAttribute("Roles At Other")]
        RolesAtOther =  204,
        Title = 205,
        Capital = 206,
        SuperCapital = 207,

        //Things that entities are read straight from a property
        Character = 301,
        Corporation = 302,
        Alliance = 303,
        Faction = 304,

    }
    //In compares to and from
    public enum eESIEntityRelationshipOperatorMask : int
    {
        //Calculated from the point of view from the query entity
       HasFrom = 100,
       //Calculated from the point of view of the character
       Has = 200,
       //Match on properties
       In = 300,
    }

    public enum eESIRelationshipDynamicEntity
    {
        CurrentTenant,
        CurrentUser,
    }
    public enum eESIDataSource
    {
        tranquility,
        singularity,
    }
    public enum eESILanguage
    {
        de,
        en_us,
        fr,
        ja,
        ru,
        zh,
    }
    public enum eESIScope 
    {
        esi_alliances_read_contacts_v1,

        esi_assets_read_assets_v1,
        esi_assets_read_corporation_assets_v1,

        esi_bookmarks_read_character_bookmarks_v1,
        esi_bookmarks_read_corporation_bookmarks_v1,

        esi_calendar_read_calendar_events_v1,
        esi_calendar_respond_calendar_events_v1,

        esi_characters_read_agents_research_v1,
        esi_characters_read_blueprints_v1,
        esi_characters_read_contacts_v1,
        esi_characters_read_corporation_roles_v1,
        esi_characters_read_fatigue_v1,
        esi_characters_read_fw_stats_v1,
        esi_characters_read_loyalty_v1,
        esi_characters_read_medals_v1,
        esi_characters_read_notifications_v1,
        esi_characters_read_opportunities_v1,
        esi_characters_read_standings_v1,
        esi_characters_read_titles_v1,

        esi_characters_write_contacts_v1,

        esi_characterstats_read_v1,

        esi_clones_read_clones_v1,
        esi_clones_read_implants_v1,

        esi_contracts_read_character_contracts_v1,
        esi_contracts_read_corporation_contracts_v1,

        esi_corporations_read_blueprints_v1,
        esi_corporations_read_contacts_v1,
        esi_corporations_read_container_logs_v1,
        esi_corporations_read_corporation_membership_v1,
        esi_corporations_read_divisions_v1,
        esi_corporations_read_facilities_v1,
        esi_corporations_read_fw_stats_v1,
        esi_corporations_read_medals_v1,
        esi_corporations_read_outposts_v1,
        esi_corporations_read_standings_v1,
        esi_corporations_read_starbases_v1,
        esi_corporations_read_structures_v1,
        esi_corporations_read_titles_v1,
        esi_corporations_track_members_v1,

        esi_fittings_read_fittings_v1,
        esi_fittings_write_fittings_v1,

        esi_fleets_read_fleet_v1 ,
        esi_fleets_write_fleet_v1,

        esi_industry_read_character_jobs_v1,
        esi_industry_read_character_mining_v1,
        esi_industry_read_corporation_jobs_v1,
        esi_industry_read_corporation_mining_v1,

        esi_killmails_read_corporation_killmails_v1,
        esi_killmails_read_killmails_v1,

        esi_location_read_location_v1,
        esi_location_read_online_v1,
        esi_location_read_ship_type_v1,

        esi_mail_organize_mail_v1,
        esi_mail_read_mail_v1,
        esi_mail_send_mail_v1,

        esi_markets_read_character_orders_v1,
        esi_markets_read_corporation_orders_v1,
        esi_markets_structure_markets_v1,

        esi_planets_manage_planets_v1,
        esi_planets_read_customs_offices_v1,

        esi_search_search_structures_v1,

        esi_skills_read_skillqueue_v1,
        esi_skills_read_skills_v1,

        esi_ui_open_window_v1,
        esi_ui_write_waypoint_v1,

        esi_universe_read_structures_v1,

        esi_wallet_read_character_wallet_v1,
        esi_wallet_read_corporation_wallets_v1,
    }
}
