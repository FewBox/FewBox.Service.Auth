﻿using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using FewBox.Core.Web.Dto;
using Microsoft.AspNetCore.Authorization;
using FewBox.Service.Auth.Model.Configs;
using S = FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Web.Config;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using FewBox.Core.Utility.Net;
using System.Text;
using FewBox.SDK.Mail;
using Microsoft.Extensions.Logging;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [Authorize(Policy = "JWTPayload_ControllerAction")]
    public class SchemaController : MapperController
    {
        private IPrincipalRepository PrincipalRepository { get; set; }
        private IUserRepository UserRepository { get; set; }
        private IGroupRepository GroupRepository { get; set; }
        private IGroup_UserRepository Group_UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private ISecurityObjectRepository SecurityObjectRepository { get; set; }
        private IApiRepository ApiRepository { get; set; }
        private IModuleRepository ModuleRepository { get; set; }
        private IRole_SecurityObjectRepository Role_SecurityObjectRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private IServiceRepository ServiceRepository { get; set; }
        private ITenantRepository TenantRepository { get; set; }
        private IMailService MailService { get; set; }
        private InitialConfig InitialConfig { get; set; }
        private FewBoxConfig FewBoxConfig { get; set; }
        private ILogger Logger { get; set; }

        public SchemaController(IUserRepository userRepository, IGroupRepository groupRepository, IRoleRepository roleRepository,
            IApiRepository apiRepository, IModuleRepository moduleRepository, IGroup_UserRepository group_UserRepository,
            IPrincipalRepository principalRepository, ISecurityObjectRepository securityObjectRepository,
            IRole_SecurityObjectRepository role_SecurityObjectRepository, IPrincipal_RoleRepository principal_RoleRepository, IServiceRepository serviceRepository,
            ITenantRepository tenantRepository, IMailService mailService, InitialConfig initialConfig, FewBoxConfig fewBoxConfig, ILogger<SchemaController> logger, IMapper mapper) : base(mapper)
        {
            this.PrincipalRepository = principalRepository;
            this.UserRepository = userRepository;
            this.GroupRepository = groupRepository;
            this.RoleRepository = roleRepository;
            this.SecurityObjectRepository = securityObjectRepository;
            this.ApiRepository = apiRepository;
            this.ModuleRepository = moduleRepository;
            this.Group_UserRepository = group_UserRepository;
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
            this.ServiceRepository = serviceRepository;
            this.TenantRepository = tenantRepository;
            this.MailService = mailService;
            this.InitialConfig = initialConfig;
            this.FewBoxConfig = fewBoxConfig;
            this.Logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("isinitial")]
        public PayloadResponseDto<bool> IsInitial()
        {
            return new PayloadResponseDto<bool> { Payload = this.UserRepository.Count() > 0 };
        }

        [AllowAnonymous]
        [HttpPost("init")]
        [Transaction]
        public PayloadResponseDto<IDictionary<string, string>> Init()
        {
            this.Logger.LogInformation($"-----------Begin Init-----------");
            # region Validate Inited.
            if (this.UserRepository.Count() > 0)
            {
                this.Logger.LogError($"The app has been init, please sign in.");
                return new PayloadResponseDto<IDictionary<string, string>> { IsSuccessful = false, ErrorCode = "APP_INIT", ErrorMessage = "The app has been init, please sign in." };
            }
            #endregion
            var passwordPairs = this.Init(this.InitialConfig.Services);
            this.Logger.LogInformation($"-----------End Init-----------");
            return new PayloadResponseDto<IDictionary<string, string>>
            {
                Payload = passwordPairs
            };
        }

        [HttpPost("batchinit")]
        [Transaction]
        public PayloadResponseDto<IDictionary<string, string>> BatchInit([FromBody] BatchInitRequestDto batchInitRequestDto)
        {

            return new PayloadResponseDto<IDictionary<string, string>>
            {
                Payload = this.Init(batchInitRequestDto.Services)
            };
        }

        [HttpPost("initproductadmin")]
        [Transaction]
        public MetaResponseDto InitProductAdmin([FromBody] InitProductAdminDto initProductAdminDto)
        {
            foreach (string swaggerUrl in initProductAdminDto.SwaggerUrls)
            {
                dynamic swagger = RestfulUtility.Get<dynamic>(swaggerUrl, new List<Header> { });
                string productName = swagger.info.Service;
                // Service
                Guid serviceId = this.InitService(productName, productName);
                // Role
                string roleName = $"{productName}_Admin";
                string roleCode = $"R_{productName.ToUpper()}_ADMIN";
                Guid roleId = this.InitRole(roleName, roleCode);
                // Tenant
                string tenantName = initProductAdminDto.Email.Split('@')[1];
                Guid tenantId = this.InitTenant(tenantName);
                // User
                User user = this.UserRepository.FindOneByEmail(initProductAdminDto.Email);
                Guid userId;
                Guid principalId;
                if (user == null)
                {
                    string password = this.GetRandomPassword();
                    principalId = this.InitUser(tenantId, $"{initProductAdminDto.Name}", initProductAdminDto.Email, password);
                    this.MailService.SendOpsNotification($"Getting Start", $"Username: {initProductAdminDto.Name}  Password: {password}", new List<string> { initProductAdminDto.Email });
                }
                else
                {
                    userId = user.Id;
                    principalId = user.PrincipalId;
                }
                // Role Binding
                Principal_Role principal_Role = this.Principal_RoleRepository.FindOneByPrincipalIdAndRoleId(principalId, roleId);
                if (principal_Role == null)
                {
                    principal_Role = new Principal_Role { PrincipalId = principalId, RoleId = roleId };
                    this.Principal_RoleRepository.Save(principal_Role);
                }
                else
                {
                    // Do Noting.
                }
                // API
                foreach (var path in swagger.paths)
                {
                    foreach (var verb in swagger.paths[path.Name])
                    {
                        string operationIdValue = verb.Value.operationId.Value;
                        string[] controllerAndAction = operationIdValue.Split('_');
                        string controller = controllerAndAction[0];
                        string action = controllerAndAction[1];
                        this.InitApiAndGrantPermission(controller, action, serviceId, roleId);
                    }
                }
            }
            return new MetaResponseDto { };
        }

        [HttpPost("initproductrole")]
        [Transaction]
        public MetaResponseDto InitProductRole([FromBody] InitProductRoleDto initProductRoleDto)
        {

            // Service
            Guid serviceId = this.InitService(initProductRoleDto.ProductName, initProductRoleDto.ProductName);
            // Role
            string freeRoleName = $"{initProductRoleDto.ProductName}_Free";
            string freeRoleCode = $"R_{initProductRoleDto.ProductName.ToUpper()}_FREE";
            string proRoleName = $"{initProductRoleDto.ProductName}_Pro";
            string proRoleCode = $"R_{initProductRoleDto.ProductName.ToUpper()}_PRO";
            Guid freeRoleId = this.InitRole(freeRoleName, freeRoleCode);
            Guid proRoleId = this.InitRole(proRoleName, proRoleCode);
            // Api
            foreach (var freeRoleApi in initProductRoleDto.FreeRoleApis)
            {
                this.InitApiAndGrantPermission(freeRoleApi.Controller, freeRoleApi.Action, serviceId, freeRoleId);
            }
            foreach (var proRoleApi in initProductRoleDto.ProRoleApis)
            {
                this.InitApiAndGrantPermission(proRoleApi.Controller, proRoleApi.Action, serviceId, proRoleId);
            }
            return new MetaResponseDto { };
        }

        private string GetRandomPassword(PasswordOptions passwordOptions = null)
        {
            if (passwordOptions == null)
            {
                passwordOptions = new PasswordOptions()
                {
                    RequiredLength = 8,
                    RequiredUniqueChars = 4,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireNonAlphanumeric = true,
                    RequireUppercase = true
                };
            }

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@$?_-#"                        // non-alphanumeric
            };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (passwordOptions.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (passwordOptions.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (passwordOptions.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (passwordOptions.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < passwordOptions.RequiredLength
                || chars.Distinct().Count() < passwordOptions.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }
            return new string(chars.ToArray());
        }

        private IDictionary<string, string> Init(IList<ServiceConfig> services)
        {
            Guid tenantId;
            if (this.TenantRepository.IsExist(this.InitialConfig.Tenant))
            {
                tenantId = this.TenantRepository.FindOneByName(this.InitialConfig.Tenant).Id;
            }
            else
            {
                Tenant tenant = new Tenant { Name = this.InitialConfig.Tenant };
                tenantId = this.TenantRepository.Save(tenant);
            }
            IDictionary<string, string> passwords = new Dictionary<string, string>();
            # region API Init.
            foreach (ServiceConfig service in services)
            {
                IDictionary<string, Guid> roleIdPair = new Dictionary<string, Guid>();
                IDictionary<string, Guid> userIdPair = new Dictionary<string, Guid>();
                IDictionary<string, Guid> groupIdPair = new Dictionary<string, Guid>();
                // 1. Service
                this.Logger.LogInformation($"-----------Init Service ({service.Name})-----------");
                Guid serviceId = this.InitService(service.Name, service.Description);
                if (service.Roles != null)
                {
                    foreach (RoleConfig role in service.Roles)
                    {
                        // 2. Role
                        this.Logger.LogInformation($"-----------Init Role ({role.Name})-----------");
                        Guid roleId = this.InitRole(role.Name, role.Code);
                        roleIdPair.Add(role.Name, roleId);
                    }
                }
                if (service.Users != null)
                {
                    foreach (UserConfig user in service.Users)
                    {
                        // 3. Principal (User)
                        this.Logger.LogInformation($"-----------Init User ({user.Name})-----------");
                        string password = this.GetRandomPassword();
                        Guid principalId = this.InitUser(tenantId, user.Name, user.Email, password);
                        userIdPair.Add(user.Name, principalId);
                        passwords.Add(user.Name, password);
                    }
                }
                if (service.Groups != null)
                {
                    foreach (GroupConfig group in service.Groups)
                    {
                        // 3. Principal (Group)
                        this.Logger.LogInformation($"-----------Init Group ({group.Name})-----------");
                        Guid principalId = this.InitGroup(group.Name, group.ParentName, group.Users);
                        groupIdPair.Add(group.Name, principalId);
                    }
                }
                if (service.RoleAssignments != null)
                {
                    foreach (RoleAssignmentConfig roleAssignment in service.RoleAssignments)
                    {
                        // 4. Bind Principal & Role
                        this.Logger.LogInformation($"-----------Init Princiapl & Role ({roleAssignment.Principal} [{roleAssignment.PrincipalType.ToString()}] : {roleAssignment.Role})-----------");
                        if (roleAssignment.PrincipalType == PrincipalTypeConfig.Group)
                        {
                            this.GrantRole(groupIdPair[roleAssignment.Principal], roleIdPair[roleAssignment.Role]);
                        }
                        else
                        {
                            this.GrantRole(userIdPair[roleAssignment.Principal], roleIdPair[roleAssignment.Role]);
                        }
                    }
                }
                if (service.Apis != null)
                {
                    // 5. Bind Api & Role
                    this.Logger.LogInformation($"-----------Init Api & Role Binding-----------");
                    foreach (ApiConfig api in service.Apis)
                    {
                        foreach (ActionConfig action in api.Actions)
                        {
                            if (action.Roles == null)
                            {
                                foreach (string role in api.DefaultRoles)
                                {
                                    this.InitApiAndGrantPermission(api.Controller, action.Name, serviceId, roleIdPair[role]);
                                }
                            }
                            else
                            {
                                foreach (string role in action.Roles)
                                {
                                    this.InitApiAndGrantPermission(api.Controller, action.Name, serviceId, roleIdPair[role]);
                                }
                            }
                        }
                    }
                }
                if (service.Modules != null)
                {
                    // 6. Bind Moudle and Role
                    this.Logger.LogInformation($"-----------Init Module & Role Binding-----------");
                    foreach (ModuleConfig module in service.Modules)
                    {
                        foreach (string roleName in module.DefaultRoles)
                        {
                            this.InitModuleAndGrantPermission(module.Name, module.Code, module.ParentName, serviceId, roleIdPair[roleName]);
                        }
                    }
                }
                this.SendPassword(passwords);
            }
            #endregion
            return passwords;
        }

        private Guid InitTenant(string name)
        {
            Tenant tenant = this.TenantRepository.FindOneByName(name);
            Guid tenantId;
            if (tenant == null)
            {
                tenant = new Tenant { Name = name };
                tenantId = this.TenantRepository.Save(tenant);
            }
            else
            {
                tenantId = tenant.Id;
            }
            return tenantId;
        }

        private Guid InitUser(Guid tenantId, string name, string email, string password)
        {
            bool isExist;
            return this.InitUser(tenantId, name, password, email, out isExist);
        }

        private Guid InitUser(Guid tenantId, string name, string password, string email, out bool isExist)
        {
            Guid principalId;
            if (this.PrincipalRepository.IsExist(name))
            {
                isExist = true;
                principalId = this.PrincipalRepository.FindOneByName(name).Id;
            }
            else
            {
                isExist = false;
                principalId = this.PrincipalRepository.Save(new Principal { Name = name, PrincipalType = PrincipalType.User });
                Guid userId = this.UserRepository.SaveWithMD5Password(new User { Type = UserType.Form, PrincipalId = principalId, TenantId = tenantId, Email = email }, password);
            }
            return principalId;
        }

        private Guid InitGroup(string name, string parentName, IList<UserConfig> users)
        {
            Guid principalId;
            if (this.PrincipalRepository.IsExist(name))
            {
                principalId = this.PrincipalRepository.FindOneByName(name).Id;
            }
            else
            {
                principalId = this.PrincipalRepository.Save(new Principal { Name = name, PrincipalType = PrincipalType.User });
                var parent = this.GroupRepository.FindOneByName(parentName);
                Guid groupId = this.GroupRepository.Save(new Group { PrincipalId = principalId, Name = name, ParentId = parent != null ? parent.Id : Guid.Empty });
                foreach (UserConfig user in users)
                {
                    var groupUser = this.UserRepository.FindOneByUsername(user.Name);
                    if (groupUser != null)
                    {
                        this.Group_UserRepository.Save(new Group_User { GroupId = groupId, UserId = groupUser.Id });
                    }
                }
            }
            return principalId;
        }

        private Guid InitRole(string name, string code)
        {
            Guid roleId;
            if (this.RoleRepository.IsExist(name, code))
            {
                roleId = this.RoleRepository.FindOneByNameAndCode(name, code).Id;
            }
            else
            {
                roleId = this.RoleRepository.Save(new Role { Name = name, Code = code });
            }
            return roleId;
        }

        private void GrantRole(Guid principalId, Guid roleId)
        {
            if (this.Principal_RoleRepository.IsExist(principalId, roleId))
            {
                // Do Nothing.
            }
            else
            {
                this.Principal_RoleRepository.Save(new Principal_Role { PrincipalId = principalId, RoleId = roleId });
            }
        }

        private void InitApiAndGrantPermission(string controller, string action, Guid serviceId, Guid roleId)
        {
            Guid securityObjectId = this.InitSecurityObject(serviceId, $"{controller}_{action}");
            Guid apiId = this.InitApi(serviceId, securityObjectId, controller, action);
            if (this.Role_SecurityObjectRepository.IsExist(roleId, securityObjectId))
            {
                // Do Nothing.
            }
            else
            {
                this.Role_SecurityObjectRepository.Save(new Role_SecurityObject { RoleId = roleId, SecurityObjectId = securityObjectId });
            }
        }

        private void InitModuleAndGrantPermission(string name, string code, string parentName, Guid serviceId, Guid roleId)
        {
            Guid securityObjectId = this.InitSecurityObject(serviceId, name);
            Guid parentId = Guid.Empty;
            Module parentModule = this.ModuleRepository.FindOneByName(parentName);
            if (parentModule != null)
            {
                parentId = parentModule.Id;
            }
            Guid moduleId = this.InitModule(serviceId, securityObjectId, parentId, name, code);
            if (this.Role_SecurityObjectRepository.IsExist(roleId, securityObjectId))
            {
                // Do Nothing.
            }
            else
            {
                this.Role_SecurityObjectRepository.Save(new Role_SecurityObject { RoleId = roleId, SecurityObjectId = securityObjectId });
            }
        }

        private Guid InitService(string name, string description)
        {
            Guid serviceid;
            if (this.ServiceRepository.IsExist(name))
            {
                serviceid = this.ServiceRepository.FindOneByName(name).Id;
            }
            else
            {
                serviceid = this.ServiceRepository.Save(new S.Service { Name = name, Description = description });
            }
            return serviceid;
        }

        private Guid InitSecurityObject(Guid serviceId, string name)
        {
            Guid securityObjectId;
            if (this.SecurityObjectRepository.IsExist(serviceId, name))
            {
                securityObjectId = this.SecurityObjectRepository.FindOneByServiceIdAndName(serviceId, name).Id;
            }
            else
            {
                securityObjectId = this.SecurityObjectRepository.Save(new SecurityObject { ServiceId = serviceId, Name = name });
            }
            return securityObjectId;
        }

        private Guid InitApi(Guid serviceId, Guid securityObjectId, string controller, string action)
        {
            Guid apiId;
            if (this.ApiRepository.IsExist(serviceId, controller, action))
            {
                apiId = this.ApiRepository.FindOneByServiceAndControllerAndAction(serviceId, controller, action).Id;
            }
            else
            {
                apiId = this.ApiRepository.Save(new Api { SecurityObjectId = securityObjectId, Controller = controller, Action = action });
            }
            return apiId;
        }

        private Guid InitModule(Guid serviceId, Guid securityObjectId, Guid parentId, string name, string code)
        {
            Guid moduleId;
            if (this.ModuleRepository.IsExist(serviceId, code))
            {
                moduleId = this.ModuleRepository.FindOneByServiceAndCode(serviceId, code).Id;
            }
            else
            {
                moduleId = this.ModuleRepository.Save(new Module { SecurityObjectId = securityObjectId, ParentId = parentId, Code = code });
            }
            return moduleId;
        }

        private void SendPassword(IDictionary<string, string> userPasswordPair)
        {
            if (userPasswordPair == null || userPasswordPair.Count == 0)
            {
                this.Logger.LogError($"No user password pair.");
                return;
            }
            this.Logger.LogInformation($"-----------Send password-----------");
            string name = "Initial Password";
            StringBuilder param = new StringBuilder();
            foreach (var password in userPasswordPair)
            {
                param.AppendLine($"{password.Key} : {password.Value}");
            }
            this.MailService.SendOpsNotification(name, param.ToString(), new List<string> { this.InitialConfig.SystemEmail });
            this.Logger.LogInformation($"Send password to {this.InitialConfig.SystemEmail}.");
        }
    }
}