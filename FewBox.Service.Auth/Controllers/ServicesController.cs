using AutoMapper;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Token;
using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S = FewBox.Service.Auth.Model.Entities;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [Authorize(Policy="JWTPayload_ControllerAction")]
    public class ServicesController : ResourcesController<IServiceRepository, S.Service, ServiceDto, ServicePersistantDto>
    {
        private ISecurityObjectRepository SecurityObjectRepository { get; set; }
        public ServicesController(IServiceRepository serviceRepository, ISecurityObjectRepository securityObjectRepository, ITokenService tokenService,
        IMapper mapper) : base(serviceRepository, tokenService, mapper)
        {
            this.SecurityObjectRepository = securityObjectRepository;
        }
    }
}
