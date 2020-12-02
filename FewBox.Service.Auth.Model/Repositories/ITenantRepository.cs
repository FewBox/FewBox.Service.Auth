using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        IEnumerable<Tenant> FindAllByKeyword(string keyword);
        Tenant FindOneByName(string name);
        bool IsExist(string name);
    }
}
