namespace FewBox.Service.Auth.Model.Dtos
{
    public class SignInRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsRememberMe { get; set; }
        public SignInTypeDto SignInType { get; set; }
    }
}
