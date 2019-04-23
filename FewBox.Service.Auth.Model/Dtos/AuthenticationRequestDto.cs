using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class AuthenticationRequestDto
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
    }
}
