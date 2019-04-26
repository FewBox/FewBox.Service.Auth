using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Controller;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class UserAggregatesController : MapperController
    {
        private IUserRepository UserRepository { get; set; }
        private IGroupRepository GroupRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IPrincipal_RoleRepository Principal_RoleRepository { get; set; }
        private IGroup_UserRepository Group_UserRepository { get; set; }

        protected UserAggregatesController(IMapper mapper, IUserRepository userRepository, IGroupRepository groupRepository,
            IRoleRepository roleRepository, IGroup_UserRepository group_UserRepository) : base(mapper)
        {
            this.UserRepository = userRepository;
            this.GroupRepository = groupRepository;
            this.RoleRepository = roleRepository;
            this.Group_UserRepository = group_UserRepository;
        }

        [HttpGet("name/{name}")]
        public UserAggregateDto Get(string name)
        {
            var user = this.UserRepository.FindOneByUsername(name);
            var userAggregateDto = this.Mapper.Map<User, UserAggregateDto>(user);
            return userAggregateDto;
        }
    }
}
