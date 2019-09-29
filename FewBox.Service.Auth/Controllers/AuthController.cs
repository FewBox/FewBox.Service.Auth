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
    public class AuthController : ControllerBase
    {
        private IUserRepository UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IModuleRepository ModuleRepository { get; set; }
        private ITokenService TokenService { get; set; }
        private IAuthService AuthService { get; set; }
        private JWTConfig JWTConfig { get; set; }
        private AuthConfig AuthConfig { get; set; }

        public AuthController(IUserRepository userRepository, IRoleRepository roleRepository, IModuleRepository moduleRepository,
        ITokenService tokenService, IAuthService authService, JWTConfig jWTConfig, AuthConfig authConfig)
        {
            this.UserRepository = userRepository;
            this.RoleRepository = roleRepository;
            this.ModuleRepository = moduleRepository;
            this.TokenService = tokenService;
            this.AuthService = authService;
            this.JWTConfig = jWTConfig;
            this.AuthConfig = authConfig;
        }

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

        [HttpPost("renewtoken")]
        [Authorize("JWTRole_ControllerAction")]
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
        [Authorize("JWTRole_ControllerAction")]
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