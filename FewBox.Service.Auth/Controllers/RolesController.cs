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

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    public class RolesController : MapperController
    {
        private IRoleRepository RoleRepository { get; set; }

        public RolesController(IRoleRepository roleRepository, IMapper mapper) : base(mapper)
        {
            this.RoleRepository = roleRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<RoleDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<RoleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAll())
            };
        }

        [HttpGet("api/{apiId}")]
        public PayloadResponseDto<IEnumerable<RoleDto>> GetByApiId(Guid apiId)
        {
            return new PayloadResponseDto<IEnumerable<RoleDto>> {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByApiId(apiId))
            };
        }

        [HttpGet("module/{moduleId}")]
        public PayloadResponseDto<IEnumerable<RoleDto>> GetByModuleId(Guid moduleId)
        {
            return new PayloadResponseDto<IEnumerable<RoleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByModuleId(moduleId))
            };
        }

        [HttpGet("user/{userId}")]
        public PayloadResponseDto<IEnumerable<RoleDto>> GetByUserId(Guid userId)
        {
            return new PayloadResponseDto<IEnumerable<RoleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByUserId(userId))
            };
        }

        [HttpGet("permission/{permissionId}")]
        public PayloadResponseDto<IEnumerable<RoleDto>> GetByPermissionId(Guid permissionId)
        {
            return new PayloadResponseDto<IEnumerable<RoleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByPermissionId(permissionId))
            };
        }

        [HttpGet("group/{groupId}")]
        public PayloadResponseDto<IEnumerable<RoleDto>> GetByGroupId(Guid groupId)
        {
            return new PayloadResponseDto<IEnumerable<RoleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByGroupId(groupId))
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
        public PayloadResponseDto<int> GetTotalNumber()
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.RoleRepository.Count()
            };
        }
    }
}
