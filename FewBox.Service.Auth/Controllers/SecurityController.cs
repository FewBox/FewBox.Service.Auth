using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "JWTRole_ControllerAction")]
    public class SecurityController : Controller
    {
        private IUserRepository UserRepository { get; set; }

        public SecurityController(IUserRepository userRepository)
        {
            this.UserRepository = userRepository;
        }

        [HttpPost("sentvalidatecode/{email}")]
        public MetaResponseDto SentValidateCode(string email)
        {
            if (this.UserRepository.IsExist(email))
            {
                // Todo: 
            }
            else
            {
                // Todo: 
            }
            return new MetaResponseDto { };
        }
    }
}
