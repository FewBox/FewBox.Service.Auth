using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Service.Auth.Model.Entities
{
    public class Role : Entity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
