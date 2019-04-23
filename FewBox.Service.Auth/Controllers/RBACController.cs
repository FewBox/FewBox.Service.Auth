using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Service.Auth.Model.Services;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    public class RBACController : Controller
    {
        private IUserRepository UserRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IRBACService RBACService { get; set; }

        public RBACController(IUserRepository userRepository, IPrincipal_RoleRepository principal_RoleRepository, 
            IRoleRepository roleRepository, IRBACService rbaService)
        {
            this.UserRepository = userRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
            this.RoleRepository = roleRepository;
            this.RBACService = rbaService;
        }

        [HttpPost("GetApiRoles")]
        public PayloadResponseDto<RBACResponseDto> GetApiRoles([FromBody] ApiRBACRequestDto apiRBACRequestDto)
        {
            return new PayloadResponseDto<RBACResponseDto> {
                Payload = new RBACResponseDto
                {
                    Roles = this.RBACService.GetApiRoles(
                        apiRBACRequestDto.Controller,
                        apiRBACRequestDto.Action)
                }
            };
        }

        [HttpPost("GetModuleRoles")]
        public PayloadResponseDto<RBACResponseDto> GetModuleRoles([FromBody] ModuleRBACRequestDto moduleRBACRequestDto)
        {
            return new PayloadResponseDto<RBACResponseDto>
            {
                Payload = new RBACResponseDto
                {
                    Roles = this.RBACService.GetModuleRoles(
                        moduleRBACRequestDto.ModuleKey)
                }
            };
        }

        [HttpPost("SentValidateCode/{email}")]
        public MetaResponseDto SentValidateCode(string email)
        {
            if (this.UserRepository.IsExist(email))
            {
                // Todo: 
            }
            else
            {
                // Todo: 
            }
            return new MetaResponseDto {};
        }

        [HttpPost("ChangePassword")]
        [Transaction]
        public MetaResponseDto ChangePassword([FromBody] ChangePasswordRequestDto changePasswordRequestDto)
        {
            // Todo: 
            string email = null;
            if (String.IsNullOrEmpty(email))
            {
                return new MetaResponseDto { IsSuccessful = false };
            }
            this.UserRepository.ResetPassword(email, changePasswordRequestDto.Password);
            return new MetaResponseDto { };
        }

        [HttpPost("ValidatePassword")]
        public PayloadResponseDto<AuthenticationResponseDto> ValidatePassword([FromBody] AuthenticationRequestDto authenticationRequestDto)
        {
            bool isValid = this.UserRepository.IsPasswordValid(authenticationRequestDto.Id, authenticationRequestDto.Password);
            Guid principalId = Guid.Empty;
            if (isValid)
            {
                var user = this.UserRepository.FindOne(authenticationRequestDto.Id);
                if (user != null)
                {
                    principalId = user.PrincipalId;
                }
            }
            return new PayloadResponseDto<AuthenticationResponseDto>
            {
                Payload = new AuthenticationResponseDto
                {
                    IsValid = isValid,
                    PrincipalId = principalId
                },
                ErrorMessage = authenticationRequestDto.Id.ToString()
            };
        }
    }
}
