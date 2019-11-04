using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IPrincipalRepository : IBaseRepository<Principal, Guid>
    {
        bool IsExist(string name);
        Principal FindOneByName(string name);
    }
}
