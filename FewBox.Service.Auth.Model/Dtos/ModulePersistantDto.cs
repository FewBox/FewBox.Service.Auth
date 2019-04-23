using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class ModulePersistantDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
        public Guid ParentId { get; set; }
        public IList<Guid> RoleIds { get; set; }
    }
}
