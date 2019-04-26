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
using FewBox.Core.Web.Security;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class ApisController : MapperController
    {
        private ISecurityObjectRepository SecurityObjectRepository { get; set; }
        private IApiRepository ApiRepository { get; set; }
        private IRole_SecurityObjectRepository Role_SecurityObjectRepository { get; set; }

        public ApisController(IApiRepository apiRepository, ISecurityObjectRepository securityObjectRepository, IRole_SecurityObjectRepository role_SecurityObjectRepository,
            IMapper mapper) : base(mapper)
        {
            this.ApiRepository = apiRepository;
            this.SecurityObjectRepository = securityObjectRepository;
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<ApiDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<ApiDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Api>, IEnumerable<ApiDto>>(this.ApiRepository.FindAll())
            };
        }

        [HttpGet("Search/{keyword}")]
        public PayloadResponseDto<IEnumerable<ApiDto>> GetByKeyword(string keyword)
        {
            return new PayloadResponseDto<IEnumerable<ApiDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Api>, IEnumerable<ApiDto>>(this.ApiRepository.FindAllByKeyword(keyword))
            };
        }

        [HttpGet("Paging/{pageRange}/{pageIndex}")]
        public PayloadResponseDto<PagingDto<ApiDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<ApiDto>>
            {
                Payload = new PagingDto<ApiDto>
                {
                    Items = this.Mapper.Map<IEnumerable<Api>, IEnumerable<ApiDto>>(this.ApiRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.ApiRepository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<ApiDto> Get(Guid id)
        {
            return new PayloadResponseDto<ApiDto>
            {
                Payload = this.Mapper.Map<Api, ApiDto>(this.ApiRepository.FindOne(id))
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]ApiPersistantDto apiDto)
        {
            var securityObject = this.Mapper.Map<ApiPersistantDto, SecurityObject>(apiDto);
            Guid securityObjectId = this.SecurityObjectRepository.Save(securityObject);
            var api = this.Mapper.Map<ApiPersistantDto, Api>(apiDto);
            api.SecurityObjectId = securityObject.Id;
            Guid apiId = this.ApiRepository.Save(api);
            if (apiDto.RoleIds != null)
            {
                foreach (Guid roleId in apiDto.RoleIds)
                {
                    this.Role_SecurityObjectRepository.Save(new Role_SecurityObject
                    {
                        SecurityObjectId = securityObjectId,
                        RoleId = roleId
                    });
                }
            }
            return new PayloadResponseDto<Guid> {
                Payload = apiId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public MetaResponseDto Put(Guid id, [FromBody]ApiPersistantDto apiDto)
        {
            var api = this.Mapper.Map<ApiPersistantDto, Api>(apiDto);
            api.Id = id;
            var updateApi = this.ApiRepository.FindOne(id);
            var securityObject = this.Mapper.Map<ApiPersistantDto, SecurityObject>(apiDto);
            securityObject.Id = updateApi.SecurityObjectId;
            this.SecurityObjectRepository.Update(securityObject);
            this.ApiRepository.Update(api);
            this.Role_SecurityObjectRepository.DeleteBySecurityId(updateApi.SecurityObjectId);
            if (apiDto.RoleIds != null)
            {
                foreach (Guid roleId in apiDto.RoleIds)
                {
                    this.Role_SecurityObjectRepository.Save(new Role_SecurityObject
                    {
                        SecurityObjectId = updateApi.SecurityObjectId,
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
            var updateApi = this.ApiRepository.FindOne(id);
            this.SecurityObjectRepository.Recycle(updateApi.SecurityObjectId);
            this.ApiRepository.RecycleAsync(id);
            return new MetaResponseDto();
        }
    }
}
