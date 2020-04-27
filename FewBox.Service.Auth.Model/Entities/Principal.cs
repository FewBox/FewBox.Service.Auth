using System;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Service.Auth.Model.Entities
{
    public class Principal : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public PrincipalType PrincipalType { get; set; }
    }
}
