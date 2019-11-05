using AutoMapper;
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
using FewBox.Core.Web.Error;
using FewBox.Core.Utility.Net;
using System.Text;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "JWTRole_ControllerAction")]
    public class SchemaController : MapperController
    {
        private SecurityConfig SecurityConfig { get; set; }
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
        private ApiConfig ApiConfig { get; set; }
        private ExternalApiConfig ExternalApiConfig { get; set; }
        private NotificationConfig NotificationConfig { get; set; }
        private ITryCatchService TryCatchService { get; set; }

        public SchemaController(SecurityConfig securityConfig, IUserRepository userRepository, IGroupRepository groupRepository, IRoleRepository roleRepository,
            IApiRepository apiRepository, IModuleRepository moduleRepository, IGroup_UserRepository group_UserRepository,
            IPrincipalRepository principalRepository, ISecurityObjectRepository securityObjectRepository,
            IRole_SecurityObjectRepository role_SecurityObjectRepository, IPrincipal_RoleRepository principal_RoleRepository,
            IServiceRepository serviceRepository, ApiConfig apiConfig, ExternalApiConfig externalApiConfig, NotificationConfig notificationConfig,
            ITryCatchService tryCatchService, IMapper mapper) : base(mapper)
        {
            this.SecurityConfig = securityConfig;
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
            this.ApiConfig = apiConfig;
            this.ExternalApiConfig = externalApiConfig;
            this.NotificationConfig = notificationConfig;
            this.TryCatchService = tryCatchService;
        }

        [AllowAnonymous]
        [HttpGet("isinit")]
        public MetaResponseDto IsInit()
        {
            if (this.UserRepository.Count() > 0)
            {
                return new MetaResponseDto { IsSuccessful = false, ErrorCode = "APP_INIT", ErrorMessage = "The app has been init, please sign in." };
            }
            return new MetaResponseDto();
        }

        [AllowAnonymous]
        [HttpPost("init")]
        [Transaction]
        public MetaResponseDto Init([FromBody]InitRequestDto initRequestDto)
        {
            # region Validate Inited.
            if (this.UserRepository.Count() > 0)
            {
                return new MetaResponseDto { IsSuccessful = false, ErrorCode = "APP_INIT", ErrorMessage = "The app has been init, please sign in." };
            }
            if (this.PrincipalRepository.IsExist(initRequestDto.AdminName))
            {
                return new MetaResponseDto { IsSuccessful = false, ErrorCode = "ADMIN_EXIST", ErrorMessage = "The administrator is exist, please sign in." };
            }
            #endregion

            # region API Init.
            // 1. Service
            Guid serviceId = this.InitService(this.SecurityConfig.Name, "Build-In Auth Service.");
            // 2. Principal
            Guid principalId = this.InitUser(initRequestDto.AdminName, initRequestDto.Password);
            // 3. Role
            Guid roleId = this.InitRole("Solution Admin", "R_SOLUTIONADMIN");
            // 4. Bind Principal & Role
            this.GrantRole(principalId, roleId);
            // 5. Bind Api & Role
            foreach (var apiItem in this.ApiConfig.ApiItems)
            {
                this.InitApiAndGrantPermission(apiItem.Controller, apiItem.Actions, serviceId, roleId);
            }
            // Init External Service Admin
            IDictionary<string, string> passwords = new Dictionary<string, string>();
            if (this.ExternalApiConfig.ExternalApiServices != null)
            {
                foreach (var externalApiService in this.ExternalApiConfig.ExternalApiServices)
                {
                    // 1. Service
                    Guid externalApiServiceId = this.InitService(externalApiService.Name, externalApiService.Name);
                    // 2. Principal
                    string username = $"{externalApiService.Name}_Admin";
                    string password = this.GetRandomPassword();
                    passwords.Add(username, password);
                    Guid externalApiServicePrincipalId = this.InitUser(username, password);
                    // 3. Role
                    Guid externalApiServiceRoleId = this.InitRole($"{externalApiService.Name} Admin", $"R_{externalApiService.Name}_SERVICEADMIN");
                    // 4. Bind Principal & Role
                    this.GrantRole(externalApiServicePrincipalId, externalApiServiceRoleId);
                    // 5. Bind Api & Role
                    foreach (var apiItem in externalApiService.ApiItems)
                    {
                        this.InitApiAndGrantPermission(apiItem.Controller, apiItem.Actions, externalApiServiceId, externalApiServiceRoleId);
                    }
                }
            }
            this.SendPassword(passwords);
            #endregion
            return new MetaResponseDto { };
        }

        [HttpPost("batchinit")]
        [Transaction]
        [Trace]
        public MetaResponseDto BatchInit([FromBody]BatchInitRequestDto batchInitRequestDto)
        {
            // 1. Service
            Guid serviceId = this.InitService(batchInitRequestDto.Service, batchInitRequestDto.Service);
            // 2. Role
            Guid roleId = this.InitRole(batchInitRequestDto.RoleName, batchInitRequestDto.RoleCode);
            IDictionary<string, string> passwords = new Dictionary<string, string>();
            foreach (string username in batchInitRequestDto.Usernames)
            {
                // 3. Principal
                string password = this.GetRandomPassword();
                Guid principalId = this.InitUser(username, password);
                passwords.Add(username, password);
                // 4. Bind Principal & Role
                this.GrantRole(principalId, roleId);
            }
            // 5. Bind Api & Role
            if (batchInitRequestDto.ApiItems != null)
            {
                foreach (var apiItem in batchInitRequestDto.ApiItems)
                {
                    this.InitApiAndGrantPermission(apiItem.Controller, apiItem.Actions, serviceId, roleId);
                }
            }
            // 6. Bind Module & Role
            if (batchInitRequestDto.ModuleItems != null)
            {
                foreach (var moduleItemDto in batchInitRequestDto.ModuleItems)
                {
                    this.InitModuleAndGrantPermission(moduleItemDto, serviceId, roleId, Guid.Empty);
                }
            }
            this.SendPassword(passwords);
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

        private Guid InitUser(string name, string password)
        {
            Guid principalId;
            if (this.PrincipalRepository.IsExist(name))
            {
                principalId = this.PrincipalRepository.FindOneByName(name).Id;
            }
            else
            {
                principalId = this.PrincipalRepository.Save(new Principal { Name = name, PrincipalType = PrincipalType.User });
                Guid userId = this.UserRepository.SaveWithMD5Password(new User { PrincipalId = principalId }, password);
            }
            return principalId;
        }

        private Guid InitRole(string name, string code)
        {
            Guid roleId;
            if (this.RoleRepository.IsExist(name))
            {
                roleId = this.RoleRepository.FindOneByName(name).Id;
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

        private void InitApiAndGrantPermission(string controller, IList<string> actions, Guid serviceId, Guid roleId)
        {
            foreach (string action in actions)
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
        }

        private void InitModuleAndGrantPermission(ModuleItemDto moduleItemDto, Guid serviceId, Guid roleId, Guid parentId)
        {
            if (moduleItemDto != null)
            {
                Guid securityObjectId = this.InitSecurityObject(serviceId, $"{moduleItemDto.Name}_{moduleItemDto.Code}");
                Guid moduleId = this.InitModule(serviceId, securityObjectId, parentId, moduleItemDto.Name, moduleItemDto.Code);
                if (this.Role_SecurityObjectRepository.IsExist(roleId, securityObjectId))
                {
                    // Do Nothing.
                }
                else
                {
                    this.Role_SecurityObjectRepository.Save(new Role_SecurityObject { RoleId = roleId, SecurityObjectId = securityObjectId });
                }
                if (moduleItemDto.Children != null)
                {
                    foreach (var childModuleItemDto in moduleItemDto.Children)
                    {
                        this.InitModuleAndGrantPermission(childModuleItemDto, serviceId, roleId, moduleId);
                    }
                }
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
            if (this.SecurityObjectRepository.IsExist(name))
            {
                securityObjectId = this.SecurityObjectRepository.FindOneByName(name).Id;
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
                moduleId = this.ModuleRepository.Save(new Module { SecurityObjectId = securityObjectId, ParentId = parentId, Name = name, Code = code });
            }
            return moduleId;
        }

        private void SendPassword(IDictionary<string, string> passwords)
        {
            string name = "Password";
            StringBuilder param = new StringBuilder();
            foreach (var password in passwords)
            {
                param.AppendLine($"{password.Key} : {password.Value}");
            }
            this.TryCatchService.TryCatchWithoutNotification(() =>
                {
                    RestfulUtility.Post<NotificationRequestDto, NotificationResponseDto>($"{this.NotificationConfig.Protocol}://{this.NotificationConfig.Host}:{this.NotificationConfig.Port}/api/notification", new Package<NotificationRequestDto>
                    {
                        Headers = new List<Header> { },
                        Body = new NotificationRequestDto
                        {
                            Name = name,
                            Param = param.ToString()
                        }
                    });
                });
        }
    }
}