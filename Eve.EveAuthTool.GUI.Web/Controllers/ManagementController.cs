using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eve.ESI.Standard;
using Eve.ESI.Standard.AuthenticatedData;
using Eve.ESI.Standard.DataItem.Corporation;
using Eve.ESI.Standard.DataItem.Search;
using Eve.EveAuthTool.Core.Security.Middleware;
using Eve.EveAuthTool.GUI.Web.Models.Management;
using Eve.EveAuthTool.Standard.Security.Rules;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eve.EveAuthTool.GUI.Web.Controllers
{
    [Authorize]
    [TenantRequired]
    public class ManagementController : Helpers.EveAuthBaseController
    {
        public ManagementController(IViewParameterProvider parameters)
            :base(parameters)
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
            return View(RoleModel.CreateFrom(Role.All(TenantController)));
        }

        [HttpGet]
        [Authorize(Roles = "Manage")]
        public IActionResult EditRule(long ruleID)
        {
            AuthRule rule = AuthRule.Get<AuthRule>(TenantController, ruleID);
            if(rule.Id != ruleID)
            {
                rule.Name = "New Rule";
            }
            
            return View(rule);
        }
        [HttpGet]
        [Authorize(Roles = "Manage")]
        public IActionResult EditRole(long roleID)
        {
            //role.Save(TenantController);
            Role role = Role.Get<Role>(TenantController, roleID);
            if(role.Id != roleID)
            {
                role.Name = "New Role";
            }
            return View("EditRole", RoleModel.CreateFrom(role));
        }
        [HttpPost]
        [Authorize(Roles = "Manage")]
        public IActionResult EditRole(RoleModel role)
        {
            Role systemRole = Role.Get<Role>(TenantController, role.Id);
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