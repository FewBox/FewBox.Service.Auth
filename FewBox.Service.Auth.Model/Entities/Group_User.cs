using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Model.Entities
{
    public class Group_User : Entity
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
    }
}
