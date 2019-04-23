using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IRole_SecurityObjectRepository : IBaseRepository<Role_SecurityObject, Guid>
    {
        IEnumerable<Role_SecurityObject> FindAllBySecurityId(Guid securityObjectId);
        void DeleteBySecurityId(Guid securityObjectId);
    }
}
