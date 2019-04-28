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

        private IRoleRepository RoleRepository { get; set; }

        public ApisController(IApiRepository apiRepository, ISecurityObjectRepository securityObjectRepository, IRole_SecurityObjectRepository role_SecurityObjectRepository,
        IRoleRepository roleRepository, IMapper mapper) : base(mapper)
        {
            this.ApiRepository = apiRepository;
            this.SecurityObjectRepository = securityObjectRepository;
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
            this.RoleRepository = roleRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<ApiDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<ApiDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Api>, IEnumerable<ApiDto>>(this.ApiRepository.FindAll())
            };
        }

        [HttpGet("search/{keyword}")]
        public PayloadResponseDto<IEnumerable<ApiDto>> Get(string keyword)
        {
            return new PayloadResponseDto<IEnumerable<ApiDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Api>, IEnumerable<ApiDto>>(this.ApiRepository.FindAllByKeyword(keyword))
            };
        }

        [HttpGet("paging/{pageIndex}/{pageRange}")]
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

        [HttpPut("{id}/roles/{roleId}")]
        [Transaction]
        public PayloadResponseDto<Guid> AddRole(Guid id, Guid roleId)
        {
            Guid newId = Guid.Empty;
            var api = this.ApiRepository.FindOne(id);
            if(!this.Role_SecurityObjectRepository.IsExist(roleId, api.SecurityObjectId))
            {
                var role_SecurityObject = new Role_SecurityObject { RoleId = roleId, SecurityObjectId = api.SecurityObjectId };
                newId = this.Role_SecurityObjectRepository.Save(role_SecurityObject);
            }
            return new PayloadResponseDto<Guid>{
                Payload = newId
            };
        }

        [HttpDelete("{id}/roles/{roleId}")]
        [Transaction]
        public PayloadResponseDto<int> RemoveRole(Guid id, Guid roleId)
        {
            int effect = 0;
            var api = this.ApiRepository.FindOne(id);
            var role_SecurityObject = this.Role_SecurityObjectRepository.FindOneByRoleIdAndSecurityObjectId(roleId, api.SecurityObjectId);
            if(role_SecurityObject != null)
            {
                effect = this.Role_SecurityObjectRepository.Delete(role_SecurityObject.Id);
            }
            return new PayloadResponseDto<int>{
                Payload = effect
            };
        }
        
        [HttpGet("seek/{controller}/{action}/roles")]
        public PayloadResponseDto<IEnumerable<RoleDto>> Get(string controller, string action)
        {
            var api = this.ApiRepository.FindOneByControllerAndAction(controller, action);
            return new PayloadResponseDto<IEnumerable<RoleDto>>{
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByApiId(api.Id))
            };
        }

        [HttpGet("{id}/roles")]
        public PayloadResponseDto<IEnumerable<RoleDto>> GetRoles(Guid id)
        {
            return new PayloadResponseDto<IEnumerable<RoleDto>> {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByApiId(id))
            };
        }
    }
}
