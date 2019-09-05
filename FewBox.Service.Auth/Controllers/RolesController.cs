using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class RolesController : ResourcesController<IRoleRepository, Role, Guid, RoleDto, RolePersistantDto>
    {
        private IGroupRepository GroupRepository { get; set; }
        public RolesController(IRoleRepository roleRepository, IGroupRepository groupRepository,
        IMapper mapper) : base(roleRepository, mapper)
        {
            this.GroupRepository = groupRepository;
        }

        [HttpGet("seek/{roleCode}/groups/count")]
        public PayloadResponseDto<int> GetGroupCount(string roleCode)
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.GroupRepository.CountByRoleCode(roleCode)
            };
        }
    }
}
