using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Model.Entities
{
    public class Service : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}