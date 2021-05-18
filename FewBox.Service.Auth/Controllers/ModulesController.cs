using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Service.Auth.Model.Services;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Token;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [Authorize(Policy = "JWTPayload_ControllerAction")]
    public class ModulesController : ResourcesController<IModuleRepository, Module, ModuleDto, ModulePersistantDto>
    {
        private ISecurityObjectRepository SecurityObjectRepository { get; set; }
        private IModuleService ModuleService { get; set; }
        private IRole_SecurityObjectRepository Role_SecurityObjectRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }

        public ModulesController(IModuleRepository moduleRepository, ISecurityObjectRepository securityObjectRepository,
        IModuleService moduleService, IRole_SecurityObjectRepository role_SecurityObjectRepository,
        IRoleRepository roleRepository, ITokenService tokenService, IMapper mapper) : base(moduleRepository, tokenService, mapper)
        {
            this.SecurityObjectRepository = securityObjectRepository;
            this.ModuleService = moduleService;
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
            this.RoleRepository = roleRepository;
        }

        [HttpGet("root")]
        public PayloadResponseDto<IEnumerable<ModuleDto>> GetRoot()
        {
            var modules = new List<ModuleDto>();
            var rootModules = this.Repository.FindAllByRoot();
            foreach (var module in rootModules)
            {
                var moduleDto = this.ModuleService.GetTree(module.Code);
                modules.Add(moduleDto);
            }
            return new PayloadResponseDto<IEnumerable<ModuleDto>>
            {
                Payload = modules
            };
        }

        [HttpPost]
        [Transaction]
        public override PayloadResponseDto<Guid> Post([FromBody] ModulePersistantDto moduleDto)
        {
            var securityObject = this.Mapper.Map<ModulePersistantDto, SecurityObject>(moduleDto);
            Guid securityObjectId = this.SecurityObjectRepository.Save(securityObject);
            var module = this.Mapper.Map<ModulePersistantDto, Module>(moduleDto);
            module.SecurityObjectId = securityObject.Id;
            Guid moduleId = this.Repository.Save(module);
            if (moduleDto.RoleIds != null)
            {
                foreach (Guid roleId in moduleDto.RoleIds)
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
                Payload = moduleId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public override PayloadResponseDto<int> Put(Guid id, [FromBody] ModulePersistantDto moduleDto)
        {
            int effect;
            var module = this.Mapper.Map<ModulePersistantDto, Module>(moduleDto);
            module.Id = id;
            var updateModule = this.Repository.FindOne(id);
            var securityObject = this.Mapper.Map<ModulePersistantDto, SecurityObject>(moduleDto);
            securityObject.Id = updateModule.SecurityObjectId;
            this.SecurityObjectRepository.Update(securityObject);
            effect = this.Repository.Update(module);
            this.Role_SecurityObjectRepository.DeleteBySecurityId(updateModule.SecurityObjectId);
            if (moduleDto.RoleIds != null)
            {
                foreach (Guid roleId in moduleDto.RoleIds)
                {
                    this.Role_SecurityObjectRepository.Save(new Role_SecurityObject
                    {
                        SecurityObjectId = updateModule.SecurityObjectId,
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
            var updateModule = this.Repository.FindOne(id);
            this.SecurityObjectRepository.Recycle(updateModule.SecurityObjectId);
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
            var module = this.Repository.FindOne(id);
            if (!this.Role_SecurityObjectRepository.IsExist(roleId, module.SecurityObjectId) &&
            this.Repository.IsExist(id) &&
            this.RoleRepository.IsExist(roleId))
            {
                var role_SecurityObject = new Role_SecurityObject { RoleId = roleId, SecurityObjectId = module.SecurityObjectId };
                newId = this.Role_SecurityObjectRepository.Save(role_SecurityObject);
            }
            return new PayloadResponseDto<Guid>
            {
                Payload = newId
            };
        }

        [HttpDelete("{id}/roles/{roleId}")]
        [Transaction]
        public PayloadResponseDto<int> RemoveRole(Guid id, Guid roleId)
        {
            int effect = 0;
            var module = this.Repository.FindOne(id);
            var role_SecurityObject = this.Role_SecurityObjectRepository.FindOneByRoleIdAndSecurityObjectId(roleId, module.SecurityObjectId);
            if (role_SecurityObject != null)
            {
                effect = this.Role_SecurityObjectRepository.Delete(role_SecurityObject.Id);
            }
            return new PayloadResponseDto<int>
            {
                Payload = effect
            };
        }

        [HttpPut("batchgrantrole")]
        [Transaction]
        public PayloadResponseDto<IList<Guid>> BatchGrantRole([FromBody] BatchGrantRoleRequestDto batchGrantRoleRequestDto)
        {
            var ids = new List<Guid>();
            if (batchGrantRoleRequestDto.Ids != null)
            {
                foreach (Guid id in batchGrantRoleRequestDto.Ids)
                {
                    var module = this.Repository.FindOne(id);
                    this.Role_SecurityObjectRepository.DeleteBySecurityId(module.SecurityObjectId);
                    if (batchGrantRoleRequestDto.RoleIds != null)
                    {
                        foreach (Guid roleId in batchGrantRoleRequestDto.RoleIds)
                        {
                            Guid newId = this.Role_SecurityObjectRepository.Save(new Role_SecurityObject
                            {
                                SecurityObjectId = module.SecurityObjectId,
                                RoleId = roleId
                            });
                            ids.Add(newId);
                        }
                    }

                }
            }
            return new PayloadResponseDto<IList<Guid>>
            {
                Payload = ids
            };
        }

        [HttpPost("{id}/modules/{parentId}")]
        [Transaction]
        public PayloadResponseDto<int> ChangeParent(Guid id, Guid parentId)
        {
            int effect = 0;
            effect = this.Repository.UpdateParent(id, parentId);
            return new PayloadResponseDto<int>
            {
                Payload = effect
            };
        }

        [HttpGet("seek/{moduleKey}/roles")]
        public PayloadResponseDto<IEnumerable<RoleDto>> Get(string moduleKey)
        {
            var module = this.Repository.FindOneByCode(moduleKey);
            return new PayloadResponseDto<IEnumerable<RoleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByModuleId(module.Id))
            };
        }

        [HttpGet("{id}/roles")]
        public PayloadResponseDto<IEnumerable<RoleDto>> GetRoles(Guid id)
        {
            return new PayloadResponseDto<IEnumerable<RoleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(this.RoleRepository.FindAllByModuleId(id))
            };
        }
    }
}
