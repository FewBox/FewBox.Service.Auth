namespace FewBox.Service.Auth.Model.Configs
{
    public class RoleAssignmentConfig
    {
        public string Principal { get; set; }
        public PrincipalTypeConfig PrincipalType { get; set; }
        public string Role { get; set; }
    }
}