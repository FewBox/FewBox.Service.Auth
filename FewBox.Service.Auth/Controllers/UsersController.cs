using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Service.Auth.Model.Services;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class UsersController : MapperController
    {
        private IPrincipalRepository PrincipalRepository { get; set; }
        private IUserRepository UserRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private IGroup_UserRepository Group_UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }

        public UsersController(IUserRepository userRepository, IPrincipalRepository principalRepository, 
            IPrincipal_RoleRepository principal_RoleRepository, IGroup_UserRepository group_UserRepository,
            IRoleRepository roleRepository, IMapper mapper): base(mapper)
        {
            this.UserRepository = userRepository;
            this.PrincipalRepository = principalRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
            this.Group_UserRepository = group_UserRepository;
            this.RoleRepository = roleRepository;
        }

        [HttpGet("count")]
        public PayloadResponseDto<int> Count()
        {
            return new PayloadResponseDto<int> {
                Payload = this.UserRepository.Count()
            };
        }
  
        [HttpGet]
        public PayloadResponseDto<IEnumerable<UserDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<UserDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(this.UserRepository.FindAll())
            };
        }

        [HttpGet("search/{keyword}")]
        public PayloadResponseDto<IEnumerable<UserDto>> Get(string keyword)
        {
            return new PayloadResponseDto<IEnumerable<UserDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(this.UserRepository.FindAllByKeyword(keyword))
            };
        }

        [HttpGet("paging/{pageIndex}/{pageRange}")]
        public PayloadResponseDto<PagingDto<UserDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<UserDto>>
            {
                Payload = new PagingDto<UserDto>
                {
                    Items = this.Mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(this.UserRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.UserRepository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<UserDto> Get(Guid id)
        {
            return new PayloadResponseDto<UserDto>
            {
                Payload = this.Mapper.Map<User, UserDto>(this.UserRepository.FindOne(id))
            };
        }

        [HttpGet("batchsearch")]
        public PayloadResponseDto<IEnumerable<UserProfileDto>> Get([FromQuery] Guid[] ids)
        {
            return new PayloadResponseDto<IEnumerable<UserProfileDto>> {
                Payload = this.Mapper.Map<IEnumerable<User>, IEnumerable<UserProfileDto>>(this.UserRepository.FindAllByIds(ids))
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]UserPersistantDto userDto)
        {
            var user = this.Mapper.Map<UserPersistantDto, User>(userDto);
            var queryUser = this.UserRepository.FindOneByUsername(user.Name, user.Type);
            if(queryUser != null){
                return new PayloadResponseDto<Guid> { IsSuccessful = false, ErrorCode="USERNAME_ALREADYEXISTS", ErrorMessage ="The user name is already exits!", Payload = Guid.Empty };
            }
            var principal = this.Mapper.Map<UserPersistantDto, Principal>(userDto);
            principal.PrincipalType = PrincipalType.User;
            Guid principalId = this.PrincipalRepository.Save(principal);
            user.PrincipalId = principalId;
            Guid userId = this.UserRepository.SaveWithMD5Password(user, userDto.Password);
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
        public MetaResponseDto Put(Guid id, [FromBody]UserPersistantDto userDto)
        {
            var user = this.Mapper.Map<UserPersistantDto, User>(userDto);
            user.Id = id;
            var updateUser = this.UserRepository.FindOne(id);
            var principal = this.Mapper.Map<UserPersistantDto, Principal>(userDto);
            principal.Id = updateUser.PrincipalId;
            this.PrincipalRepository.Update(principal);
            this.UserRepository.Update(user);
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
            return new MetaResponseDto();
        }

        [HttpDelete("{id}")]
        [Transaction]
        public MetaResponseDto Delete(Guid id)
        {
            var updateUser = this.UserRepository.FindOne(id);
            this.PrincipalRepository.Recycle(updateUser.PrincipalId);
            this.UserRepository.RecycleAsync(id);
            return new MetaResponseDto();
        }

        [HttpPut("{id}/resetpassword")]
        [Transaction]
        public MetaResponseDto ResetPassword(Guid id, [FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            this.UserRepository.ResetPassword(id, resetPasswordRequestDto.Password);
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

        [HttpDelete("{id}/groups/{groupId}")]
        [Transaction]
        public PayloadResponseDto<int> RemoveGroup(Guid id, Guid groupId)
        {
            int effect = 0;
            var group_User = this.Group_UserRepository.FindOneByGroupIdAndUserId(groupId, id);
            if(group_User != null)
            {
                effect = this.Group_UserRepository.Delete(group_User.Id);
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
            var user = this.UserRepository.FindOne(id);
            if(!this.Principal_RoleRepository.IsExist(user.PrincipalId, roleId))
            {
                var principal_Role = new Principal_Role { PrincipalId = user.PrincipalId, RoleId = roleId };
                newId = this.Principal_RoleRepository.Save(principal_Role);
            }
            return new PayloadResponseDto<Guid>{
                Payload = newId
            };
        }

        [HttpDelete("{id}/roles/{roleId}")]
        [Transaction]
        public PayloadResponseDto<int> RemoveRole(Guid id, Guid roleId)
        {
            int effect = 0;
            var user = this.UserRepository.FindOne(id);
            var principal_Role = this.Principal_RoleRepository.FindOneByPrincipalIdAndRoleId(user.PrincipalId, roleId);
            if(principal_Role != null)
            {
                effect = this.Principal_RoleRepository.Delete(principal_Role.Id);
            }
            return new PayloadResponseDto<int>{
                Payload = effect
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
