using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Service.Auth.Model.Services;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class ModulesController : MapperController
    {
        private ISecurityObjectRepository SecurityObjectRepository { get; set; }
        private IModuleRepository ModuleRepository { get; set; }
        private IModuleService ModuleService { get; set; }
        private IRole_SecurityObjectRepository Role_SecurityObjectRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }

        public ModulesController(IModuleRepository moduleRepository, ISecurityObjectRepository securityObjectRepository,
        IModuleService moduleService, IRole_SecurityObjectRepository role_SecurityObjectRepository,
        IRoleRepository roleRepository, IMapper mapper) : base(mapper)
        {
            this.ModuleRepository = moduleRepository;
            this.SecurityObjectRepository = securityObjectRepository;
            this.ModuleService = moduleService;
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
            this.RoleRepository = roleRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<ModuleDto>> Get()
        {
            return new PayloadResponseDto<IEnumerable<ModuleDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<Module>, IEnumerable<ModuleDto>>(this.ModuleRepository.FindAll())
            };
        }

        [HttpGet("root")]
        public PayloadResponseDto<IEnumerable<ModuleDto>> GetRoot()
        {
            var modules = new List<ModuleDto>();
            var rootModules = this.ModuleRepository.FindAllByRoot();
            foreach (var module in rootModules)
            {
                var moduleDto = this.ModuleService.GetTree(module.Key);
                modules.Add(moduleDto);
            }
            return new PayloadResponseDto<IEnumerable<ModuleDto>>
            {
                Payload = modules
            };
        }

        [HttpGet("count")]
        public PayloadResponseDto<int> Count()
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.ModuleRepository.Count()
            };
        }

        [HttpGet("paging/{pageIndex}/{pageRange}")]
        public PayloadResponseDto<PagingDto<ModuleDto>> Get(int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<ModuleDto>>
            {
                Payload = new PagingDto<ModuleDto>
                {
                    Items = this.Mapper.Map<IEnumerable<Module>, IEnumerable<ModuleDto>>(this.ModuleRepository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.ModuleRepository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<ModuleDto> Get(Guid id)
        {
            return new PayloadResponseDto<ModuleDto>
            {
                Payload = this.Mapper.Map<Module, ModuleDto>(this.ModuleRepository.FindOne(id))
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]ModulePersistantDto moduleDto)
        {
            var securityObject = this.Mapper.Map<ModulePersistantDto, SecurityObject>(moduleDto);
            Guid securityObjectId = this.SecurityObjectRepository.Save(securityObject);
            var module = this.Mapper.Map<ModulePersistantDto, Module>(moduleDto);
            module.SecurityObjectId = securityObject.Id;
            Guid moduleId = this.ModuleRepository.Save(module);
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
            return new PayloadResponseDto<Guid> {
                Payload = moduleId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public MetaResponseDto Put(Guid id, [FromBody]ModulePersistantDto moduleDto)
        {
            var module = this.Mapper.Map<ModulePersistantDto, Module>(moduleDto);
            module.Id = id;
            var updateModule = this.ModuleRepository.FindOne(id);
            var securityObject = this.Mapper.Map<ModulePersistantDto, SecurityObject>(moduleDto);
            securityObject.Id = updateModule.SecurityObjectId;
            this.SecurityObjectRepository.Update(securityObject);
            this.ModuleRepository.Update(module);
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
            return new MetaResponseDto();
        }

        [HttpDelete("{id}")]
        [Transaction]
        public MetaResponseDto Delete(Guid id)
        {
            var updateModule = this.ModuleRepository.FindOne(id);
            this.SecurityObjectRepository.Recycle(updateModule.SecurityObjectId);
            this.ModuleRepository.RecycleAsync(id);
            return new MetaResponseDto();
        }

        [HttpPut("{id}/roles/{roleId}")]
        [Transaction]
        public PayloadResponseDto<Guid> AddRole(Guid id, Guid roleId)
        {
            Guid newId = Guid.Empty;
            var module = this.ModuleRepository.FindOne(id);
            if(!this.Role_SecurityObjectRepository.IsExist(roleId, module.SecurityObjectId))
            {
                var role_SecurityObject = new Role_SecurityObject { RoleId = roleId, SecurityObjectId = module.SecurityObjectId };
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
            var module = this.ModuleRepository.FindOne(id);
            var role_SecurityObject = this.Role_SecurityObjectRepository.FindOneByRoleIdAndSecurityObjectId(roleId, module.SecurityObjectId);
            if(role_SecurityObject != null)
            {
                effect = this.Role_SecurityObjectRepository.Delete(role_SecurityObject.Id);
            }
            return new PayloadResponseDto<int>{
                Payload = effect
            };
        }

        [HttpPut("batchgrantrole")]
        [Transaction]
        public MetaResponseDto BatchGrantRole([FromBody]BatchGrantRoleRequestDto batchGrantRoleRequestDto)
        {
            if (batchGrantRoleRequestDto.Ids != null)
            {
                foreach (Guid id in batchGrantRoleRequestDto.Ids)
                {
                    var module = this.ModuleRepository.FindOne(id);
                    this.Role_SecurityObjectRepository.DeleteBySecurityId(module.SecurityObjectId);
                    if (batchGrantRoleRequestDto.RoleIds != null)
                    {
                        foreach (Guid roleId in batchGrantRoleRequestDto.RoleIds)
                        {
                            this.Role_SecurityObjectRepository.Save(new Role_SecurityObject
                            {
                                SecurityObjectId = module.SecurityObjectId,
                                RoleId = roleId
                            });
                        }
                    }

                }
            }
            return new MetaResponseDto { };
        }

        [HttpPost("{id}/modules/{parentId}")]
        [Transaction]
        public PayloadResponseDto<int> ChangeParent(Guid id, Guid parentId)
        {
            int effect = 0;
            effect = this.ModuleRepository.UpdateParent(id, parentId);
            return new PayloadResponseDto<int>{
                Payload = effect
            };
        }
        
        [HttpGet("seek/{moduleKey}/roles")]
        public PayloadResponseDto<IEnumerable<RoleDto>> Get(string moduleKey)
        {
            var module = this.ModuleRepository.FindOneByKey(moduleKey);
            return new PayloadResponseDto<IEnumerable<RoleDto>>{
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
