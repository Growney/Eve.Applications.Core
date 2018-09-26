using Eve.ESI.Standard;
using Eve.ESI.Standard.Account;
using Eve.ESI.Standard.Authentication.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.EveAuthTool.Standard.Security.Middleware
{
    public class ScopeGroupProvider : IScopeGroupProvider
    {

        public static readonly ScopeGroupCollection CharacterScopes = new ScopeGroupCollection(
            new ScopeGroup(0x01, "Assets", "Read your assets and add them to your total wealth statistics.", eESIScope.esi_assets_read_assets_v1),
            new ScopeGroup(0x02, "Finance", "Read all things finance including your wallet, contracts and market orders.", eESIScope.esi_wallet_read_character_wallet_v1, eESIScope.esi_contracts_read_character_contracts_v1, eESIScope.esi_markets_read_character_orders_v1),
            new ScopeGroup(0x04, "Fittings", "Read and write your fittings.", eESIScope.esi_fittings_read_fittings_v1),
            new ScopeGroup(0x08, "Industry", "Read your industry jobs, blueprints and mining stats. Providing the ability to include them in your total wealth statistics", eESIScope.esi_industry_read_character_jobs_v1, eESIScope.esi_industry_read_character_mining_v1, eESIScope.esi_characters_read_blueprints_v1),
            new ScopeGroup(0x10, "Skills", "Read all things skill related including active skill queue, clones,implants and all skills", eESIScope.esi_skills_read_skillqueue_v1, eESIScope.esi_skills_read_skills_v1, eESIScope.esi_clones_read_clones_v1, eESIScope.esi_clones_read_implants_v1),
            new ScopeGroup(0x20, "Social", "Read your contacts and your standings", eESIScope.esi_characters_read_contacts_v1, eESIScope.esi_characters_read_standings_v1),
            new ScopeGroup(0x40, "Stats", "Read your character statistics including jump fatigue", eESIScope.esi_characters_read_fatigue_v1, eESIScope.esi_characterstats_read_v1),
            new ScopeGroup(0x80, "Security", "Read your characters roles", eESIScope.esi_characters_read_corporation_roles_v1, eESIScope.esi_characters_read_titles_v1),
            new ScopeGroup(0x100, "Fleet", "Read your fleet information to allow relationship information", eESIScope.esi_fleets_read_fleet_v1)
            );
        public static readonly ScopeGroupCollection CorporationScopes = new ScopeGroupCollection(
            new ScopeGroup(0x01, "Roles/Members", "View the roles assigned in your corporation to allow permissions to be set automatically", eESIScope.esi_corporations_read_corporation_membership_v1, eESIScope.esi_corporations_read_titles_v1, eESIScope.esi_characters_read_corporation_roles_v1,eESIScope.esi_corporations_read_titles_v1),
            new ScopeGroup(0x02, "Standings", "Access your corporation standings to allow allies permissions to be set automatically", eESIScope.esi_corporations_read_contacts_v1)
            );
        public static readonly ScopeGroupCollection AllianceScopes = new ScopeGroupCollection(
            new ScopeGroup(0x01, "Roles/Members", "View the roles assigned in your corporation to allow permissions to be set automatically", eESIScope.esi_corporations_read_corporation_membership_v1, eESIScope.esi_corporations_read_titles_v1, eESIScope.esi_characters_read_corporation_roles_v1, eESIScope.esi_corporations_read_titles_v1),
            new ScopeGroup(0x02, "Standings", "Access your corporation standings to allow allies permissions to be set automatically", eESIScope.esi_corporations_read_contacts_v1)
            );

        private readonly IViewParameterProvider m_parameters;

        public ScopeGroupProvider(IViewParameterProvider parameters)
        {
            m_parameters = parameters;
        }

        private ulong GetCharacterRequired()
        {
            return (m_parameters.Package.IsTenant ? CharacterScopes.GetValue("Security") : 0);
        }

        private ulong GetCorporationRequired()
        {
            return CorporationScopes.GetValue("Roles/Members");
        }

        private ulong GetAllianceRequired()
        {
            return AllianceScopes.GetValue("Roles/Members");
        }

        public ScopeGroup[] GetAllianceScopesGroups()
        {
            return AllianceScopes.GetScopeGroups(GetAllianceRequired());
        }

        public ScopeGroup[] GetCharacterScopeGroups()
        {
            return CharacterScopes.GetScopeGroups(GetCharacterRequired());
        }

        public ScopeGroup[] GetCorporationScopesGroups()
        {
            return CorporationScopes.GetScopeGroups(GetCorporationRequired());
        }

        public eESIScope[] GetAllianceScopes(uint[] selected)
        {
            return AllianceScopes.GetScopes(selected, GetAllianceRequired());
        }

        public eESIScope[] GetCharacterScopes(uint[] selected)
        {
            return CharacterScopes.GetScopes(selected,GetCharacterRequired());
        }

        public eESIScope[] GetCorporationScopes(uint[] selected)
        {
            return CorporationScopes.GetScopes(selected,GetCorporationRequired());
        }

        public eESIScope[] GetAllianceScopes()
        {
            return AllianceScopes.GetScopes(uint.MaxValue, GetAllianceRequired());
        }

        public eESIScope[] GetCharacterScopes()
        {
            return CharacterScopes.GetScopes(uint.MaxValue, GetCharacterRequired());
        }

        public eESIScope[] GetCorporationScopes()
        {
            return CorporationScopes.GetScopes(uint.MaxValue, GetCorporationRequired());
        }
    }
}
