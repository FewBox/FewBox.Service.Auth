using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class SigninResponseDto
    {
        public bool IsValid { get; set; }
        public string Token { get; set; }
    }
}
