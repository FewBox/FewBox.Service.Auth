using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class ChangePasswordRequestDto
    {
        public Guid ValidateCode { get; set; }
        public string Password { get; set; }
    }
}
