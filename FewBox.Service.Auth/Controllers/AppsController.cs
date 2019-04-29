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

namespace FewBox.Service.Auth.Controllers
{
    [Route("App/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class AppsController : MapperController
    {
        private IAppRepository AppRepository { get; set; }
        private ISecurityObjectRepository SecurityObjectRepository { get; set; }
        public AppsController(IAppRepository appRepository, ISecurityObjectRepository securityObjectRepository,
        IMapper mapper) : base(mapper)
        {
            this.AppRepository = appRepository;
            this.SecurityObjectRepository = securityObjectRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<AppDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<AppDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<App>, IEnumerable<AppDto>>(this.AppRepository.FindAll())
            };
        }

        [HttpGet("paging/{pageIndex}/{pageRange}")]
        public PayloadResponseDto<PagingDto<AppDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<AppDto>>
            {
                Payload = new PagingDto<AppDto>
                {
                    Items = this.Mapper.Map<IEnumerable<App>, IEnumerable<AppDto>>(this.AppRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.AppRepository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<AppDto> Get(Guid id)
        {
            return new PayloadResponseDto<AppDto>
            {
                Payload = this.Mapper.Map<App, AppDto>(this.AppRepository.FindOne(id))
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]AppPersistantDto appDto)
        {
            var app = this.Mapper.Map<AppPersistantDto, App>(appDto);
            Guid appId = this.AppRepository.Save(app);
            return new PayloadResponseDto<Guid> {
                Payload = appId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public PayloadResponseDto<int> Put(Guid id, [FromBody]AppPersistantDto appDto)
        {
            var app = this.Mapper.Map<AppPersistantDto, App>(appDto);
            app.Id = id;
            return new PayloadResponseDto<int>{
                Payload = this.AppRepository.Update(app)
            };
        }

        [HttpDelete("{id}")]
        [Transaction]
        public PayloadResponseDto<int> Delete(Guid id)
        {
            return new PayloadResponseDto<int>{
                Payload = this.AppRepository.Recycle(id)
            };
        }
        
        [HttpPut("{id}/securityobjects/{securityObjectId}")]
        [Transaction]
        public PayloadResponseDto<int> Put(Guid id, Guid securityObjectId)
        {
            return new PayloadResponseDto<int>{
                Payload = this.SecurityObjectRepository.UpdateAppId(securityObjectId, id)
            };
        }
    }
}
