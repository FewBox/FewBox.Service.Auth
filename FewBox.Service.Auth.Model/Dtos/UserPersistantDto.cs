using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class UserPersistantDto
    {
        public UserTypeDto Type { get; set; }
        public string Tenant { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public IList<Guid> RoleIds { get; set; }
    }
}
