using Eve.ESI.Standard;
using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Blazor.Client.Services
{
    public class SessionState
    {
        public bool IsAuthenticated { get; set; }
        public bool HasTenant { get; set; }
        public string TenantName { get; set; }
        public long? TenantEntityID { get; set; }
        public int? TenantEntityType { get; set; }

        private readonly AppState m_app;
        
        public SessionState(AppState app)
        {
            m_app = app;
        }

        public async Task Init()
        {
            SessionState sessionState = await m_app.GetJsonAsync<SessionState>("/api/registration/getsessionstate");
            CopyFrom(sessionState);
        }

        private void CopyFrom(SessionState state)
        {
            if(state != null)
            {
                IsAuthenticated = state.IsAuthenticated;
                HasTenant = state.HasTenant;
                TenantName = state.TenantName;
                TenantEntityID = state.TenantEntityID;
                TenantEntityType = state.TenantEntityType;
            }
            else
            {
                IsAuthenticated = false;
                HasTenant = false;
                TenantName = "null State";
            }
            
        }
    }
}
