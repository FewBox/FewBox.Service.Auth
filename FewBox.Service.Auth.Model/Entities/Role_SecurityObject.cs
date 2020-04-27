using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Model.Entities
{
    public class Role_SecurityObject : Entity
    {
        public Guid SecurityObjectId { get; set; }
        public Guid RoleId { get; set; }
    }
}
