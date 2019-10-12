using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FewBox.Core.Web.Token;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Security;
using FewBox.Service.Auth.Model.Configs;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Repositories;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "JWTRole_ControllerAction")]
    public class AuthController : ControllerBase
    {
        private IUserRepository UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IModuleRepository ModuleRepository { get; set; }
        private IApiRepository ApiRepository { get; set; }
        private IRole_SecurityObjectRepository Role_SecurityObjectRepository { get; set; }
        private ITokenService TokenService { get; set; }
        private JWTConfig JWTConfig { get; set; }
        private AuthConfig AuthConfig { get; set; }

        public AuthController(IUserRepository userRepository, IRoleRepository roleRepository, IModuleRepository moduleRepository,
        IApiRepository apiRepository, IRole_SecurityObjectRepository role_SecurityObjectRepository, ITokenService tokenService, JWTConfig jWTConfig, AuthConfig authConfig)
        {
            this.UserRepository = userRepository;
            this.RoleRepository = roleRepository;
            this.ModuleRepository = moduleRepository;
            this.ApiRepository = apiRepository;
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
            this.TokenService = tokenService;
            this.JWTConfig = jWTConfig;
            this.AuthConfig = authConfig;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public PayloadResponseDto<SignInResponseDto> SignIn([FromBody]SignInRequestDto signInRequestDto)
        {
            Guid userId;
            if (signInRequestDto.UserType == "Form" && this.UserRepository.IsPasswordValid(signInRequestDto.Username, signInRequestDto.Password, out userId))
            {
                var claims = from role in (from role in this.RoleRepository.FindAllByUserId(userId) select role.Code)
                             select new Claim(ClaimTypes.Role, role);
                var userInfo = new UserInfo
                {
                    Id = userId.ToString(),
                    Key = this.JWTConfig.Key,
                    Issuer = this.JWTConfig.Issuer,
                    Claims = claims
                };
                string token = this.TokenService.GenerateToken(userInfo, this.AuthConfig.ExpireTime);
                return new PayloadResponseDto<SignInResponseDto>
                {
                    Payload = new SignInResponseDto { IsValid = true, Token = token, AuthorizedModules = this.ModuleRepository.FindAllByUserId(userId).Select(m => m.Key).ToList() }
                };
            }
            return new PayloadResponseDto<SignInResponseDto>
            {
                Payload = new SignInResponseDto { IsValid = false }
            };
        }

        [AllowAnonymous]
        [HttpGet("{serviceName}/{controllerName}/{actionName}")]
        public PayloadResponseDto<IList<string>> GetRoles(string serviceName, string controllerName, string actionName)
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
            var claims = this.HttpContext.User.Claims.Where(
                c => c.Type == ClaimTypes.Role);
            var userInfo = new UserInfo
            {
                Id = Guid.NewGuid().ToString(),
                Key = this.JWTConfig.Key,
                Issuer = this.JWTConfig.Issuer,
                Claims = claims
            };
            string token = this.TokenService.GenerateToken(userInfo, this.AuthConfig.ExpireTime);
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
    }
}