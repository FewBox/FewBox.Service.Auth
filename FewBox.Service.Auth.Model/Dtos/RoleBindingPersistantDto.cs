using System;
using System.Collections.Generic;
using System.Text;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class RoleBindingPersistantDto
    {
        public Guid SecurityObjectId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PrincipalId { get; set; }
    }
}
