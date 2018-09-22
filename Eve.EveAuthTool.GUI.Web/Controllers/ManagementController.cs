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
            return View();
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
        [HttpPost]
        public IActionResult FormEntityRule(long entityID,int searchType,int index)
        {
            return PartialView("IndexedAuthRuleRelationship", new IndexAuthRuleRelationship(index, AuthRuleRelationship.GetStaticRelationship(entityID, SearchResults.SearchTypeToEntityType((eSearchEntity)searchType))));
        }
        [HttpPost]
        public IActionResult FormRoleRule(long roleID,int location, int index)
        {
            return PartialView("IndexedAuthRuleRelationship", new IndexAuthRuleRelationship(index, 
                AuthRuleRelationship.GetRelationshipRule(roleID,eESIEntityType.role,
                ESI.Standard.DataItem.Helper.RoleLocationToRelationship((ESI.Standard.DataItem.eRoleLocation)location))));
        }

        [HttpPost]
        public IActionResult FormStandingRule(long entityID, int entityType,int standingType, int index)
        {
            return PartialView("IndexedAuthRuleRelationship", new IndexAuthRuleRelationship(index,
                AuthRuleRelationship.GetRelationshipRule(entityID, (eESIEntityType)entityType,
                ESI.Standard.DataItem.Helper.StandingToRelationship((eESIStanding)standingType))));
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
                retVal.AddRange(titles.Data);
            }
            return PartialView("CorporationTitles", retVal);
        }
    }
}