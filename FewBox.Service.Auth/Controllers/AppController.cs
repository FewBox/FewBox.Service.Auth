using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using FewBox.Core.Web.Security;
using FewBox.Core.Web.Dto;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [RemoteRoleAuthorize(Policy="RemoteRole_Pure")]
    public class AppController : MapperController
    {
        private IPrincipalRepository PrincipalRepository { get; set; }
        private IUserRepository UserRepository { get; set; }
        private IGroupRepository GroupRepository { get; set; }
        private IGroup_UserRepository Group_UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private ISecurityObjectRepository SecurityObjectRepository { get; set; }
        private IApiRepository ApiRepository { get; set; }
        private IModuleRepository ModuleRepository { get; set; }
        private IRole_SecurityObjectRepository Role_SecurityObjectRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }

        public AppController(IUserRepository userRepository, IGroupRepository groupRepository, IRoleRepository roleRepository, IApiRepository apiRepository,
            IModuleRepository moduleRepository, IGroup_UserRepository group_UserRepository,
            IPrincipalRepository principalRepository, ISecurityObjectRepository securityObjectRepository,
            IRole_SecurityObjectRepository role_SecurityObjectRepository, IPrincipal_RoleRepository principal_RoleRepository, IMapper mapper) : base(mapper)
        {
            this.PrincipalRepository = principalRepository;
            this.UserRepository = userRepository;
            this.GroupRepository = groupRepository;
            this.RoleRepository = roleRepository;
            this.SecurityObjectRepository = securityObjectRepository;
            this.ApiRepository = apiRepository;
            this.ModuleRepository = moduleRepository;
            this.Group_UserRepository = group_UserRepository;
            this.Role_SecurityObjectRepository = role_SecurityObjectRepository;
            this.Principal_RoleRepository = principal_RoleRepository;
        }

        [HttpPost("Init")]
        [Transaction]
        public MetaResponseDto Init()
        {
            /*string username = "fewbox";
            if(this.UserRepository.FindOneByUsername(username)!=null)
            {
                return new MetaResponseDto { IsSuccessful = false };
            }
            // 主体
            Guid adminPrincipalId = this.PrincipalRepository.Save(new Principal { Name = username, PrincipalType = PrincipalType.User });
            // 用户
            Guid adminId = this.UserRepository.SaveWithMD5Password(new User { PrincipalId = adminPrincipalId }, "Admin");
            // 角色
            Guid adminRoleId = this.RoleRepository.Save(new Role { Name = "App Admin", Code = "R_APPADMIN" });
            // 安全对象
            Guid apiSecurityObjectId = this.SecurityObjectRepository.Save(new SecurityObject { Name = "Api" });
            // 资源
            Guid usersApiId = this.ApiRepository.Save(new Api { SecurityObjectId=apiSecurityObjectId, Controller="Users", Action="Get" });
            var usersApi = this.ApiRepository.FindOne(usersApiId);
            // 角色分配资源
            this.Role_SecurityObjectRepository.Save(new Role_SecurityObject { RoleId = adminRoleId, SecurityObjectId = usersApi.SecurityObjectId });
            // 用户分配角色
            this.Principal_RoleRepository.Save(new Principal_Role { PrincipalId = adminPrincipalId, RoleId = adminRoleId }); */
            return new MetaResponseDto {};
        }

        [HttpGet("exception")]
        public void ThrowException()
        {
            throw new Exception("FewBox Exception");
        }
    }
}
