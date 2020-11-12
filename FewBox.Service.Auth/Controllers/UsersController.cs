using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [Authorize]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class UsersController : ResourcesController<IUserRepository, User, UserDto, UserPersistantDto>
    {
        private IPrincipalRepository PrincipalRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private IGroup_UserRepository Group_UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }

        public UsersController(IUserRepository userRepository, IPrincipalRepository principalRepository, 
            IPrincipal_RoleRepository principal_RoleRepository, IGroup_UserRepository group_UserRepository,
            IRoleRepository roleRepository, IMapper mapper): base(userRepository, mapper)
        {
            this.PrincipalRepository = principalRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
            this.Group_UserRepository = group_UserRepository;
            this.RoleRepository = roleRepository;
        }
  
        [HttpGet("search/{keyword}")]
        public PayloadResponseDto<IEnumerable<UserDto>> Get(string keyword)
        {
            return new PayloadResponseDto<IEnumerable<UserDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(this.Repository.FindAllByKeyword(keyword))
            };
        }

        [HttpGet("batchsearch")]
        public PayloadResponseDto<IEnumerable<UserProfileDto>> Get([FromQuery] Guid[] ids)
        {
            return new PayloadResponseDto<IEnumerable<UserProfileDto>> {
                Payload = this.Mapper.Map<IEnumerable<User>, IEnumerable<UserProfileDto>>(this.Repository.FindAllByIds(ids))
            };
        }

        [HttpPost]
        [Transaction]
        public override PayloadResponseDto<Guid> Post([FromBody]UserPersistantDto userDto)
        {
            var user = this.Mapper.Map<UserPersistantDto, User>(userDto);
            var queryUser = this.Repository.FindOneByUsername(user.Name, user.Type);
            if(queryUser != null){
                return new PayloadResponseDto<Guid> { IsSuccessful = false, ErrorCode="USERNAME_ALREADYEXISTS", ErrorMessage ="The user name is already exits!", Payload = Guid.Empty };
            }
            var principal = this.Mapper.Map<UserPersistantDto, Principal>(userDto);
            principal.PrincipalType = PrincipalType.User;
            Guid principalId = this.PrincipalRepository.Save(principal);
            user.PrincipalId = principalId;
            Guid userId = this.Repository.SaveWithMD5Password(user, userDto.Password);
            if (userDto.RoleIds != null)
            {
                foreach (Guid roleId in userDto.RoleIds)
                {
                    this.Principal_RoleRepository.Save(new Principal_Role {
                        PrincipalId = principalId,
                        RoleId = roleId
                    });
                }
            }
            return new PayloadResponseDto<Guid> {
                Payload = userId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public override PayloadResponseDto<int> Put(Guid id, [FromBody]UserPersistantDto userDto)
        {
            int effect;
            var user = this.Mapper.Map<UserPersistantDto, User>(userDto);
            user.Id = id;
            var updateUser = this.Repository.FindOne(id);
            var principal = this.Mapper.Map<UserPersistantDto, Principal>(userDto);
            principal.Id = updateUser.PrincipalId;
            this.PrincipalRepository.Update(principal);
            effect = this.Repository.Update(user);
            this.Principal_RoleRepository.DeleteByPrincipalId(principal.Id);
            if (userDto.RoleIds != null)
            {
                foreach (Guid roleId in userDto.RoleIds)
                {
                    this.Principal_RoleRepository.Save(new Principal_Role
                    {
                        PrincipalId = principal.Id,
                        RoleId = roleId
                    });
                }
            }
            return new PayloadResponseDto<int>{
                Payload = effect
            };
        }

        [HttpDelete("{id}")]
        [Transaction]
        public override PayloadResponseDto<int> Delete(Guid id)
        {
            var updateUser = this.Repository.FindOne(id);
            this.PrincipalRepository.Recycle(updateUser.PrincipalId);
            return new PayloadResponseDto<int>{
                Payload = this.Repository.Recycle(id)
            };
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
            this.Repository.ResetPassword(email, changePasswordRequestDto.Password);
            return new MetaResponseDto { };
        }

        [HttpPut("{id}/resetpassword")]
        [Transaction]
        public MetaResponseDto ResetPassword(Guid id, [FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            this.Repository.ResetPassword(id, resetPasswordRequestDto.Password);
            return new MetaResponseDto { };
        }

        [HttpPost("{id}/groups/{groupId}")]
        [Transaction]
        public PayloadResponseDto<int> AddGroup(Guid id, Guid groupId)
        {
            int effect = 0;
            if(!this.Group_UserRepository.IsExist(groupId, id))
            {
                var group_User = new Group_User{ GroupId=id, UserId= id };
                group_User.Id = id;
                effect = this.Group_UserRepository.Update(group_User);
            }
            return new PayloadResponseDto<int>{
                Payload = effect
            };
        }

        [HttpPut("{id}/roles/{roleId}")]
        [Transaction]
        public PayloadResponseDto<Guid> AddRole(Guid id, Guid roleId)
        {
            Guid newId = Guid.Empty;
            var user = this.Repository.FindOne(id);
            if(!this.Principal_RoleRepository.IsExist(user.PrincipalId, roleId))
            {
                var principal_Role = new Principal_Role { PrincipalId = user.PrincipalId, RoleId = roleId };
                newId = this.Principal_RoleRepository.Save(principal_Role);
            }
            return new PayloadResponseDto<Guid>{
                Payload = newId
            };
        }
        
        [HttpGet("{id}/roles")]
        public PayloadResponseDto<IEnumerable<RoleDto>> GetRoles(Guid id)
        {
            return new PayloadResponseDto<IEnumerable<RoleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByUserId(id))
            };
        }
    }
}
