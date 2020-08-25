using System.Collections.Generic;
using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Service.Auth.Model.Services;

namespace FewBox.Service.Auth.Domain
{
    public class ModuleService : IModuleService
    {
        private IModuleRepository ModuleRepository { get; set; }
        private IMapper Mapper { get; set; }

        public ModuleService(IModuleRepository moduleRepository, IMapper mapper)
        {
            this.ModuleRepository = moduleRepository;
            this.Mapper = mapper;
        }


        public ModuleDto GetTree(string key)
        {
            var module = this.ModuleRepository.FindOneByCode(key);
            var moduleDto = this.Mapper.Map<Module, ModuleDto>(module);
            return this.BuildTree(moduleDto);

        }

        private ModuleDto BuildTree(ModuleDto moduleDto)
        {
            var modules = this.ModuleRepository.FindAllByParent(moduleDto.Id);
            if (modules != null)
            {
                moduleDto.Children = new List<ModuleDto>();
                foreach (var module in modules)
                {
                    var childModuleDto = this.Mapper.Map<Module, ModuleDto>(module);
                    moduleDto.Children.Add(childModuleDto);
                    this.BuildTree(childModuleDto);
                }
            }
            return moduleDto;
        }
    }
}