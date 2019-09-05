using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Services;
using FewBox.Core.Web.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    public class HealthyController : ControllerBase
    {
        private IAppService AppService { get; set; }

        public HealthyController(IAppService appService)
        {
            this.AppService = appService;
        }

        [HttpGet]
        public PayloadResponseDto<HealthyDto> Get()
        {
            return new PayloadResponseDto<HealthyDto>
            {
                Payload = this.AppService.GetHealtyInfo()
            };
        }
    }
}
