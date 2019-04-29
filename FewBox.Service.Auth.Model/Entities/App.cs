using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Model.Entities
{
    public class App : Entity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}