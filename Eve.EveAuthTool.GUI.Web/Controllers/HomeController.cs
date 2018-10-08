using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Eve.EveAuthTool.Standard.Security.Middleware;
using Eve.EveAuthTool.GUI.Web.Controllers.Helpers;
using Gware.Standard.Web.Tenancy.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Eve.EveAuthTool.GUI.Web.Controllers
{
    public class HomeController : EveAuthBaseController<HomeController>
    {
        public HomeController(ILogger<HomeController> logger,IViewParameterProvider parameters)
            : base(logger,parameters)
        {

        }
        public IActionResult AboutMe()
        {
            return View();
        }

        public IActionResult AboutESA()
        {
            return View();
        }
        public IActionResult CreateNewTenant()
        {
            return View();
        }

        public IActionResult TenantNotFound()
        {
            return View();
        }

        [TenantRequired]
        public IActionResult Welcome()
        {
            if(CurrentAccount != null)
            {
                return View("WelcomeLoggedIn");
            }
            else
            {
                return View("WelcomeLoggedOut");
            }
        }
        
    }
}
