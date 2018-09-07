using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eve.EveAuthTool.Blazor.Client.Services
{
    public class AppState
    {
        public event Action OnDisplayRegistration;

        public void TriggerDisplayRegistration() => OnDisplayRegistration?.Invoke();
    }
}
