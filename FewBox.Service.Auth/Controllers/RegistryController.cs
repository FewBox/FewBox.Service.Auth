using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [Authorize]
    [Authorize(Policy = "JWTRole_ControllerAction")]
    public class RegistryController : MapperController
    {
        private IPrincipalRepository PrincipalRepository { get; set; }
        private IUserRepository UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private ITenantRepository TenantRepository { get; set; }

        public RegistryController(IMapper mapper, IPrincipalRepository principalRepository, IUserRepository userRepository, IRoleRepository roleRepository, IPrincipal_RoleRepository principal_RoleRepository, ITenantRepository tenantRepository) : base(mapper)
        {
            this.PrincipalRepository = principalRepository;
            this.UserRepository = userRepository;
            this.RoleRepository = roleRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
            this.TenantRepository = tenantRepository;
        }

        [AllowAnonymous]
        [HttpPost("userregister")]
        [Transaction]
        public PayloadResponseDto<Guid> UserRegister([FromBody] UserRegistryRequestDto userRegistryRequestDto)
        {
            // Todo
            if (userRegistryRequestDto.ValidateCode != "sfewwRfsfs8462")
            {
                return new PayloadResponseDto<Guid>
                {
                    IsSuccessful = false,
                    ErrorCode = "VALIDATECODE_ERROR",
                    ErrorMessage = "Validate code is not match."
                };
            }
            if (this.TenantRepository.IsExist(userRegistryRequestDto.Tenant))
            {
                return new PayloadResponseDto<Guid>
                {
                    IsSuccessful = false,
                    ErrorCode = "TENANT_EXIST",
                    ErrorMessage = "Tenant is exist."
                };
            }
            if (this.PrincipalRepository.IsExist(userRegistryRequestDto.Name))
            {
                return new PayloadResponseDto<Guid>
                {
                    IsSuccessful = false,
                    ErrorCode = "USER_EXIST",
                    ErrorMessage = "User is exist."
                };
            }
            var tenant = new Tenant { Name = userRegistryRequestDto.Tenant };
            Guid tenantId = this.TenantRepository.Save(tenant);
            var principal = this.Mapper.Map<UserRegistryRequestDto, Principal>(userRegistryRequestDto);
            principal.PrincipalType = PrincipalType.User;
            Guid principalId = this.PrincipalRepository.Save(principal);
            var user = this.Mapper.Map<UserRegistryRequestDto, User>(userRegistryRequestDto);
            user.Type = UserType.Form;
            user.PrincipalId = principalId;
            user.TenantId = tenantId;
            Guid userId = this.UserRepository.SaveWithMD5Password(user, userRegistryRequestDto.Password);
            var role = new Role { Name = $"Tenant Admin ({userRegistryRequestDto.Tenant})", Code = $"{userRegistryRequestDto.Tenant.ToUpper()}_ADMIN", Description = "The admin of tenant." };
            Guid roleId = this.RoleRepository.Save(role);
            this.Principal_RoleRepository.Save(new Principal_Role { PrincipalId = principalId, RoleId = roleId });
            return new PayloadResponseDto<Guid>
            {
                Payload = userId
            };
        }

        [HttpPost("appregister")]
        [Transaction]
        public PayloadResponseDto<Guid> AppRegister([FromBody] AppRegistryRequestDto appRegistryRequestDto)
        {
            // Todo
            /*if (appRegistryRequestDto.ValidateCode != "sfewwRfsfs8462")
            {
                return new PayloadResponseDto<Guid>
                {
                    IsSuccessful = false,
                    ErrorCode = "VALIDATECODE_ERROR",
                    ErrorMessage = "Validate code is not match."
                };
            }
            if (this.PrincipalRepository.IsExist(appRegistryRequestDto.Name))
            {
                return new PayloadResponseDto<Guid>
                {
                    IsSuccessful = false,
                    ErrorCode = "APPNAME_EXIST",
                    ErrorMessage = "APP name is exist."
                };
            }
            if (!this.TenantRepository.IsExist(appRegistryRequestDto.TenantId))
            {
                return new PayloadResponseDto<Guid>
                {
                    IsSuccessful = false,
                    ErrorCode = "TENANT_DOESNOTEXIST",
                    ErrorMessage = "Tenant not exist."
                };
            }
            Tenant tenant = this.TenantRepository.FindOne(appRegistryRequestDto.TenantId);
            var principal = this.Mapper.Map<AppRegistryRequestDto, Principal>(appRegistryRequestDto);
            principal.PrincipalType = PrincipalType.User;
            Guid principalId = this.PrincipalRepository.Save(principal);
            var user = new User { 
                PrincipalId = principalId,
                TenantId = appRegistryRequestDto.TenantId,
            };
            user.PrincipalId = principalId;
            user.TenantId = appRegistryRequestDto.TenantId;
            Guid userId = this.UserRepository.SaveWithMD5Password(user, appRegistryRequestDto.Password);
            var role = new Role { Name = $"Tenant Admin ({appRegistryRequestDto.Tenant})", Code = $"{tenant.ToUpper()}_ADMIN", Description = "The admin of tenant." };
            Guid roleId = this.RoleRepository.Save(role);
            this.Principal_RoleRepository.Save(new Principal_Role { PrincipalId = principalId, RoleId = roleId });*/
            return new PayloadResponseDto<Guid>
            {
                //Payload = userId
            };
        }
    }
}
