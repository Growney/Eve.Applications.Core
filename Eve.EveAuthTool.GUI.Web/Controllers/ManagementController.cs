using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eve.ESI.Standard;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.ESI.Standard.DataItem.Corporation;
using Eve.ESI.Standard.DataItem.Search;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.GUI.Web.Models.Management;
using Eve.EveAuthTool.Standard.Security.Rules;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Eve.EveAuthTool.Standard.Discord.Configuration.Tenant;
using Microsoft.Extensions.Logging;

namespace Eve.EveAuthTool.GUI.Web.Controllers
{
    [Authorize]
    [TenantRequired]
    public class ManagementController : Helpers.EveAuthBaseController<ManagementController>
    {
        public ManagementController(ILogger<ManagementController> logger,IViewParameterProvider parameters)
            :base(logger, parameters)
        {

        }
        [Authorize(Roles="Manage")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Manage")]
        public IActionResult Rules()
        {
            return View(AuthRule.InOrder(TenantController));
        }

        [Authorize(Roles = "Manage")]
        public IActionResult Roles()
        {
            return View(RoleModel.CreateFrom(Role.All(TenantController), DiscordRoleConfiguration.All(TenantController)));
        }

        [HttpGet]
        [Authorize(Roles = "Manage")]
        public IActionResult EditRule(long ruleID)
        {
            AuthRule rule = AuthRule.Get<AuthRule>(TenantController, ruleID);
            if(rule == null)
            {
                rule = new AuthRule()
                {
                    Name = "New Rule"
                };
            }
            
            return View(rule);
        }
        [HttpGet]
        [Authorize(Roles = "Manage")]
        public IActionResult EditRole(long roleID)
        {
            //role.Save(TenantController);
            Role role = Role.Get<Role>(TenantController, roleID);
            if(role == null)
            {
                role = new Role()
                {
                    Name = "New Role"
                };
            }

            EditRoleModel model = RoleModel.CreateFrom<EditRoleModel>(role, DiscordRoleConfiguration.Get<DiscordRoleConfiguration>(TenantController, role.DiscordRoleConfigurationID));
            model.AllConfigurations = Models.Discord.DiscordConfiguration.CreateFrom<Models.Discord.DiscordConfiguration>(DiscordRoleConfiguration.All(TenantController),new List<Models.Discord.DiscordRole>(),new List<Models.Discord.DiscordInvite>());
            return View("EditRole", model);
        }
        [HttpPost]
        [Authorize(Roles = "Manage")]
        public IActionResult EditRole(RoleModel role)
        {
            Role systemRole = Role.Get<Role>(TenantController, role.Id);
            if(systemRole == null)
            {
                systemRole = new Role();
            }
            systemRole.Name = role.Name;
            systemRole.Permissions = Standard.Security.eRulePermission.None;
            if (role.Register)
            {
                systemRole.Permissions |= Standard.Security.eRulePermission.Register;
            }
            if (role.Manage)
            {
                systemRole.Permissions |= Standard.Security.eRulePermission.Manage;
            }
            systemRole.DiscordRoleConfigurationID = role.DiscordRoleConfigurationID;
            systemRole.Save(TenantController);
            return RedirectToAction("Roles");
        }
        
        [HttpPost]
        [Authorize(Roles = "Manage")]
        public IActionResult EditRule(AuthRule rule)
        {
            rule.Save(TenantController);
            return RedirectToAction("Rules");
        }

        [HttpPost]
        [Authorize(Roles = "Manage")]
        public IActionResult DeleteRule(long ruleID)
        {
            AuthRule.Delete<AuthRule>(TenantController, ruleID);
            return RedirectToAction("Rules");
        }
        [HttpGet]
        public IActionResult FormEntityRule(long entityID,int searchType,int index)
        {
            return PartialView("IndexedAuthRuleRelationship", new IndexAuthRuleRelationship(index, AuthRuleRelationship.GetStaticRelationship(entityID, SearchResults.SearchTypeToEntityType((eSearchEntity)searchType))));
        }
        [HttpGet]
        public IActionResult FormRoleRule(long roleID,int location, int index)
        {
            return PartialView("IndexedAuthRuleRelationship", new IndexAuthRuleRelationship(index, 
                AuthRuleRelationship.GetRelationshipRule(roleID,eESIEntityType.role,
                ESI.Standard.DataItem.Helper.RoleLocationToRelationship((ESI.Standard.DataItem.eRoleLocation)location))));
        }
        [HttpGet]
        public IActionResult FormStandingRule(long entityID, int entityType,int standingType, int index)
        {
            return PartialView("IndexedAuthRuleRelationship", new IndexAuthRuleRelationship(index,
                AuthRuleRelationship.GetRelationshipRule(entityID, (eESIEntityType)entityType,
                ESI.Standard.DataItem.Helper.StandingToRelationship((eESIStanding)standingType))));
        }

        [HttpGet]
        public IActionResult FormTitleRole(int corporationID,int titleID, int index)
        {
            return PartialView("IndexedAuthRuleRelationship", new IndexAuthRuleRelationship(index,
                AuthRuleRelationship.GetRelationshipRule(titleID,eESIEntityType.title,eESIEntityRelationship.Title,corporationID,eESIEntityType.corporation)));
        }
        [HttpGet]
        [Authorize(Roles = "Manage")]
        public async Task<IActionResult> CorporationTitles(int corporationID)
        {
            List<CorporationTitle> retVal = new List<CorporationTitle>();
            AuthenticatedEntity entity = AuthenticatedEntity.GetForEntity(ESIConfiguration, TenantController, Cache, PublicDataProvider, corporationID, eESIEntityType.corporation);
            if(entity != null)
            {
                ESICollectionCallResponse<CorporationTitle> titles = await entity.GetCorporationTitles();
                retVal.AddRange(titles.Data.Where(x => !string.IsNullOrWhiteSpace(x.Name)));
            }
            return PartialView("CorporationTitles", retVal);
        }
    }
}