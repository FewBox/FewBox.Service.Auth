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
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class GroupsController : MapperController
    {
        private IPrincipalRepository PrincipalRepository { get; set; }
        private IGroupRepository GroupRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private IGroup_UserRepository Group_UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IUserRepository UserRepository { get; set; }

        public GroupsController(IGroupRepository groupRepository, IPrincipalRepository principalRepository,
            IPrincipal_RoleRepository principal_RoleRepository, IGroup_UserRepository group_UserRepository,
            IRoleRepository roleRepository, IUserRepository userRepository, IMapper mapper) : base(mapper)
        {
            this.GroupRepository = groupRepository;
            this.PrincipalRepository = principalRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
            this.Group_UserRepository = group_UserRepository;
            this.RoleRepository = roleRepository;
            this.UserRepository = userRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<GroupDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<GroupDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Group>, IEnumerable<GroupDto>>(this.GroupRepository.FindAll())
            };
        }

        [HttpGet("root")]
        public PayloadResponseDto<IEnumerable<GroupDto>> GetRoot()
        {
            return new PayloadResponseDto<IEnumerable<GroupDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Group>, IEnumerable<GroupDto>>(this.GroupRepository.FindAllByRoot())
            };
        }

        [HttpGet("paging/{pageIndex}/{pageRange}")]
        public PayloadResponseDto<PagingDto<GroupDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<GroupDto>>
            {
                Payload = new PagingDto<GroupDto>
                {
                    Items = this.Mapper.Map<IEnumerable<Group>, IEnumerable<GroupDto>>(this.GroupRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.GroupRepository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<GroupDto> Get(Guid id)
        {
            return new PayloadResponseDto<GroupDto>
            {
                Payload = this.Mapper.Map<Group, GroupDto>(this.GroupRepository.FindOne(id))
            };
        }

        [HttpGet("count")]
        public PayloadResponseDto<int> Count()
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.GroupRepository.Count()
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]GroupPersistantDto groupDto)
        {
            var principal = this.Mapper.Map<GroupPersistantDto, Principal>(groupDto);
            principal.PrincipalType = PrincipalType.Group;
            Guid principalId = this.PrincipalRepository.Save(principal);
            var group = this.Mapper.Map<GroupPersistantDto, Group>(groupDto);
            group.PrincipalId = principalId;
            Guid groupId = this.GroupRepository.Save(group);
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
        public MetaResponseDto Put(Guid id, [FromBody]GroupPersistantDto groupDto)
        {
            var group = this.Mapper.Map<GroupPersistantDto, Group>(groupDto);
            var updateGroup = this.GroupRepository.FindOne(id);
            var principal = this.Mapper.Map<GroupPersistantDto, Principal>(groupDto);
            principal.Id = updateGroup.PrincipalId;
            this.PrincipalRepository.Update(principal);
            this.GroupRepository.Update(group);
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
            return new MetaResponseDto();
        }

        [HttpDelete("{id}")]
        [Transaction]
        public MetaResponseDto Delete(Guid id)
        {
            var updateGroup = this.GroupRepository.FindOne(id);
            this.PrincipalRepository.Recycle(updateGroup.PrincipalId);
            this.GroupRepository.Recycle(id);
            return new MetaResponseDto();
        }

        [HttpPost("{id}/users/{userId}")]
        [Transaction]
        public PayloadResponseDto<Guid> AddUser(Guid id, Guid userId)
        {
            Guid newId = Guid.Empty;
            if(!this.Group_UserRepository.IsExist(id, userId)&&
            this.GroupRepository.IsExist(id)&&
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

        [HttpDelete("{id}/users/{userId}")]
        [Transaction]
        public PayloadResponseDto<int> RemoveUser(Guid id, Guid userId)
        {
            int effect = 0;
            var group_User = this.Group_UserRepository.FindOneByGroupIdAndUserId(id, userId);
            if(group_User != null)
            {
                effect = this.Group_UserRepository.Delete(group_User.Id);
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
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByGroupId(id))
            };
        }
        
        [HttpPost("{id}/groups/{parentId}")]
        [Transaction]
        public PayloadResponseDto<int> ChangeParent(Guid id, Guid parentId)
        {
            int effect = 0;
            effect = this.GroupRepository.UpdateParent(id, parentId);
            return new PayloadResponseDto<int>{
                Payload = effect
            };
        }
    }
}
