using System;
using System.Collections.Generic;
using System.Linq;
using FewBox.Core.Web.Security;
using FewBox.Service.Auth.Model.Repositories;
using Microsoft.AspNetCore.Http;

namespace FewBox.Service.Auth.Domain
{
    public class LocalAuthenticationService : IRemoteAuthenticationService
    {
        private IUserRepository UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        public LocalAuthenticationService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            this.UserRepository = userRepository;
            this.RoleRepository = roleRepository;
        }

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

        public bool IsValid(string username, string password, string userType, out IList<string> roles)
        {
            bool isValid = false;
            roles = null;
            if(userType == "Form")
            {
                Guid userId;
                isValid = this.UserRepository.IsPasswordValid(username, password, out userId);
                roles = (from role in this.RoleRepository.FindAllByUserId(userId)
                        select role.Code).ToList();
            }
            return isValid;
        }
    }
}