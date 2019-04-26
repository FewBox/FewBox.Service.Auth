using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class Role_SecurityObjectsController : MapperController
    {
        private IRole_SecurityObjectRepository Role_SecurityObjectRepository {get;set;}

        public Role_SecurityObjectsController(IRole_SecurityObjectRepository role_SecurityObjectRepository, IMapper mapper) : base(mapper)
        {
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<RoleBindingDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<RoleBindingDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Role_SecurityObject>, IEnumerable<RoleBindingDto>>(this.Role_SecurityObjectRepository.FindAll())
            };
        }

        [HttpGet("Paging/{pageRange}/{pageIndex}")]
        public PayloadResponseDto<PagingDto<RoleBindingDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<RoleBindingDto>>
            {
                Payload = new PagingDto<RoleBindingDto>
                {
                    Items = this.Mapper.Map<IEnumerable<Role_SecurityObject>, IEnumerable<RoleBindingDto>>(this.Role_SecurityObjectRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.Role_SecurityObjectRepository.Count() / pageRange)
                }
            };
        }

        [HttpPost("GrantPermission")]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody] RoleBindingDto roleBindingDto)
        {
            var roleBinding = new Role_SecurityObject { SecurityObjectId = roleBindingDto.SecurityObjectId, RoleId = roleBindingDto.RoleId };
            Guid role_securityId = this.Role_SecurityObjectRepository.Save(roleBinding);
            return new PayloadResponseDto<Guid> {
                Payload = role_securityId
            };
        }

        [HttpDelete("{id}")]
        [Transaction]
        public MetaResponseDto Delete(Guid id)
        {
            this.Role_SecurityObjectRepository.Recycle(id);
            return new MetaResponseDto();
        }
    }
}
