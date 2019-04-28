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
    public class RolesController : MapperController
    {
        private IRoleRepository RoleRepository { get; set; }
        private IGroupRepository GroupRepository { get; set; }
        public RolesController(IRoleRepository roleRepository, IGroupRepository groupRepository,
        IMapper mapper) : base(mapper)
        {
            this.RoleRepository = roleRepository;
            this.GroupRepository = groupRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<RoleDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<RoleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAll())
            };
        }

        [HttpGet("seek/{roleCode}/groups/count")]
        public PayloadResponseDto<int> GetGroupCount(string roleCode)
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.GroupRepository.CountByRoleCode(roleCode)
            };
        }

        [HttpGet("paging/{pageRange}/{pageIndex}")]
        public PayloadResponseDto<PagingDto<RoleDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<RoleDto>>
            {
                Payload = new PagingDto<RoleDto>
                {
                    Items = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.RoleRepository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<RoleDto> Get(Guid id)
        {
            return new PayloadResponseDto<RoleDto>
            {
                Payload = this.Mapper.Map<Role, RoleDto>(this.RoleRepository.FindOne(id))
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]RolePersistantDto roleDto)
        {
            Role role = this.Mapper.Map<RolePersistantDto, Role>(roleDto);
            Guid roleId = this.RoleRepository.Save(role);
            return new PayloadResponseDto<Guid> {
                Payload = roleId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public MetaResponseDto Put(Guid id, [FromBody]RolePersistantDto roleDto)
        {
            Role role = this.Mapper.Map<RolePersistantDto, Role>(roleDto);
            role.Id = id;
            this.RoleRepository.Update(role);
            return new MetaResponseDto();
        }

        [HttpDelete("{id}")]
        [Transaction]
        public MetaResponseDto Delete(Guid id)
        {
            this.RoleRepository.RecycleAsync(id);
            return new MetaResponseDto();
        }

        [HttpGet("count")]
        public PayloadResponseDto<int> Count()
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.RoleRepository.Count()
            };
        }
    }
}
