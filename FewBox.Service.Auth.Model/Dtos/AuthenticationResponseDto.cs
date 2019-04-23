using System;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class AuthenticationResponseDto
    {
        public bool IsValid { get; set; }
        public Guid PrincipalId { get; set; }
    }
}
