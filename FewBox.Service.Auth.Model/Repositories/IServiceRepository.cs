using FewBox.Core.Persistence.Orm;
using S = FewBox.Service.Auth.Model.Entities;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IServiceRepository : IRepository<S.Service>
    {
        bool IsExist(string name);
        S.Service FindOneByName(string name);
    }
}
