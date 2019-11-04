using FewBox.Core.Persistence.Orm;
using System;
using S = FewBox.Service.Auth.Model.Entities;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IServiceRepository : IBaseRepository<S.Service, Guid>
    {
        bool IsExist(string name);
        S.Service FindOneByName(string name);
    }
}
