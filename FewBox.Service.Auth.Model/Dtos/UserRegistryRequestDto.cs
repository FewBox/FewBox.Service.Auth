namespace FewBox.Service.Auth.Model.Dtos
{
    public class UserRegistryRequestDto
    {
        public string ProductName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string ValidateCode { get; set; }
    }
}
