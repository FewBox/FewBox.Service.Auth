using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class GroupPersistantDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ParentId { get; set; }
        public IList<Guid> RoleIds { get; set; }
    }
}
