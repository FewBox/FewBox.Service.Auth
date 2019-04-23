using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class SignInResponseDto
    {
        public bool IsAuthorized { get; set; }
        public string Token { get; set;  }
        public IList<string> RoleCodes { get; set; }
    }
}
