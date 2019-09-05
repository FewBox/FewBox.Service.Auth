using System;
using AutoMapper;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S = FewBox.Service.Auth.Model.Entities;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class ServicesController : ResourcesController<IServiceRepository, S.Service, Guid, ServiceDto, ServicePersistantDto>
    {
        private ISecurityObjectRepository SecurityObjectRepository { get; set; }
        public ServicesController(IServiceRepository serviceRepository, ISecurityObjectRepository securityObjectRepository,
        IMapper mapper) : base(serviceRepository, mapper)
        {
            this.SecurityObjectRepository = securityObjectRepository;
        }
        
        [HttpPut("{id}/securityobjects/{securityObjectId}")]
        [Transaction]
        public PayloadResponseDto<int> Put(Guid id, Guid securityObjectId)
        {
            return new PayloadResponseDto<int>{
                Payload = this.SecurityObjectRepository.UpdateServiceId(securityObjectId, id)
            };
        }
    }
}
