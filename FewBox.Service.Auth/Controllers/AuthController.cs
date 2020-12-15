using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FewBox.Core.Web.Token;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Config;
using FewBox.Service.Auth.Model.Configs;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Repositories;
using Google.Apis.Auth;
using System.Threading.Tasks;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Utility.Compress;
using FewBox.Core.Utility.Formatter;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Policy = "JWTPayload_ControllerAction")]
    public class AuthController : ControllerBase
    {
        private IUserRepository UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IModuleRepository ModuleRepository { get; set; }
        private IApiRepository ApiRepository { get; set; }
        private IRole_SecurityObjectRepository Role_SecurityObjectRepository { get; set; }
        private ITenantRepository TenantRepository { get; set; }
        private IPrincipalRepository PrincipalRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private IServiceRepository ServiceRepository { get; set; }
        private ITokenService TokenService { get; set; }
        private FewBoxConfig FewBoxConfig { get; set; }
        private AuthConfig AuthConfig { get; set; }

        public AuthController(IUserRepository userRepository, IRoleRepository roleRepository, IModuleRepository moduleRepository, IApiRepository apiRepository,
        IRole_SecurityObjectRepository role_SecurityObjectRepository, ITenantRepository tenantRepository, IPrincipalRepository principalRepository,
        IPrincipal_RoleRepository principal_RoleRepository, IServiceRepository serviceRepository, ITokenService tokenService, FewBoxConfig fewBoxConfig, AuthConfig authConfig)
        {
            this.UserRepository = userRepository;
            this.RoleRepository = roleRepository;
            this.ModuleRepository = moduleRepository;
            this.ApiRepository = apiRepository;
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
            this.TenantRepository = tenantRepository;
            this.PrincipalRepository = principalRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
            this.ServiceRepository = serviceRepository;
            this.TokenService = tokenService;
            this.FewBoxConfig = fewBoxConfig;
            this.AuthConfig = authConfig;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public PayloadResponseDto<SigninResponseDto> Signin([FromBody] SigninRequestDto signinRequestDto)
        {
            Guid userId, tenantId;
            if (this.UserRepository.IsPasswordValid(signinRequestDto.Username, signinRequestDto.Password, out userId, out tenantId))
            {
                var userProfile = this.GetUserProfile(tenantId, userId);
                string token = this.TokenService.GenerateToken(userProfile, DateTime.Now.Add(this.AuthConfig.ExpireTime));
                return new PayloadResponseDto<SigninResponseDto>
                {
                    Payload = new SigninResponseDto { IsValid = true, Token = token }
                };
            }
            return new PayloadResponseDto<SigninResponseDto>
            {
                Payload = new SigninResponseDto { IsValid = false }
            };
        }

        [AllowAnonymous]
        [HttpPost("signingoogle")]
        public async Task<PayloadResponseDto<SigninResponseDto>> SigninGoogleAsync([FromBody] SigninGoogleRequestDto signinGoogleRequestDto)
        {
            GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(signinGoogleRequestDto.Token);
            if (validPayload != null)
            {
                Guid userId;
                IDictionary<Guid, string> serviceDictionary = new Dictionary<Guid, string>();
                if (this.UserRepository.IsGoogleAccountExists(validPayload.Subject))
                {
                    var user = this.UserRepository.FindOneByUserGoogleId(validPayload.Subject);
                    userId = user.Id;
                }
                else
                {
                    Role role = this.RoleRepository.FindOneByCode("TENANT");
                    var tenant = new Tenant { Name = $"google-{validPayload.Subject}" };
                    Guid tenantId = this.TenantRepository.Save(tenant);
                    Principal principal = new Principal { Name = validPayload.Name };
                    Guid principalId = this.PrincipalRepository.Save(principal);
                    userId = this.UserRepository.SaveGoogleAccount(tenantId, principalId, validPayload.Subject, validPayload.Email);
                    this.Principal_RoleRepository.Save(new Principal_Role { PrincipalId = principalId, RoleId = role.Id });
                }
                var userProfile = new UserProfile
                {
                    Tenant = validPayload.Email,
                    Id = userId.ToString(),
                    Key = this.FewBoxConfig.JWT.Key,
                    Issuer = this.FewBoxConfig.JWT.Issuer,
                    Audience = this.FewBoxConfig.JWT.Audience,
                    Roles = this.GetRoles(userId),
                    Apis = this.GetApis(userId, serviceDictionary),
                    Modules = this.GetModules(userId, serviceDictionary)
                };
                string token = this.TokenService.GenerateToken(userProfile, DateTime.Now.Add(this.AuthConfig.ExpireTime));
                return new PayloadResponseDto<SigninResponseDto>
                {
                    Payload = new SigninResponseDto { IsValid = true, Token = token }
                };
            }
            return new PayloadResponseDto<SigninResponseDto>
            {
                Payload = new SigninResponseDto { IsValid = false }
            };
        }

        [AllowAnonymous]
        [HttpPost("checkin")]
        public PayloadResponseDto<CheckinResponseDto> Checkin([FromBody] CheckinRequestDto checkinRequestDto)
        {
            Guid userId, tenantId;
            if (this.UserRepository.IsPasswordValid(checkinRequestDto.AccessKey, checkinRequestDto.SecurityKey, out userId, out tenantId))
            {
                Tenant tenant = this.TenantRepository.FindOne(tenantId);
                User user = this.UserRepository.FindOne(userId);
                IDictionary<Guid, string> serviceDictionary = new Dictionary<Guid, string>();
                var userProfile = new UserProfile
                {
                    Tenant = tenant.Name,
                    Id = userId.ToString(),
                    Name = user.Name,
                    Key = this.FewBoxConfig.JWT.Key,
                    Issuer = this.FewBoxConfig.JWT.Issuer,
                    MobilePhone = user.Mobile,
                    Email = user.Email,
                    Roles = this.GetRoles(userId),
                    Modules = this.GetModules(userId, serviceDictionary),
                    Apis = this.GetApis(userId, serviceDictionary)
                };
                string token = this.TokenService.GenerateToken(userProfile, DateTime.Now.Add(this.AuthConfig.ExpireTime));
                return new PayloadResponseDto<CheckinResponseDto>
                {
                    Payload = new CheckinResponseDto { IsValid = true, Token = token }
                };
            }
            return new PayloadResponseDto<CheckinResponseDto>
            {
                Payload = new CheckinResponseDto { IsValid = false }
            };
        }

        [AllowAnonymous]
        [ResponseCache(CacheProfileName = "default", VaryByQueryKeys = new[] { "datetime" })]
        [HttpGet("{serviceName}/{controllerName}/{actionName}/roles")]
        public PayloadResponseDto<IList<string>> GetRoles(string serviceName, string controllerName, string actionName, [FromQuery] string datetime)
        {
            var apiRoles = new List<string>();
            var api = this.ApiRepository.FindOneByServiceAndControllerAndAction(serviceName, controllerName, actionName);
            if (api != null)
            {
                var role_SecurityObjects = this.Role_SecurityObjectRepository.FindAllBySecurityId(api.SecurityObjectId);
                if (role_SecurityObjects != null)
                {
                    foreach (var role_SecurityObject in role_SecurityObjects)
                    {
                        var role = this.RoleRepository.FindOne(role_SecurityObject.RoleId);
                        apiRoles.Add(role.Code);
                    }
                }
            }
            return new PayloadResponseDto<IList<string>>
            {
                Payload = apiRoles
            };
        }

        [HttpPost("renewtoken")]
        public PayloadResponseDto<RenewTokenResponseDto> RenewToken([FromBody] RenewTokenRequestDto renewTokenRequestDto)
        {
            var userProfile = this.TokenService.GetUserProfileByToken(this.HttpContext.Request.Headers["Authorization"]);
            string token = this.TokenService.GenerateToken(userProfile, DateTime.Now.Add(this.AuthConfig.ExpireTime));
            return new PayloadResponseDto<RenewTokenResponseDto>
            {
                Payload = new RenewTokenResponseDto { Token = token }
            };
        }

        [HttpGet("currentuser")]
        public object GetCurrentClaims()
        {
            int i = this.User.Claims.Count();
            return User.Claims.Select(c =>
            new
            {
                Type = c.Type,
                Value = c.Value
            });
        }

        [AllowAnonymous]
        [HttpPost("sendverificationcode")]
        public MetaResponseDto SendVerificationCode(string email)
        {
            // Todo: Notification
            return new MetaResponseDto();
        }

        [HttpPost("parsetoken")]
        public PayloadResponseDto<UserProfile> ParseToken([FromBody] TokenDto tokenDto)
        {
            return new PayloadResponseDto<UserProfile>
            {
                Payload = this.TokenService.GetUserProfileByToken(tokenDto.Value)
            };
        }
        private UserProfile GetUserProfile(Guid tenantId, Guid userId)
        {
            Tenant tenant = this.TenantRepository.FindOne(tenantId);
            User user = this.UserRepository.FindOne(userId);
            IDictionary<Guid, string> serviceDictionary = new Dictionary<Guid, string>();
            var userProfile = new UserProfile
            {
                Tenant = tenant.Name,
                Id = userId.ToString(),
                Name = user.Name,
                Key = this.FewBoxConfig.JWT.Key,
                Issuer = this.FewBoxConfig.JWT.Issuer,
                Audience = this.FewBoxConfig.JWT.Audience,
                MobilePhone = user.Mobile,
                Email = user.Email,
                Roles = this.GetRoles(userId),
                Modules = this.GetModules(userId, serviceDictionary),
                Apis = this.GetApis(userId, serviceDictionary)
            };
            return userProfile;
        }

        private IList<string> GetRoles(Guid userId)
        {
            return this.RoleRepository.FindAllByUserId(userId).Select(role => role.Code).ToList();
        }

        private IList<string> GetApis(Guid userId, IDictionary<Guid, string> serviceDictionary)
        {
            var apis = this.ApiRepository.FindAllByUserId(userId);
            return apis.Select(api => $"{this.GetServiceName(api.ServiceId, serviceDictionary)}/{api.Controller}/{api.Action}").ToList();
        }

        private IList<string> GetModules(Guid userId, IDictionary<Guid, string> serviceDictionary)
        {
            var modules = this.ModuleRepository.FindAllByUserId(userId);
            return modules.Select(module => $"{this.GetServiceName(module.ServiceId, serviceDictionary)}/{module.Code}").ToList();
        }

        private string GetServiceName(Guid serviceId, IDictionary<Guid, string> serviceDictionary)
        {
            string serviceName;
            if (serviceDictionary.ContainsKey(serviceId))
            {
                serviceName = serviceDictionary[serviceId];
            }
            else
            {
                serviceName = this.ServiceRepository.FindOne(serviceId).Name;
                serviceDictionary.Add(serviceId, serviceName);
            }
            return serviceName;
        }
    }
}