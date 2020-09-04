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
    [Route("api/v{v:apiVersion}/[controller]")]
    [Authorize(Policy = "JWTRole_ControllerAction")]
    public class ApisController : ResourcesController<IApiRepository, Api, ApiDto, ApiPersistantDto>
    {
        private ISecurityObjectRepository SecurityObjectRepository { get; set; }
        private IRole_SecurityObjectRepository Role_SecurityObjectRepository { get; set; }

        private IRoleRepository RoleRepository { get; set; }

        public ApisController(IApiRepository apiRepository, ISecurityObjectRepository securityObjectRepository, IRole_SecurityObjectRepository role_SecurityObjectRepository,
        IRoleRepository roleRepository, IMapper mapper) : base(apiRepository, mapper)
        {
            this.SecurityObjectRepository = securityObjectRepository;
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
            this.RoleRepository = roleRepository;
        }

        [HttpGet("search")]
        public PayloadResponseDto<IEnumerable<ApiDto>> Get([FromQuery]string keyword)
        {
            return new PayloadResponseDto<IEnumerable<ApiDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Api>, IEnumerable<ApiDto>>(this.Repository.FindAllByKeyword(keyword))
            };
        }

        [HttpPost]
        [Transaction]
        public override PayloadResponseDto<Guid> Post([FromBody]ApiPersistantDto apiDto)
        {
            var securityObject = this.Mapper.Map<ApiPersistantDto, SecurityObject>(apiDto);
            Guid securityObjectId = this.SecurityObjectRepository.Save(securityObject);
            var api = this.Mapper.Map<ApiPersistantDto, Api>(apiDto);
            api.SecurityObjectId = securityObject.Id;
            Guid apiId = this.Repository.Save(api);
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
            return new PayloadResponseDto<Guid>
            {
                Payload = apiId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public override PayloadResponseDto<int> Put(Guid id, [FromBody]ApiPersistantDto apiDto)
        {
            int effect;
            var api = this.Mapper.Map<ApiPersistantDto, Api>(apiDto);
            api.Id = id;
            var updateApi = this.Repository.FindOne(id);
            var securityObject = this.Mapper.Map<ApiPersistantDto, SecurityObject>(apiDto);
            securityObject.Id = updateApi.SecurityObjectId;
            this.SecurityObjectRepository.Update(securityObject);
            effect = this.Repository.Update(api);
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
            return new PayloadResponseDto<int>
            {
                Payload = effect
            };
        }

        [HttpDelete("{id}")]
        [Transaction]
        public override PayloadResponseDto<int> Delete(Guid id)
        {
            var updateApi = this.Repository.FindOne(id);
            this.SecurityObjectRepository.Recycle(updateApi.SecurityObjectId);
            return new PayloadResponseDto<int>
            {
                Payload = this.Repository.Recycle(id)
            };
        }

        [HttpPut("{id}/roles/{roleId}")]
        [Transaction]
        public PayloadResponseDto<Guid> AddRole(Guid id, Guid roleId)
        {
            Guid newId = Guid.Empty;
            var api = this.Repository.FindOne(id);
            if (!this.Role_SecurityObjectRepository.IsExist(roleId, api.SecurityObjectId) &&
            this.Repository.IsExist(id) &&
            this.RoleRepository.IsExist(roleId))
            {
                var role_SecurityObject = new Role_SecurityObject { RoleId = roleId, SecurityObjectId = api.SecurityObjectId };
                newId = this.Role_SecurityObjectRepository.Save(role_SecurityObject);
            }
            return new PayloadResponseDto<Guid>
            {
                Payload = newId
            };
        }

        [HttpGet("{id}/roles")]
        public PayloadResponseDto<IEnumerable<RoleDto>> GetRoles(Guid id)
        {
            return new PayloadResponseDto<IEnumerable<RoleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByApiId(id))
            };
        }

        [HttpGet("seek")]
        public PayloadResponseDto<ApiDto> Get([FromQuery] string serviceName, string controllerName, string actionName)
        {
            return new PayloadResponseDto<ApiDto>
            {
                Payload = this.Mapper.Map<Api, ApiDto>(this.Repository.FindOneByServiceAndControllerAndAction(serviceName, controllerName, actionName))
            };
        }
    }
}
