using Eve.EveAuthTool.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.EveAuthTool.Standard.Security.Middleware
{
    public interface IViewParameterProvider
    {
        ViewParameterPackage Package { get; }
    }
}
