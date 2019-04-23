using System.Collections.Generic;
using FewBox.Core.Web.Security;
using Microsoft.AspNetCore.Http;

namespace FewBox.Service.Auth.Domain
{
    public class LocalAuthenticationService : IRemoteAuthenticationService
    {
        public IList<string> FindRolesByControllerAndAction(string controller, string action)
        {
            throw new System.NotImplementedException();
        }

        public IList<string> FindRolesByControllerAndAction(string controller, string action, IHeaderDictionary headers)
        {
            throw new System.NotImplementedException();
        }

        public IList<string> FindRolesByUserIdentity(object userIdentity)
        {
            throw new System.NotImplementedException();
        }

        public bool IsValid(string username, string password, out IList<string> roles)
        {
            throw new System.NotImplementedException();
        }
    }
}