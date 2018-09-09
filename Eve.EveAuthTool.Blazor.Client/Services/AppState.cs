using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.Services;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Blazor.Client.Services
{
    public class AppState 
    {
        public bool IsAuthenticated { get; set; }
        public event Action OnDisplayRegistration;

        private readonly IUriHelper m_helper;
        private readonly HttpClient m_client;

        public AppState(HttpClient client,IUriHelper helper)
        {
            m_helper = helper;
            m_client = client;
        }
        public void TriggerDisplayRegistration() => OnDisplayRegistration?.Invoke();

        public Task NavigateExternally(string url)
        {
            return JSRuntime.Current.InvokeAsync<string>(
                "externalJSFunctions.goToExternalSite",
                url);
        }

        public Uri GetTenantedUrl(string href)
        {
            return m_helper.ToAbsoluteUri(href);
        }

        public Task<string> GetStringAsync(string href)
        {
            return m_client.GetStringAsync(GetTenantedUrl(href));
        }

        public Task<T> GetJsonAsync<T>(string href)
        {
            return m_client.GetJsonAsync<T>(GetTenantedUrl(href).ToString());
        }
    }
}
