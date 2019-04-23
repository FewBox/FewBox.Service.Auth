using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Model.Entities
{
    public class Principal_Role : Entity<Guid>
    {
        public Guid PrincipalId { get; set; }
        public Guid RoleId { get; set; }
    }
}
