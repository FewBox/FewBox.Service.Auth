using AutoMapper;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Entities;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FewBox.Core.Web.Token;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [Authorize(Policy = "JWTPayload_ControllerAction")]
    public class RolesController : ResourcesController<IRoleRepository, Role, RoleDto, RolePersistantDto>
    {
        public RolesController(IRoleRepository roleRepository, ITokenService tokenService, IMapper mapper) : base(roleRepository, tokenService, mapper)
        {
        }
    }
}
