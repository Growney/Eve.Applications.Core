using Eve.EveAuthTool.Standard;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.EveAuthTool.Core.Security.Middleware
{
    public class ViewParameterProvider : IViewParameterProvider
    {
        private readonly IHttpContextAccessor m_context;

        public ViewParameterPackage Package
        {
            get
            {
                return m_context.HttpContext.Features.Get<ViewParameterPackage>();
            }
        }

        public ViewParameterProvider(IHttpContextAccessor context)
        {
            m_context = context;
        }
    }
}
