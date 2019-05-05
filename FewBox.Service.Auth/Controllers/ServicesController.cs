using System;
using System.Collections.Generic;
using AutoMapper;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S = FewBox.Service.Auth.Model.Entities;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class ServicesController : MapperController
    {
        private IServiceRepository ServiceRepository { get; set; }
        private ISecurityObjectRepository SecurityObjectRepository { get; set; }
        public ServicesController(IServiceRepository serviceRepository, ISecurityObjectRepository securityObjectRepository,
        IMapper mapper) : base(mapper)
        {
            this.ServiceRepository = serviceRepository;
            this.SecurityObjectRepository = securityObjectRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<ServiceDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<ServiceDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<S.Service>, IEnumerable<ServiceDto>>(this.ServiceRepository.FindAll())
            };
        }

        [HttpGet("paging/{pageIndex}/{pageRange}")]
        public PayloadResponseDto<PagingDto<ServiceDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<ServiceDto>>
            {
                Payload = new PagingDto<ServiceDto>
                {
                    Items = this.Mapper.Map<IEnumerable<S.Service>, IEnumerable<ServiceDto>>(this.ServiceRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.ServiceRepository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<ServiceDto> Get(Guid id)
        {
            return new PayloadResponseDto<ServiceDto>
            {
                Payload = this.Mapper.Map<S.Service, ServiceDto>(this.ServiceRepository.FindOne(id))
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]ServicePersistantDto serviceDto)
        {
            var service = this.Mapper.Map<ServicePersistantDto, S.Service>(serviceDto);
            Guid serviceId = this.ServiceRepository.Save(service);
            return new PayloadResponseDto<Guid> {
                Payload = serviceId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public PayloadResponseDto<int> Put(Guid id, [FromBody]ServicePersistantDto serviceDto)
        {
            var service = this.Mapper.Map<ServicePersistantDto, S.Service>(serviceDto);
            service.Id = id;
            return new PayloadResponseDto<int>{
                Payload = this.ServiceRepository.Update(service)
            };
        }

        [HttpDelete("{id}")]
        [Transaction]
        public PayloadResponseDto<int> Delete(Guid id)
        {
            return new PayloadResponseDto<int>{
                Payload = this.ServiceRepository.Recycle(id)
            };
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
