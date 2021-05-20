namespace FewBox.Service.Auth.Model.Dtos
{
    public class ForgotPasswordRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string ProductName { get; set; }
        public string ResetUrl { get; set; }
    }
}
