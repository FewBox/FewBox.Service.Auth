using System;
using System.Collections.Generic;
using System.Text;
using FewBox.SDK.Auth;
using FewBox.SDK.Core;
using FewBox.SDK.Mail;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using Microsoft.Extensions.Logging;

namespace FewBox.Service.Auth.Domain
{
    public class MQPlanHandler : IMQPlanHandler
    {
        private ITenantRepository TenantRepository { get; set; }
        private IPrincipalRepository PrincipalRepository { get; set; }
        private IUserRepository UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private IMailService MailService { get; set; }
        private ILogger Logger { get; set; }
        public MQPlanHandler(ITenantRepository tenantRepository, IPrincipalRepository principalRepository, IUserRepository userRepository, IRoleRepository roleRepository,
        IPrincipal_RoleRepository principal_RoleRepository, IMailService mailService, ILogger<MQPlanHandler> logger)
        {
            this.TenantRepository = tenantRepository;
            this.PrincipalRepository = principalRepository;
            this.UserRepository = userRepository;
            this.RoleRepository = roleRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
            this.MailService = mailService;
            this.Logger = logger;
        }
        public Func<PlanMessage, bool> Handle()
        {
            return (planMessage) =>
            {
                string freeRoleCode = $"R_{planMessage.Product.Name.ToUpper()}_FREE";
                string proRoleCode = $"R_{planMessage.Product.Name.ToUpper()}_PRO";
                User user = this.UserRepository.FindOneByEmail(planMessage.Customer.Email);
                Tenant tenant = this.TenantRepository.FindOneByName(planMessage.Customer.Email);
                if (tenant == null)
                {
                    tenant = new Tenant { Name = planMessage.Customer.Email };
                    this.TenantRepository.Save(tenant);
                }
                if (user == null)
                {
                    user = new User();
                    var principal = new Principal { Name = planMessage.Customer.Email };
                    principal.PrincipalType = PrincipalType.User;
                    Guid principalId = this.PrincipalRepository.Save(principal);
                    user.PrincipalId = principalId;
                    user.TenantId = tenant.Id;
                    string password = this.GenerateRandomPassword();
                    Guid userId = this.UserRepository.SaveWithMD5Password(user, password);
                    this.MailService.SendOpsNotification("Getting Start", $"Wellcome to join us, your password is: {password}. You can conact our support team to bind your exist account.", new List<string> { planMessage.Customer.Email });
                }
                Role proRole = this.RoleRepository.FindOneByCode(proRoleCode);
                Guid proRoleId;
                if (proRole == null)
                {
                    proRole = new Role { Name = $"{planMessage.Product.Name}_Pro", Code = proRoleCode };
                    proRoleId = this.RoleRepository.Save(proRole);
                }
                else
                {
                    proRoleId = proRole.Id;
                }
                switch (planMessage.Type)
                {
                    case PlanType.Pro:
                        // Todo: Current User issue.
                        this.Principal_RoleRepository.Save(new Principal_Role { PrincipalId = user.PrincipalId, RoleId = proRoleId });
                        break;
                    case PlanType.Free:
                        Principal_Role principal_ProRole = this.Principal_RoleRepository.FindOneByPrincipalIdAndRoleId(user.PrincipalId, proRoleId);
                        if (principal_ProRole != null)
                        {
                            this.Principal_RoleRepository.Delete(principal_ProRole.Id);
                        }
                        Role freeRole = this.RoleRepository.FindOneByCode(freeRoleCode);
                        Guid freeRoleId;
                        if (freeRole == null)
                        {
                            freeRole = new Role { Name = $"{planMessage.Product.Name}_Free", Code = freeRoleCode };
                            freeRoleId = this.RoleRepository.Save(freeRole);
                        }
                        else
                        {
                            freeRoleId = freeRole.Id;
                        }
                        if (!this.Principal_RoleRepository.IsExist(user.PrincipalId, freeRoleId))
                        {
                            Principal_Role principal_FreeRole = new Principal_Role { PrincipalId = user.PrincipalId, RoleId = freeRoleId };
                            this.Principal_RoleRepository.Save(principal_FreeRole);
                        }
                        break;
                    default:
                        break;
                }
                return true;
            };
        }

        private string GenerateRandomPassword()
        {
            StringBuilder password = new StringBuilder();
            Random random = new Random();
            bool nonAlphanumeric = true;
            bool digit = true;
            bool lowercase = true;
            bool uppercase = true;
            int length = 6;
            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);
                password.Append(c);
                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }
            if (nonAlphanumeric)
            {
                password.Append((char)random.Next(33, 48));
            }
            if (digit)
            {
                password.Append((char)random.Next(48, 58));
            }
            if (lowercase)
            {
                password.Append((char)random.Next(97, 123));
            }
            if (uppercase)
            {
                password.Append((char)random.Next(65, 91));
            }
            return password.ToString();
        }
    }
}