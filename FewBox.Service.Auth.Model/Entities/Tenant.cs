using FewBox.Core.Persistence.Orm;

namespace FewBox.Service.Auth.Model.Entities
{
    public class Tenant : Entity
    {
        public string Name { get; set; }
    }
}
