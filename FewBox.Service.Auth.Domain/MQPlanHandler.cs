using System;
using FewBox.SDK.Auth;
using FewBox.SDK.Core;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;

namespace FewBox.Service.Auth.Domain
{
    public class MQPlanHandler : IMQPlanHandler
    {
        private IUserRepository UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        public MQPlanHandler(IUserRepository userRepository, IRoleRepository roleRepository, IPrincipal_RoleRepository principal_RoleRepository)
        {
            this.UserRepository = userRepository;
            this.RoleRepository = roleRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
        }
        public Func<PlanMessage, bool> Handle()
        {
            return (planMessage) =>
            {
                User user = this.UserRepository.FindOneByEmail(planMessage.Customer.Email);
                Role proRole = this.RoleRepository.FindOneByCode($"{planMessage.Product.Name}Pro");
                switch (planMessage.Type)
                {
                    case PlanType.Pro:
                        this.Principal_RoleRepository.Save(new Principal_Role { PrincipalId = user.PrincipalId, RoleId = proRole.Id });
                        break;
                    case PlanType.Free:
                        Principal_Role principal_Role = this.Principal_RoleRepository.FindOneByPrincipalIdAndRoleId(user.PrincipalId, proRole.Id);
                        this.Principal_RoleRepository.Delete(principal_Role.Id);
                        break;
                    default:
                        break;
                }
                return true;
            };
        }
    }
}