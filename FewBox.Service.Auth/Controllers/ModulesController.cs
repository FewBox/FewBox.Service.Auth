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

        public ModulesController(IModuleRepository moduleRepository, ISecurityObjectRepository securityObjectRepository,
            IModuleService moduleService, IRole_SecurityObjectRepository role_SecurityObjectRepository, IMapper mapper) : base(mapper)
        {
            this.ModuleRepository = moduleRepository;
            this.SecurityObjectRepository = securityObjectRepository;
            this.ModuleService = moduleService;
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
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
        public PayloadResponseDto<IEnumerable<ModuleDto>> GetByRoot()
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

        [HttpGet("Paging/{pageRange}/{pageIndex}")]
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

        [HttpGet("count")]
        public PayloadResponseDto<int> GetTotalNumber()
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.ModuleRepository.Count()
            };
        }

        [HttpPut("changeparent")]
        [Transaction]
        public MetaResponseDto ChangeParent([FromBody]ChangeModuleParentRequestDto changeModuleParentRequestDto)
        {
            this.ModuleRepository.ChangeModuleParentId(changeModuleParentRequestDto.ParentId, changeModuleParentRequestDto.SourceIds);
            return new MetaResponseDto { };
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
    }
}
