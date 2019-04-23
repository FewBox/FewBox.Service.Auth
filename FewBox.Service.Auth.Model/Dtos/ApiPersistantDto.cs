using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class ApiPersistantDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public IList<Guid> RoleIds { get; set; }
    }
}
