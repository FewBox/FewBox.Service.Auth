using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IPrincipalRepository : IRepository<Principal>
    {
        bool IsExist(string name);
        Principal FindOneByName(string name);
    }
}
