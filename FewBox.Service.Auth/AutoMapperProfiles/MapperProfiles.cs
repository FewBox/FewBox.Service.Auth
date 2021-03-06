using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using S = FewBox.Service.Auth.Model.Entities;

namespace FewBox.Service.Auth.AutoMapperProfiles
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<Group, GroupDto>();
            CreateMap<GroupPersistantDto, Group>();
            CreateMap<GroupPersistantDto, Principal>()
                .ForMember("PrincipalType", opt => opt.Ignore());

            CreateMap<User, UserDto>();
            CreateMap<User, UserProfileDto>();
            CreateMap<UserPersistantDto, User>();
            CreateMap<UserPersistantDto, Principal>()
                .ForMember("PrincipalType",opt => opt.Ignore());
            CreateMap<UserRegistryRequestDto, User>();
            CreateMap<UserRegistryRequestDto, Principal>()
                .ForMember("PrincipalType", opt => opt.Ignore());

            CreateMap<Api, ApiDto>();
            CreateMap<ApiPersistantDto, Api>();
            CreateMap<ApiPersistantDto, SecurityObject>();

            CreateMap<S.Service, ServiceDto>();
            CreateMap<ServicePersistantDto, S.Service>();

            CreateMap<Module, ModuleDto>();
            CreateMap<ModulePersistantDto, Module>();
            CreateMap<ModulePersistantDto, SecurityObject>();

            CreateMap<Role, RoleDto>();
            CreateMap<RolePersistantDto, Role>();

            CreateMap<Tenant, TenantDto>();
            CreateMap<TenantPersistantDto, Tenant>();
        }
    }
}