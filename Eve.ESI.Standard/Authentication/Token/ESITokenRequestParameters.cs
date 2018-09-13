using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.ESI.Standard.Authentication.Token
{
    public class ESITokenRequestParameters
    {
        public ESIToken Token { get; set; }
        public IEnumerable<eESIEntityType> CreateFrom { get; set; }
    }
}
