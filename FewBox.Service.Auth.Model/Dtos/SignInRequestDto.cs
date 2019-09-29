namespace FewBox.Service.Auth.Model.Dtos
{
    public class SignInRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
    }
}
