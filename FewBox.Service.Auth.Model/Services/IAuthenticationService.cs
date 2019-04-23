using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Services
{
    public interface IRBACService
    {
        IList<string> GetModuleRoles(string moduleKey);
        IList<string> GetApiRoles(string controller, string action);
    }
}
