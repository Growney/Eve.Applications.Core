﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eve.ESI.Standard;
using Eve.ESI.Standard.DataItem.Search;
using Eve.EveAuthTool.Core.Security.Middleware;
using Eve.EveAuthTool.GUI.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Eve.EveAuthTool.GUI.Web.Controllers
{
    public class SearchController : Helpers.EveAuthBaseController
    {

        public SearchController(IViewParameterProvider provider)
            :base(provider)
        {

        }

        public async Task<IActionResult> SearchEntities(string query, bool oldData , string resultID)
        {
            ESICallResponse<SearchResults> result = await PublicDataProvider.Search(query, eSearchEntity.alliance | eSearchEntity.corporation | eSearchEntity.character | eSearchEntity.faction);
            if (result.HasData)
            {
                return PartialView("SearchResult", new SearchResult(PublicDataProvider, result.Data,oldData, resultID));
            }
            return new JsonResult("No Results");
        }
    }
}