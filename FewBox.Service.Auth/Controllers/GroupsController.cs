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

        public GroupsController(IGroupRepository groupRepository, IPrincipalRepository principalRepository,
            IPrincipal_RoleRepository principal_RoleRepository, IMapper mapper) : base(mapper)
        {
            this.GroupRepository = groupRepository;
            this.PrincipalRepository = principalRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
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

        [HttpGet("Paging/{pageRange}/{pageIndex}")]
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

        [HttpGet("count")]
        public PayloadResponseDto<int> GetTotalNumber()
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.GroupRepository.Count()
            };
        }

        [HttpGet("count/rolecode/{roleCode}")]
        public PayloadResponseDto<int> GetTotalNumberByRoleCode(string roleCode)
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.GroupRepository.CountByRoleCode(roleCode)
            };
        }
    }
}
