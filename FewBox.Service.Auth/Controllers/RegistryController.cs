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
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class RegistryController : MapperController
    {
        private IPrincipalRepository PrincipalRepository { get; set; }
        private IUserRepository UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }

        public RegistryController(IMapper mapper, IPrincipalRepository principalRepository, IUserRepository userRepository, IRoleRepository roleRepository, IPrincipal_RoleRepository principal_RoleRepository) : base(mapper)
        {
            this.PrincipalRepository = principalRepository;
            this.UserRepository = userRepository;
            this.RoleRepository = roleRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]UserRegistryRequestDto userRegistryRequestDto)
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
            if (this.PrincipalRepository.IsExist(userRegistryRequestDto.Name))
            {
                return new PayloadResponseDto<Guid> {
                    IsSuccessful = false,
                    ErrorCode = "USER_EXIST",
                    ErrorMessage = "User is exist."
                };
            }
            var principal = this.Mapper.Map<UserRegistryRequestDto, Principal>(userRegistryRequestDto);
            principal.PrincipalType = PrincipalType.User;
            Guid principalId = this.PrincipalRepository.Save(principal);
            var user = this.Mapper.Map<UserRegistryRequestDto, User>(userRegistryRequestDto);
            user.PrincipalId = principalId;
            Guid userId = this.UserRepository.SaveWithMD5Password(user, userRegistryRequestDto.Password);
            var role = this.RoleRepository.FindOneByCode("R_VENDOR");
            this.Principal_RoleRepository.Save(new Principal_Role { PrincipalId = principalId, RoleId = role.Id });
            return new PayloadResponseDto<Guid>
            {
                Payload = userId
            };
        }
    }
}
