using System;
using System.Collections.Generic;
using FewBox.Core.Web.Security;
using FewBox.Service.Auth.Model.Repositories;

namespace FewBox.Service.Auth.Domain
{
    public class LocalAuthService : IAuthService
    {
        private IApiRepository ApiRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IRole_SecurityObjectRepository  Role_SecurityObjectRepository { get; set; } 
        private IUserRepository UserRepository { get; set; }
        public LocalAuthService(IApiRepository apiRepository, IRoleRepository roleRepository, 
        IRole_SecurityObjectRepository role_SecurityObjectRepository, IUserRepository userRepository)
        {
            this.ApiRepository = apiRepository;
            this.RoleRepository = roleRepository;
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
            this.UserRepository = userRepository;
        }

        public IList<string> FindRoles(string service, string controller, string action)
        {
            var roles = new List<string>();
            var api = this.ApiRepository.FindOneByServiceAndControllerAndAction(service, controller, action);
            if(api!=null)
            {
                var role_SecurityObjects = this.Role_SecurityObjectRepository.FindAllBySecurityId(api.SecurityObjectId);
                if(role_SecurityObjects != null)
                {
                    foreach(var role_SecurityObject in role_SecurityObjects)
                    {
                        var role = this.RoleRepository.FindOne(role_SecurityObject.RoleId);
                        roles.Add(role.Code);
                    }
                }
            }
            return roles;
        }

        public IList<string> FindRoles(string method)
        {
            throw new NotImplementedException();
        }
    }
}