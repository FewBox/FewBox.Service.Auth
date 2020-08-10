using System;

namespace FewBox.Service.Auth.Model.Entities
{
    public class User : Principal
    {
        public Guid Salt { get; set; }
        public string SaltMD5Password { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public Guid PrincipalId { get; set; }
        public Guid TenantId { get; set; }
        public UserType Type { get; set; }
        #region Google
        public string GoogleId { get; set; }
        public string GoogleEmail { get; set; }
        #endregion
    }
}
