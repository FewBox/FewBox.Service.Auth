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
using Microsoft.AspNetCore.Authorization;
using FewBox.Service.Auth.Model.Configs;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
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
        private ApiConfig ApiConfig { get; set; }

        public AppController(IUserRepository userRepository, IGroupRepository groupRepository, IRoleRepository roleRepository,
            IApiRepository apiRepository, IModuleRepository moduleRepository, IGroup_UserRepository group_UserRepository,
            IPrincipalRepository principalRepository, ISecurityObjectRepository securityObjectRepository,
            IRole_SecurityObjectRepository role_SecurityObjectRepository, IPrincipal_RoleRepository principal_RoleRepository,
            ApiConfig apiConfig, IMapper mapper) : base(mapper)
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
            this.ApiConfig = apiConfig;
        }

        [AllowAnonymous]
        [HttpGet("isinit")]
        public MetaResponseDto IsInit()
        {
            if(this.UserRepository.Count() > 0)
            {
                return new MetaResponseDto { IsSuccessful = false, ErrorCode = "APP_INIT", ErrorMessage = "The app has been init, please sign in." };
            }
            return new MetaResponseDto();
        }

        [AllowAnonymous]
        [HttpPost("initadministrator")]
        [Transaction]
        public MetaResponseDto InitAdministrator()
        {
            string username = "fewbox";
            if(this.UserRepository.IsExist(username))
            {
                return new MetaResponseDto { IsSuccessful = false, ErrorCode = "ADMIN_EXIST", ErrorMessage = "The administrator is exist, please sign in." };
            }
            Guid principalId = this.PrincipalRepository.Save(new Principal { Name = username, PrincipalType = PrincipalType.User });
            Guid userId = this.UserRepository.SaveWithMD5Password(new User { PrincipalId = principalId }, "landpy");
            Guid roleId = this.RoleRepository.Save(new Role { Name = "App Admin", Code = "R_APPADMIN" });
            // Init Api
            foreach(var apiItem in this.ApiConfig.ApiItems)
            {
                this.InitApi(apiItem.Controller, apiItem.Actions, roleId);
            }
            this.Principal_RoleRepository.Save(new Principal_Role { PrincipalId = principalId, RoleId = roleId });
            return new MetaResponseDto {};
        }

        [HttpGet("exception")]
        public void ThrowException()
        {
            throw new Exception("FewBox Exception");
        }

        private void InitApi(string controller, string[] actions, Guid roleId)
        {
            foreach(string action in actions)
            {
                Guid securityObjectId = this.SecurityObjectRepository.Save(new SecurityObject { Name = $"{controller}_{action}" });
                Guid usersApiId = this.ApiRepository.Save(new Api { SecurityObjectId=securityObjectId, Controller=controller, Action=action });
                var usersApi = this.ApiRepository.FindOne(usersApiId);
                this.Role_SecurityObjectRepository.Save(new Role_SecurityObject { RoleId = roleId, SecurityObjectId = usersApi.SecurityObjectId });
            }
        }
    }
}