﻿using Eve.ESI.Standard.Authentication.Token;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Eve.ESI.Standard.Authentication
{
    public static class ESIScopeHelper
    {
        private static readonly string[] c_scopeStrings = new string[]
        {
            "esi-alliances.read_contacts.v1",
                "esi-assets.read_assets.v1",
                "esi-assets.read_corporation_assets.v1",
                "esi-bookmarks.read_character_bookmarks.v1",
                "esi-bookmarks.read_corporation_bookmarks.v1",
                "esi-calendar.read_calendar_events.v1",
                "esi-calendar.respond_calendar_events.v1",
                "esi-characters.read_agents_research.v1",
                "esi-characters.read_blueprints.v1",
                "esi-characters.read_contacts.v1",
                "esi-characters.read_corporation_roles.v1",
                "esi-characters.read_fatigue.v1",
                "esi-characters.read_fw_stats.v1",
                "esi-characters.read_loyalty.v1",
                "esi-characters.read_medals.v1",
                "esi-characters.read_notifications.v1",
                "esi-characters.read_opportunities.v1",
                "esi-characters.read_standings.v1",
                "esi-characters.read_titles.v1",
                "esi-characters.write_contacts.v1",
                "esi-characterstats.read.v1",
                "esi-clones.read_clones.v1",
                "esi-clones.read_implants.v1",
                "esi-contracts.read_character_contracts.v1",
                "esi-contracts.read_corporation_contracts.v1",
                "esi-corporations.read_blueprints.v1",
                "esi-corporations.read_contacts.v1",
                "esi-corporations.read_container_logs.v1",
                "esi-corporations.read_corporation_membership.v1",
                "esi-corporations.read_divisions.v1",
                "esi-corporations.read_facilities.v1",
                "esi-corporations.read_fw_stats.v1",
                "esi-corporations.read_medals.v1",
                "esi-corporations.read_outposts.v1",
                "esi-corporations.read_standings.v1",
                "esi-corporations.read_starbases.v1",
                "esi-corporations.read_structures.v1",
                "esi-corporations.read_titles.v1",
                "esi-corporations.track_members.v1",
                "esi-fittings.read_fittings.v1",
                "esi-fittings.write_fittings.v1",
                "esi-fleets.read_fleet.v1",
                "esi-fleets.write_fleet.v1",
                "esi-industry.read_character_jobs.v1",
                "esi-industry.read_character_mining.v1",
                "esi-industry.read_corporation_jobs.v1",
                "esi-industry.read_corporation_mining.v1",
                "esi-killmails.read_corporation_killmails.v1",
                "esi-killmails.read_killmails.v1",
                "esi-location.read_location.v1",
                "esi-location.read_online.v1",
                "esi-location.read_ship_type.v1",
                "esi-mail.organize_mail.v1",
                "esi-mail.read_mail.v1",
                "esi-mail.send_mail.v1",
                "esi-markets.read_character_orders.v1",
                "esi-markets.read_corporation_orders.v1",
                "esi-markets.structure_markets.v1",
                "esi-planets.manage_planets.v1",
                "esi-planets.read_customs_offices.v1",
                "esi-search.search_structures.v1",
                "esi-skills.read_skillqueue.v1",
                "esi-skills.read_skills.v1",
                "esi-ui.open_window.v1",
                "esi-ui.write_waypoint.v1",
                "esi-universe.read_structures.v1",
                "esi-wallet.read_character_wallet.v1",
                "esi-wallet.read_corporation_wallets.v1"
        };
        
        public static eESIScope[] Merge(IEnumerable<eESIScope> sourceA, IEnumerable<eESIScope> sourceB)
        {
            HashSet<eESIScope> retVal = new HashSet<eESIScope>();
            if(sourceA != null)
            {
                foreach (eESIScope scope in sourceA)
                {
                    if (!retVal.Contains(scope))
                    {
                        retVal.Add(scope);
                    }
                }
            }
            
            if(sourceB != null)
            {
                foreach (eESIScope scope in sourceB)
                {
                    if (!retVal.Contains(scope))
                    {
                        retVal.Add(scope);
                    }
                }
            }
            

            return retVal.ToArray();
        }

        public static string GetScopeString(params eESIScope[] scopes)
        {
            return GetScopeString(scopes.ToList());
        }
        public static string GetScopeString(IEnumerable<eESIScope> scopes)
        {
            StringBuilder retVal = new StringBuilder();

            int counter = 0;
            foreach (eESIScope scope in scopes)
            {
                int scopeInt = (int)scope;
                if (scopeInt > 0 && scopeInt < c_scopeStrings.Length)
                {
                    if (counter != 0)
                    {
                        retVal.Append(" ");
                    }

                    retVal.Append(c_scopeStrings[scopeInt]);

                    counter++;
                }
            }

            return retVal.ToString();
        }
        public static string GetScopeEnumString(string scope)
        {
            return scope.Replace('-', '_').Replace('.', '_');
        }
        

        
    }
}
