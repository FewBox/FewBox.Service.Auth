namespace FewBox.Service.Auth.Model.Dtos
{
    public class SignInResponseDto
    {
        public bool IsValid { get; set; }
        public string Token { get; set; }
    }
}
