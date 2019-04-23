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
using System.Linq;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    public class Principal_RolesController : MapperController
    {
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }

        public Principal_RolesController(IPrincipal_RoleRepository principal_RoleRepository, IMapper mapper) : base(mapper)
        {
            this.Principal_RoleRepository = principal_RoleRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<Principal_RoleDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<Principal_RoleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Principal_Role>, IEnumerable<Principal_RoleDto>>(this.Principal_RoleRepository.FindAll())
            };
        }

        [HttpGet("Paging/{pageRange}/{pageIndex}")]
        public PayloadResponseDto<PagingDto<Principal_RoleDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<Principal_RoleDto>>
            {
                Payload = new PagingDto<Principal_RoleDto>
                {
                    Items = this.Mapper.Map<IEnumerable<Principal_Role>, IEnumerable<Principal_RoleDto>>(this.Principal_RoleRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.Principal_RoleRepository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<Principal_RoleDto> Get(Guid id)
        {
            return new PayloadResponseDto<Principal_RoleDto>
            {
                Payload = this.Mapper.Map<Principal_Role, Principal_RoleDto>(this.Principal_RoleRepository.FindOne(id))
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]Principal_RolePersistantDto principal_RoleDto)
        {
            var principal_Role = this.Mapper.Map<Principal_RolePersistantDto, Principal_Role>(principal_RoleDto);
            Guid principal_RoleId = this.Principal_RoleRepository.Save(principal_Role);
            return new PayloadResponseDto<Guid> {
                Payload = principal_RoleId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public MetaResponseDto Put(Guid id, [FromBody]Principal_RolePersistantDto principal_RoleDto)
        {
            var principal_Role = this.Mapper.Map<Principal_RolePersistantDto, Principal_Role>(principal_RoleDto);
            principal_Role.Id = id;
            this.Principal_RoleRepository.Update(principal_Role);
            return new MetaResponseDto();
        }

        [HttpDelete("{id}")]
        [Transaction]
        public MetaResponseDto Delete(Guid id)
        {
            this.Principal_RoleRepository.RecycleAsync(id);
            return new MetaResponseDto();
        }
    }
}
