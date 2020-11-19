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
    [Authorize(Policy="JWTPayload_ControllerAction")]
    public class GroupsController : ResourcesController<IGroupRepository, Group, GroupDto, GroupPersistantDto>
    {
        private IPrincipalRepository PrincipalRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private IGroup_UserRepository Group_UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IUserRepository UserRepository { get; set; }

        public GroupsController(IGroupRepository groupRepository, IPrincipalRepository principalRepository,
            IPrincipal_RoleRepository principal_RoleRepository, IGroup_UserRepository group_UserRepository,
            IRoleRepository roleRepository, IUserRepository userRepository, IMapper mapper) : base(groupRepository, mapper)
        {
            this.PrincipalRepository = principalRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
            this.Group_UserRepository = group_UserRepository;
            this.RoleRepository = roleRepository;
            this.UserRepository = userRepository;
        }

        [HttpGet("root")]
        public PayloadResponseDto<IEnumerable<GroupDto>> GetRoot()
        {
            return new PayloadResponseDto<IEnumerable<GroupDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Group>, IEnumerable<GroupDto>>(this.Repository.FindAllByRoot())
            };
        }

        [HttpPost]
        [Transaction]
        public override PayloadResponseDto<Guid> Post([FromBody]GroupPersistantDto groupDto)
        {
            var principal = this.Mapper.Map<GroupPersistantDto, Principal>(groupDto);
            principal.PrincipalType = PrincipalType.Group;
            Guid principalId = this.PrincipalRepository.Save(principal);
            var group = this.Mapper.Map<GroupPersistantDto, Group>(groupDto);
            group.PrincipalId = principalId;
            Guid groupId = this.Repository.Save(group);
            if (groupDto.RoleIds != null)
            {
                foreach (Guid roleId in groupDto.RoleIds)
                {
                    this.Principal_RoleRepository.Save(new Principal_Role
                    {
                        PrincipalId = principalId,
                        RoleId = roleId
                    });
                }
            }
            return new PayloadResponseDto<Guid> {
                Payload = groupId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public override PayloadResponseDto<int> Put(Guid id, [FromBody]GroupPersistantDto groupDto)
        {
            int effect;
            var group = this.Mapper.Map<GroupPersistantDto, Group>(groupDto);
            var updateGroup = this.Repository.FindOne(id);
            var principal = this.Mapper.Map<GroupPersistantDto, Principal>(groupDto);
            principal.Id = updateGroup.PrincipalId;
            this.PrincipalRepository.Update(principal);
            effect = this.Repository.Update(group);
            if (groupDto.RoleIds != null)
            {
                foreach (Guid roleId in groupDto.RoleIds)
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
            var updateGroup = this.Repository.FindOne(id);
            this.PrincipalRepository.Recycle(updateGroup.PrincipalId);
            return new PayloadResponseDto<int>{
                Payload = this.Repository.Recycle(id)
            };
        }

        [HttpPost("{id}/users/{userId}")]
        [Transaction]
        public PayloadResponseDto<Guid> AddUser(Guid id, Guid userId)
        {
            Guid newId = Guid.Empty;
            if(!this.Group_UserRepository.IsExist(id, userId)&&
            this.Repository.IsExist(id)&&
            this.UserRepository.IsExist(userId))
            {
                var group_User = new Group_User{ GroupId=id, UserId=userId };
                group_User.Id = id;
                newId = this.Group_UserRepository.Save(group_User);
            }
            return new PayloadResponseDto<Guid>{
                Payload = newId
            };
        }

        [HttpPut("{id}/roles/{roleId}")]
        [Transaction]
        public PayloadResponseDto<Guid> AddRole(Guid id, Guid roleId)
        {
            Guid newId = Guid.Empty;
            var group = this.Repository.FindOne(id);
            if(!this.Principal_RoleRepository.IsExist(group.PrincipalId, roleId)&&
            this.Repository.IsExist(id)&&
            this.RoleRepository.IsExist(roleId))
            {
                var principal_Role = new Principal_Role { PrincipalId = group.PrincipalId, RoleId = roleId };
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
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByGroupId(id))
            };
        }
    }
}
