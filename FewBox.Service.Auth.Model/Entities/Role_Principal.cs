using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Model.Entities
{
    public class Role_Principal : Entity
    {
        public Guid RoleId { get; set; }
        public Guid PrincipalId { get; set; }
    }
}
