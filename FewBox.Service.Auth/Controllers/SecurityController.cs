using FewBox.Service.Auth.Model.Dtos;
using FewBox.Service.Auth.Model.Repositories;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FewBox.Service.Auth.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost("changepassword")]
        [Transaction]
        public MetaResponseDto ChangePassword([FromBody] ChangePasswordRequestDto changePasswordRequestDto)
        {
            // Todo: 
            string email = null;
            if (String.IsNullOrEmpty(email))
            {
                return new MetaResponseDto { IsSuccessful = false };
            }
            this.UserRepository.ResetPassword(email, changePasswordRequestDto.Password);
            return new MetaResponseDto { };
        }
    }
}
