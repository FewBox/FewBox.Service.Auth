namespace FewBox.Service.Auth.Model.Dtos
{
    public class UserRegistryRequestDto
    {
        public string Tenant { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string ValidateCode { get; set; }
    }
}
