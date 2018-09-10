using Eve.EveAuthTool.Core.Helpers;
using Eve.EveAuthTool.Core.Security.Middleware;
using Eve.EveAuthTool.Standard;
using Eve.EveAuthTool.Standard.Security;
using Gware.Standard.Web.Tenancy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.EveAuthTool.Core.Security.Routing
{
    public class ViewParameterResolverFilter : TypeFilterAttribute
    {
        public ViewParameterResolverFilter() : base(typeof(ViewParameterResolverFilterImpl))
        {

        }
        private class ViewParameterResolverFilterImpl : IActionFilter
        {
            private readonly IControllerParameters m_controllerParameters;

            public ViewParameterResolverFilterImpl(IControllerParameters parameters)
            {
                m_controllerParameters = parameters;

            }

            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                context.HttpContext.Features.Set<ViewParameterPackage>(
                    m_controllerParameters.CreateViewParameters(context.HttpContext));
            }
        }
    }
}