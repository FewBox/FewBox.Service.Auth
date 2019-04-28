using FewBox.Service.Auth.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;
using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Repositories
{
    public interface IRoleRepository : IBaseRepository<Role, Guid>
    {
        Role FindOneByCode(string code);
        IEnumerable<Role> FindAllByApiId(Guid apiId);
        IEnumerable<Role> FindAllByModuleId(Guid moduleId);
        IEnumerable<Role> FindAllByUserId(Guid userId);
        IEnumerable<Role> FindAllByGroupId(Guid groupId);
    }
}
