using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [Authorize(Policy = "JWTPayload_ControllerAction")]
    public class TenantsController : ResourcesController<ITenantRepository, Tenant, TenantDto, TenantPersistantDto>
    {
        public TenantsController(ITenantRepository tenantRepository, IMapper mapper) : base(tenantRepository, mapper)
        {
        }

        [HttpGet("search")]
        public PayloadResponseDto<IEnumerable<TenantDto>> Get([FromQuery] string keyword)
        {
            return new PayloadResponseDto<IEnumerable<TenantDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Tenant>, IEnumerable<TenantDto>>(this.Repository.FindAllByKeyword(keyword))
            };
        }
    }
}
