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
using Microsoft.AspNetCore.Authorization;
using FewBox.Core.Web.Security;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    public class SecurityController : Controller
    {
        private IUserRepository UserRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IAuthenticationService AuthenticationService { get; set; }

        public SecurityController(IUserRepository userRepository, IPrincipal_RoleRepository principal_RoleRepository,
            IRoleRepository roleRepository, IAuthenticationService authenticationService)
        {
            this.UserRepository = userRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
            this.RoleRepository = roleRepository;
            this.AuthenticationService = authenticationService;
        }

        [HttpGet("{serviceName}/{controllerName}/{actionName}")]
        public PayloadResponseDto<IList<string>> GetRoles(string serviceName, string controllerName, string actionName)
        {
            return new PayloadResponseDto<IList<string>>
            {
                Payload = this.AuthenticationService.FindRolesByServiceAndControllerAndAction(serviceName, controllerName, actionName)
            };
        }

        [HttpPost("sentvalidatecode/{email}")]
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
            return new MetaResponseDto { };
        }

        [HttpPost("changepassword")]
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
    }
}
