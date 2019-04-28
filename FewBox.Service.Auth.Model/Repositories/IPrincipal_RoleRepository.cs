using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IPrincipal_RoleRepository: IBaseRepository<Principal_Role, Guid>
    {
        IEnumerable<Principal_Role> FindAllByPrincipalId(Guid principalId);
        IEnumerable<Principal_Role> FindAllByPrincipalIds(IList<Guid> principalIds);
        void DeleteByPrincipalId(Guid principalId);
        Principal_Role FindOneByPrincipalIdAndRoleId(Guid principalId, Guid roleId);
        bool IsExist(Guid principalId, Guid roleId);
    }
}
